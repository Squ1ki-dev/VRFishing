using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Code.Services.Input
{
    public class InputHandler : IInputHandler
    {
        private InputDevice _rightController;

        public void InitializeInputDevice()
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);
            if (devices.Count > 0)
                _rightController = devices[0];
        }

        public void ValidateInputDevice()
        {
            if (!_rightController.isValid)
                InitializeInputDevice();
        }

        public bool IsTriggerPressed() => 
            _rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool gripPressed) && gripPressed;

        public Vector3 GetControllerVelocity()
        {
            _rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
            return velocity;
        }

        public Vector3 GetControllerForward()
        {
            return _rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion controllerRotation)
                ? controllerRotation * Vector3.forward
                : Vector3.forward;
        }
    }
}