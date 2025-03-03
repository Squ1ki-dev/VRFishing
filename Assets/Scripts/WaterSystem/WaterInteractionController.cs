using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Code.Logic.FloatingSystem;

namespace Code.Logic.WaterSystem
{
    public class WaterInteractionController : MonoBehaviour
    {
        private List<Bobber> _activeBobbers = new List<Bobber>();
        private List<IWaterLineUpdatable> _waterLineUpdatables = new List<IWaterLineUpdatable>();

        public Vector3[] GetBobberPositions()
        {
            return _activeBobbers
                .Select(b => new Vector3(b.transform.position.x, b.transform.position.y, b.transform.position.z))
                .ToArray();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Bobber>(out Bobber bobber))
                _activeBobbers.Add(bobber);
        
            if (other.gameObject.TryGetComponent<IWaterLineUpdatable>(out IWaterLineUpdatable waterLineUpdatable))
            {
                _waterLineUpdatables.Add(waterLineUpdatable);
                waterLineUpdatable.UpdateWaterLineState(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<Bobber>(out Bobber bobber))
                _activeBobbers.Remove(bobber);
        
            if (other.gameObject.TryGetComponent<IWaterLineUpdatable>(out IWaterLineUpdatable waterLineUpdatable))
            {
                _waterLineUpdatables.Remove(waterLineUpdatable);
                waterLineUpdatable.UpdateWaterLineState(false);
            }
        }
        
        public void UpdateBobberHeight(int index, float height)
        {
            if (index < _activeBobbers.Count)
                _activeBobbers[index].UpdateWaterLine(height);
        }
    }
}