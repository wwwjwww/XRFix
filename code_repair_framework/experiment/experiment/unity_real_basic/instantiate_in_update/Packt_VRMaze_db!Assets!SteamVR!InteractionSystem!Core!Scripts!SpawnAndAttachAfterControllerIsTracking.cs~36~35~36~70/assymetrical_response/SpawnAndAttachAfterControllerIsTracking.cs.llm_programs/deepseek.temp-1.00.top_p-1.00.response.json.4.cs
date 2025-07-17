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
        if (hand.IsTracking())
        {
            if (Input.GetMouseButtonDown(0))  // replace 0 with whatever button you want to use for spawning
            {
                // Spawn and attach the item to the hand
                GameObject spawnedObject = Instantiate(itemPrefab, transform.position, transform.rotation);
                spawnedObject.transform.SetParent(hand.transform);
            }
        }
    }


	}
}
