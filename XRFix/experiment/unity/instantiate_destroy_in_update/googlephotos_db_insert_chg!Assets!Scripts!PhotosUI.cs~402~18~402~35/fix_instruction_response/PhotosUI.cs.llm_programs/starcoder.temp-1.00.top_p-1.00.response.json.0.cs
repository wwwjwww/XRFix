// Instantiate/Destroy in Update() method
using UnityEngine;

public class InstantiateDestroyTest : MonoBehaviour
{
	public float timeLimit = 5f;
	public GameObject gobj;
	private float timer;
	private bool instantiate_gobj;

	// Start is called before the first frame update
	void Start()
	{
		instantiate_gobj = false;
		timer = 0f;
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;

		if (!instantiate_gobj && timer >= timeLimit)
		{
			GameObject a3 = Instantiate(gobj);
			timer = 0f;
			instantiate_gobj = true;
		}
		if (instantiate_gobj && timer >= timeLimit)
		{
			Destroy(a3);
			timer = 0f;
			instantiate_gobj = false;
		}
	}


    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
