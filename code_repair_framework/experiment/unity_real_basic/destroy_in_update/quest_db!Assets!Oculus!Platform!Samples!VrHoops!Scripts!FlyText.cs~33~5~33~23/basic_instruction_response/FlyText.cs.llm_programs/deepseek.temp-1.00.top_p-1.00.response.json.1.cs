namespace Oculus.Platform.Samples.VrHoops
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;


	public class FlyText : MonoBehaviour
	{

    private const float LIFESPAN = 3.0f;


    private readonly Vector3 m_movePerFrame = 0.5f * Vector3;


		private float m_eol;

    private void Start()
    {
        m_text = GetComponent<Text>();
        StartCoroutine(FadeAndDestroyAfter(LIFESPAN * 1.7f));
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


    private GameObject m_text;

    private IEnumerator FadeAndDestroyAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_text.CrossFadeColor(Color.black, 0, false, true);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.localPosition += m_movePerFrame;
    }


	}
}
