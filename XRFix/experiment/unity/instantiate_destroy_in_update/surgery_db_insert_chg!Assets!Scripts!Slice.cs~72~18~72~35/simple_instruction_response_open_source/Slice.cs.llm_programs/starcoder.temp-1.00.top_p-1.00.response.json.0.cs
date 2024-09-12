Fixed Code:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
	private Mesh mesh;
	private MeshCollider meshCollider;

	private Rigidbody rb2;

	public GameObject prefab;
	private float timeLimit = 5f;
	private float timer  = 0f;
	private bool instantiate_prefab = false;

	void Start()
	{
		var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
		mesh = smr.sharedMesh;
		meshCollider = GetComponent<MeshCollider>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
			{
				List<int> triangles = new List<int>();
				triangles.AddRange(mesh.triangles);

				int startIndex = hit.triangleIndex * 3;
				triangles.RemoveRange(startIndex, 3);

				mesh.triangles = triangles.ToArray();
				meshCollider.sharedMesh = mesh;
			}
		}
	}

	// you can try to build an object pool before Update() method.
	void InstantiatePrefab()
	{
		if (!instantiate_prefab)
		{
			instantiate_prefab = true;
			GameObject new_prefab = Instantiate(prefab, transform.position, transform.rotation);
			Destroy(new_prefab, 10f);
		}
	}

}