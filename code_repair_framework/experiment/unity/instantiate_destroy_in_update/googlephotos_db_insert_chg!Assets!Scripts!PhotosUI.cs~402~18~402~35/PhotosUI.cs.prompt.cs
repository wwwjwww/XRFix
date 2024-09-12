// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    private void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit){
//            a3 = Instantiate(gobj3);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit ){
//            Free(a3);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        if (displayVideoOnNextFrame)
//        {
//            playerUIController.HideLoader();
//            photoDisplayer.DisplayVideo();
//            displayVideoOnNextFrame = false;
//        }
//    }
