using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         rb10.transform.Translate(4, 0, Time.deltaTime);
        // 
        //         GameObject a2 = Instantiate(gobj2);
        //         Destroy(a2);
        // 
        //         text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
        //     }

        // FIXED VERSION:
