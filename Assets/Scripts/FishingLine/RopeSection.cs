using UnityEngine;

namespace Code.Logic.Curve
{
    public class RopeSection : MonoBehaviour
    {
        public Vector3 pos;
        public Vector3 vel;

        //To write RopeSection.zero
        public static readonly RopeSection zero = new RopeSection(Vector3.zero);

        public RopeSection(Vector3 pos)
        {
            this.pos = pos;

            this.vel = Vector3.zero;
        }
    }
}

