using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour {
    public void Startup() {
        // FIXED CODE: use UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        SceneManager.LoadScene("TitleScene");
    }
}
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
