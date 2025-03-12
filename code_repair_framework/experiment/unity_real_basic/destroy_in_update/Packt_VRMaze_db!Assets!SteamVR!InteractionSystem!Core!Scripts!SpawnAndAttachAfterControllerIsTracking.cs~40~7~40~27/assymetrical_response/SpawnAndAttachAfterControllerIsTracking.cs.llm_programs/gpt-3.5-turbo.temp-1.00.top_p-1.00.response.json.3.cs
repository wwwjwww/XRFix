//======= Copyright (c) Valve Corporation, All rights reserved. ===============






using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
		private Hand hand;
		public GameObject itemPrefab;

	

		void Start()
		{
			hand = GetComponentInParent<Hand>();
		}






void Update()
		{
			if ( itemPrefab != null && hand.controller != null && hand.controller.hasTracking )
			{
				GameObject objectToAttach = GameObject.Instantiate( itemPrefab );
				objectToAttach.SetActive( true );
				hand.AttachObject( objectToAttach );
				hand.controller.TriggerHapticPulse( 800 );
				Destroy( gameObject );
				objectToAttach.transform.localScale = itemPrefab.transform.localScale;
			}
		}


	}
}
