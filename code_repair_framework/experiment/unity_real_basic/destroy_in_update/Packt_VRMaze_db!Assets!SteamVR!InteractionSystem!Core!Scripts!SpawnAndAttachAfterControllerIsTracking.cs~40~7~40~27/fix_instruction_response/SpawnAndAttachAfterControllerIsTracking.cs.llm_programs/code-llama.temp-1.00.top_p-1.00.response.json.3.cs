//======= Copyright (c) Valve Corporation, All rights reserved. ===============






using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
﻿		private Hand hand;
﻿		public GameObject itemPrefab;

	

﻿		void Start()
﻿		{
﻿			hand = GetComponentInParent<Hand>();
﻿
﻿			objectToAttach = GameObject.Instantiate(itemPrefab);
﻿			objectToAttach.SetActive(true);
﻿		}



/// 		void Update()
// 		{
// 			if ( itemPrefab != null )
// 			{
// 				if ( hand.controller != null )
// 				{
// 					if ( hand.controller.hasTracking )
// 					{
// 						GameObject objectToAttach = GameObject.Instantiate( itemPrefab );
// 						objectToAttach.SetActive( true );
// 						hand.AttachObject( objectToAttach );
// 						hand.controller.TriggerHapticPulse( 800 );
						// BUG: Destroy in Update() method
						// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
						// 						Destroy( gameObject );
						// 
						// 
						// 
						// 						objectToAttach.transform.localScale = itemPrefab.transform.localScale;
						// 					}
						// 				}
						// 			}
						// 		}

						// FIXED CODE:


﻿		﻿private GameObject objectToAttach;

﻿		void Update()
﻿		{
﻿			if (hand.controller != null && hand.controller.hasTracking)
﻿			{
﻿				hand.AttachObject(objectToAttach);
﻿				hand.controller.TriggerHapticPulse(800);
﻿			}
﻿		}

﻿		void OnDestroy()
﻿		{
﻿			if (objectToAttach != null)
﻿			{
﻿				GameObject.Destroy(objectToAttach);
﻿			}
﻿		}


	}
}
