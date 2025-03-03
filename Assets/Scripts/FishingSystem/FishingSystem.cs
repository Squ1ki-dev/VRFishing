using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Code.Logic;
using Code.Logic.FloatingSystem;
using Unity.VisualScripting;
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
        [SerializeField] private AutoDropAfterTime  _autoDropAfterTime;
        [SerializeField] private FishCollectionSO  _fishCollection;
        [SerializeField] private float  _fishWegtCoffmin = 0.5f;
        [SerializeField] private float  _fishWegtCoffmax = 2f;

        private Fish _fish;
        private bool _isWeightUnderwater;
        private bool _isWaitingForBite;
        private Vector3 _previousRodPosition;
        private float _rodSpeed;

        private CancellationTokenSource _cancellationTokenSource;
        private Coroutine _biteCoroutine;
        private void OnEnable()
        {
            _biteSystem.OnBite += HandleBite;
            _fishCollector.OnFishCollected += OnFishColected;
            _hookSystem.OnRelease += HandleFishRelease;
            sinker.OnSinkerChangedState += SetWeigtUnderwaterState;
            _autoDropAfterTime.OnFishInHand += OnFishHandTransition;
        }

        private void Start()
        {
            _previousRodPosition = transform.position;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnFishColected(Fish fish)
        {
            Debug.Log($"Total weight catched _fish ??? {fish.weight}");
                HandleCollectFish(fish);
        }

        private void Update()
        {
            if (_isWaitingForBite || !_fish)
            {
                return;
            }

            _rodSpeed = (transform.position - _previousRodPosition).magnitude / Time.deltaTime;
            _previousRodPosition = transform.position;
            float totalWeight = _weightCalculator.GetTotalWeight();

            if (_fish.IsUnderWater)
                totalWeight *= 0.5f;

            if (totalWeight > _maximumRodWeight)
                HandleFishRelease(_fish.IsUnderWater);
        }

        private async void StartWaitingForBite()
        {
            _isWaitingForBite = true;
            await Task.Delay((int) (Random.Range(_minWaitingSec, _maxWaitingSec) * 1000));

            if (_biteCoroutine == null) 
                _biteCoroutine = StartCoroutine(BiteRoutine());
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
                Rigidbody rig =_fish.GetComponent<Rigidbody>();
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
                ActFallFishWithDelay(1f, 2f);
            
            _hookSystem.UnHookFish();
        }

        private void SetWeigtUnderwaterState(bool isUnderwater)
        {
            _isWeightUnderwater = isUnderwater;

            if (_isWeightUnderwater && !_hookSystem.HookIsEmpty)
                StartWaitingForBite();
            else
                _isWaitingForBite = false;
        }

        private void HandleCollectFish(Fish fish)
        {
            Destroy(fish.gameObject);
        }

        private async void ActFallFishWithDelay(float randomLowSec, float randomeUpSec)
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                return;
            try
            {
                await Task.Delay((int) (Random.Range(randomLowSec, randomeUpSec) * 1000),
                    _cancellationTokenSource.Token);
                if (_cancellationTokenSource.IsCancellationRequested) return;

                Rigidbody rig =_fish.GetComponent<Rigidbody>();
                rig.isKinematic = false;
                rig.useGravity = true;
                _fish.transform.SetParent(transform);
                
                Destroy(_fish, 2);
            }
            catch
            {
                Debug.Log("Task.Delay canceled");
            }
        }


        public void OnFishHandTransition()
        {
            if (_fish) {}
        }

        private IEnumerator BiteRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(2, 5));

                if (!_isWaitingForBite)
                {
                    _biteCoroutine = null;
                    yield break; 
                }

                _biteSystem.CheckForBite();
            }
        }
        
        private void OnDisable()
        {
            _biteSystem.OnBite -= HandleBite;
            _fishCollector.OnFishCollected -= OnFishColected;
            _hookSystem.OnRelease -= HandleFishRelease;
            sinker.OnSinkerChangedState -= SetWeigtUnderwaterState;
            _autoDropAfterTime.OnFishInHand -= OnFishHandTransition;
        }

        private void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            StopAllCoroutines();
            _biteCoroutine = null;
        }
    }
}
 