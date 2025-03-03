using System;
using System.Collections.Generic;
using System.Linq;
using Code.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Logic.Fishing
{
    public class FishCollector : MonoBehaviour
    {
        public event Action<Fish> OnFishCollected; 
        
        private AudioSource _audioSource;
        private readonly List<float> _collectedFishWeights = new List<float>();
        [SerializeField] private Text _totalWeightText;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out HandTransition handTransition))
            {
                if (handTransition.GetITransitable() is Fish fish)
                {
                    handTransition.EndTransition(out ITransitable fishInstance);
                    fish = fishInstance as Fish;

                    if (fish == null) return;

                    AddFish(fish);
                }
            }
        }

        private void AddFish(Fish fish)
        {
            _collectedFishWeights.Add(fish.weight);
            OnFishCollected?.Invoke(fish);

            float totalWeight = _collectedFishWeights.Sum();
            _totalWeightText.text = totalWeight.ToString("F2"); // Formatting for readability
            
            Debug.Log($"Caught Fish weight: {totalWeight}");
            
            PlayAudio();
        }

        private void PlayAudio()
        {
            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();
        }
    }
}
