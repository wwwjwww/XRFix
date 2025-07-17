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




		void Update()
		{
			// Move the text based on its initial velocity
			transform.Translate(movePerFrame * Time.deltaTime);

			// If the text has reached the end of its life, destroy it
			if (Time.time >= m_eol)
			{
				Destroy(gameObject);
			}
		}


	}
}
