using UnityEngine;
using UnityEngine.Events;

public class CatchTrigger : MonoBehaviour
{
	[Header("Events")]
	[SerializeField] private UnityEvent<FishBehaviour> onFishCatch;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent(out FishBehaviour fish)) return;
		
		Debug.Log($"You caught a fish weighing {fish.Weight} kg.");
		
		Destroy(fish.gameObject);
		onFishCatch?.Invoke(fish);
	}
}