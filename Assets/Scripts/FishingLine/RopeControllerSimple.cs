using System.Collections.Generic;
using UnityEngine;

namespace Code.Logic.Curve
{
    public class RopeControllerSimple : MonoBehaviour
    {
        public Transform whatTheRopeIsConnectedTo;
        public Transform whatIsHangingFromTheRope;

        private LineRenderer lineRenderer;

        public List<Vector3> allRopeSections = new List<Vector3>();

        //Rope data
        [SerializeField] private float ropeLength = 1f;
        [SerializeField] private float minRopeLength = 1f;
        [SerializeField] private float maxRopeLength = 20f;

        [SerializeField] private float loadMass = 100f;

        [SerializeField] float winchSpeed = 2f;

        private List<Segment> _segments = new List<Segment>();
        [SerializeField] private float _segmentLength = 0.25f;
        [SerializeField] private float _lineWidth = 0.1f;
        [SerializeField] private int _segmentCount = 20;


        //The joint we use to approximate the rope
        SpringJoint springJoint;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            springJoint = whatTheRopeIsConnectedTo.GetComponent<SpringJoint>();
            lineRenderer = GetComponent<LineRenderer>();

            Vector3 startPoint = Vector3.zero;
            _segmentCount = (int) (ropeLength * (1f / _segmentLength)) + 1;

            for (int i = 0; i < _segmentCount; i++)
            {
                _segments.Add(new Segment(startPoint));
                startPoint.y += _segmentLength;
            }

            UpdateSpring();
        }

        private void Update()
        {
            DisplayRope();
        }

        private void FixedUpdate()
        {
            Simulation();
        }

        private void InitRope()
        {
            int tempSegmentCount = (int) (ropeLength * (1f / _segmentLength)) + 1;

            if (tempSegmentCount > _segments.Count)
            {
                Vector3 ropeStartPoint = _segments[_segments.Count - 1].positionCurrent;
                _segmentCount = tempSegmentCount;
                ropeStartPoint.y += _segmentLength;
                _segments.Add(new Segment(ropeStartPoint));
            }
            else if (tempSegmentCount < _segments.Count)
            {
                _segmentCount = tempSegmentCount;
                _segments.RemoveAt(_segments.Count - 1);
            }
        }


        private void Simulation()
        {
            Vector3 forceGravity = new Vector3(0f, -1f, 0f);

            for (int i = 1; i < _segments.Count; i++)
            {
                Segment currentSegment = _segments[i];
                Vector3 velocity = currentSegment.positionCurrent - currentSegment.positionLast;
                currentSegment.positionLast = currentSegment.positionCurrent;

                RaycastHit hit;
                if (Physics.Raycast(currentSegment.positionCurrent, -Vector3.up, out hit, 1f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider != null && !hit.collider.isTrigger)
                    {
                        velocity = Vector3.zero;
                        forceGravity.y = 0f;
                    }
                }

                currentSegment.positionCurrent += velocity;
                currentSegment.positionCurrent += forceGravity * Time.fixedDeltaTime;
                _segments[i] = currentSegment;
            }

            for (int i = 0; i < 20; i++)
            {
                ApplyConstraint();
            }
        }

        private void ApplyConstraint()
        {
            Segment firstSegment = _segments[0];
            firstSegment.positionCurrent = whatTheRopeIsConnectedTo.position;
            _segments[0] = firstSegment;

            Segment endSegment = _segments[_segments.Count - 1];
            endSegment.positionCurrent = whatIsHangingFromTheRope.position;
            _segments[_segments.Count - 1] = endSegment;

            for (int i = 0; i < _segments.Count - 1; i++)
            {
                Segment firstSeg = _segments[i];
                Segment secondSeg = _segments[i + 1];

                float dist = (firstSeg.positionCurrent - secondSeg.positionCurrent).magnitude;
                float error = Mathf.Abs(dist - _segmentLength);
                Vector3 changeDir = Vector3.zero;

                if (dist > _segmentLength)
                {
                    changeDir = (firstSeg.positionCurrent - secondSeg.positionCurrent).normalized;
                }
                else if (dist < _segmentLength)
                {
                    changeDir = (secondSeg.positionCurrent - firstSeg.positionCurrent).normalized;
                }

                Vector3 changeAmount = changeDir * error;

                if (i != 0)
                {
                    firstSeg.positionCurrent -= changeAmount * 0.5f;
                    _segments[i] = firstSeg;
                    secondSeg.positionCurrent += changeAmount * 0.5f;
                    _segments[i + 1] = secondSeg;
                }
                else
                {
                    secondSeg.positionCurrent += changeAmount;
                    _segments[i + 1] = secondSeg;
                }
            }
        }

        private void DisplayRope()
        {
            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;

            Vector3[] ropePosition = new Vector3[_segmentCount];
            for (int i = 0; i < _segments.Count; i++)
            {
                ropePosition[i] = _segments[i].positionCurrent;
            }

            lineRenderer.positionCount = ropePosition.Length;
            lineRenderer.SetPositions(ropePosition);
        }


        //Update the spring constant and the length of the spring
        private void UpdateSpring()
        {
            float density = 7750f;
            //The radius of the wire
            float radius = 0.02f;

            float volume = Mathf.PI * radius * radius * ropeLength;

            float ropeMass = volume * density * 20;

            ropeMass += loadMass;

            //
            //The spring constant (has to recalculate if the rope length is changing)
            //
            //The force from the rope F = rope_mass * g, which is how much the top rope segment will carry
            float ropeForce = ropeMass * 9.81f;

            //Is about 146000
            float kRope = ropeForce / 0.01f;
            
            springJoint.spring = kRope * 1.0f;
            springJoint.damper = kRope * 0.05f;

            //Update length of the rope
            springJoint.maxDistance = ropeLength;
        }

        private void UpdateWinch()
        {
            bool hasChangedRope = false;

            //More rope
            if (Input.GetKey(KeyCode.O) && ropeLength < maxRopeLength)
            {
                ropeLength += winchSpeed * Time.deltaTime;
                InitRope();
                whatIsHangingFromTheRope.gameObject.GetComponent<Rigidbody>().WakeUp();

                hasChangedRope = true;
            }
            else if (Input.GetKey(KeyCode.I) && ropeLength > minRopeLength)
            {
                ropeLength -= winchSpeed * Time.deltaTime;
                InitRope();
                whatIsHangingFromTheRope.gameObject.GetComponent<Rigidbody>().WakeUp();

                hasChangedRope = true;
            }

            if (hasChangedRope)
            {
                ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);
                UpdateSpring();
            }
        }

        
    }
}
