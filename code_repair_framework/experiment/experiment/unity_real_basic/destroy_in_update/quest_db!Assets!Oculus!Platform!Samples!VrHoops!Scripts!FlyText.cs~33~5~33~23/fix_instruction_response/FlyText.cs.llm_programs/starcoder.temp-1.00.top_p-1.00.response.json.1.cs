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

        private void Start()
        {
            eol = Time.time + LIFESPAN;
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


        public float LIFESPAN = 3.0f;

        public Vector3 movePerFrame = 0.5f * Vector3.up;

        private float eol;

        private void Update()
        {
            if (Time.time < eol)
            {
                transform.localPosition += movePerFrame;
            }
            else
            {
                // Destroy in late update
                Invoke("Destroy", LIFESPAN);
            }
        }


	}
}
