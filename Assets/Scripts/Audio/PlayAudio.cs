using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudio : MonoBehaviour
{
    [Header("Clip")]
    [Tooltip("The sound that is played")]
    [SerializeField] private AudioClip sound;

    [Header("Settings")]
    [Tooltip("The volume of the sound")]
    [SerializeField] private float volume = 1.0f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneTime()
    {
        _audioSource.loop = false;
        _audioSource.PlayOneShot(sound, volume);
    }
    
    public void PlayLoop()
    {
        _audioSource.loop = true;
        _audioSource.PlayOneShot(sound, volume);
    }
    
    public void StopPlaying()
    {
        _audioSource.Stop();
    }
}