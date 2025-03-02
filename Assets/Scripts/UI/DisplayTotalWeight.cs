using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayTotalWeight : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;

	public void SetTotalWeight(float weight)
	{
		_text.text = $"{weight} kg";
	}
}