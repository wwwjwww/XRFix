//======= Copyright (c) Valve Corporation, All rights reserved. ===============





using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class FireSource : MonoBehaviour
	{
		public GameObject fireParticlePrefab;
		public bool startActive;
		private GameObject fireObject;

		public ParticleSystem customParticles;

		public bool isBurning;

		public float burnTime;
		public float ignitionDelay = 0;
		private float ignitionTime;

		private Hand hand;

		public AudioSource ignitionSound;

		public bool canSpreadFromThisSource = true;


		void Start()
		{
			if ( startActive )
			{
				StartBurning();
			}
		}



/// 		void Update()
// 		{
// 			if ( ( burnTime != 0 ) && ( Time.time > ( ignitionTime + burnTime ) ) && isBurning )
// 			{
// 				isBurning = false;
// 				if ( customParticles != null )
// 				{
// 					customParticles.Stop();
// 				}
// 				else
// 				{
					// BUG: Destroy in Update() method
					// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
					// 					Destroy( fireObject );
					// 				}
					// 			}
					// 		}

					// FIXED CODE:


You can apply for a Canadian passport either online or in person. Here's a step-by-step guide on how to apply for a passport in Canada:

    Gather the required documents: You will need to provide proof of citizenship, identity, and your current address. This could include your birth certificate, citizenship certificate, driver's license, or other government-issued photo ID. You will also need one passport-style photo.

    Choose a processing method: You can apply for a passport by mail, in person at a passport agency, or at a regional passport office. The processing times will vary depending on which method you choose.

    Complete the passport application: You will need to fill out an Application for Canadian Passport or a renewal form online. Be sure to read the instructions carefully and fill out all the required information.

    Submit your application: You can submit your application in person, by mail, or



		//-------------------------------------------------
		void OnTriggerEnter( Collider other )
		{
			if ( isBurning && canSpreadFromThisSource )
			{
				other.SendMessageUpwards( "FireExposure", SendMessageOptions.DontRequireReceiver );
			}
		}


		//-------------------------------------------------
		private void FireExposure()
		{
			if ( fireObject == null )
			{
				Invoke( "StartBurning", ignitionDelay );
			}

			if ( hand = GetComponentInParent<Hand>() )
			{
				hand.controller.TriggerHapticPulse( 1000 );
			}
		}


		//-------------------------------------------------
		private void StartBurning()
		{
			isBurning = true;
			ignitionTime = Time.time;

			// Play the fire ignition sound if there is one
			if ( ignitionSound != null )
			{
				ignitionSound.Play();
			}

			if ( customParticles != null )
			{
				customParticles.Play();
			}
			else
			{
				if ( fireParticlePrefab != null )
				{
					fireObject = Instantiate( fireParticlePrefab, transform.position, transform.rotation ) as GameObject;
					fireObject.transform.parent = transform;
				}
			}
		}
	}
}
