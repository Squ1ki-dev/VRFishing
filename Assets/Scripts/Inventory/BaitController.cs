using UnityEngine;

namespace Code.Gameplay.Inventory
{
    public class BaitController : MonoBehaviour
    {
        private Bait _bait;
        private BaitInputHandler _inputHandler;
        private BaitEffects _effects;

        private void Awake()
        {
            _bait = GetComponent<Bait>();
            _inputHandler = GetComponent<BaitInputHandler>();
            _effects = GetComponent<BaitEffects>();
        }

        private void Start() => _bait.ResetBait();
    }
}