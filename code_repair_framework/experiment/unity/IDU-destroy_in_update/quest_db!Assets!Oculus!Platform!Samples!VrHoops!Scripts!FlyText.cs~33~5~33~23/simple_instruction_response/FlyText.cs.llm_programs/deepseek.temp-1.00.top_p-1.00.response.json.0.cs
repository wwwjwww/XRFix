namespace Oculus.Platform.Samples.VrHoops
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;


	public class FlyText : MonoBehaviour
	{

		private const float LIFESPAN = 3.0f;


		private readonly Vector3 m_movePerFrame = 0.5f * Vector3.up;


		private float m_eol;

		void Start()
		{
			m_eol = Time.time + LIFESPAN;
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
				// 				Destroy(gameObject);
				// 			}
				// 		}

				// FIXED CODE:


void Update()
		{
			if (Time.time < m_eol)
			{
				transform.localPosition += m_movePerFrame;
			}
			else
			{
				StartCoroutine(FadeAndDestroy());
			}
		}

		IEnumerator FadeAndDestroy()
		{
			GetComponent<Text>().CrossFadeColor(Color.clear, LIFESPAN * 1.7f, false, true);
			yield return new WaitForSeconds(LIFESPAN * 1.8f);
			Destroy(gameObject);
		}


	}
}
