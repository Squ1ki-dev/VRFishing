using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	[Header("Events")]
	[SerializeField] private UnityEvent<float> onTotalWeightChanged;
	
	private float _currentWeight;

	public void AddFish(FishBehavior fish)
	{
		_currentWeight += fish.Weight;
		
		onTotalWeightChanged?.Invoke(_currentWeight);
	}
}