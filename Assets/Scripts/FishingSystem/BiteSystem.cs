using UnityEngine;

namespace Code.Logic.Fishing
{
    public class BiteSystem : MonoBehaviour
    {
        [SerializeField] private float _minThresholdgripStrength;
        [SerializeField] private float _maxThresholdgripStrength;
        [SerializeField] private float _biteChance;
        
        public delegate void BiteHandler(float gripStrength);
        public event BiteHandler OnBite;

        public void CheckForBite()
        {
            float randomVal = Random.value;

            if (randomVal > 1-_biteChance)
            {
                float gripStrength = Random.Range(_minThresholdgripStrength, _maxThresholdgripStrength);
                OnBite?.Invoke(gripStrength);
            }
        }
    }
}
