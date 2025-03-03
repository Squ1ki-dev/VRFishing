using UnityEngine;

namespace Code.Logic.FloatingSystem
{
    public class Floater : MonoBehaviour
    {
        private bool _underWater;
        [SerializeField] private float _floatingPower = 20f;
        
        public bool FloaterUpdate(Rigidbody rb, float waterLine)
        {
            float difference = transform.position.y - waterLine;

            if (difference <= 0)
            {
                rb.AddForceAtPosition(Vector3.up * _floatingPower * Mathf.Abs(difference), transform.position,
                    ForceMode.Force);

                if (!_underWater)
                    _underWater = true;

            }
            else if (_underWater)
                _underWater = false;

            return _underWater;
        }
    }
}