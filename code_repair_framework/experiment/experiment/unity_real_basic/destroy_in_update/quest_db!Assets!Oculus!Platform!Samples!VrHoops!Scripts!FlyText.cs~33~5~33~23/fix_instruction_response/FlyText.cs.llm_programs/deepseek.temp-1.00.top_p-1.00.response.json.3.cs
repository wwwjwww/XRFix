namespace Oculus.Platform.Samples.VrHoops
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;


	public class FlyText : MonoBehaviour
	{

		private const float LIFESPAN = 3.0f;


		private readonly Vector3 m_movePerFrame = 0.5f * Vector3Vector3.up; m_eol;


		private float m_eol;

		void Start()
		{
			if (pooledObjects.Count == 0)
			{
				for (int i = 0; i < amountToPool; i++)
				{
					GameObject obj = Instantiate(gameObject);
					obj.SetActive(false);
					pooledObjects.Add(obj);
				}
			}

			GetComponent<Text>().CrossFadeColor(Color.black, LIFESPAN * 1.7f, false, true);
		}

/// 		void Update()
// 		{
// 			if (Time.time < m_eol)
// 			{
// 				transform.localPosition += m_movePerFrame;
// 			}
// 			else
// 			{
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Destroy(gameObject);
				// 			}
				// 		}

				// FIXED CODE:


		private static List<GameObject> pooledObjects = new List<GameObject>();

		private static int amountToPool = 10;

		void Update()
		{
			if (Time.time < m_eol)
			{
				transform.localPosition += m_movePerFrame;
			}
			else
			{
				if(this.gameObject.activeInHierarchy)
				{
					gameObject.SetActive(false);
					pooledObjects.Add(gameObject);
				}
			}
		}

		public static GameObject GetPooledObject()
		{
			for (int i = 0; i < pooledObjects.Count; i++)
			{
				if (!pooledObjects[i].activeInHierarchy)
				{
					return pooledObjects[i];
				}
			}
			return null;
		}


	}
}
