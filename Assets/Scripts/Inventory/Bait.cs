using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Code.Services.Input;

namespace Code.Gameplay.Inventory
{
    public class Bait : MonoBehaviour
    {
        public bool IsInWater { get; private set; }

        [Header("Events")]
        [SerializeField] private UnityEvent onBaitEnteredWater;
        [SerializeField] private UnityEvent<Transform> onFishBite;

        [Header("Effects")]
        [SerializeField] private ParticleSystem splashEffect;
        [SerializeField] private ParticleSystem pullingEffect;

        [Header("Settings")]
        [SerializeField] private Transform targetPoint;
        [SerializeField, Min(0)] private float moveSpeed = 2f;
        [SerializeField] private float yUnderWaterOffset = 0.2f;
        [SerializeField] private float maxThrowPower = 10f;
        [SerializeField] private float minThrowPower = 2f;
        [SerializeField] private float angleInfluence = 0.5f;

        private Rigidbody _rigidbody;
        private Transform _defaultParent;
        private Vector3 _defaultPosition;
        private Vector3 _lastVelocity;
        private IInputHandler _inputHandler;

        private bool _isHolding;
        private bool _isPulling;
        private float _holdStartTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _inputHandler = new InputHandler();
        }

        private void Start()
        {
            CacheDefaults();
            _inputHandler.InitializeInputDevice();
        }

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
            if (_isPulling)
                MoveToTarget();
            else if (_isHolding)
                CaptureControllerVelocity();
            else
                ApplyAirResistance();
        }

        private void ApplyAirResistance() => _rigidbody.velocity *= 0.99f;

        private void StartHolding()
        {
            if (_isHolding) return;

            _isHolding = true;
            transform.SetParent(null);
            _holdStartTime = Time.time;
        }

        private void ThrowBait()
        {
            _isHolding = false;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = CalculateThrowDirection() * CalculateThrowPower();
            WaitForFish().Forget();
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

        public void ResetBait()
        {
            _isPulling = false;
            IsInWater = false;
            _rigidbody.isKinematic = true;
            pullingEffect.Stop();
            RestoreDefaultTransformAfterDelay().Forget();
        }

        private async UniTaskVoid RestoreDefaultTransformAfterDelay()
        {
            await UniTask.Delay(1000);
            RestoreDefaultTransform();
        }

        private void RestoreDefaultTransform()
        {
            transform.SetParent(_defaultParent);
            transform.localPosition = _defaultPosition;
        }

        private void MoveToTarget() =>
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);

        private async UniTaskVoid WaitForFish()
        {
            await UniTask.Delay(UnityEngine.Random.Range(5000, 10000));
            splashEffect.Play();
            onFishBite?.Invoke(transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out LowPolyWater.LowPolyWater _))
                EnterWater();
        }

        private void EnterWater()
        {
            _rigidbody.isKinematic = true;
            AdjustPositionForWater();
            splashEffect.Play();
            onBaitEnteredWater?.Invoke();
        }

        private void AdjustPositionForWater() =>
            transform.position += Vector3.down * yUnderWaterOffset;

        private void CacheDefaults()
        {
            _defaultParent = transform.parent;
            _defaultPosition = transform.localPosition;
            ResetBait();
        }
    }
}