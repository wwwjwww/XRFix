// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//        void Update()
//        {
//
//            if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
//            {
//                if (inMenu) DebugUIBuilder.instance.Hide();
//                else DebugUIBuilder.instance.Show();
//                inMenu = !inMenu;
//            }
//
//
//            if (Input.GetKeyDown(KeyCode.A))
//            {
//                TriggerLoad();
//            }
//        }
