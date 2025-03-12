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
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(Rigidbody))]
public class OVRGrabber : MonoBehaviour
{

    public float grabBegin = 0.55f; // The grab strength multiplier applied when the grab button is first pressed.
    public float grabEnd = 0.35f; // The grab strength multiplier applied when the grab button is held down.






    [SerializeField]
    protected OVRGrabbable grabbable;







    [SerializeField]
    protected OVRGrabbable grabbable;



    [SerializeField]
    protected OVRGrabbable grabbable;


    [SerializeField]
    protected OVRGrabbable grabbable;


    [SerializeField]
    protected OVRGrabbable grabbable;




    [SerializeField]
    protected OVRGrabbable grabbable;

    [SerializeField]
    protected OVRGrabbable grabbable;

    protected bool m_grabVolumeEnabled = true;
    protected Vector3 m_lastPos;
    protected Quaternion m_lastRot;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;
    protected float m_prevFlex;
    protected OVRGrabbable m_grabbedObj = null;
    protected Vector3 m_grabbedObjectPosOff;
    protected Quaternion m_grabbedObjectRotOff;
    private Dictionary<OVRGrabbable, int> _grabCandidates = new Dictionary<OVRGrabbable, int>(); // A map of available grab candidates and their grab strength modifiers.
    protected bool m_operatingWithoutOVRCameraRig = true;




    public OVRGrabbable grabbedObject
    {
        get { return m_grabbedObj; }
    }

    public void ForceRelease(OVRGrabbable grabbable)
    {
        // Check if the specified object is currently being held by the grabber.
        bool canRelease = (_grabbedObj!= null && _grabbedObj == grabbable);
        if (canRelease)
        {
            // If so, end the grab operation.
            GrabEnd();
        }
    }

    protected virtual void Awake()
    {
        // Set the initial position and rotation of the grabber.
        transform.position = _lastPos;
        transform.rotation = _lastRot;

        // Get the grab transform and grab volumes from the inspector.
        _gripTransform = transform.Find("GripPoint");
        _grabVolumes = GetComponentsInChildren<Collider>();

        // Set the anchor offset properties based on the inspector values.
        _anchorOffsetPosition = transform.localPosition;
        _anchorOffsetRotation = transform.localRotation;

        // Get the parent transform and player object from the inspector.
        _parentTransform = transform.parent;
        _player = GameObject.Find("Player");

        // Check if the OVRCameraRig component is present in the hierarchy.
        OVRCameraRig rig = transform.GetComponentInParent<OVRCameraRig>();
        if (rig!= null)
        {
            // If so, subscribe to the UpdatedAnchors event and set the flag to indicate that we are operating with a rig.
            rig.UpdatedAnchors += (r) => { OnUpdatedAnchors(); };
            _operatingWithoutOVRCameraRig = false;
        }
    }

    protected virtual void Start()
    {
        // Set the initial position and rotation of the grabber.
        _lastPos = transform.position;
        _lastRot = transform.rotation;

        // Check if the parent transform is set to null, and if so, use the transform of the game object itself.
        if (_parentTransform == null)
        {
            _parentTransform = transform;
        }

        // Set the player object as an ignore collision to prevent collisions with the grabbed object.
        SetPlayerIgnoreCollision(gameObject, true);
    }











// BUG: Transform object of Rigidbody in Update() methods
// MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
//    virtual public void Update()
//    {
//        if (m_operatingWithoutOVRCameraRig)
//        {
//            OnUpdatedAnchors();
//        }
//    }








    private bool _parentHeldObject = false; // If true, the grabbed object is parented to the grabber rather than having its own transform.

    private bool _moveHandPosition = false; // If true, the grabber's position is moved to match the camera's position.

    private Transform _gripTransform; // The transform that represents the location of the grab button.

    private Collider[] _grabVolumes; // The list of colliders used to detect when objects are within grab range.

    private OVRInput.Controller _controller; // The controller that is used for grab interactions.

    private Transform _parentTransform; // The transform that is used as the parent for the grabbed object.

    private GameObject _player; // The player object that is used to check for collisions with the grabbed object.

    private bool _grabVolumeEnabled = true; // If false, the grab volumes are ignored and the grab button must be manually pressed to initiate a grab.

    private Vector3 _lastPos; // The last known position of the grabber.

    private Quaternion _lastRot; // The last known rotation of the grabber.

    private Quaternion _anchorOffsetRotation; // The rotation offset applied to the grabber's transform.

    private Vector3 _anchorOffsetPosition; // The position offset applied to the grabber's transform.

    private float _prevFlex; // The previous value of the primary hand trigger.

