using Code.Logic.WaterSystem;
using UnityEngine;

namespace Code.Logic
{
    public class Fish : MonoBehaviour, ITransitable
    {
        [SerializeField] private WaterLevelTrigger _waterLevelTrigger;
        public FishType type;
        public float weight;
        public float hookGrip;

        public bool IsUnderWater
        {
            get;
            set;
        }

        private void OnEnable()
        {
            _waterLevelTrigger.OnEnterWater += OnEnterWater;
            _waterLevelTrigger.OnExitWater += OnExitWater;
        }

        private void OnEnterWater() => IsUnderWater = true;
        private void OnExitWater() => IsUnderWater = false;

        public bool IsInTransition { get; set; }


        public void BeginTransition() => IsInTransition = true;

        public void EndTransition()
        {
            IsInTransition = false;
        }
    }

    public enum FishType
    {
        Carp,
        Perch,
        Pike,
        Roach
    }
}