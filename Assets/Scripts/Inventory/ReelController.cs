using UnityEngine;
using UnityEngine.Events;

public class ReelController : MonoBehaviour
{
	[Header("Events")] 
	[SerializeField] private UnityEvent onStartReeling;
	[SerializeField] private UnityEvent onStopReeling;
	
	[SerializeField] private Bait bait;
	
	[Header("Settings")]
	[SerializeField, Min(0)] private float rotationSpeed = 360f;
	
	private bool _isReeling;

	private void Update()
	{
		if (_isReeling)
		{
			RotateReel();
		}
	}

	public void StartReeling()
	{
		if (!bait.IsInWater) return;
		
		Debug.Log("Start reeling");
		
		_isReeling = true;
		onStartReeling?.Invoke();
	}

	public void StopReeling()
	{
		Debug.Log("Stop reeling");
		
		_isReeling = false;
		onStopReeling?.Invoke();
	}

	private void RotateReel()
	{
		transform.Rotate(Vector3.right * (rotationSpeed * Time.deltaTime));
	}
}