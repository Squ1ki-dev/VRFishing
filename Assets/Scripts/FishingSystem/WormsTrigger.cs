using Code.Services.Input;
using UnityEngine;
using Code;

namespace Code.Logic.Fishing
{
    public class WormsTrigger : MonoBehaviour
    {
        [SerializeField] private Worm _wormPrefab;
        private AudioSource _audioSource;
        private IInputHandler _inputHandler;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _inputHandler = new InputHandler();
            Debug.Log("[WormsTrigger] Initialized InputHandler.");
        }

        private void Update() => _inputHandler.ValidateInputDevice();


        private void OnTriggerEnter(Collider other)
        {
            HandTransition handTransition = other.GetComponent<HandTransition>();

            if (handTransition != null)
            {

                if (!handTransition.IsContainerFull && _inputHandler.IsGripPressed())
                {

                    Worm wormInstance = Instantiate(_wormPrefab);
                    ITransitable worm = wormInstance;
                    handTransition.BeginTransition(worm);
                    
                    if (_audioSource != null && !_audioSource.isPlaying)
                    {
                        _audioSource.Play();
                        Debug.Log($"[WormsTrigger] Played sound.");
                    }
                }
            }
        }
    }
}
