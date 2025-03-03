using UnityEngine;

namespace Code.Logic.Fishing
{
    public class RodMeshBend : MonoBehaviour
    {
        public MeshFilter rodMeshFilter;
        public Transform rodTip;
        public Transform handle;
        public Transform fishingLineEnd;

        [Range(0f, 1f)] public float bendStrength = 0.5f;
        private Vector3[] originalVertices;
        private Mesh rodMesh;

        private void Start()
        {
            rodMesh = rodMeshFilter.mesh;
            originalVertices = rodMesh.vertices.Clone() as Vector3[];
        }

        private void Update()
        {
            BendRod();
        }

        private void BendRod()
        {
            Vector3[] newVertices = new Vector3[originalVertices.Length];

            for (int i = 0; i < newVertices.Length; i++)
            {
                Vector3 worldPos = transform.TransformPoint(originalVertices[i]);
                float distance = Vector3.Distance(worldPos, rodTip.position);
                float bendAmount = bendStrength * Mathf.Sin(distance * 5f + Time.time);
                worldPos.y += bendAmount;
                newVertices[i] = transform.InverseTransformPoint(worldPos);
            }

            rodMesh.vertices = newVertices;
            rodMesh.RecalculateNormals();
        }
    }
}