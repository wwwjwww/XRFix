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
            StartCoroutine(LifeSpanRoutine());
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

				// you can try to build an object pool before Update() method has been called.
				// FIXED CODE:


        private bool isDestroyed = false;

        private IEnumerator LifeSpanRoutine()
        {
            while (Time.time < m_eol)
            {
                transform.localPosition += m_movePerFrame;
                yield return null;
            }

            Destroy(gameObject);
        }


	}
}
