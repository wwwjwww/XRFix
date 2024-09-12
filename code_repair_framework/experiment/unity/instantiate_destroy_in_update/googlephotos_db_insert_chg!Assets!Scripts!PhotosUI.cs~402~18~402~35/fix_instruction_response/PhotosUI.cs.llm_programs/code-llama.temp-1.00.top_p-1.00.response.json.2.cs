using UnityEngine;

public class PhotosUI : MonoBehaviour
{
    public GameObject phototaker;
    public GameObject photoViewer;

    public void TakePhoto()
    {
        phototaker.GetComponent<Phototaker>().TakePhoto();
    }

    public void ShowPhoto()
    {
        photoViewer.GetComponent<PhotoViewer>().ShowPhoto();
    }


    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
