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




The above code implements the `Update` method in the `FlyText` class. This method will be called once per frame while the MonoBehaviour is enabled. In this method, the position of the game object is updated by adding the `m_movePerFrame` to the current position. This will cause the text to fly in the upward direction over time.

The lifespan of the text is checked in each frame. If the current time is greater than `m_eol` (which is the time the text was spawned plus the lifespan), the game object will be destroyed. The `Destroy(gameObject);` line of code will destroy the game object this script is attached to, effectively removing it from the scene.

	}
}
