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
            if (_currentFishFood != null)
            {
                Destroy(_currentFishFood);
                _currentFishFood = null;
            }
        }

        public void SetupCurrentHookedFish(GameObject fish)
        {
            _currentHookedFish = fish;
        }

        private void OnTriggerEnter(Collider other)
        {
            HandTransition handTransition = other.GetComponent<HandTransition>();

            if (handTransition == null)
                return;

            //Worm connection
            if (handTransition.IsContainerFull && HookIsEmpty)
            {
                ITransitable wormInstance;
                handTransition.EndTransition(out wormInstance);
                if (wormInstance != null)
                {
                    if (wormInstance is MonoBehaviour mono)
                    {
                        mono.transform.position = _hookContainer.position;
                        mono.transform.SetParent(_hookContainer);
                        _currentFishFood = mono.gameObject;
                        OnWormHooked?.Invoke();
                    }
                }
            }

            //Fish to hand 
            if (!handTransition.IsContainerFull && _currentHookedFish != null)
            {
                ITransitable fishInstance = _currentHookedFish.GetComponent<ITransitable>();
                handTransition.BeginTransition(fishInstance);
                Debug.Log($"fish in hand");

            }
            
            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();
        }
    }
}
