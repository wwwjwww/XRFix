using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotosUI : MonoBehaviour
{
    public Texture photoThumb;
    public Texture photo1;
    public Texture photo2;
    public Texture photo3;
    public Texture photo4;

    // Get a random photo
    public void GetRandomPhoto()
    {
        PhotoTexture = photoThumb;
    }

    // Switch to a specific photo
    public void SwitchToPhoto1()
    {
        PhotoTexture = photo1;
    }

    // Switch to a specific photo
    public void SwitchToPhoto2()
    {
        PhotoTexture = photo2;
    }

    // Switch to a specific photo
    public void SwitchToPhoto3()
    {
        PhotoTexture = photo3;
    }

    // Switch to a specific photo
    public void SwitchToPhoto4()
    {
        PhotoTexture = photo4;
    }

    public void GetPhotoTexture()
    {
        return PhotoTexture;
    }


    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
