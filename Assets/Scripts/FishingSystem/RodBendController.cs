using UnityEngine;
using System.Collections.Generic;

namespace Code.Logic.Fishing
{
    public class RodBendController : MonoBehaviour
    {
        public Transform rodTip;
        public Transform handle;
        public Transform fishingLineEnd;

        [Range(0f, 1f)] public float bendStrength = 0.5f;
        public int curveResolution = 20;

        private LineRenderer lineRenderer;
        private List<Vector3> curvePoints = new List<Vector3>();

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = curveResolution;
        }

        private void Update()
        {
            UpdateRodBend();
        }

        private void UpdateRodBend()
        {
            curvePoints.Clear();

            Vector3 start = handle.position;
            Vector3 end = fishingLineEnd.position;
            Vector3 control = rodTip.position + (end - rodTip.position) * bendStrength;

            for (int i = 0; i < curveResolution; i++)
            {
                float t = i / (float) (curveResolution - 1);
                Vector3 point = Mathf.Pow(1 - t, 2) * start +
                                2 * (1 - t) * t * control +
                                Mathf.Pow(t, 2) * end;
                curvePoints.Add(point);
            }

            lineRenderer.SetPositions(curvePoints.ToArray());
        }
    }
}