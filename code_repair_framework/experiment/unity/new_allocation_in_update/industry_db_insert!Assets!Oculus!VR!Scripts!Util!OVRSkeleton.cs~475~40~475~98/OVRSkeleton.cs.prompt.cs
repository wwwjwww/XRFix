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

using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class OVRSkeleton : MonoBehaviour
{
    public interface IOVRSkeletonDataProvider
    {
        SkeletonType GetSkeletonType();
        SkeletonPoseData GetSkeletonPoseData();
        bool enabled { get; }
    }

    public struct SkeletonPoseData
    {
        public OVRPlugin.Posef RootPose { get; set; }
        public float RootScale { get; set; }
        public OVRPlugin.Quatf[] BoneRotations { get; set; }
        public bool IsDataValid { get; set; }
        public bool IsDataHighConfidence { get; set; }
        public OVRPlugin.Vector3f[] BoneTranslations { get; set; }
        public int SkeletonChangedCount { get; set; }
    }

    public enum SkeletonType
    {
        None = OVRPlugin.SkeletonType.None,
        HandLeft = OVRPlugin.SkeletonType.HandLeft,
        HandRight = OVRPlugin.SkeletonType.HandRight,
        Body = OVRPlugin.SkeletonType.Body,
    }

    public enum BoneId
    {
        Invalid                 = OVRPlugin.BoneId.Invalid,

        // hand bones
        Hand_Start              = OVRPlugin.BoneId.Hand_Start,
        Hand_WristRoot          = OVRPlugin.BoneId.Hand_WristRoot,          // root frame of the hand, where the wrist is located
        Hand_ForearmStub        = OVRPlugin.BoneId.Hand_ForearmStub,        // frame for user's forearm
        Hand_Thumb0             = OVRPlugin.BoneId.Hand_Thumb0,             // thumb trapezium bone
        Hand_Thumb1             = OVRPlugin.BoneId.Hand_Thumb1,             // thumb metacarpal bone
        Hand_Thumb2             = OVRPlugin.BoneId.Hand_Thumb2,             // thumb proximal phalange bone
        Hand_Thumb3             = OVRPlugin.BoneId.Hand_Thumb3,             // thumb distal phalange bone
        Hand_Index1             = OVRPlugin.BoneId.Hand_Index1,             // index proximal phalange bone
        Hand_Index2             = OVRPlugin.BoneId.Hand_Index2,             // index intermediate phalange bone
        Hand_Index3             = OVRPlugin.BoneId.Hand_Index3,             // index distal phalange bone
        Hand_Middle1            = OVRPlugin.BoneId.Hand_Middle1,            // middle proximal phalange bone
        Hand_Middle2            = OVRPlugin.BoneId.Hand_Middle2,            // middle intermediate phalange bone
        Hand_Middle3            = OVRPlugin.BoneId.Hand_Middle3,            // middle distal phalange bone
        Hand_Ring1              = OVRPlugin.BoneId.Hand_Ring1,              // ring proximal phalange bone
        Hand_Ring2              = OVRPlugin.BoneId.Hand_Ring2,              // ring intermediate phalange bone
        Hand_Ring3              = OVRPlugin.BoneId.Hand_Ring3,              // ring distal phalange bone
        Hand_Pinky0             = OVRPlugin.BoneId.Hand_Pinky0,             // pinky metacarpal bone
        Hand_Pinky1             = OVRPlugin.BoneId.Hand_Pinky1,             // pinky proximal phalange bone
        Hand_Pinky2             = OVRPlugin.BoneId.Hand_Pinky2,             // pinky intermediate phalange bone
        Hand_Pinky3             = OVRPlugin.BoneId.Hand_Pinky3,             // pinky distal phalange bone
        Hand_MaxSkinnable       = OVRPlugin.BoneId.Hand_MaxSkinnable,
        // Bone tips are position only. They are not used for skinning but are useful for hit-testing.
        // NOTE: Hand_ThumbTip == Hand_MaxSkinnable since the extended tips need to be contiguous
        Hand_ThumbTip           = OVRPlugin.BoneId.Hand_ThumbTip,           // tip of the thumb
        Hand_IndexTip           = OVRPlugin.BoneId.Hand_IndexTip,           // tip of the index finger
        Hand_MiddleTip          = OVRPlugin.BoneId.Hand_MiddleTip,          // tip of the middle finger
        Hand_RingTip            = OVRPlugin.BoneId.Hand_RingTip,            // tip of the ring finger
        Hand_PinkyTip           = OVRPlugin.BoneId.Hand_PinkyTip,           // tip of the pinky
        Hand_End                = OVRPlugin.BoneId.Hand_End,

        // body bones
        Body_Start                       = OVRPlugin.BoneId.Body_Start,
        Body_Root                        = OVRPlugin.BoneId.Body_Root,
        Body_Hips                        = OVRPlugin.BoneId.Body_Hips,
        Body_SpineLower                  = OVRPlugin.BoneId.Body_SpineLower,
        Body_SpineMiddle                 = OVRPlugin.BoneId.Body_SpineMiddle,
        Body_SpineUpper                  = OVRPlugin.BoneId.Body_SpineUpper,
        Body_Chest                       = OVRPlugin.BoneId.Body_Chest,
        Body_Neck                        = OVRPlugin.BoneId.Body_Neck,
        Body_Head                        = OVRPlugin.BoneId.Body_Head,
        Body_LeftShoulder                = OVRPlugin.BoneId.Body_LeftShoulder,
        Body_LeftScapula                 = OVRPlugin.BoneId.Body_LeftScapula,
        Body_LeftArmUpper                = OVRPlugin.BoneId.Body_LeftArmUpper,
        Body_LeftArmLower                = OVRPlugin.BoneId.Body_LeftArmLower,
        Body_LeftHandWristTwist          = OVRPlugin.BoneId.Body_LeftHandWristTwist,
        Body_RightShoulder               = OVRPlugin.BoneId.Body_RightShoulder,
        Body_RightScapula                = OVRPlugin.BoneId.Body_RightScapula,
        Body_RightArmUpper               = OVRPlugin.BoneId.Body_RightArmUpper,
        Body_RightArmLower               = OVRPlugin.BoneId.Body_RightArmLower,
        Body_RightHandWristTwist         = OVRPlugin.BoneId.Body_RightHandWristTwist,
        Body_LeftHandPalm                = OVRPlugin.BoneId.Body_LeftHandPalm,
        Body_LeftHandWrist               = OVRPlugin.BoneId.Body_LeftHandWrist,
        Body_LeftHandThumbMetacarpal     = OVRPlugin.BoneId.Body_LeftHandThumbMetacarpal,
        Body_LeftHandThumbProximal       = OVRPlugin.BoneId.Body_LeftHandThumbProximal,
        Body_LeftHandThumbDistal         = OVRPlugin.BoneId.Body_LeftHandThumbDistal,
        Body_LeftHandThumbTip            = OVRPlugin.BoneId.Body_LeftHandThumbTip,
        Body_LeftHandIndexMetacarpal     = OVRPlugin.BoneId.Body_LeftHandIndexMetacarpal,
        Body_LeftHandIndexProximal       = OVRPlugin.BoneId.Body_LeftHandIndexProximal,
        Body_LeftHandIndexIntermediate   = OVRPlugin.BoneId.Body_LeftHandIndexIntermediate,
        Body_LeftHandIndexDistal         = OVRPlugin.BoneId.Body_LeftHandIndexDistal,
        Body_LeftHandIndexTip            = OVRPlugin.BoneId.Body_LeftHandIndexTip,
        Body_LeftHandMiddleMetacarpal    = OVRPlugin.BoneId.Body_LeftHandMiddleMetacarpal,
        Body_LeftHandMiddleProximal      = OVRPlugin.BoneId.Body_LeftHandMiddleProximal,
        Body_LeftHandMiddleIntermediate  = OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate,
        Body_LeftHandMiddleDistal        = OVRPlugin.BoneId.Body_LeftHandMiddleDistal,
        Body_LeftHandMiddleTip           = OVRPlugin.BoneId.Body_LeftHandMiddleTip,
        Body_LeftHandRingMetacarpal      = OVRPlugin.BoneId.Body_LeftHandRingMetacarpal,
        Body_LeftHandRingProximal        = OVRPlugin.BoneId.Body_LeftHandRingProximal,
        Body_LeftHandRingIntermediate    = OVRPlugin.BoneId.Body_LeftHandRingIntermediate,
        Body_LeftHandRingDistal          = OVRPlugin.BoneId.Body_LeftHandRingDistal,
        Body_LeftHandRingTip             = OVRPlugin.BoneId.Body_LeftHandRingTip,
        Body_LeftHandLittleMetacarpal    = OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal,
        Body_LeftHandLittleProximal      = OVRPlugin.BoneId.Body_LeftHandLittleProximal,
        Body_LeftHandLittleIntermediate  = OVRPlugin.BoneId.Body_LeftHandLittleIntermediate,
        Body_LeftHandLittleDistal        = OVRPlugin.BoneId.Body_LeftHandLittleDistal,
        Body_LeftHandLittleTip           = OVRPlugin.BoneId.Body_LeftHandLittleTip,
        Body_RightHandPalm               = OVRPlugin.BoneId.Body_RightHandPalm,
        Body_RightHandWrist              = OVRPlugin.BoneId.Body_RightHandWrist,
        Body_RightHandThumbMetacarpal    = OVRPlugin.BoneId.Body_RightHandThumbMetacarpal,
        Body_RightHandThumbProximal      = OVRPlugin.BoneId.Body_RightHandThumbProximal,
        Body_RightHandThumbDistal        = OVRPlugin.BoneId.Body_RightHandThumbDistal,
        Body_RightHandThumbTip           = OVRPlugin.BoneId.Body_RightHandThumbTip,
        Body_RightHandIndexMetacarpal    = OVRPlugin.BoneId.Body_RightHandIndexMetacarpal,
        Body_RightHandIndexProximal      = OVRPlugin.BoneId.Body_RightHandIndexProximal,
        Body_RightHandIndexIntermediate  = OVRPlugin.BoneId.Body_RightHandIndexIntermediate,
        Body_RightHandIndexDistal        = OVRPlugin.BoneId.Body_RightHandIndexDistal,
        Body_RightHandIndexTip           = OVRPlugin.BoneId.Body_RightHandIndexTip,
        Body_RightHandMiddleMetacarpal   = OVRPlugin.BoneId.Body_RightHandMiddleMetacarpal,
        Body_RightHandMiddleProximal     = OVRPlugin.BoneId.Body_RightHandMiddleProximal,
        Body_RightHandMiddleIntermediate = OVRPlugin.BoneId.Body_RightHandMiddleIntermediate,
        Body_RightHandMiddleDistal       = OVRPlugin.BoneId.Body_RightHandMiddleDistal,
        Body_RightHandMiddleTip          = OVRPlugin.BoneId.Body_RightHandMiddleTip,
        Body_RightHandRingMetacarpal     = OVRPlugin.BoneId.Body_RightHandRingMetacarpal,
        Body_RightHandRingProximal       = OVRPlugin.BoneId.Body_RightHandRingProximal,
        Body_RightHandRingIntermediate   = OVRPlugin.BoneId.Body_RightHandRingIntermediate,
        Body_RightHandRingDistal         = OVRPlugin.BoneId.Body_RightHandRingDistal,
        Body_RightHandRingTip            = OVRPlugin.BoneId.Body_RightHandRingTip,
        Body_RightHandLittleMetacarpal   = OVRPlugin.BoneId.Body_RightHandLittleMetacarpal,
        Body_RightHandLittleProximal     = OVRPlugin.BoneId.Body_RightHandLittleProximal,
        Body_RightHandLittleIntermediate = OVRPlugin.BoneId.Body_RightHandLittleIntermediate,
        Body_RightHandLittleDistal       = OVRPlugin.BoneId.Body_RightHandLittleDistal,
        Body_RightHandLittleTip          = OVRPlugin.BoneId.Body_RightHandLittleTip,
        Body_End                         = OVRPlugin.BoneId.Body_End,

        // add new bones here

        Max                     = OVRPlugin.BoneId.Max
    }

    [SerializeField]
    protected SkeletonType _skeletonType = SkeletonType.None;
    [SerializeField]
    private IOVRSkeletonDataProvider _dataProvider;

    [SerializeField]
    private bool _updateRootPose = false;
    [SerializeField]
    private bool _updateRootScale = false;
    [SerializeField]
    private bool _enablePhysicsCapsules = false;
    [SerializeField]
    private bool _applyBoneTranslations = true;

    private GameObject _bonesGO;
    private GameObject _bindPosesGO;
    private GameObject _capsulesGO;

    protected List<OVRBone> _bones;
    private List<OVRBone> _bindPoses;
    private List<OVRBoneCapsule> _capsules;

    protected OVRPlugin.Skeleton2 _skeleton = new OVRPlugin.Skeleton2();
    private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    public bool IsInitialized { get; private set; }
    public bool IsDataValid { get; private set; }
    public bool IsDataHighConfidence { get; private set; }
    public IList<OVRBone> Bones { get; protected set; }
    public IList<OVRBone> BindPoses { get; private set; }
    public IList<OVRBoneCapsule> Capsules { get; private set; }
    public SkeletonType GetSkeletonType() { return _skeletonType; }
    public int SkeletonChangedCount { get; private set; }

    private void Awake()
    {
        if (_dataProvider == null)
        {
            _dataProvider = GetComponent<IOVRSkeletonDataProvider>();
        }

        _bones = new List<OVRBone>();
        Bones = _bones.AsReadOnly();

        _bindPoses = new List<OVRBone>();
        BindPoses = _bindPoses.AsReadOnly();

        _capsules = new List<OVRBoneCapsule>();
        Capsules = _capsules.AsReadOnly();
    }

    private void Start()
    {
        if (ShouldInitialize())
        {
            Initialize();
        }
    }

    private bool ShouldInitialize()
    {
        if (IsInitialized)
        {
            return false;
        }

        if (_dataProvider != null && !_dataProvider.enabled)
        {
            return false;
        }

        if (_skeletonType == SkeletonType.None)
        {
            return false;
        }
        else if (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight)
        {
#if UNITY_EDITOR
            return OVRInput.IsControllerConnected(OVRInput.Controller.Hands);
#else
            return true;
#endif
        }
        else
        {
            return true;
        }
    }

    private void Initialize()
    {
        if (OVRPlugin.GetSkeleton2((OVRPlugin.SkeletonType)_skeletonType, ref _skeleton))
        {
            InitializeBones();
            InitializeBindPose();
            InitializeCapsules();

            IsInitialized = true;
        }
    }

    protected virtual Transform GetBoneTransform(BoneId boneId) => null;

    protected virtual void InitializeBones()
    {
        bool flipX = (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight);

        if (!_bonesGO)
        {
            _bonesGO = new GameObject("Bones");
            _bonesGO.transform.SetParent(transform, false);
            _bonesGO.transform.localPosition = Vector3.zero;
            _bonesGO.transform.localRotation = Quaternion.identity;
        }

        if (_bones == null || _bones.Count != _skeleton.NumBones)
        {
            _bones = new List<OVRBone>(new OVRBone[_skeleton.NumBones]);
            Bones = _bones.AsReadOnly();
        }

        // pre-populate bones list before attempting to apply bone hierarchy
        for (int i = 0; i < _bones.Count; ++i)
        {
            OVRBone bone = _bones[i] ?? (_bones[i] = new OVRBone());
            bone.Id = (OVRSkeleton.BoneId)_skeleton.Bones[i].Id;
            bone.ParentBoneIndex = _skeleton.Bones[i].ParentBoneIndex;

            bone.Transform = GetBoneTransform(bone.Id);
            if (bone.Transform == null)
            {
                bone.Transform = new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id)).transform;
            }

            var pose = _skeleton.Bones[i].Pose;

            if (_applyBoneTranslations)
            {
                bone.Transform.localPosition = flipX
                    ? pose.Position.FromFlippedXVector3f()
                    : pose.Position.FromFlippedZVector3f();
            }

            bone.Transform.localRotation = flipX
                ? pose.Orientation.FromFlippedXQuatf()
                : pose.Orientation.FromFlippedZQuatf();
        }

        for (int i = 0; i < _bones.Count; ++i)
        {
            if ((BoneId)_bones[i].ParentBoneIndex == BoneId.Invalid ||
                _skeletonType == SkeletonType.Body)  // Body bones are always in tracking space
            {
                _bones[i].Transform.SetParent(_bonesGO.transform, false);
            }
            else
            {
                _bones[i].Transform.SetParent(_bones[_bones[i].ParentBoneIndex].Transform, false);
            }
        }
    }

    private void InitializeBindPose()
    {
        if (!_bindPosesGO)
        {
            _bindPosesGO = new GameObject("BindPoses");
            _bindPosesGO.transform.SetParent(transform, false);
            _bindPosesGO.transform.localPosition = Vector3.zero;
            _bindPosesGO.transform.localRotation = Quaternion.identity;
        }

        if (_bindPoses == null || _bindPoses.Count != _bones.Count)
        {
            _bindPoses = new List<OVRBone>(new OVRBone[_bones.Count]);
            BindPoses = _bindPoses.AsReadOnly();
        }

        // pre-populate bones list before attempting to apply bone hierarchy
        for (int i = 0; i < _bindPoses.Count; ++i)
        {
            OVRBone bone = _bones[i];
            OVRBone bindPoseBone = _bindPoses[i] ?? (_bindPoses[i] = new OVRBone());
            bindPoseBone.Id = bone.Id;
            bindPoseBone.ParentBoneIndex = bone.ParentBoneIndex;

            Transform trans = bindPoseBone.Transform ? bindPoseBone.Transform : (bindPoseBone.Transform =
                new GameObject(BoneLabelFromBoneId(_skeletonType, bindPoseBone.Id)).transform);
            trans.localPosition = bone.Transform.localPosition;
            trans.localRotation = bone.Transform.localRotation;
        }

        for (int i = 0; i < _bindPoses.Count; ++i)
        {
            if ((BoneId)_bindPoses[i].ParentBoneIndex == BoneId.Invalid ||
                _skeletonType == SkeletonType.Body) // Body bones are always in tracking space
            {
                _bindPoses[i].Transform.SetParent(_bindPosesGO.transform, false);
            }
            else
            {
                _bindPoses[i].Transform.SetParent(_bindPoses[_bindPoses[i].ParentBoneIndex].Transform, false);
            }
        }
    }

    private void InitializeCapsules()
    {
        bool flipX = (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight);

        if (_enablePhysicsCapsules)
        {
            if (!_capsulesGO)
            {
                _capsulesGO = new GameObject("Capsules");
                _capsulesGO.transform.SetParent(transform, false);
                _capsulesGO.transform.localPosition = Vector3.zero;
                _capsulesGO.transform.localRotation = Quaternion.identity;
            }

            if (_capsules == null || _capsules.Count != _skeleton.NumBoneCapsules)
            {
                _capsules = new List<OVRBoneCapsule>(new OVRBoneCapsule[_skeleton.NumBoneCapsules]);
                Capsules = _capsules.AsReadOnly();
            }

            for (int i = 0; i < _capsules.Count; ++i)
            {
                OVRBone bone = _bones[_skeleton.BoneCapsules[i].BoneIndex];
                OVRBoneCapsule capsule = _capsules[i] ?? (_capsules[i] = new OVRBoneCapsule());
                capsule.BoneIndex = _skeleton.BoneCapsules[i].BoneIndex;

                if (capsule.CapsuleRigidbody == null)
                {
                    capsule.CapsuleRigidbody = new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id) + "_CapsuleRigidbody").AddComponent<Rigidbody>();
                    capsule.CapsuleRigidbody.mass = 1.0f;
                    capsule.CapsuleRigidbody.isKinematic = true;
                    capsule.CapsuleRigidbody.useGravity = false;
                    capsule.CapsuleRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }

                GameObject rbGO = capsule.CapsuleRigidbody.gameObject;
                rbGO.transform.SetParent(_capsulesGO.transform, false);
                rbGO.transform.position = bone.Transform.position;
                rbGO.transform.rotation = bone.Transform.rotation;

                if (capsule.CapsuleCollider == null)
                {
                    capsule.CapsuleCollider = new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id) + "_CapsuleCollider").AddComponent<CapsuleCollider>();
                    capsule.CapsuleCollider.isTrigger = false;
                }

                var p0 = flipX ? _skeleton.BoneCapsules[i].StartPoint.FromFlippedXVector3f() : _skeleton.BoneCapsules[i].StartPoint.FromFlippedZVector3f();
                var p1 = flipX ? _skeleton.BoneCapsules[i].EndPoint.FromFlippedXVector3f() : _skeleton.BoneCapsules[i].EndPoint.FromFlippedZVector3f();
                var delta = p1 - p0;
                var mag = delta.magnitude;
                var rot = Quaternion.FromToRotation(Vector3.right, delta);
                capsule.CapsuleCollider.radius = _skeleton.BoneCapsules[i].Radius;
                capsule.CapsuleCollider.height = mag + _skeleton.BoneCapsules[i].Radius * 2.0f;
                capsule.CapsuleCollider.direction = 0;
                capsule.CapsuleCollider.center = Vector3.right * mag * 0.5f;

                GameObject ccGO = capsule.CapsuleCollider.gameObject;
                ccGO.transform.SetParent(rbGO.transform, false);
                ccGO.transform.localPosition = p0;
                ccGO.transform.localRotation = rot;
            }
        }
    }

    private void Update()
    {
                // BUG: Using New() allocation in Update() method.
                // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                //         if (ShouldInitialize())
                //         {
                //             Initialize();
                //         }
                // 
                //         if (!IsInitialized || _dataProvider == null)
                //         {
                //             IsDataValid = false;
                //             IsDataHighConfidence = false;
                //             return;
                //         }
                // 
                //         var data = _dataProvider.GetSkeletonPoseData();
                // 
                //         IsDataValid = data.IsDataValid;
                //         if (data.IsDataValid)
                //         {
                // 
                //             if (SkeletonChangedCount != data.SkeletonChangedCount)
                //             {
                //                 SkeletonChangedCount = data.SkeletonChangedCount;
                //                 IsInitialized = false;
                //                 Initialize();
                //             }
                // 
                //             IsDataHighConfidence = data.IsDataHighConfidence;
                // 
                //             if (_updateRootPose)
                //             {
                //                 transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
                //                 transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
                //             }
                // 
                //             if (_updateRootScale)
                //             {
                //                 transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);
                //             }
                // 
                //             for (var i = 0; i < _bones.Count; ++i)
                //             {
                //                 var boneTransform = _bones[i].Transform;
                //                 if (boneTransform == null) continue;
                // 
                //                 if (_skeletonType == SkeletonType.Body)
                //                 {
                //                     boneTransform.localPosition = data.BoneTranslations[i].FromFlippedZVector3f();
                //                     boneTransform.localRotation = data.BoneRotations[i].FromFlippedZQuatf();
                //                 }
                //                 else if (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight)
                //                 {
                //                     boneTransform.localRotation = data.BoneRotations[i].FromFlippedXQuatf();
                // 
                //                     if (_bones[i].Id == BoneId.Hand_WristRoot)
                //                     {
                //                         boneTransform.localRotation *= wristFixupRotation;
                //                     }
                //                 }
                //                 else
                //                 {
                //                     boneTransform.localRotation = data.BoneRotations[i].FromFlippedZQuatf();
                //                 }
                //             }
                //         }
                //     }

                // FIXED VERSION:
