using System.Collections.Generic;
using UnityEngine;

namespace Code.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Background")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _ambientSource;

        [Header("SFX")]
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private AudioClip[] _fxClips;
        private Dictionary<string, AudioClip> _fxDictionary;
        
        private AudioSource[] _sfxPool;
        private int _poolIndex = 0;

        private void Awake()
        {
            Instance = this;

            _sfxPool = new AudioSource[_poolSize];
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject obj = new GameObject("SFX_" + i);
                obj.transform.parent = transform;
                _sfxPool[i] = obj.AddComponent<AudioSource>();
            }

            LoadFXDictionary();
        }

        private void LoadFXDictionary()
        {
            _fxDictionary = new Dictionary<string, AudioClip>();
            foreach (var clip in _fxClips)
            {
                _fxDictionary[clip.name] = clip;
            }
        }

        public void PlayMusic(AudioClip clip, float volume = 1.0f)
        {
            if (_musicSource.clip == clip) return;
            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.volume = volume;
            _musicSource.Play();
        }

        public void StopMusic() => _musicSource.Stop();

        public void PlayAmbient(AudioClip clip, float volume = 1.0f)
        {
            if (_ambientSource.clip == clip) return;
            _ambientSource.clip = clip;
            _ambientSource.loop = true;
            _ambientSource.volume = volume;
            _ambientSource.Play();
        }

        public void StopAmbient() => _ambientSource.Stop();

        public void PlaySFX(AudioClip clip, float volume = 1.0f)
        {
            AudioSource source = _sfxPool[_poolIndex];
            source.clip = clip;
            source.volume = volume;
            source.Play();

            _poolIndex = (_poolIndex + 1) % _poolSize;
        }

        public void PlaySFX(string soundName, float volume = 1.0f)
        {
            if (_fxDictionary.TryGetValue(soundName, out var clip))
                PlaySFX(clip, volume);
            else
                Debug.LogWarning($"[SoundManager] Sound '{soundName}' not exist!");
        }
    }

}
