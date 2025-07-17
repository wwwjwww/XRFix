
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
