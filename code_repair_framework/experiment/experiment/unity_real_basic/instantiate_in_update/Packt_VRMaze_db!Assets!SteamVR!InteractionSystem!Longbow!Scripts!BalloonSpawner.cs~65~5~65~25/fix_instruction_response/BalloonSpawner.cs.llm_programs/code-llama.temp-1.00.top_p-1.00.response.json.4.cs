//======= Copyright (c) Valve Corporation, All rights reserved. ===============





using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class BalloonSpawner : MonoBehaviour
	{
		public float minSpawnTime = 5f;
		public float maxSpawnTime = 15f;
		private float nextSpawnTime;
		public GameObject balloonPrefab;

		public bool autoSpawn = true;
		public bool spawnAtStartup = true;

		public bool playSounds = true;
		public SoundPlayOneshot inflateSound;
		public SoundPlayOneshot stretchSound;

		public bool sendSpawnMessageToParent = false;

		public float scale = 1f;

		public Transform spawnDirectionTransform;
		public float spawnForce;

		public bool attachBalloon = false;

		public Balloon.BalloonColor color = Balloon.BalloonColor.Random;



 void Start () {

if ( balloonPrefab == null ) {

return; }

balloon = Instantiate( balloonPrefab, transform.position, transform.rotation ) as GameObject;

balloon.transform.localScale = new Vector3( scale, scale, scale );

if ( attachBalloon ) {

balloon.transform.parent = transform; }

if ( sendSpawnMessageToParent ) { 

if ( transform.parent != null ) {

transform.parent.SendMessage( "OnBalloonSpawned", balloon, SendMessageOptions.DontRequireReceiver ); } }

if ( playSounds ) {

if ( inflateSound != null ) {

inflateSound.Play(); }

if ( stretchSound != null ) {

stretchSound.Play(); } }

balloon.GetComponentInChildren<Balloon>().SetColor( color );

if ( spawnDirectionTransform != null ) {

balloon.GetComponentInChildren<Rigidbody>().AddForce( spawnDirectionTransform.forward * spawnForce ); } }




// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//		void Update()
//		{
//			if ( balloonPrefab == null )
//			{
//				return;
//			}
//
//			if ( ( Time.time > nextSpawnTime ) && autoSpawn )
//			{
//				SpawnBalloon( color );
//				nextSpawnTime = Random.Range( minSpawnTime, maxSpawnTime ) + Time.time;
//			}
//		}







Here's the fixed code snippet:

// FIXED CODE:

private GameObject balloon; 

public GameObject SpawnBalloon( Balloon.BalloonColor color = Balloon.BalloonColor.Red ) {

gameObject.SetActive( false );

if ( spawnDirectionTransform != null ) {

transform.position = spawnDirectionTransform.position; }

gameObject.SetActive( true );

return balloon; } }

You need to create a new object of the balloon class and set its properties before you return it. Also, you can try to use the gameObject.SetActive( false ) call to disable this object before you play your sound (it will have no visible effect but will eliminate the sound of the game object sounding in the background).



		//-------------------------------------------------
		public void SpawnBalloonFromEvent( int color )
		{
			// Copy of SpawnBalloon using int because we can't pass in enums through the event system
			SpawnBalloon( (Balloon.BalloonColor)color );
		}
	}
}
