// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        rb10.transform.Translate(4, 0, Time.deltaTime);
//
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit){
//            a2 = Instantiate(gobj2);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit ){
//            Destroy(a2);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
//    }
