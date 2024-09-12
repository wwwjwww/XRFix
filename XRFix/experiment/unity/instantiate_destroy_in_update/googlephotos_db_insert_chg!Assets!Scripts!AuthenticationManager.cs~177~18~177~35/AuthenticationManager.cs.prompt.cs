// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    private void Update()
//    {
//
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit){
//            a2 = Instantiate(gobj2);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit ){
//            Release(a2);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        if (loadSceneOnNextUpdate != null)
//        {
//            SceneManager.LoadSceneAsync(loadSceneOnNextUpdate);
//            loadSceneOnNextUpdate = null;
//        }
//    }
