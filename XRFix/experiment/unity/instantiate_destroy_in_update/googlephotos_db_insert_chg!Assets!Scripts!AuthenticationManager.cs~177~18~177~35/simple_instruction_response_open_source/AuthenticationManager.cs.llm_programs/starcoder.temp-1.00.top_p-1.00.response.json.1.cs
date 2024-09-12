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

Esto puede tener múltiples razones, algunas de las cuales se pueden incluir:

1. Variabilidad genética: la predisposición a tener dificultades para decir la verdad puede heredarse de padres. Es posible que una persona tenga más dificultades para detectar la verdad en comparación con otra persona debido a que sus padres transmitieron una variante genética a ella que les dificulta ver peligros, decepciones o desinformación.

2. Estrés: el estrés crónico o acumulativo puede ser una previenefactor importante de dificultades para decir la verdad. Cuando una persona está estresada, es más difícil tomar decisiones informadas y para ver peligros o desafíos en situaciones cotidianas. También puede ser más fácil mentir para evitar el estrés o para salir de un escenario difícil.

3. Condiciones sociales: la formación de opiniones y la capacidad de decir la verdad puede ser influenciada por factores sociales, como la exposición a modelos de comportamiento que perpetúan la falta de valiosidad en la verdad y en la honestidad. También puede haber una mayor tendencia a mentirse o a evitar confrontaciones cuando una persona se siente humillada, o cuando no se perciben o no tienen confianza en sus deb
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
