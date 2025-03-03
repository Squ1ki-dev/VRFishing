using UnityEngine;

namespace Code.Logic
{
    public class Bite : MonoBehaviour
    {
        public Fish fish;
        public float minHookGrip = 0.3f;
        public float maxHookGrip = 0.9f;
        private bool isBiting;

        public delegate void OnHookAttempt(Fish hookedFish);

        public event OnHookAttempt Hooked;

        public void StartBite(Fish fish)
        {
            this.fish = fish;
            isBiting = true;
        }

        public void TryHook()
        {
            if (isBiting)
            {
                float hookGrip = Random.Range(minHookGrip, maxHookGrip);
                fish.hookGrip = hookGrip;
                Hooked?.Invoke(fish);
                isBiting = false;
            }
        }
    }
}