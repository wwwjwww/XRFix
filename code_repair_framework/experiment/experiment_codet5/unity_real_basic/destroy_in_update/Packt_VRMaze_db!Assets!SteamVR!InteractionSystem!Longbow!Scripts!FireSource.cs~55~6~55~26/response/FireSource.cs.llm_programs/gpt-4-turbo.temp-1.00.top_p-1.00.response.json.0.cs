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
			if (startActive)
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

					// you can try to build an object pool before Update() method has been called.
					// FIXED CODE:


		private bool isBurningStopping = false; // Tracks if we initiated the stop process

		private float stopTime;

		void Update()
		{
			if ((burnTime != 0) && (Time.time > (ignitionTime + burnTime)) && isBurning && !isBurningStopping)
			{
				isBurning = false;
				isBurningStopping = true;
				stopTime = Time.time;

				if (customParticles != null)
				{
					customParticles.Stop();
				}
				else if (fireObject != null)
				{
					Invoke("DestroyFireObject", 1f); // Defer destruction
				}
			}
		}

		private void DestroyFireObject()
		{
			if (fireObject != null)
			{
				Destroy(fireObject);
				fireObject = null;
				isBurningStopping = false; // reset so we can burn again if needed
			}
		}




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
