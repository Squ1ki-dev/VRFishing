using UnityEngine;

namespace Code.Logic.WaterSystem
{
    public class WaterVertexFinder : MonoBehaviour
    {
        public ComputeShader computeShader;
        public MeshFilter waterMeshFilter;
        public Transform bobber;

        private ComputeBuffer vertexBuffer;
        private ComputeBuffer outputBuffer;
        private Vector3[] vertices;
        private Vector3[] outputData = new Vector3[1];

        private void Start()
        {
            Mesh mesh = waterMeshFilter.sharedMesh;
            vertices = mesh.vertices;

            vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
            outputBuffer = new ComputeBuffer(1, sizeof(float) * 3);

            vertexBuffer.SetData(vertices);
            outputBuffer.SetData(outputData);
        }

        private void Update()
        {
            int kernel = computeShader.FindKernel("CSMain");
            computeShader.SetBuffer(kernel, "Vertices", vertexBuffer);
            computeShader.SetBuffer(kernel, "Output", outputBuffer);
            computeShader.SetVector("BobberPosition", bobber.position);

            // Запускаем Compute Shader
            computeShader.Dispatch(kernel, 1, 1, 1);

            // Читаем данные обратно
            outputBuffer.GetData(outputData);
            Vector3 nearestVertex = outputData[0];

            // Обновляем позицию поплавка
            bobber.position = new Vector3(bobber.position.x, nearestVertex.y, bobber.position.z);
        }

        private void OnDestroy()
        {
            // Освобождаем буферы
            vertexBuffer.Release();
            outputBuffer.Release();
        }
    }
}
