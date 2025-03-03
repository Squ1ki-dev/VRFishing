using System.Threading;
using Code.Logic.FloatingSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Logic.Fishing
{
    public class FishingSystem : MonoBehaviour
    {
        [SerializeField] private BiteSystem _biteSystem;
        [SerializeField] private HookSystem _hookSystem;
        [SerializeField] private JointWeightCalculator _weightCalculator;
        [SerializeField] private float _maximumRodWeight;
        [SerializeField] private float _maxWaitingSec;
        [SerializeField] private float _minWaitingSec;
        [SerializeField] private float _minGripStrength;
        [SerializeField] private FishCollector _fishCollector;
        [SerializeField] private Sinker sinker;
        [SerializeField] private AutoDropAfterTime _autoDropAfterTime;
        [SerializeField] private FishCollectionSO _fishCollection;
        [SerializeField] private float _fishWegtCoffmin = 0.5f;
        [SerializeField] private float _fishWegtCoffmax = 2f;

        private Fish _fish;
        private bool _isWeightUnderwater;
        private bool _isWaitingForBite;
        private Vector3 _previousRodPosition;
        private float _rodSpeed;
        private CancellationTokenSource _cancellationTokenSource;

        private void OnEnable()
        {
            _biteSystem.OnBite += HandleBite;
            _fishCollector.OnFishCollected += OnFishCollected;
            _hookSystem.OnRelease += HandleFishRelease;
            sinker.OnSinkerChangedState += SetWeightUnderwaterState;
            _autoDropAfterTime.OnFishInHand += OnFishHandTransition;
        }

        private void Start()
        {
            _previousRodPosition = transform.position;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Update()
        {
            if (_isWaitingForBite || !_fish)
                return;

            _rodSpeed = (transform.position - _previousRodPosition).magnitude / Time.deltaTime;
            _previousRodPosition = transform.position;
            float totalWeight = _weightCalculator.GetTotalWeight();

            if (_fish.IsUnderWater)
                totalWeight *= 0.5f;

            if (totalWeight > _maximumRodWeight)
                HandleFishRelease(_fish.IsUnderWater);
        }

        private async UniTaskVoid StartWaitingForBite()
        {
            _isWaitingForBite = true;
            await UniTask.Delay((int)(Random.Range(_minWaitingSec, _maxWaitingSec) * 1000), cancellationToken: _cancellationTokenSource.Token);
            if (!_isWaitingForBite) return;

            await BiteRoutine();
        }

        private void HandleBite(float gripStrength)
        {
            _isWaitingForBite = false;

            if (gripStrength < _minGripStrength)
            {
                _hookSystem.FalseBiting(gripStrength);
            }
            else
            {
                _fish = Instantiate(_fishCollection.GetRandomFish()).GetComponent<Fish>();
                _fish.weight *= Random.Range(_fishWegtCoffmin, _fishWegtCoffmax);
                Rigidbody rig = _fish.GetComponent<Rigidbody>();
                rig.mass = _fish.weight;
                _hookSystem.HookFish(gripStrength, _fish);
                _hookSystem.EatBite();
            }
        }

        private void HandleFishRelease(bool isFishUnderwater)
        {
            if (isFishUnderwater)
                Destroy(_fish);
            else
                ActFallFishWithDelay(1f, 2f).Forget();

            _hookSystem.UnHookFish();
        }

        private void SetWeightUnderwaterState(bool isUnderwater)
        {
            _isWeightUnderwater = isUnderwater;

            if (_isWeightUnderwater && !_hookSystem.HookIsEmpty)
                StartWaitingForBite().Forget();
            else
                _isWaitingForBite = false;
        }

        private void OnFishCollected(Fish fish)
        {
            Debug.Log($"Total weight caught: {fish.weight}");
            HandleCollectFish(fish);
        }

        private void HandleCollectFish(Fish fish)
        {
            Destroy(fish.gameObject);
        }

        private async UniTask ActFallFishWithDelay(float minDelay, float maxDelay)
        {
            try
            {
                await UniTask.Delay((int)(Random.Range(minDelay, maxDelay) * 1000), cancellationToken: _cancellationTokenSource.Token);
                if (_cancellationTokenSource.Token.IsCancellationRequested) return;

                Rigidbody rig = _fish.GetComponent<Rigidbody>();
                rig.isKinematic = false;
                rig.useGravity = true;
                _fish.transform.SetParent(transform);

                Destroy(_fish, 2);
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("Task canceled: ActFallFishWithDelay");
            }
        }

        private async UniTask BiteRoutine()
        {
            while (_isWaitingForBite)
            {
                await UniTask.Delay(Random.Range(2000, 5000), cancellationToken: _cancellationTokenSource.Token);
                if (!_isWaitingForBite) return;
                _biteSystem.CheckForBite();
            }
        }

        private void OnFishHandTransition()
        {
            if (_fish) {}
        }

        private void OnDisable()
        {
            _biteSystem.OnBite -= HandleBite;
            _fishCollector.OnFishCollected -= OnFishCollected;
            _hookSystem.OnRelease -= HandleFishRelease;
            sinker.OnSinkerChangedState -= SetWeightUnderwaterState;
            _autoDropAfterTime.OnFishInHand -= OnFishHandTransition;
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
