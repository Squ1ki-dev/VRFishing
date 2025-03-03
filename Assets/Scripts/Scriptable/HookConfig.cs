using UnityEngine;

namespace Code.Logic.Configs
{
    [CreateAssetMenu(fileName = "HookConfig", menuName = "Hook Config")]
    public class HookConfig : ScriptableObject
    {
        [Header("Bite Settings")]
        public int biteCountRangeMin = 1; 
        public int biteCountRangeMax = 3; 
        public float biteForce = 2.5f; 
        public float biteDuration = 0.1f;
        public float biteEatenChance = 0.2f; 
        public float biteDelayMin = 0.2f; 
        public float biteDelayMax = 2f; 

        [Header("Hook Settings")]
        public float pullForceMax;
        public float speedLooseGrip = 0.005f;
    }
}