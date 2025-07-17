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
			GetComponent<Text>().CrossFadeColor(Color.black, LIFESPAN, false, true);
		}




		void Update()
		{
			if (m_eol < Time.time)
			{
				Destroy(gameObject);
			}
			else
			{
				transform.position -= m_movePerFrame * Time.deltaTime;
			}
		}

In this code snippet, the issue is that the text's lifetime is not properly controlled. The `m_eol` variable is used to track the time at which the text will expire, but the `m_eol` variable is not updated in the `Update()` method. To fix this issue, we can update the `m_eol` variable in the `Update()` method to ensure that the text expires after the desired lifetime.

		void Update()
		{
			m_eol = Time.time + LIFESPAN;
			if (m_eol < Time.time)
			{
				Destroy(gameObject);
			}
			else
			{
				transform.position -= m_movePerFrame * Time.deltaTime;
			}
		}


	}
}