    private OVRGrabbable _grabbedObj; // The object that is currently being grabbed, or null if no object is being grabbed.

    private Vector3 _grabbedObjectPosOff; // The position offset applied to the grabbed object.

    private Quaternion _grabbedObjectRotOff; // The rotation offset applied to the grabbed object.

    private bool _operatingWithoutOVRCameraRig = true; // If true, the OVRCameraRig component is not present in the hierarchy.

    protected virtual void Update()
    {
        // If we are operating without an OVRCameraRig, update the anchor positions manually.
        if (_operatingWithoutOVRCameraRig)
        {
            OnUpdatedAnchors();
        }

        // Get the current value of the primary hand trigger.
        _prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller);

        // Check for grab or release based on the previous and current trigger values.
        CheckForGrabOrRelease(_prevFlex);
    }



    void OnDestroy()
    {
        if (m_grabbedObj != null)
        {
            GrabEnd();
        }
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger
        OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ??
                                 otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        // Add the grabbable
        int refCount = 0;
        m_grabCandidates.TryGetValue(grabbable, out refCount);
        m_grabCandidates[grabbable] = refCount + 1;
    }

    void OnTriggerExit(Collider otherCollider)
    {
        OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ??
                                 otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        // Remove the grabbable
        int refCount = 0;
        bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
        if (!found)
        {
            return;
        }

        if (refCount > 1)
        {
            m_grabCandidates[grabbable] = refCount - 1;
        }
        else
        {
            m_grabCandidates.Remove(grabbable);
        }
    }

    protected void CheckForGrabOrRelease(float prevFlex)
    {
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        {
            GrabBegin();
        }
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            GrabEnd();
        }
    }

    protected virtual void GrabBegin()
    {
        float closestMagSq = float.MaxValue;
        OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
        foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            }

            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Set up offsets for grabbed object desired position relative to hand.
            if (m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if (m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // NOTE: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);

            // NOTE: This is to get around having to setup collision layers, but in your own project you might
            // choose to remove this line in favor of your own collision layer setup.
            SetPlayerIgnoreCollision(m_grabbedObj.gameObject, true);

            if (m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }

    protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
    {
        if (m_grabbedObj == null)
        {
            return;
        }

        Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
        Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
        Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

        if (forceTeleport)
        {
            grabbedRigidbody.transform.position = grabbablePosition;
            grabbedRigidbody.transform.rotation = grabbableRotation;
        }
        else
        {
            grabbedRigidbody.MovePosition(grabbablePosition);
            grabbedRigidbody.MoveRotation(grabbableRotation);
        }
    }

    protected void GrabEnd()
    {
        if (m_grabbedObj != null)
        {
            OVRPose localPose = new OVRPose
            {
                position = OVRInput.GetLocalControllerPosition(m_controller),
                orientation = OVRInput.GetLocalControllerRotation(m_controller)
            };
            OVRPose offsetPose = new OVRPose
            {
                position = m_anchorOffsetPosition,
                orientation = m_anchorOffsetRotation
            };
            localPose = localPose * offsetPose;

            OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
            Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
            Vector3 angularVelocity =
                trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

            GrabbableRelease(linearVelocity, angularVelocity);
        }

        // Re-enable grab volumes to allow overlap events
        GrabVolumeEnable(true);
    }

    protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
        if (m_parentHeldObject) m_grabbedObj.transform.parent = null;
        m_grabbedObj = null;
    }

    protected virtual void GrabVolumeEnable(bool enabled)
    {
        if (m_grabVolumeEnabled == enabled)
        {
            return;
        }

        m_grabVolumeEnabled = enabled;
        for (int i = 0; i < m_grabVolumes.Length; ++i)
        {
            Collider grabVolume = m_grabVolumes[i];
            grabVolume.enabled = m_grabVolumeEnabled;
        }

        if (!m_grabVolumeEnabled)
        {
            m_grabCandidates.Clear();
        }
    }

    protected virtual void OffhandGrabbed(OVRGrabbable grabbable)
    {
        if (m_grabbedObj == grabbable)
        {
            GrabbableRelease(Vector3.zero, Vector3.zero);
        }
    }

    protected void SetPlayerIgnoreCollision(GameObject grabbable, bool ignore)
    {
        if (m_player != null)
        {
            Collider[] playerColliders = m_player.GetComponentsInChildren<Collider>();
            foreach (Collider pc in playerColliders)
            {
                Collider[] colliders = grabbable.GetComponentsInChildren<Collider>();
                foreach (Collider c in colliders)
                {
                    if (!c.isTrigger && !pc.isTrigger)
                        Physics.IgnoreCollision(c, pc, ignore);
                }
            }
        }
    }
}
