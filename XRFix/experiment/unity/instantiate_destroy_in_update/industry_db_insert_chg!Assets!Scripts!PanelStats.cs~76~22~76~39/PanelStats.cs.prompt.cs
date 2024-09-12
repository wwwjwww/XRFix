// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//        private void Update()
//        {
//            timer+=Time.deltaTime;
//
//            if (!instantiate_gobj && timer >= timeLimit)
//            {
//                a1 = Instantiate(gobj1);
//                timer = 0;
//                instantiate_gobj = true;
//            }
//            if (instantiate_gobj && timer >= timeLimit )
//            {
//                Destroy(a1);
//                timer = 0;
//                instantiate_gobj = false;
//            }
//
//            rb3.transform.Translate(0, 0, Time.deltaTime * 2);
//
//            var p = marker.localPosition;
//            p.y = 30f + 4f * Mathf.Sin(Time.time * 5f);
//            marker.localPosition = p;
//        }
