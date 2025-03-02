using UnityEngine;
using UnityEngine.XR;

[CreateAssetMenu (menuName = "HapticSettingsSO/HapticSettingsSO")]
public class HapticSettingsSO : ScriptableObject
{
	public XRNode Node => node;

	public float Intensity => intensity;
	public float Duration => duration;
	
	[SerializeField] private XRNode node = XRNode.RightHand;
	
	[SerializeField, Range(0f, 1f)] private float intensity = 0.1f;
	[SerializeField, Range(0f, 1f)] private float duration = 0.1f;
}