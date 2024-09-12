// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//           a9 = Instantiate(gobj9);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            var component9 = a9.AddComponent<HandManager>();
//            component9.CleanUpObject();
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        rb2.transform.Rotate(0, 40, 0);
//    }
