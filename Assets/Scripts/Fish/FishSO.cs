using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu (menuName = "FishSO/FishSO")]
public class FishSO : ScriptableObject
{
	public Sprite Sprite => sprite;
	
	[Header("Sprite")]
	[SerializeField] private Sprite sprite;
	
	[Header("Weight in KG")]
	[SerializeField, Range(0f, 100f)] private float minWeight;
	[SerializeField, Range(0f, 100f)] private float maxWeight;
	
	public float GetRandomWeight()
	{
		return MathF.Round(Random.Range(minWeight, maxWeight), 2);
	}
}