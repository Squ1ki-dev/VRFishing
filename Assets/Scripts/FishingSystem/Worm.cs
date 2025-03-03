using UnityEngine;

namespace Code.Logic.Fishing
{
    public class Worm : MonoBehaviour, ITransitable
    {
        public bool IsInTransition { get; set; }

        public void BeginTransition(){}

        public void EndTransition(){}
    }
}
