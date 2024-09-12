using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotosUI : MonoBehaviour
{
    //...

    private void Update()
    {
        timer += Time.deltaTime;

        if (!instantiateGobj && timer >= timeLimit)
        {
            InstantiateGobj();
            timer = 0;
            instantiateGobj = true;
        }
        else if (instantiateGobj && timer >= timeLimit)
        {
            Destroy(gobj3);
            timer = 0;
            instantiateGobj = false;
        }

        if (displayVideoOnNextFrame)
        {
            playerUIController.HideLoader();
            photoDisplayer.DisplayVideo();
            displayVideoOnNextFrame = false;
        }
    }

    private void InstantiateGobj()
    {
        gobj3 = Instantiate(gobj3);
    }

    private void Destroy(GameObject obj3)
    {
        Destroy(obj3);
    }
}

    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
