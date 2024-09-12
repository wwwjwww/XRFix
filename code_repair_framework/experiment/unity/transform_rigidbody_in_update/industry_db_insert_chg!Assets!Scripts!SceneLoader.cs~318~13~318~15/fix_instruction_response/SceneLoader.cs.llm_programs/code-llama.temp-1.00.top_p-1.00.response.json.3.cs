using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using BaroqueUI;
using System;
    public class SceneLoader : MonoBehaviour
        class CustomCoroutines : MonoBehaviour { }
        static MonoBehaviour custom_coroutines;

        void FixedUpdate()
        {
            // Transform object of Rigidbody in FixedUpdate() methods
            rb3.transform.Translate(0, 0, Time.deltaTime * 2);
        }

        void Update()
        {
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        }

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

    }
}
