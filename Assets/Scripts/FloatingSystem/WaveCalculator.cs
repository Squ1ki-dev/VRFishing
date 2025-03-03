using UnityEngine;

namespace Code.Logic.FloatingSystem
{
    public class WaveCalculator : MonoBehaviour
    {
        [SerializeField] private float waveFrequency = 1.0f; 
        [SerializeField] private float waveAmplitude = 0.5f; 
        [SerializeField] private float waveSpeed = 1.0f; 

        public float GetWaveHeight(Vector3 position, float time)
        {
            float waveX = Mathf.Sin(position.x * waveFrequency + time * waveSpeed);
            float waveZ = Mathf.Cos(position.z * waveFrequency + time * waveSpeed);

            return (waveX + waveZ) * waveAmplitude;
        }
    }
}