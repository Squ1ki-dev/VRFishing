using UnityEngine;

namespace Code.Gameplay.Inventory
{
    public class BaitEffects : MonoBehaviour
    {
        [Header("Effects")]
        [SerializeField] private ParticleSystem splashEffect;
        [SerializeField] private ParticleSystem pullingEffect;

        private Bait _bait;

        private void Awake() => _bait = GetComponent<Bait>();

        private void OnEnable()
        {
            _bait.onBaitEnteredWater.AddListener(PlaySplashEffect);
            _bait.onFishBite.AddListener(PlayPullingEffect);
        }

        private void OnDisable()
        {
            _bait.onBaitEnteredWater.RemoveListener(PlaySplashEffect);
            _bait.onFishBite.RemoveListener(PlayPullingEffect);
        }

        private void PlaySplashEffect() => splashEffect.Play();
        private void PlayPullingEffect(Transform transform) => pullingEffect.Play();
    }
}