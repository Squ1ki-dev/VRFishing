using System;
using System.Collections.Generic;
using Code.Logic.WaterSystem;
using UnityEngine;

namespace Code.Logic.FloatingSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bobber : MonoBehaviour
    {
        [SerializeField] private List<Floater> _floaters = new List<Floater>();
        [SerializeField] private float _waterLine = 0f;
        [SerializeField] private float _waterLineOffset = 0f;
        [SerializeField] private WaterLevelTrigger _waterLevelTrigger;
        [SerializeField] private WaterLevelDragHandler _waterLevelDragHandler;

        private Rigidbody rb;
        bool _underWater = false;

        private void Start() => rb = GetComponent<Rigidbody>();

        private void OnEnable()
        {
            _waterLevelTrigger.OnEnterWater += OnEnterWater;
            _waterLevelTrigger.OnExitWater += OnExitWater;
        }

        private void OnDisable()
        {
            _waterLevelTrigger.OnEnterWater -= OnEnterWater;
            _waterLevelTrigger.OnExitWater -= OnExitWater;
        }

        private void OnEnterWater()
        {
            _underWater = true;
            SetState(_underWater);
        }

        private void OnExitWater()
        {
            _underWater = false;
            SetState(_underWater);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _floaters.Count; i++)
            {
                _floaters[i].FloaterUpdate(rb, _waterLine * _waterLineOffset);
            }
        }

        private void SetState(bool isUnderWater) => _waterLevelDragHandler.SetUnderwaterState(isUnderWater);

        public void UpdateWaterLine(in float waterHeight) => _waterLine = waterHeight;
    }

}