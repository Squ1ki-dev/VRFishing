using UnityEngine;

namespace Code.Logic.Curve
{
    public struct Segment
    {
        public Vector3 positionCurrent;
        public Vector3 positionLast;

        public Segment(Vector3 pos)
        {
            positionCurrent = pos;
            positionLast = pos;
        }
    }
}
