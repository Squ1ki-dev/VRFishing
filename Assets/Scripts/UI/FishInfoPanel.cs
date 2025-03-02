using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishInfoPanel : MonoBehaviour
{
	[Header("Links")]
	[SerializeField] private Image fishImage;
	[SerializeField] private TextMeshProUGUI congratulationsText;

	[Header("Settings")] 
	[SerializeField, Min(0f)] private float displayDuration = 3f;
	
	public void SetFishInfo(FishBehavior fish)
	{
		gameObject.SetActive(true);
		
		SetSprite(fish.Sprite);
		SetWeightText(fish.Weight);

		StartCoroutine(Deactivate());
	}

	private void SetSprite(Sprite sprite)
	{
		fishImage.sprite = sprite;
	}

	private void SetWeightText(float weight)
	{
		congratulationsText.text = $"You caught a fish weighing {weight} kg.";
	}

	private IEnumerator Deactivate()
	{
		yield return new WaitForSeconds(displayDuration);
		
		gameObject.SetActive(false);
	}
}