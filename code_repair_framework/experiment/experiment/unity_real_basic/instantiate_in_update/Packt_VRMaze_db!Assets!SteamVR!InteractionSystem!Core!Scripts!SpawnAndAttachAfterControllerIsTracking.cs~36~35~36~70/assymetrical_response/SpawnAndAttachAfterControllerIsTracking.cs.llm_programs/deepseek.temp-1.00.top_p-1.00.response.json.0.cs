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






void SpawnAndAttach()
		{
			// Spawn GameObject
			GameObject spawnedItem = Instantiate(itemPrefab);

			// Attach item to the hand
			var attachTransform = spawnedItem.GetComponent<IVRAttachable>()?.attachTransform;
			if (attachTransform)
			{
				var attachment = hand.AttachObject(spawnedItem, attachTransform);
			}
			else
		{
			// If no attach transform found, clean up the spawned item
			Destroy(spawnedItem);
		}
	}

	void Update()
	{
		if (!hand.controller.isTracking)
		{
			SpawnAndAttach();
		}
	}


	}
}
