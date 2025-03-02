using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Bait : MonoBehaviour
{
	public bool IsInWater { get; private set; }
	
	[Header("Events")] 
	[SerializeField] private UnityEvent onBaitInWater;
	[FormerlySerializedAs("onFishCatch")] [SerializeField] private UnityEvent<Transform> onFishBite;

	[Header("Effects")] 
	[SerializeField] private ParticleSystem splashEffect;
	[SerializeField] private ParticleSystem pullingEffect;
	
	[Header("Settings")] 
	[SerializeField] private Transform targetPoint;
	
	[Header("Settings")] 
	[SerializeField, Range(0f, 120f)] private float minWaitFishTime = 5f;
	[SerializeField, Range(0f, 120f)] private float maxWaitFishTime = 10f;
	[SerializeField, Range(5f, 25f)] private float minThrowForce = 15f;
	[SerializeField, Range(25f, 50f)] private float maxThrowForce = 50f;
	[SerializeField, Min(0)] private float moveSpeed = 3f;
	[SerializeField] private float yUnderWaterOffset = 0.1f;

	private Rigidbody _rigidbody;

	private Transform _defaultParent;
	private Vector3 _defaultPosition;

	private bool _isPulling;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		_defaultParent = transform.parent;
		_defaultPosition = transform.localPosition;
		
		Init();
	}
	
	private void FixedUpdate()
	{
		if (!_isPulling) return;

		MoveToTarget();
	}

	public void Init()
	{
		_isPulling = false;
		IsInWater = false;
		_rigidbody.isKinematic = true;
		pullingEffect.Stop();
		
		Invoke(nameof(DelayedParentAndPosition), 1f);
	}

	public void CastBait()
	{
		if (IsInWater) return;
		
		IsInWater = true;
		_rigidbody.isKinematic = false;
		transform.parent = null;

		Vector3 throwDirection = transform.forward;
		_rigidbody.AddForce(throwDirection * GetRandomThrowForce(), ForceMode.Impulse);

		StartCoroutine(WaitForFish());
	}
	
	public void StartPullTheFish()
	{
		if (!IsInWater) return;
		
		_isPulling = true;
		pullingEffect.Play();
	}

	public void StopPullTheFish()
	{
		if (!IsInWater) return;
		
		_isPulling = false;
		pullingEffect.Stop();
	}
	
	private void DelayedParentAndPosition()
	{
		transform.parent = _defaultParent;
		transform.localPosition = _defaultPosition;
	}
	
	private void MoveToTarget()
	{
		transform.position = Vector3.MoveTowards(transform.position, 
			targetPoint.position, moveSpeed * Time.fixedDeltaTime);
	}

	private IEnumerator WaitForFish()
	{
		yield return new WaitForSeconds(GetRandomWaitingTime());
		
		splashEffect.Play();
		onFishBite?.Invoke(transform);
	}

	private void WaterImmersion()
	{
		_rigidbody.isKinematic = true;
		SetYOffset();
		splashEffect.Play();
		onBaitInWater?.Invoke();
	}

	private void SetYOffset()
	{
		Vector3 currentPosition = transform.position;
		currentPosition.y -= yUnderWaterOffset;
		
		transform.position = currentPosition;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent(out LowPolyWater.LowPolyWater water)) return;

		WaterImmersion();
	}

	private float GetRandomThrowForce()
	{
		return Random.Range(minThrowForce, maxThrowForce);
	}

	private float GetRandomWaitingTime()
	{
		return Random.Range(minWaitFishTime, maxWaitFishTime);
	}
}