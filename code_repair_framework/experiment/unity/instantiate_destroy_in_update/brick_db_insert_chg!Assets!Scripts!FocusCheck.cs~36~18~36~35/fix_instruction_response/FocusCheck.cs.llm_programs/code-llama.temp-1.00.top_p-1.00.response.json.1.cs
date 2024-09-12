using UnityEngine;

public class FocusCheck : MonoBehaviour
{
    // Pool contains pre-instantiated objects
    public GameObject[] pool;

    // The index of the next object to be destroyed
    public int nextObjectIndex = 0;

    // The index of the next object to be instantiated
    public int nextPoolIndex = 0;

    // Text component to display status message
    public TextMeshProUGUI text;

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Start()
    {
        // Initialize pool
        foreach (GameObject obj in pool)
        {
            obj.SetActive(false);
        }
        // Set text message
        text.text = $"Pool size: {pool.Length}";
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (instantiate_gobj && timer >= timeLimit)
        {
            // Instantiate a new object
            GameObject obj = pool[nextPoolIndex];
            obj.SetActive(true);

            // Update next pool index
            nextPoolIndex = (nextPoolIndex + 1) % pool.Length;

            // Display status message
            text.text = $"Pool size: {pool.Length}\nNext index: {nextPoolIndex}";

            return;
        }

        if (instantiate_gobj && timer >= timeLimit)
        {
            // Destroy the current object
            Destroy(pool[nextObjectIndex]);

            // Update next object index
            nextObjectIndex = (nextObjectIndex + 1) % pool.Length;

            // Display status message
            text.text = $"Pool size: {pool.Length}\nNext index: {nextPoolIndex}";
        }
    }
}

