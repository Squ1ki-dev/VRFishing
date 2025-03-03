using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Code.Services.Input
{
    public class InputHandler : IInputHandler
    {
        private InputDevice _leftController;
        private InputDevice _rightController;

        public void InitializeInputDevice()
        {
            var leftDevices = new List<InputDevice>();
            var rightDevices = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, leftDevices);
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, rightDevices);

            if (leftDevices.Count > 0)
                _leftController = leftDevices[0];

            if (rightDevices.Count > 0)
                _rightController = rightDevices[0];
        }

        public void ValidateInputDevice()
        {
            if (!_leftController.isValid || !_rightController.isValid)
                InitializeInputDevice();
        }

        public bool IsGripPressed() => 
            (_rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGrip) && rightGrip) ||
            (_leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool leftGrip) && leftGrip);

        public bool IsTriggerPressed() => 
            (_rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTrigger) && rightTrigger) ||
            (_leftController.TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTrigger) && leftTrigger);

        public Vector3 GetControllerVelocity()
        {
            if (_rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity))
                return rightVelocity;

            if (_leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity))
                return leftVelocity;

            return Vector3.zero;
        }

        public Vector3 GetControllerForward()
        {
            if (_rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRotation))
                return rightRotation * Vector3.forward;

            if (_leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRotation))
                return leftRotation * Vector3.forward;

            return Vector3.forward;
        }
    }
}
