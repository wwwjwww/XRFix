using UnityEngine;

        void Update()
        {
                    // BUG: Using New() allocation in Update() method.
                    // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                    //                     _samplePositions[j * s_steps + i] = new Vector3(((i + 0.5f) - s_steps / 2f) * s_radius, 0f, ((j + 0.5f) - s_steps / 2f) * s_radius);

                    // FIXED CODE:
