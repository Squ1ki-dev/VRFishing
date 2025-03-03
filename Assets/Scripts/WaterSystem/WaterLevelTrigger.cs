using System;
using UnityEngine;

namespace Code.Logic.WaterSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class WaterLevelTrigger : MonoBehaviour
    {
        public event Action OnEnterWater;
        public event Action OnExitWater;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.enabled = false;
            Invoke("EnableAudioSource", 3f);
        }
        private void EnableAudioSource()
        {
            _audioSource.enabled = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out WaterController water))
            {
                WaterInteractable interactable = GetComponentInParent<WaterInteractable>();
                if (interactable != null)
                {
                    water.SetInteractorTransform(this.transform);
                }
                
                OnEnterWater?.Invoke(); 
                if (_audioSource != null && !_audioSource.isPlaying && _audioSource.enabled)
                {
                    _audioSource.Play();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out WaterController water))
            {
                WaterInteractable interactable = GetComponentInParent<WaterInteractable>();

                if (interactable!= null)
                {
                    water.SetInteractorTransform(null);
                    if (_audioSource != null && !_audioSource.isPlaying)
                    {
                        _audioSource.Play();
                    }
                }
                
                OnExitWater?.Invoke();
            }
        }
    }
}