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
        hand = GetComponent<Hand>();
    }






    void Update()
    {
        if (hand && hand.controller.isTracked)
        {
            // Spawn prefab at the hand's position and rotation
            Instantiate(itemPrefab, hand.transform.position, hand.transform.rotation);
        }
    }


	}
}
