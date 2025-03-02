using UnityEngine;
using UnityEngine.Events;

public class CatchTrigger : MonoBehaviour
{
	[Header("Events")]
	[SerializeField] private UnityEvent<FishBehavior> onFishCatch;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent(out FishBehavior fish)) return;
		
		Debug.Log($"You caught a fish weighing {fish.Weight} kg.");
		
		Destroy(fish.gameObject);
		onFishCatch?.Invoke(fish);
	}
}