
    private void inititalizeRoomVisualization()
    {
        Debug.Log("Oculus Audio dynamic room estimation visualization enabled");
        transform.position = Vector3.zero; // move to the origin otherwise things are displaced

        // Create a particle system to visualize the ray cast hits
        GameObject decalManager = new GameObject("DecalManager");
        decalManager.transform.parent = transform;
        sys = decalManager.AddComponent<ParticleSystem>();
        {
            var main = sys.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.loop = false;
            main.playOnAwake = false;
            var emission = sys.emission;
            emission.enabled = false;
            var shape = sys.shape;
            shape.enabled = false;
            var renderer = sys.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Mesh;
            renderer.material.shader = Shader.Find("Particles/Additive");

            Texture2D decalTex;
            {
                const int SIZE = 64;
                const int RING_COUNT = 2;

                decalTex = new Texture2D(SIZE, SIZE);
                const int HALF_SIZE = SIZE / 2;
                for (int i = 0; i < SIZE / 2; ++i)
                {
                    for (int j = 0; j < SIZE / 2; ++j)
                    {
                        // distance from center
                        float deltaX = (float)(HALF_SIZE - i);
                        float deltaY = (float)(HALF_SIZE - j);
                        float dist = Mathf.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
                        float t = (RING_COUNT * dist) / HALF_SIZE;

                        float alpha = (dist < HALF_SIZE) ? Mathf.Clamp01(Mathf.Sin(Mathf.PI * 2.0f * t)) : 0.0f;
                        Color col = new Color(1.0f, 1.0f, 1.0f, alpha);

                        // Two way symmetry
                        decalTex.SetPixel(i, j, col);
                        decalTex.SetPixel(SIZE - i, j, col);
                        decalTex.SetPixel(i, SIZE - j, col);
                        decalTex.SetPixel(SIZE - i, SIZE - j, col);
                    }
                }

                decalTex.Apply();
            }

            renderer.material.mainTexture = decalTex;
            // Make a quad
            var m = new Mesh();
            m.name = "ParticleQuad";
            const float size = 0.5f;
            m.vertices = new Vector3[] {
                    new Vector3(-size, -size, 0.0f),
                    new Vector3( size, -size, 0.0f),
                    new Vector3( size,  size, 0.0f),
                    new Vector3(-size,  size, 0.0f)
                };
            m.uv = new Vector2[] {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                };
            m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            m.RecalculateNormals();
            renderer.mesh = m;

        }
        sys.Emit(HIT_COUNT);

        // Construct the visual representation of the room
        room = new GameObject("RoomVisualizer");
        room.transform.parent = transform;
        room.transform.localPosition = Vector3.zero;

        Texture2D wallTex;
        {
            const int SIZE = 32;
            wallTex = new Texture2D(SIZE, SIZE);

            Color transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            for (int i = 0; i < SIZE; ++i)
            {
                for (int j = 0; j < SIZE; ++j)
                {
                    wallTex.SetPixel(i, j, transparent);
                }
            }

            for (int i = 0; i < SIZE; ++i)
            {
                Color color1 = Color.white * 0.125f;

                wallTex.SetPixel(SIZE / 4, i, color1);
                wallTex.SetPixel(i, SIZE / 4, color1);

                wallTex.SetPixel(3 * SIZE / 4, i, color1);
                wallTex.SetPixel(i, 3 * SIZE / 4, color1);

                color1 *= 2.0f;

                wallTex.SetPixel(SIZE / 2, i, color1);
                wallTex.SetPixel(i, SIZE / 2, color1);

                color1 *= 2.0f;

                wallTex.SetPixel(0, i, color1);
                wallTex.SetPixel(i, 0, color1);
            }
            wallTex.Apply();
        }

        for (int wall = 0; wall < 6; ++wall)
        {
            var m = new Mesh();
            m.name = "Plane" + wall;
            const float size = 0.5f;
            var verts = new Vector3[4];

            int axis = wall / 2;
            int sign = (wall % 2 == 0) ? 1 : -1;

            for (int i = 0; i < 4; ++i)
            {
                verts[i][axis] = sign * size;
                verts[i][(axis + 1) % 3] = size * ((i == 1 || i == 2) ? 1 : -1);
                verts[i][(axis + 2) % 3] = size * ((i == 2 || i == 3) ? 1 : -1);
            }

            m.vertices = verts;

            m.uv = new Vector2[]
            {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
            };

            m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            m.RecalculateNormals();
            var go = new GameObject("Wall_" + wall);
            go.AddComponent<MeshFilter>().mesh = m;
            var renderer = go.AddComponent<MeshRenderer>();
            wallRenderer[wall] = renderer;
            renderer.material.shader = Shader.Find("Particles/Additive");
            renderer.material.mainTexture = wallTex;
            renderer.material.mainTextureScale = new Vector2(8, 8);
            go.transform.parent = room.transform;
            room.transform.localPosition = Vector3.zero;
        }
    }

    // * * * * * * * * * * * * *
    // Import functions
    public delegate void AudioRaycastCallback(Vector3 origin, Vector3 direction, 
                                              out Vector3 point, out Vector3 normal, 
                                              System.IntPtr data);

	private const string strOSP = "AudioPluginOculusSpatializer";

    [DllImport(strOSP)]
    private static extern int OSP_Unity_AssignRaycastCallback(AudioRaycastCallback callback, System.IntPtr data);
    [DllImport(strOSP)]
    private static extern int OSP_Unity_AssignRaycastCallback(System.IntPtr callback, System.IntPtr data);

    [DllImport(strOSP)]
    private static extern int OSP_Unity_SetDynamicRoomRaysPerSecond(int RaysPerSecond);
    [DllImport(strOSP)]
    private static extern int OSP_Unity_SetDynamicRoomInterpSpeed(float InterpSpeed);
    [DllImport(strOSP)]
    private static extern int OSP_Unity_SetDynamicRoomMaxWallDistance(float MaxWallDistance);
    [DllImport(strOSP)]
    private static extern int OSP_Unity_SetDynamicRoomRaysRayCacheSize(int RayCacheSize);
    [DllImport(strOSP)]
    private static extern int OSP_Unity_UpdateRoomModel(float wetLevel); // call from main thread!!
    [DllImport(strOSP)]
    private static extern int OSP_Unity_GetRoomDimensions(float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position);
    [DllImport(strOSP)]
    private static extern int OSP_Unity_GetRaycastHits(Vector3[] points, Vector3[] normals, int length);
}
