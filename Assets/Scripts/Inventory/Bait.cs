using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Gameplay.Inventory
{
    public class Bait : MonoBehaviour
    {
        public bool IsInWater { get; private set; }

        [Header("Events")]
        public UnityEvent onBaitEnteredWater;
        public UnityEvent<Transform> onFishBite;

        [Header("Settings")]
        [SerializeField] private Transform targetPoint;
        [SerializeField, Min(0)] private float moveSpeed = 2f;
        [SerializeField] private float yUnderWaterOffset = 0.2f;

        [SerializeField] private ReelController _reelController;

        private Rigidbody _rigidbody;
        private Transform _defaultParent;
        private Vector3 _defaultPosition;

        private bool _isPulling;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            CacheDefaults();
        }

        private void OnEnable()
        {
            if (_reelController != null)
                _reelController.OnReelRotating += ResetBait;
        }

        private void OnDisable()
        {
            if (_reelController != null)
                _reelController.OnReelRotating -= ResetBait;
        }

        public void Throw(Vector3 direction, float power)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = direction * power;
            WaitForFish().Forget();
        }

        public void ResetBait()
        {
            _isPulling = false;
            IsInWater = false;
            _rigidbody.isKinematic = true;
            RestoreDefaultTransformAfterDelay().Forget();
        }

        private async UniTaskVoid RestoreDefaultTransformAfterDelay()
        {
            await UniTask.Delay(1000);
            RestoreDefaultTransform();
        }

        private void RestoreDefaultTransform()
        {
            transform.SetParent(_defaultParent);
            transform.localPosition = _defaultPosition;
        }

        private async UniTaskVoid WaitForFish()
        {
            await UniTask.Delay(UnityEngine.Random.Range(5000, 10000));
            onFishBite?.Invoke(transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out LowPolyWater.LowPolyWater _))
                EnterWater();
        }

        private void EnterWater()
        {
            _rigidbody.isKinematic = true;
            AdjustPositionForWater();
            onBaitEnteredWater?.Invoke();
        }

        private void AdjustPositionForWater() =>
            transform.position += Vector3.down * yUnderWaterOffset;

        private void CacheDefaults()
        {
            _defaultParent = transform.parent;
            _defaultPosition = transform.localPosition;
            ResetBait();
        }
    }
}