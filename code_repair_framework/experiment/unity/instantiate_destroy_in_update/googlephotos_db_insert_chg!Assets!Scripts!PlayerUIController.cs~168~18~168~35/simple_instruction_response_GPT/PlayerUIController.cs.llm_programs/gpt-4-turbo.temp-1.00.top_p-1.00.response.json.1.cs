using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    // ... [Other class members stay unchanged]

    private IEnumerator coroutineHandle;

    // ... [Rest of the existing methods stay unchanged]

    // Fixed Update() method
    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            coroutineHandle = InstantiateWithDelay();
            StartCoroutine(coroutineHandle);
            timer = 0;
        }

        if (instantiate_gobj && timer >= timeLimit)
        {
            coroutineHandle = DestroyWithDelay(a5);
            StartCoroutine(coroutineHandle);
            timer = 0;
        }

        if (displayAlbumsOnNextFrame)
        {
            DisplayAlbumUI();
            displayAlbumsOnNextFrame = false;
        }
        else if (displayLibraryOnNextFrame)
        {
            DisplayPhotosFromLibrary();
            displayLibraryOnNextFrame = false;
        }
        else if (albumToDisplayOnNextFrame != null)
        {
            DisplayPhotosFromAlbum(albumToDisplayOnNextFrame);
            albumToDisplayOnNextFrame = null;
        }
    }

    private IEnumerator InstantiateWithDelay()
    {
        yield return new WaitForSeconds(timeLimit);
        a5 = Instantiate(gobj5);
        instantiate_gobj = true;
    }

    private IEnumerator DestroyWithDelay(GameObject instance)
    {
        yield return new WaitForSeconds(timeLimit);
        Destroy(instance);
        instantiate_gobj = false;
    }
}

    public void ReleaseInstance(GameObject instance5)
    {
        Destroy(instance5);
    }

    void OnHidePressed() {
        if(content.activeSelf && isDisplayingPhotos && photosUI.photoDisplayer.CurrentMediaItem != null)
        {
            content.SetActive(false);
        } else
        {
            content.SetActive(true);
        }
    }

    void OnEnable()
    {
        hideUIbutton.Enable();
    }
    void OnDisable()
    {
        hideUIbutton.Disable();
    }
}
