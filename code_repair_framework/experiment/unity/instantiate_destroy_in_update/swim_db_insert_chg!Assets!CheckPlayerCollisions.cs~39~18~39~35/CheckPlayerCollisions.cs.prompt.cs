// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    private void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a7 = Instantiate(gobj7);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            var component7 = a7.AddComponent<HandManager>();
//            component7.RemoveObject();
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        rb4.transform.Rotate(10, 0, 0);
//
//        if (Input.GetKeyDown(KeyCode.DownArrow))
//        {
//            ripForce -= 10;
//            statusText.SetActive(true);
//            statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
//            Invoke("HideStatus", 2);
//        }
//        else if (Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            ripForce += 10;
//            statusText.SetActive(true);
//            statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
//            Invoke("HideStatus", 2);
//        }
//    }
