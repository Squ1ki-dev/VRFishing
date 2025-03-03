using System;
using Code.Logic.WaterSystem;
using UnityEngine;

namespace Code.Logic
{
    public class FishBehavior : MonoBehaviour, IWaterLineUpdatable
    {
        [SerializeField] public Rigidbody _hookRigidbody;
        [SerializeField] private Fish _currentFish;
        [SerializeField] private bool _isHooked = true;
        [SerializeField] private float _currentGrip;
        [SerializeField] private Vector3 _pullForce;

        public bool IsUnderwater { get; set; }

        private void Start() => HookFish(_currentFish);

        private void FixedUpdate()
        {
            if (_isHooked && IsUnderwater)
            {
                Debug.Log($"Is Underwater and hooked");
                _hookRigidbody.AddForce(_pullForce, ForceMode.Acceleration);

                _currentGrip = Mathf.Clamp01(_currentGrip);
                if (_currentGrip <= 0)
                    ReleaseFish();
            }
        }

        public void HookFish(Fish fish)
        {
            _isHooked = true;
            _hookRigidbody.mass = fish.weight;
        }

        public void ApplyPull(Vector3 force)
        {
            if (_isHooked)
                _pullForce = force;
        }

        public void ReleaseFish()
        {
            _isHooked = false;
            _pullForce = Vector3.zero;
            _currentFish = null;
            _hookRigidbody.mass = 0.001f;
        }

        public void UpdateWaterLineState(bool isUnderwater) => IsUnderwater = isUnderwater;
    }
}