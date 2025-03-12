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
﻿		}






﻿		void Update()
﻿		{
﻿			if (hand.IsTracked())
﻿			{
﻿				GameObject item = Instantiate(itemPrefab) as GameObject;
﻿				item.transform.SetParent(hand.transform);
﻿				item.transform.localPosition = Vector3.zero;
﻿				item.transform.localRotation = Quaternion.identity;
﻿			}
﻿		}


	}
}
