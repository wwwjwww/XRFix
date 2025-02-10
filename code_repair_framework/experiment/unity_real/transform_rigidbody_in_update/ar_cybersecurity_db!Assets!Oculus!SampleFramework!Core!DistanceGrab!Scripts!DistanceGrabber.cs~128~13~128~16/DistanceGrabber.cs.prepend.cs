/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https:
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif


namespace OculusSampleFramework
{
    
    
    
    [RequireComponent(typeof(Rigidbody))]
    public class DistanceGrabber : OVRGrabber
    {
        
        [SerializeField]
        float m_spherecastRadius = 0;

        
        
        [SerializeField]
        float m_noSnapThreshhold = 0.05f;

        [SerializeField]
        bool m_useSpherecast;

        public bool UseSpherecast
        {
            get { return m_useSpherecast; }
            set
            {
                m_useSpherecast = value;
                GrabVolumeEnable(!m_useSpherecast);
            }
        }

        
        [SerializeField]
        public bool m_preventGrabThroughWalls;

        [SerializeField]
        float m_objectPullVelocity = 10.0f;

        float m_objectPullMaxRotationRate = 360.0f; 

        bool m_movingObjectToHand = false;

        
        [SerializeField]
        float m_maxGrabDistance;

        
        
        [SerializeField]
        int m_grabObjectsInLayer = 0;

        [SerializeField]
        int m_obstructionLayer = 0;

        DistanceGrabber m_otherHand;

        protected DistanceGrabbable m_target;

        
        protected Collider m_targetCollider;

        protected override void Start()
        {
            base.Start();

            
            
            
            Collider sc = m_player.GetComponentInChildren<Collider>();
            if (sc != null)
            {
                m_maxGrabDistance = sc.bounds.size.z * 0.5f + 3.0f;
            }
            else
            {
                m_maxGrabDistance = 12.0f;
            }

            if (m_parentHeldObject == true)
            {
                Debug.LogError("m_parentHeldObject incompatible with DistanceGrabber. Setting to false.");
                m_parentHeldObject = false;
            }

            DistanceGrabber[] grabbers = FindObjectsOfType<DistanceGrabber>();
            for (int i = 0; i < grabbers.Length; ++i)
            {
                if (grabbers[i] != this) m_otherHand = grabbers[i];
            }

            Debug.Assert(m_otherHand != null);

#if UNITY_EDITOR
            OVRPlugin.SendEvent("distance_grabber", (SceneManager.GetActiveScene().name == "DistanceGrab").ToString(),
                "sample_framework");
#endif
        }

