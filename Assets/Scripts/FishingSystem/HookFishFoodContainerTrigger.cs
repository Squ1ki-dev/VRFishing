using System;
using UnityEngine;

namespace Code.Logic.Fishing
{
    public class HookFishFoodContainerTrigger : MonoBehaviour
    {
        [SerializeField] private Transform _hookContainer;

        private AudioSource _audioSource;
        private GameObject _currentFishFood;
        private GameObject _currentHookedFish;

        public event Action OnWormHooked;
        public bool HookIsEmpty => _currentFishFood == null;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void CleanContainerFromBite()
        {
            if (_currentFishFood == null) return;
            
            Destroy(_currentFishFood);
            _currentFishFood = null;
        }

        public void SetupCurrentHookedFish(GameObject fish) => _currentHookedFish = fish;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out HandTransition handTransition)) return;

            if (handTransition.IsContainerFull && HookIsEmpty)
                HandleWormHooking(handTransition);
            else if (!handTransition.IsContainerFull && _currentHookedFish != null)
                HandleFishToHand(handTransition);

            PlayAudio();
        }

        private void HandleWormHooking(HandTransition handTransition)
        {
            handTransition.EndTransition(out ITransitable wormInstance);
            if (wormInstance is not MonoBehaviour mono) return;
            
            mono.transform.SetParent(_hookContainer);
            mono.transform.localPosition = Vector3.zero; // Reset local position to align with the hook
            _currentFishFood = mono.gameObject;

            OnWormHooked?.Invoke();
        }

        private void HandleFishToHand(HandTransition handTransition)
        {
            if (_currentHookedFish.TryGetComponent(out ITransitable fishInstance))
            {
                handTransition.BeginTransition(fishInstance);
                Debug.Log("Fish in hand");
            }
        }

        private void PlayAudio()
        {
            if (_audioSource == null || _audioSource.isPlaying) return;
            _audioSource.Play();
        }
    }
}
