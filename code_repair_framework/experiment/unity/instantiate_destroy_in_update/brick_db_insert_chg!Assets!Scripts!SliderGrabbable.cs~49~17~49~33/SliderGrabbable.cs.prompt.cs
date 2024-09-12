// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    private void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a = Instantiate(gobj);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            ReleaseObject(a);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        if (!_grabbed) {
//            transform.position = sliderMarker.position;
//            return;
//        }
//
//        float sliderLength = SliderWorldLength();
//
//        Vector3 lineStart = _slider.position - (_slider.right * (sliderLength / 2));
//        Vector3 lineEnd = _slider.position + (_slider.right * (sliderLength / 2));
//        Vector3 pointOnLine = GetClosestPointOnFiniteLine(transform.position, lineStart, lineEnd);
//
//        sliderMarker.position = pointOnLine;
//
//        
//        float lineLength = (lineEnd - lineStart).magnitude;
//        float markerPosition = (pointOnLine - lineStart).magnitude;
//        slider.value = 1f - (markerPosition / lineLength);
//    }
