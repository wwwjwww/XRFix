using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void LoadUserData()
    {
        // FIXED CODE: Added a null check for the playerPrefs to prevent errors
        if (PlayerPrefs.HasKey("username"))
        {
            // FIXED CODE: Changed the way the player data is loaded into the class
            username = PlayerPrefs.GetString("username");
            age = PlayerPrefs.GetInt("age");
        }
    }

    public void SaveUserData()
    {
        // FIXED CODE: Added a null check for the player data to prevent errors
        if (username != null && age != null)
        {
            // FIXED CODE: Changed the way the player data is saved to the class
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetInt("age", age);
        }
        else
        {
            Debug.LogWarning("Error saving player data. Username and age are required");
        }
    }

    public string username;
    public int age;
}
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
