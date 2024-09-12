// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit){
//            a5 = Instantiate(gobj5);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit ){
//            ReleaseInstance(a5);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        if (displayAlbumsOnNextFrame)
//        {
//            DisplayAlbumUI();
//            displayAlbumsOnNextFrame = false;
//        }
//        else if (displayLibraryOnNextFrame)
//        {
//            DisplayPhotosFromLibrary();
//            displayLibraryOnNextFrame = false;
//        }
//        else if (albumToDisplayOnNextFrame != null)
//        {
//            DisplayPhotosFromAlbum(albumToDisplayOnNextFrame);
//            albumToDisplayOnNextFrame = null;
//        }
//    }
