using UnityEngine;
using UnityEngine.Events;

public class FishBehavior : MonoBehaviour
{
	public Sprite Sprite => fishSO.Sprite;
	
	public float Weight { get; private set; }
	
	[Header("Events")]
	[SerializeField] private UnityEvent onFishCatch;
	
	[Header("Settings")] 
	[SerializeField] private FishSO fishSO;

	private void Start()
	{
		Init();
	}

	public void Catch(Transform bait)
	{
		transform.parent = bait;
		transform.localPosition = Vector3.zero;
		
		Debug.Log("Fish on the hook");
	}

	private void Init()
	{
		Weight = fishSO.GetRandomWeight();
		onFishCatch?.Invoke();
	}
}