using UnityEngine;
using UnityEngine.XR;

public class HapticController : MonoBehaviour
{
	[Header("Settings SO")] 
	[SerializeField] private HapticSettingsSO baitInWater;
	[SerializeField] private HapticSettingsSO fishBite;
	[SerializeField] private HapticSettingsSO catchFish;
	
	private HapticSettingsSO _currentSettings;
	
	public void BaitInWater()
	{
		_currentSettings = baitInWater;
		TriggerHapticFeedback();
	}
	
	public void FishBite()
	{
		_currentSettings = fishBite;
		TriggerHapticFeedback();
	}
	
	public void CatchFish()
	{
		_currentSettings = catchFish;
		TriggerHapticFeedback();
	}

	private void TriggerHapticFeedback()
	{
		InputDevice device = InputDevices.GetDeviceAtXRNode(_currentSettings.Node);
		
		if (device.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse)
		{
			device.SendHapticImpulse(0, _currentSettings.Intensity, _currentSettings.Duration);
			Debug.Log($"Haptic with intensity: {_currentSettings.Intensity}, and duration: {_currentSettings.Duration}");
		}
		else
		{
			Debug.LogWarning($"The device does not support vibration: {device.name}");
		}
	}
}