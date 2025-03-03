using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Code.Logic
{
    public class AutoDropAfterTime : MonoBehaviour
    {
        [SerializeField] private float _maxHoldTime = 2.0f;
        [SerializeField] private Transform _dropParent;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private XRInteractionManager _interactionManager;
        private float _grabStartTime;

        public event Action OnFishInHand;

        private void Awake()
        {
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        private void OnEnable()
        {
            _grab.selectEntered.AddListener(OnGrab);
            _grab.selectExited.AddListener(OnDrop);
        }

        private void OnDisable()
        {
            _grab.selectEntered.RemoveListener(OnGrab);
            _grab.selectExited.RemoveListener(OnDrop);
        }

        private void Update()
        {
            if (_grab.isSelected && Time.time - _grabStartTime >= _maxHoldTime)
            {
                ForceDrop();
                OnFishInHand?.Invoke();
            }
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            _grabStartTime = Time.time;
        }

        private void OnDrop(SelectExitEventArgs args)
        {
            _grabStartTime = 0;
            transform.SetParent(_dropParent);
           
        }

        private void ForceDrop()
        {
            if (_grab.interactorsSelecting.Count > 0)
            {
                var interactor = _grab.interactorsSelecting[0];
                _interactionManager.SelectExit(interactor, _grab);
            }
        }
    }
}