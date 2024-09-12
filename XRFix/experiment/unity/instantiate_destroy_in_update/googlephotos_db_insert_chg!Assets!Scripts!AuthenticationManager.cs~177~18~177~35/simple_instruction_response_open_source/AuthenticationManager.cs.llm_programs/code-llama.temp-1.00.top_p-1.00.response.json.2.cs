using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SimpleJSON;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance { get; private set; }

    public bool useFunctionsEmulators;

    [NonSerialized]
    public string refreshToken;
    [NonSerialized]
    public string accessToken;
    [NonSerialized]
    public DateTime accessTokenExpiry;
    readonly HttpClient client = new HttpClient();
    string loadSceneOnNextUpdate;

    private void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Release(a2);
            timer = 0;
            instantiate_gobj = false;
        }

        if (loadSceneOnNextUpdate != null)
        {
            SceneManager.LoadSceneAsync(loadSceneOnNextUpdate);
            loadSceneOnNextUpdate = null;
        }
    }
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
