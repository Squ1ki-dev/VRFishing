using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Bait : MonoBehaviour
{
    public bool IsInWater { get; private set; }
    private bool _isHolding;
    private bool _isPulling;
    private float _holdStartTime;

    [Header("Events")]
    [SerializeField] private ParticleSystem splashEffect;
    [SerializeField] private ParticleSystem pullingEffect;

    [Header("Settings")]
    [SerializeField] private Transform targetPoint;
    [SerializeField, Min(0)] private float moveSpeed;
    [SerializeField] private float yUnderWaterOffset;
    [SerializeField] private float maxThrowPower;
    [SerializeField] private float minThrowPower;
    [SerializeField] private float angleInfluence;

    private Rigidbody _rigidbody;
    private Transform _defaultParent;
    
    private Vector3 _defaultPosition;
    private Vector3 _lastVelocity;
    private Vector3 _lastAngularVelocity;
    
    private InputDevice _rightController;

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

        if (!_isHolding && !_isPulling)
            ApplyAirResistance();
    }

    private void ApplyAirResistance() => _rigidbody.velocity *= 0.99f;

    private void StartHolding()
    {
        _isHolding = true;
        transform.SetParent(null);
        _holdStartTime = Time.time;
    }

    private void ThrowBait()
    {
        _isHolding = false;
        _rigidbody.isKinematic = false;

        // Use the controller's velocity magnitude to determine throw power
        float throwPower = Mathf.Clamp(_lastVelocity.magnitude * 2f, minThrowPower, maxThrowPower);
        
        Vector3 throwDirection = CalculateThrowDirection();
        _rigidbody.velocity = throwDirection * throwPower;

        StartCoroutine(WaitForFish());
    }

    private Vector3 CalculateThrowDirection()
    {
        // Get controller forward direction
        Vector3 controllerForward = _rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion controllerRotation) 
            ? controllerRotation * Vector3.forward 
            : Vector3.forward;

        // Blend between actual velocity and controller's forward direction for a more natural throw
        Vector3 throwDirection = Vector3.Lerp(_lastVelocity.normalized, controllerForward, 0.5f).normalized;
        
        return throwDirection;
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

    private void MoveToTarget() => transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);

    private IEnumerator WaitForFish()
    {
        yield return new WaitForSeconds(Random.Range(5f, 10f));
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