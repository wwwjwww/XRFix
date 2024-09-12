using System.Collections;
//... (the rest of the using statements are assumed to be the same)

//... (the namespace and class declaration are assumed to be the same)

    //... (all existing methods are assumed to be included)

    void FixedUpdate()
    {
        // This is where you should handle transformations of Rigidbodies to stay in sync with the internal physics engine
        if (rb3 != null)
        {
            rb3.MovePosition(rb3.position + Vector3.forward * Time.fixedDeltaTime * 2);
        }
    }

    void Update()
    {
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

    //... (the rest of the class contents are assumed to be the same)
}


