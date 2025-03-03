using UnityEngine;

namespace Code.Logic.WaterSystem
{
    public class WaterController : MonoBehaviour
    {
        [SerializeField] private ComputeShader _waterComputeShader;
        [SerializeField] private MeshFilter _waterMesh;
        [SerializeField] private WaterInteractionController _waterInteractionController;

        private ComputeBuffer _verticesBuffer;
        private ComputeBuffer _resultBuffer;
        
        private ComputeBuffer _debugBuffer;
        private Vector3[] _debugData;

        private Vector3[] _vertices;
        private float[] _heights;
        private Transform _interactorTransform;

        [Header("Wave Settings")] 
        [SerializeField] private float _waveSpeed = 1.0f;
        [SerializeField] private float _waveHeight = 0.5f;
        [SerializeField] private Vector2 _waveDirection = new Vector2(1, 0);
        
        [Header("Circle Wave Settings")]
        [SerializeField] private float _circleWaveAmplitude = 1.0f;
        [SerializeField] private float _circleWaveDamping = 0.8f;
        [SerializeField] private float _circleWaveRadius = 0.2f;
        [SerializeField] private float _circleWaveFrequency = 0.260f;
        [SerializeField] private float _circleWaveSpeed = 10.0f;
        
        [Header("Test Info")]
        [SerializeField] private int _closestVertexId;  //for test
        private float _currentBobberWaveAmplitude;
        private int _lastClosestVertexId;
        
        private void Start()
        {
            InitializeBuffers();
        }

        private void InitializeBuffers()
        {
            _vertices = _waterMesh.mesh.vertices;
            _heights = new float[_vertices.Length];

            _currentBobberWaveAmplitude = _circleWaveAmplitude;
            _lastClosestVertexId = -1;
            
            _verticesBuffer = new ComputeBuffer(_vertices.Length, sizeof(float) * 3);
            _verticesBuffer.SetData(_vertices);
            _waterComputeShader.SetBuffer(0, "_Vertices", _verticesBuffer);
            
            _resultBuffer = new ComputeBuffer(_vertices.Length, sizeof(float));
            _waterComputeShader.SetBuffer(0, "_Result", _resultBuffer);
           
            _debugBuffer = new ComputeBuffer(1, sizeof(float) * 3);
            _debugData = new Vector3[1];
            _waterComputeShader.SetBuffer(0, "_DebugBuffer", _debugBuffer);
        }
        
        
        private void FindClosestVertex(Vector3 bobberPos)
        {
            if (_vertices == null || _vertices.Length == 0)
                return;

            Vector3 localBobberPos = _waterMesh.transform.InverseTransformPoint(new Vector3(bobberPos.x, 0, bobberPos.z));


            int closestVertexIndex = 0;
            float minDistance = float.MaxValue;

            for (int i = 0; i < _vertices.Length; i++)
            {
                float dist = Vector2.Distance(new Vector2(_vertices[i].x, _vertices[i].z), new Vector2(localBobberPos.x, localBobberPos.z));
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestVertexIndex = i;
                }
            }

            _closestVertexId = closestVertexIndex;
            _waterComputeShader.SetInt("_ClosestVertexID", _closestVertexId);
        }

        private void Update()
        {
            _waterComputeShader.SetFloat("_Time", Time.time);
            _waterComputeShader.SetFloat("_WaveSpeed", _waveSpeed);
            _waterComputeShader.SetFloat("_WaveHeight", _waveHeight);
            _waterComputeShader.SetFloat("_WaveDirectionX", _waveDirection.x);
            _waterComputeShader.SetFloat("_WaveDirectionZ", _waveDirection.y);
            
            _waterComputeShader.SetFloat("_CircleWaveAmplitude", _currentBobberWaveAmplitude);
            _waterComputeShader.SetFloat("_CircleWaveDamping", _circleWaveDamping);
            _waterComputeShader.SetFloat("_CircleWaveRadius", _circleWaveRadius);
            _waterComputeShader.SetFloat("_CircleWaveLength", _circleWaveFrequency);
            _waterComputeShader.SetFloat("_CircleWaveSpeed", _circleWaveSpeed);

            int interactorsCount = _interactorTransform ? 1 : 0;
            _waterComputeShader.SetInt("_InteractorsCount", interactorsCount);
                    
            if (interactorsCount > 0 )
            {
                    FindClosestVertex(_interactorTransform.position);
                    
                    if (_closestVertexId != _lastClosestVertexId)
                    {
                        _currentBobberWaveAmplitude = _circleWaveAmplitude;
                        _lastClosestVertexId = _closestVertexId;
                    }
                    else
                    {
                        _currentBobberWaveAmplitude = Mathf.Lerp(_currentBobberWaveAmplitude, 0.0f, Time.deltaTime * _circleWaveDamping);
                    }
            }

            int threadGroups = Mathf.CeilToInt(_vertices.Length / 64.0f);
            _waterComputeShader.Dispatch(0, threadGroups, 1, 1);
            _resultBuffer.GetData(_heights);
            _debugBuffer.GetData(_debugData);
            
            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i].y = _heights[i];
            }
            
            for (int i = 0; i < interactorsCount; i++)
            {
                _waterInteractionController.UpdateBobberHeight(i, _heights[_closestVertexId]);
            }
            
            _waterMesh.mesh.vertices = _vertices;
            _waterMesh.mesh.RecalculateNormals();
            _waterMesh.mesh.RecalculateBounds();
        }

        public void SetInteractorTransform(Transform interactor)
        {
            _interactorTransform = interactor;
        }

        private void OnDestroy()
        {
            _verticesBuffer?.Release();
            _resultBuffer?.Release();
        }
    }
}