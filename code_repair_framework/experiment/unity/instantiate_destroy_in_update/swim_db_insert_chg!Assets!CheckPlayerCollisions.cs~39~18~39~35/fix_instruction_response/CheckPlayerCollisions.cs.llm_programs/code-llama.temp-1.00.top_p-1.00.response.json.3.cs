// Assets/CheckPlayerCollisions.cs
using UnityEngine;
using System.Collections;

public class CheckPlayerCollisions : MonoBehaviour
{
    public GameObject player;
    public GameObject ground;
    public GameObject wall;

    void Update()
    {
        if (ground.GetComponent<Collider2D>().IsTouchingLayers(player.GetComponent<Collider2D>()))
        {
            Debug.Log("Player is touching the ground");
        }
        if (wall.GetComponent<Collider2D>().IsTouchingLayers(player.GetComponent<Collider2D>()))
        {
            Debug.Log("Player is touching the wall");
        }
    }


    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision with " + collision.name);
        if (collision.name == "DeathZone")
        {
            deathText.SetActive(true);
            winText.SetActive(false);
        } else if (collision.name == "WinZone")
        {
            winText.SetActive(true);
            deathText.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "ForceField")
        {
            rb.AddForce(collision.transform.forward * ripForce);
        }
    }
}
