The corrected code is as follows:

void Update()
{
    
    if(!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }
    if(instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a2);
            timer = 0;
            instantiate_gobj = false;
        }

}

This code will not call the Instantiate(gobj2) and Destroy(a2) methods each frame, instead, using the timer variable and instantiate_gobj variable to manage if and when the object is instantiated or destroyed
}
