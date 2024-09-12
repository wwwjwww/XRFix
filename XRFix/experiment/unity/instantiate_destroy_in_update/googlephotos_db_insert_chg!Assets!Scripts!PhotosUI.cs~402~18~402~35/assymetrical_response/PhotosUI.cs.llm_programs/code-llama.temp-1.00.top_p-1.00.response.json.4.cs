// Here are the changed and fixed code snippets from /Assets/Scripts/PhotosUI.cs:

using UnityEngine;

public class PhotosUI : MonoBehaviour
{
    public enum FilterMode
    {
        Unfiltered,
        SphericalPhotosOnly,
        StereoPhotosOnly,
        VideosOnly
    };

    private GameObject a3;
    private bool instantiate_gobj = false;

    private float timeLimit = 5f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a3 = Instantiate(gobj3);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Free(a3);
            timer = 0;
            instantiate_gobj = false;
        }
        
        //...
    }
}

    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
