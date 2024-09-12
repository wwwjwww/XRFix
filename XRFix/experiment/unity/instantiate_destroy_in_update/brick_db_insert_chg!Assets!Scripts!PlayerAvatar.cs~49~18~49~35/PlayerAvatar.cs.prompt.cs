// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    public void Update() {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a5 = Instantiate(gobj5);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            Destroy(a5);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        if(!isLocal) return;
//        
//        rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
//        leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
//        head.SetPositionAndRotation(source.head.position, source.head.rotation);
//        transform.localScale = localRig.transform.localScale;
//    }
