
 		void Update()
 		{
 			if (Time.time < m_eol)
 			{
 				transform.localPosition += m_movePerFrame;
 			}
 			else
 			{
 				planeObject.SetActive(false);
 			}
 		}
