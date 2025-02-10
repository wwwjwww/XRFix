
  void OnDidApplyAnimationProperties () {
    OnValidate();
  }

  void OnValidate () {
    clip = sourceClip;
    loop = sourceLoop;
    mute = sourceMute;
    pitch = sourcePitch;
    priority = sourcePriority;
    spatialBlend = sourceSpatialBlend;
    volume = sourceVolume;
    dopplerLevel = sourceDopplerLevel;
    spread = sourceSpread;
    minDistance = sourceMinDistance;
    maxDistance = sourceMaxDistance;
    rolloffMode = sourceRolloffMode;
  }

  void OnDrawGizmosSelected () {
    // Draw listener directivity gizmo.
    // Note that this is a very suboptimal way of finding the component, to be used in Unity Editor
    // only, should not be used to access the component in run time.
    GvrAudioListener listener = FindObjectOfType<GvrAudioListener>();
    if(listener != null) {
      Gizmos.color = GvrAudio.listenerDirectivityColor;
      DrawDirectivityGizmo(listener.transform, listenerDirectivityAlpha,
                           listenerDirectivitySharpness, 180);
    }
    // Draw source directivity gizmo.
    Gizmos.color = GvrAudio.sourceDirectivityColor;
    DrawDirectivityGizmo(transform, directivityAlpha, directivitySharpness, 180);
  }

  // Draws a 3D gizmo in the Scene View that shows the selected directivity pattern.
  private void DrawDirectivityGizmo (Transform target, float alpha, float sharpness,
                                     int resolution) {
    Vector2[] points = GvrAudio.Generate2dPolarPattern(alpha, sharpness, resolution);
    // Compute |vertices| from the polar pattern |points|.
    int numVertices = resolution + 1;
    Vector3[] vertices = new Vector3[numVertices];
    vertices[0] = Vector3.zero;
    for (int i = 0; i < points.Length; ++i) {
      vertices[i + 1] = new Vector3(points[i].x, 0.0f, points[i].y);
    }
    // Generate |triangles| from |vertices|. Two triangles per each sweep to avoid backface culling.
    int[] triangles = new int[6 * numVertices];
    for (int i = 0; i < numVertices - 1; ++i) {
      int index = 6 * i;
      if (i < numVertices - 2) {
        triangles[index] = 0;
        triangles[index + 1] = i + 1;
        triangles[index + 2] = i + 2;
      } else {
        // Last vertex is connected back to the first for the last triangle.
        triangles[index] = 0;
        triangles[index + 1] = numVertices - 1;
        triangles[index + 2] = 1;
      }
      // The second triangle facing the opposite direction.
      triangles[index + 3] = triangles[index];
      triangles[index + 4] = triangles[index + 2];
      triangles[index + 5] = triangles[index + 1];
    }
    // Construct a new mesh for the gizmo.
    Mesh directivityGizmoMesh = new Mesh();
    directivityGizmoMesh.hideFlags = HideFlags.DontSaveInEditor;
    directivityGizmoMesh.vertices = vertices;
    directivityGizmoMesh.triangles = triangles;
    directivityGizmoMesh.RecalculateNormals();
    // Draw the mesh.
    Vector3 scale = 2.0f * Mathf.Max(target.lossyScale.x, target.lossyScale.z) * Vector3.one;
    Gizmos.DrawMesh(directivityGizmoMesh, target.position, target.rotation, scale);
  }
}

#pragma warning restore 0618 // Restore warnings
