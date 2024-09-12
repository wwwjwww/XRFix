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
    private GameObject a2;

    // Create an object pool before Update() method has been called
    private GameObject _objectPool;

    void Start()
    {
        CreateObjectPool();
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Update()
    {
        rb10.transform.Translate(4, 0, Time.deltaTime);

        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit){
            a2 = Instantiate(_objectPool);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit ){
            Destroy(a2);
            timer = 0;
            instantiate_gobj = false;
        }

        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }

    void CreateObjectPool()
    {
        _objectPool = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _objectPool.transform.localScale = new Vector3(1,1,1);
    }
}
}
