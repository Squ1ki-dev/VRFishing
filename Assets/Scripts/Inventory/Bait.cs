using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Bait : MonoBehaviour
{
    public bool IsInWater { get; private set; }
    
    [Header("Events")] 
    [SerializeField] private ParticleSystem splashEffect;
    [SerializeField] private ParticleSystem pullingEffect;
    
    [Header("Settings")] 
    [SerializeField] private Transform targetPoint;
    [SerializeField, Min(0)] private float moveSpeed = 3f;
    [SerializeField] private float yUnderWaterOffset = 0.1f;

    private Rigidbody _rigidbody;
    private Transform _defaultParent;
    private Vector3 _defaultPosition;
    private bool _isPulling;
    private InputDevice _rightController;
    
    private bool _isHolding;
    private Vector3 _lastVelocity;
    private Vector3 _lastAngularVelocity;

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    private void Start()
    {
        _defaultParent = transform.parent;
        _defaultPosition = transform.localPosition;
        Init();
        InitializeInputDevice();
    }

    private void Update()
    {
        if (!_rightController.isValid)
            InitializeInputDevice();
        
        if (_rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool gripPressed))
        {
            if (gripPressed && !_isHolding)
                StartHolding();
            else if (!gripPressed && _isHolding)
                ThrowBait();
        }
    }

    private void FixedUpdate()
    {
        if (_isPulling)
            MoveToTarget();

        if (_isHolding)
        {
            _rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out _lastVelocity);
            _rightController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out _lastAngularVelocity);
        }
    }

    private void StartHolding()
    {
        _isHolding = true;
        _rigidbody.isKinematic = true;
        transform.SetParent(null);  // Detach from any previous parent
    }

    private void ThrowBait()
    {
        _isHolding = false;
        _rigidbody.isKinematic = false;
        
        // Apply the stored velocity from the controller
        _rigidbody.velocity = _lastVelocity;
        _rigidbody.angularVelocity = _lastAngularVelocity;
        
        StartCoroutine(WaitForFish());
    }

    public void Init()
    {
        _isPulling = false;
        IsInWater = false;
        _rigidbody.isKinematic = true;
        pullingEffect.Stop();
        
        Invoke(nameof(DelayedParentAndPosition), 1f);
    }

    private void DelayedParentAndPosition()
    {
        transform.parent = _defaultParent;
        transform.localPosition = _defaultPosition;
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator WaitForFish()
    {
        yield return new WaitForSeconds(Random.Range(5f, 10f)); // Adjust time for fish biting
        splashEffect.Play();
    }

	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent(out LowPolyWater.LowPolyWater water))
			return;

		WaterImmersion();
	}

    private void WaterImmersion()
    {
        _rigidbody.isKinematic = true;
        SetYOffset();
        splashEffect.Play();
    }

    private void SetYOffset()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y -= yUnderWaterOffset;
        transform.position = currentPosition;
    }

    private void InitializeInputDevice()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);
        
		if (devices.Count > 0)
            _rightController = devices[0];
    }
}