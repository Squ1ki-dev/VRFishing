using System;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Code.Gameplay.Inventory
{
    public class ReelController : MonoBehaviour
    {
        [Header("XR Knob Reference")]
        [SerializeField] private XRSimpleInteractable reelKnob;
        [SerializeField] private XRNode controllerNode = XRNode.LeftHand;

        [Header("Settings")]
        [SerializeField] private float maxRotationSpeed = 720f;

        public event Action OnReelRotating;

        private InputDevice _rightController;
        private bool _isGrabbed;
        private float _currentReelSpeed;

        private void Start() => _rightController = InputDevices.GetDeviceAtXRNode(controllerNode);

        private void OnEnable()
        {
            reelKnob.selectEntered.AddListener(OnGrab);
            reelKnob.selectExited.AddListener(OnRelease);
        }

        private void OnDisable()
        {
            reelKnob.selectEntered.RemoveListener(OnGrab);
            reelKnob.selectExited.RemoveListener(OnRelease);
        }

        private void Update()
        {
            _rightController = InputDevices.GetDeviceAtXRNode(controllerNode);

            if (_rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed) && gripPressed && _isGrabbed)
            {
                AdjustReelSpeed();
                RotateReel();
                OnReelRotating?.Invoke();
            }
            else
                _currentReelSpeed = 0f;
        }

        private void OnGrab(SelectEnterEventArgs args) => _isGrabbed = true;

        private void OnRelease(SelectExitEventArgs args)
        {
            _isGrabbed = false;
            _currentReelSpeed = 0f;
        }

        private void AdjustReelSpeed() => _currentReelSpeed = 0.5f * maxRotationSpeed;

        private void RotateReel() => transform.Rotate(Vector3.right * (-_currentReelSpeed * Time.deltaTime));
    }
}