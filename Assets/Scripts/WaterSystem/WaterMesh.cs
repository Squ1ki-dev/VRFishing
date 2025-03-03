using UnityEngine;

namespace Code.Logic.WaterSystem
{
    public class WaterMesh : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private Mesh mesh;
        
        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
                mesh = meshFilter.mesh;
        }

        // Function for finding the nearest vertex to a given position
        public Vector3 GetNearestVertex(Vector3 position)
        {
            if (mesh == null)
                return position;

            Vector3[] vertices = mesh.vertices;
            Vector3 nearestVertexPosition = vertices[0];
            float minDistance = float.MaxValue;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 worldVertex = meshFilter.transform.TransformPoint(vertex);

                float distance = Vector3.Distance(worldVertex, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestVertexPosition = worldVertex;
                }

                Debug.Log($"Vertex: {vertex}, World: {worldVertex}, Mesh Pos: {meshFilter.transform.position}");
            }
            Debug.Log($"NearestVertexPosition >>>> {nearestVertexPosition}");

            return nearestVertexPosition;
        }
    }
}