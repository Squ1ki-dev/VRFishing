using UnityEngine;
using Code.Services.Input;

namespace Code.Gameplay.Inventory
{
    public class BaitInputHandler : MonoBehaviour
    {
        private Bait _bait;
        private IInputHandler _inputHandler;

        private bool _isHolding;
        private float _holdStartTime;
        private Vector3 _lastVelocity;

        [Header("Throw Settings")]
        [SerializeField] private float maxThrowPower = 10f;
        [SerializeField] private float minThrowPower = 2f;
        [SerializeField] private float angleInfluence = 0.5f;

        private void Awake()
        {
            _bait = GetComponent<Bait>();
            _inputHandler = new InputHandler();
        }

        private void Start() => _inputHandler.InitializeInputDevice();

        private void Update()
        {
            _inputHandler.ValidateInputDevice();

            if (_inputHandler.IsTriggerPressed())
                StartHolding();
            else if (_isHolding)
                ThrowBait();
        }

        private void FixedUpdate()
        {
            if (_isHolding)
                CaptureControllerVelocity();
        }

        private void StartHolding()
        {
            if (_isHolding) return;

            _isHolding = true;
            _bait.transform.SetParent(null);
            _holdStartTime = Time.time;
        }

        private void ThrowBait()
        {
            _isHolding = false;
            _bait.Throw(CalculateThrowDirection(), CalculateThrowPower());
        }

        private float CalculateThrowPower() =>
            Mathf.Clamp(_lastVelocity.magnitude * 2f, minThrowPower, maxThrowPower);

        private Vector3 CalculateThrowDirection()
        {
            Vector3 controllerForward = _inputHandler.GetControllerForward();
            return Vector3.Lerp(_lastVelocity.normalized, controllerForward, angleInfluence).normalized;
        }

        private void CaptureControllerVelocity() =>
            _lastVelocity = _inputHandler.GetControllerVelocity();
    }
}