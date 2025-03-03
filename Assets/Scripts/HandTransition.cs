using UnityEngine;

namespace Code
{
    public class HandTransition : MonoBehaviour
    {
        private Transform _handPointTransform;
        private ITransitable _currentTransitable;
        private bool _isContainerFull = false;

        public bool IsContainerFull => _isContainerFull;

        private void Start()
        {
            _handPointTransform = transform;
        }

        public void BeginTransition(ITransitable transitable)
        {
            _currentTransitable = transitable;

            if (_currentTransitable is MonoBehaviour mono)
            {
                mono.transform.position = _handPointTransform.position;
                mono.transform.SetParent(_handPointTransform);
                _isContainerFull = true;
            }
            else
            {
                Debug.LogError("BeginTransition: Переданный объект не является MonoBehaviour!");
            }
        }
        
        public void EndTransition(out ITransitable transitable)
        {
            transitable = _currentTransitable; 
            _currentTransitable = null;  
            _isContainerFull = false;
        }

        public ITransitable GetITransitable()
        {
            return _currentTransitable;
        }
    }
}