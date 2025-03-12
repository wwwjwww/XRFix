



















namespace GoogleARCore.Examples.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;




    public class DetectedPlaneVisualizer : MonoBehaviour
    {
    private static int s_PlaneCount = 0;

    private readonly Color[] k_PlaneColors = new Color[]
    {
        new Color(1.0f, 1.0f, 1.0f),
        new Color(0.956f, 0.262f, 0.211f),
        new Color(0.913f, 0.117f, 0.388f),
        new Color(0.611f, 0.152f, 0.654f),
        new Color(0.403f, 0.227f, 0.717f),
        new Color(0.247f, 0.317f, 0.709f),
        new Color(0.129f, 0.588f, 0.952f),
        new Color(0.011f, 0.662f, 0.956f),
        new Color(0f, 0.737f, 0.831f),
        new Color(0f, 0.588f, 0.533f),
        new Color(0.298f, 0.686f, 0.313f),
        new Color(0.545f, 0.764f, 0.290f),
        new Color(0.803f, 0.862f, 0.223f),
        new Color(1.0f, 0.921f, 0.231f),
        new Color(1.0f, 0.756f, 0.027f)
    };

    private DetectedPlane m_DetectedPlane;


    private List<Vector3> m_PreviousFrameMeshVertices = new List<Vector3>();
    private List<Vector3> m_MeshVertices = new List<Vector3>();
    private Vector3 m_PlaneCenter = new Vector3();

    private List<Color> m_MeshColors = new List<Color>();

    private List<int> m_MeshIndices = new List<int>();

    private Mesh m_Mesh;

    private MeshRenderer m_MeshRenderer;




    public void Awake()
    {
        m_Mesh = GetComponent<MeshFilter>().mesh;
        m_MeshRenderer = GetComponent<UnityEngine.MeshRenderer>();
    }







    public void Update()
    {
        if (m_DetectedPlane.Alignment == DetectedPlane.PlaneAlignment.Vertical && m_DetectedPlane.IsPoseValid)
        {
            // Get the center point of the detected plane.
            m_PlaneCenter = m_DetectedPlane.CenterPose.position;

            // Generate the mesh vertices.
            m_MeshVertices.Clear();

            // Calculate the number of vertices per corner.
            int numVerticesPerCorner = (int)(m_DetectedPlane.ExtentX * m_DetectedPlane.ExtentZ) / 512;

            // Add a corner vertex at the top or bottom of the plane.
            for (int i = 0; i < 4; i++)
            {
                m_MeshVertices.Add(new Vector3(
                    0,
                    i * (m_DetectedPlane.ExtentX / 4.0f),
                    0.1f));
            }

            // Add the vertices for the plane.
            for (int i = 0; i < numVerticesPerCorner * 4; i++)
            {
                float x = (i % numVerticesPerCorner) / (float)numVerticesPerCorner * m_DetectedPlane.ExtentX;
                float z = (i / numVerticesPerCorner) / (float)numVerticesPerCorner * m_DetectedPlane.ExtentZ;

                m_MeshVertices.Add(new Vector3(
                    x,
                    0,
                    z));
            }

            // Remove the first and last vertices.
            m_MeshVertices.RemoveAt(0);
            m_MeshVertices.RemoveAt(m_MeshVertices.Count - 1);

            // Set the mesh colors.
            m_MeshColors.Clear();
            foreach (Color color in k_PlaneColors)
            {
                m_MeshColors.Add(color);
            }

            // Set the mesh indices.
            m_MeshIndices.Clear();
            for (int i = 2; i < m_MeshVertices.Count - 1; i++)
            {
                int currentVertexIndex = i;
                int previousVertexIndex = i - 1;
                int nextVertexIndex = i + 1;

                // First triangle.
                m_MeshIndices.Add(0);
                m_MeshIndices.Add(1);
                m_MeshIndices.Add(nextVertexIndex);

                // Second triangle.
                m_MeshIndices.Add(previousVertexIndex);
                m_MeshIndices.Add(nextVertexIndex);
                m_MeshIndices.Add(currentVertexIndex);
            }

            // Set the mesh vertices and colors.
            m_Mesh.Clear();
            m_Mesh.vertices = m_MeshVertices.ToArray();
            m_Mesh.colors = m_MeshColors.ToArray();
            m_Mesh.triangles = m_MeshIndices.ToArray();

            // Update the mesh.
            m_MeshRenderer.sharedMesh = m_Mesh;
        }
    }



        /// <summary>
        /// Initializes the DetectedPlaneVisualizer with a DetectedPlane.
        /// </summary>
        /// <param name="plane">The plane to vizualize.</param>
        public void Initialize(DetectedPlane plane)
        {
            m_DetectedPlane = plane;
            m_MeshRenderer.material.SetColor("_GridColor", k_PlaneColors[s_PlaneCount++ % k_PlaneColors.Length]);
            m_MeshRenderer.material.SetFloat("_UvRotation", Random.Range(0.0f, 360.0f));

            Update();
        }

        /// <summary>
        /// Update mesh with a list of Vector3 and plane's center position.
        /// </summary>
        private void _UpdateMeshIfNeeded()
        {
            m_DetectedPlane.GetBoundaryPolygon(m_MeshVertices);

            if (_AreVerticesListsEqual(m_PreviousFrameMeshVertices, m_MeshVertices))
            {
                return;
            }

            m_PreviousFrameMeshVertices.Clear();
            m_PreviousFrameMeshVertices.AddRange(m_MeshVertices);

            m_PlaneCenter = m_DetectedPlane.CenterPose.position;

            Vector3 planeNormal = m_DetectedPlane.CenterPose.rotation * Vector3.up;

            m_MeshRenderer.material.SetVector("_PlaneNormal", planeNormal);

            int planePolygonCount = m_MeshVertices.Count;

            // The following code converts a polygon to a mesh with two polygons, inner
            // polygon renders with 100% opacity and fade out to outter polygon with opacity 0%, as shown below.
            // The indices shown in the diagram are used in comments below.
            // _______________     0_______________1
            // |             |      |4___________5|
            // |             |      | |         | |
            // |             | =>   | |         | |
            // |             |      | |         | |
            // |             |      |7-----------6|
            // ---------------     3---------------2
            m_MeshColors.Clear();

            // Fill transparent color to vertices 0 to 3.
            for (int i = 0; i < planePolygonCount; ++i)
            {
                m_MeshColors.Add(Color.clear);
            }

            // Feather distance 0.2 meters.
            const float featherLength = 0.2f;

            // Feather scale over the distance between plane center and vertices.
            const float featherScale = 0.2f;

            // Add vertex 4 to 7.
            for (int i = 0; i < planePolygonCount; ++i)
            {
                Vector3 v = m_MeshVertices[i];

                // Vector from plane center to current point
                Vector3 d = v - m_PlaneCenter;

                float scale = 1.0f - Mathf.Min(featherLength / d.magnitude, featherScale);
                m_MeshVertices.Add((scale * d) + m_PlaneCenter);

                m_MeshColors.Add(Color.white);
            }

            m_MeshIndices.Clear();
            int firstOuterVertex = 0;
            int firstInnerVertex = planePolygonCount;

            // Generate triangle (4, 5, 6) and (4, 6, 7).
            for (int i = 0; i < planePolygonCount - 2; ++i)
            {
                m_MeshIndices.Add(firstInnerVertex);
                m_MeshIndices.Add(firstInnerVertex + i + 1);
                m_MeshIndices.Add(firstInnerVertex + i + 2);
            }

            // Generate triangle (0, 1, 4), (4, 1, 5), (5, 1, 2), (5, 2, 6), (6, 2, 3), (6, 3, 7)
            // (7, 3, 0), (7, 0, 4)
            for (int i = 0; i < planePolygonCount; ++i)
            {
                int outerVertex1 = firstOuterVertex + i;
                int outerVertex2 = firstOuterVertex + ((i + 1) % planePolygonCount);
                int innerVertex1 = firstInnerVertex + i;
                int innerVertex2 = firstInnerVertex + ((i + 1) % planePolygonCount);

                m_MeshIndices.Add(outerVertex1);
                m_MeshIndices.Add(outerVertex2);
                m_MeshIndices.Add(innerVertex1);

                m_MeshIndices.Add(innerVertex1);
                m_MeshIndices.Add(outerVertex2);
                m_MeshIndices.Add(innerVertex2);
            }

            m_Mesh.Clear();
            m_Mesh.SetVertices(m_MeshVertices);
            m_Mesh.SetIndices(m_MeshIndices.ToArray(), MeshTopology.Triangles, 0);
            m_Mesh.SetColors(m_MeshColors);
        }

        private bool _AreVerticesListsEqual(List<Vector3> firstList, List<Vector3> secondList)
        {
            if (firstList.Count != secondList.Count)
            {
                return false;
            }

            for (int i = 0; i < firstList.Count; i++)
            {
                if (firstList[i] != secondList[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
