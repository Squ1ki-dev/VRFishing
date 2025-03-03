using System;
using Code.Logic.WaterSystem;
using UnityEngine;

namespace Code.Logic.FloatingSystem
{
    public class Sinker : MonoBehaviour, IWaterLineUpdatable
    {
        [SerializeField] private WaterLevelDragHandler _waterLevelDragHandler;
        public event Action<bool> OnSinkerChangedState;

        public void UpdateWaterLineState(bool underWater)
        {
            _waterLevelDragHandler.SetUnderwaterState(underWater);
            OnSinkerChangedState?.Invoke(underWater); 
        }
    }
}
