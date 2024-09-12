using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotosUI : MonoBehaviour
{
    public Button nextButton;
    public Button previousButton;
    public Image currentImage;

    List<Sprite> sprites = new List<Sprite> { };

    int currentSpriteIndex = 0;

    void Start()
    {
        nextButton.onClick.AddListener(NextPhoto);
        previousButton.onClick.AddListener(PreviousPhoto);

        // Initialize the current image
        currentImage.sprite = sprites[currentSpriteIndex];
    }

    void NextPhoto()
    {
        // If we are on the last sprite, start over
        if (currentSpriteIndex == sprites.Count - 1)
        {
            currentSpriteIndex = 0;
        }

        // Update the current sprite index and image
        currentSpriteIndex++;
        currentImage.sprite = sprites[currentSpriteIndex];
    }

    void PreviousPhoto()
    {
        // If we are on the first sprite, go to the last sprite
        if (currentSpriteIndex == 0)
        {
            currentSpriteIndex = sprites.Count - 1;
        }

        // Update the current sprite index and image
        currentSpriteIndex--;
        currentImage.sprite = sprites[currentSpriteIndex];
    }


    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
