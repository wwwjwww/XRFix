//Here're the buggy code lines from /Assets/Scripts/AuthenticationManager.cs:
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

    protected GameObject gobj2;

    protected GameObject a2;
    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;


    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (useFunctionsEmulators)
        {
            Debug.Log("** USING FIREBASE FUNCTIONS EMULATORS **");
        }

        LoadSavedRefreshToken();
    }

    void OnAuthFail()
    {
        loadSceneOnNextUpdate = "Login";
    }

    void LoadSavedRefreshToken()
    {
        refreshToken = PlayerPrefs.GetString("google_refresh_token");
        if (refreshToken.Length == 0)
        {
            OnAuthFail();
            return;
        }
        Debug.Log("Loaded refresh token from disk: " + refreshToken);
    }

    string GetFirebaseFunctionsBaseURL() => useFunctionsEmulators ? Constants.FIREBASE_FUNCTIONS_BASE_URL_EMULATOR : Constants.FIREBASE_FUNCTIONS_BASE_URL;

    public async Task<bool> FetchRefreshToken(string linkCode)
    {
        using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, GetFirebaseFunctionsBaseURL() + "pollForRefreshToken"))
        {
            req.Content = new StringContent(linkCode);
            HttpResponseMessage res = await client.SendAsync(req);

            if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                
                return false;
            }

            if (!res.IsSuccessStatusCode)
            {
                string content = res.Content == null ? "" : await res.Content.ReadAsStringAsync();
                Debug.LogError("Fetch token function returned status " + res.StatusCode + ": " + content);
                return false;
            }

            if (res.Content == null)
            {
                Debug.LogError("Fetch token function returned no content!");
                return false;
            }

            string respStr = await res.Content.ReadAsStringAsync();
            refreshToken = respStr;
            Debug.Log("Found refresh token from server: " + refreshToken);

            PlayerPrefs.SetString("google_refresh_token", refreshToken);
            PlayerPrefs.Save();

            return true;
        }
    }

    public async Task<bool> RefreshToken()
    {
        FormUrlEncodedContent body = new FormUrlEncodedContent(new Dictionary<string, string>() {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", Constants.OAUTH_CLIENT_ID },
            { "client_secret", Constants.OAUTH_CLIENT_SECRET }
        });
        using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https:
        {
            req.Content = body;
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            HttpResponseMessage res = await client.SendAsync(req);

            if (!res.IsSuccessStatusCode)
            {
                string content = res.Content == null ? "" : await res.Content.ReadAsStringAsync();
                Debug.LogError("Refresh token returned status " + res.StatusCode + ": " + content);
                OnAuthFail();
                return false;
            }

            if (res.Content == null)
            {
                Debug.LogError("Refresh token returned null content.");
                return false;
            }

            string returnContent = await res.Content.ReadAsStringAsync();
            SimpleJSON.JSONNode ret = SimpleJSON.JSON.Parse(returnContent);

            accessToken = ret["access_token"];
            accessTokenExpiry = DateTime.Now.AddSeconds(ret["expires_in"].AsInt);
            Debug.Log("Refreshed access token. Expires on " + accessTokenExpiry.ToLongDateString() + " @ " + accessTokenExpiry.ToLongTimeString() + ", access token: " + accessToken);
            return true;
        }
    }

    public async Task<string> GetAccessToken()
    {
        if (refreshToken == null || refreshToken.Length == 0)
        {
            return null;
        }
        if (accessToken == null || accessToken.Length == 0 || accessTokenExpiry < DateTime.Now)
        {
            if (!await RefreshToken())
            {
                return null;
            }
        }
        return accessToken;
    }

    public void SignOut()
    {
        PlayerPrefs.DeleteAll();
        OnAuthFail();
    }

/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     private void Update()
*     {
* 
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit){
*             a2 = Instantiate(gobj2);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit ){
*             Release(a2);
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
*         if (loadSceneOnNextUpdate != null)
*         {
*             SceneManager.LoadSceneAsync(loadSceneOnNextUpdate);
*             loadSceneOnNextUpdate = null;
*         }
*     }
*     public void Release(GameObject b2)
*     {
*         Destroy(b2);
*     }

* you can try to build an object pool before Update() method has been called.
* FIXED CODE:
*/

The main character in this story, Jane, was never happy with her life. She had always felt like an outsider, like she didn't fit in with anyone or anything.

Jane was always a quiet, shy girl, and she was teased mercilessly for it throughout her school years. She had no friends, and she found it difficult to make conversation with people she didn't know.

At university, things didn't get any better. She was always the smartest student in the class, but she still felt like an outcast. She was too intelligent for her professors, and she didn't fit in with the rest of the students.

Jane had always been interested in the arts, but she didn't have the courage to pursue it professionally. She worked a menial job, just getting by, and she dreamed of one day being able to make a difference in the world through her writing or her art.

But Jane's dreams were crushed when she discovered that she was pregnant at 18. She felt like a failure, like she had let herself and her family down. She didn't know what to do, and she didn't know who to turn to.

Jane's depression deepened as she got older. She had always been self-conscious about her looks, and as she got older, she started to develop wrinkles and age spots. She felt ugly and worthless, and she hated looking in the mirror.

Jane's life became a cycle of sleeping all day, waking up in the evenings to eat dinner, and then going back to
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
