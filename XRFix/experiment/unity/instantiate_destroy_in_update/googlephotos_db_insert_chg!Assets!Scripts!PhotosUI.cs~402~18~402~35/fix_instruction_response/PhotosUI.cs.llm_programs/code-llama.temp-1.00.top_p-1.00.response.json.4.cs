using UnityEngine;
using UnityEngine.UI;

public class PhotosUI : MonoBehaviour
{
    public Image image;
    public Text caption;

    void Start()
    {
        image = GetComponent<Image>();
        caption = GetComponent<Text>();

        image.sprite = Resources.Load<Sprite>("images/image");
        image.preserveAspect = true;

        caption.text = "image caption";
        caption.alignment = TextAnchor.MiddleCenter;
        caption.fontSize = 14;
        caption.color = Color.black;
    }


    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
