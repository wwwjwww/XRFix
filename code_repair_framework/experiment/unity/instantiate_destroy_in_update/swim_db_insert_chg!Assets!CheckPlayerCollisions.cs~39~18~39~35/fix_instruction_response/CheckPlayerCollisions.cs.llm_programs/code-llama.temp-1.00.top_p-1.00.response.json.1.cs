using UnityEngine;

public class CheckPlayerCollisions : MonoBehaviour
{
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerCollisions();
    }

    void CheckPlayerCollisions()
    {
        // Check for and resolve player collisions with objects in the scene
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            if (hit.collider.gameObject.tag == "Solid")
            {
                // Resolve collision with the solid object
                player.transform.position = hit.collider.gameObject.transform.position + Vector3.up;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            // Collect the pickup
            other.gameObject.transform.SetParent(player.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            // Drop the pickup
            other.gameObject.transform.SetParent(null);
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
