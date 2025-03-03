using UnityEngine;

namespace Code.Logic.WaterSystem
{
    public class WaterLevelDragHandler : MonoBehaviour
    {
        [SerializeField] private Rigidbody targetRigidbody;
        [SerializeField] private float defaultDrag = 0.5f;
        [SerializeField] private float defaultAngularDrag = 0.05f;
        [SerializeField] private float underwaterDrag = 2.0f;
        [SerializeField] private float underwaterAngularDrag = 1.0f;

        public void SetUnderwaterState(bool isUnderWater)
        {
            if (targetRigidbody == null)
            {
                Debug.LogError("Target Rigidbody is not assigned!");
                return;
            }

            if (isUnderWater)
            {
                targetRigidbody.drag = underwaterDrag;
                targetRigidbody.angularDrag = underwaterAngularDrag;
            }
            else
            {
                targetRigidbody.drag = defaultDrag;
                targetRigidbody.angularDrag = defaultAngularDrag;
            }
        }
    }
}