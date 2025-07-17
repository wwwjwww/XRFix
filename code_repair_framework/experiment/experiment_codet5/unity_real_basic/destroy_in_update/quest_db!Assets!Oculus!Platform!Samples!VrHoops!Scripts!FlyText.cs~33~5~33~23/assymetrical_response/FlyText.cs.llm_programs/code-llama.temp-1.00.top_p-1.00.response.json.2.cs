namespace Oculus.Platform.Samples.VrHoops
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;


	public class FlyText : MonoBehaviour
	{

		private const float LIFESPAN = 3.0f;


		private readonly Vector3 m_movePerFrame = 0.5f * Vector3.up;


		private float m_eol = 0;

		void Start()
		{
			m_eol = Time.time + LIFESPAN;
			GetComponent<Text>().CrossFadeColor(Color.black, LIFESPAN * 1.7f, false, true);
		}




		void Update()
		{
			if (Time.time >= m_eol)
			{
				Destroy(gameObject);
			}
			else
			{
				float dt = Time.deltaTime;
				float speed = m_movePerFrame * dt;
				transform.Position = Vector3.MoveTowards(transform.Position,
					transform.forward * 10,
					speed);
			}
		}


	}
}
