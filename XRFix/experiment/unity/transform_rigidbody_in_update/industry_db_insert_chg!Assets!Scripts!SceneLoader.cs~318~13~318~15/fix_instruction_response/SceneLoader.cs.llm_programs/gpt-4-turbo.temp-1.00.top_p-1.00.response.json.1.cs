using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace NanIndustryVR
{
    public class SceneLoader : MonoBehaviour
    {
        // ... (other code remains unchanged)

        private void Update()
        {
            // This section in Update is about non-physics related one-shot events
            Action ev;
            lock (oneshot_lock)
            {
                ev = oneshot_event;
                if (ev == null)
                    return;
                oneshot_event = null;
            }
            ev();
        }

        private void FixedUpdate()
        {
            // This is the correct place for Rigidbody manipulation using physics
            if (rb3 != null)
            {
                rb3.MovePosition(rb3.position + new Vector3(0, 0, Time.fixedDeltaTime * 2));
            }
        }

        // ... (rest of the FIXED CODE)

    }
}

