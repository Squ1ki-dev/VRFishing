using UnityEngine;
using System.Collections.Generic;

namespace Code.Logic.Fishing
{
    public class JointWeightCalculator : MonoBehaviour
    {
        public Rigidbody []  _rigsToCount; 
        
        public float _testTotalWeight;

        public float GetTotalWeight()
        {
            return CalculateWeight(_rigsToCount);
        }

        private float CalculateWeight(Rigidbody[] rigsToCount)
        {
            float totalWeight = 0f;

            foreach (Rigidbody rb in rigsToCount)
            {
                if (rb != null)
                    totalWeight += rb.mass;
            }

            _testTotalWeight = totalWeight;
            return totalWeight;
        }
    }
}