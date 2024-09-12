using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCollisions : MonoBehaviour
{
    public GameObject player;
    public GameObject[] enemies;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void FixedUpdate()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (player.transform.position.x < enemies[i].transform.position.x)
                player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(enemies[i].transform.position.x, player.transform.position.y, player.transform.position.z), Time.deltaTime * 5);
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
