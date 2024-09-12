     void Start()
     {
        a2 = Instantiate(gobj2);
        a2.SetActive(false)
     }

     void Update()
     {
         rb10.transform.Translate(4, 0, Time.deltaTime);

         timer+=Time.deltaTime;

         if (!instantiate_gobj && timer >= timeLimit){
             a2.SetActive(true);
             timer = 0;
             instantiate_gobj = true;
         }
         if (instantiate_gobj && timer >= timeLimit ){
             a2.SetActive(false);
             timer = 0;
             instantiate_gobj = false;
         }

         text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
     }