using Code.Logic;
using DG.Tweening;
using UnityEngine;
using Code.Logic.Configs;
using Random = UnityEngine.Random;

namespace Code.Logic.Fishing
{
    public class HookSystem : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private HookFishFoodContainerTrigger _hookFishFoodContainerTrigger; 
        [SerializeField] private Transform _hookContainer; 
        [SerializeField] private HookConfig _hookConfig; // Reference to SO
        
        public delegate void ReleaseHandler(bool isUnderwater);
        public event ReleaseHandler OnRelease;

        private Vector3 _pullForce;
        private Fish _hookedFish;
        private float _currentGrip;
        private bool _isFishHooked;

        public bool HookIsEmpty => _hookFishFoodContainerTrigger.HookIsEmpty;

        private void FixedUpdate()
        {
            if (_isFishHooked)
            {
                if (_hookedFish.IsUnderWater)
                    _rigidbody.AddForce(_pullForce * 5, ForceMode.Acceleration);
                
                _currentGrip = Mathf.Clamp01(_currentGrip - Time.deltaTime * _pullForce.magnitude * _hookConfig.speedLooseGrip);

                if (_currentGrip <= 0)
                    OnRelease?.Invoke(_hookedFish.IsUnderWater);
            }
        }

        public void HookFish(float gripStrength, Fish fish)
        {
            _hookedFish = fish;
            _hookedFish.transform.position = _hookContainer.position;
            _hookedFish.transform.rotation = _hookContainer.rotation;
            _hookedFish.transform.SetParent(_hookContainer);
            _isFishHooked = true;
            _hookFishFoodContainerTrigger.CleanContainerFromBite();
            _hookFishFoodContainerTrigger.SetupCurrentHookedFish(_hookedFish.gameObject);
            _currentGrip = gripStrength;

            ApplyPull(new Vector3(Random.Range(-_hookConfig.pullForceMax, _hookConfig.pullForceMax), -1f, Random.Range(-_hookConfig.pullForceMax, _hookConfig.pullForceMax)) * gripStrength);
        }
        
        public void UnHookFish()
        {
            _isFishHooked = false;
            _currentGrip = 0;
            _rigidbody.mass = 0.01f;
            ApplyPull(Vector3.zero);
        }

        public void ApplyPull(Vector3 force)
        {
            if (_isFishHooked)
                _pullForce = force ;
        }
        
        public void FalseBiting(float gripStrength)
        {
            float force = _hookConfig.biteForce * gripStrength;
            int biteCount = Random.Range(_hookConfig.biteCountRangeMin, _hookConfig.biteCountRangeMax);
            float biteDelay;
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < biteCount; i++)
            {
                sequence.AppendCallback(() =>
                {
                    Vector3 biteDirection = -(Vector3.up + Random.insideUnitSphere * 0.2f).normalized;
                    _rigidbody.velocity = Vector3.zero; 
                    _rigidbody.AddForce(biteDirection * force , ForceMode.Impulse);
                });
                
                Debug.Log($"False Bite");
                biteDelay = Random.Range(_hookConfig.biteDelayMin, _hookConfig.biteDelayMax);
                sequence.AppendInterval(biteDelay);
            }
            CheckHookAfterBite();
            sequence.Play();
        }

        public void EatBite()
        {
            _hookFishFoodContainerTrigger.CleanContainerFromBite();
        }

        private void CheckHookAfterBite()
        {
            float chance = Random.value;
            if (chance > 1 - _hookConfig.biteEatenChance)
                EatBite();
        }
    }
}
