
    private void FixedUpdate()
    {
        if (!IsInitialized || _dataProvider == null)
        {
            IsDataValid = false;
            IsDataHighConfidence = false;

            return;
        }

        Update();

        if (_enablePhysicsCapsules)
        {
            var data = _dataProvider.GetSkeletonPoseData();

            IsDataValid = data.IsDataValid;
            IsDataHighConfidence = data.IsDataHighConfidence;

            for (int i = 0; i < _capsules.Count; ++i)
            {
                OVRBoneCapsule capsule = _capsules[i];
                var capsuleGO = capsule.CapsuleRigidbody.gameObject;

                if (data.IsDataValid && data.IsDataHighConfidence)
                {
                    Transform bone = _bones[(int)capsule.BoneIndex].Transform;

                    if (capsuleGO.activeSelf)
                    {
                        capsule.CapsuleRigidbody.MovePosition(bone.position);
                        capsule.CapsuleRigidbody.MoveRotation(bone.rotation);
                    }
                    else
                    {
                        capsuleGO.SetActive(true);
                        capsule.CapsuleRigidbody.position = bone.position;
                        capsule.CapsuleRigidbody.rotation = bone.rotation;
                    }
                }
                else
                {
                    if (capsuleGO.activeSelf)
                    {
                        capsuleGO.SetActive(false);
                    }
                }
            }
        }
    }

    public BoneId GetCurrentStartBoneId()
    {
        switch (_skeletonType)
        {
        case SkeletonType.HandLeft:
        case SkeletonType.HandRight:
            return BoneId.Hand_Start;
        case SkeletonType.Body:
            return BoneId.Body_Start;
        case SkeletonType.None:
        default:
            return BoneId.Invalid;
        }
    }

    public BoneId GetCurrentEndBoneId()
    {
        switch (_skeletonType)
        {
        case SkeletonType.HandLeft:
        case SkeletonType.HandRight:
            return BoneId.Hand_End;
        case SkeletonType.Body:
            return BoneId.Body_End;
        case SkeletonType.None:
        default:
            return BoneId.Invalid;
        }
    }

    private BoneId GetCurrentMaxSkinnableBoneId()
    {
        switch (_skeletonType)
        {
        case SkeletonType.HandLeft:
        case SkeletonType.HandRight:
            return BoneId.Hand_MaxSkinnable;
        case SkeletonType.Body:
            return BoneId.Body_End;
        case SkeletonType.None:
        default:
            return BoneId.Invalid;
        }
    }

    public int GetCurrentNumBones()
    {
        switch (_skeletonType)
        {
        case SkeletonType.HandLeft:
        case SkeletonType.HandRight:
        case SkeletonType.Body:
            return GetCurrentEndBoneId() - GetCurrentStartBoneId();
        case SkeletonType.None:
        default:
            return 0;
        }
    }

    public int GetCurrentNumSkinnableBones()
    {
        switch (_skeletonType)
        {
        case SkeletonType.HandLeft:
        case SkeletonType.HandRight:
        case SkeletonType.Body:
            return GetCurrentMaxSkinnableBoneId() - GetCurrentStartBoneId();
        case SkeletonType.None:
        default:
            return 0;
        }
    }


    // force aliased enum values to the more appropriate value
    public static string BoneLabelFromBoneId(OVRSkeleton.SkeletonType skeletonType, BoneId boneId)
    {
        if (skeletonType == OVRSkeleton.SkeletonType.Body)
        {
            switch (boneId)
            {
                case BoneId.Body_Root:
                    return "Body_Root";
                case BoneId.Body_Hips:
                    return "Body_Hips";
                case BoneId.Body_SpineLower:
                    return "Body_SpineLower";
                case BoneId.Body_SpineMiddle:
                    return "Body_SpineMiddle";
                case BoneId.Body_SpineUpper:
                    return "Body_SpineUpper";
                case BoneId.Body_Chest:
                    return "Body_Chest";
                case BoneId.Body_Neck:
                    return "Body_Neck";
                case BoneId.Body_Head:
                    return "Body_Head";
                case BoneId.Body_LeftShoulder:
                    return "Body_LeftShoulder";
                case BoneId.Body_LeftScapula:
                    return "Body_LeftScapula";
                case BoneId.Body_LeftArmUpper:
                    return "Body_LeftArmUpper";
                case BoneId.Body_LeftArmLower:
                    return "Body_LeftArmLower";
                case BoneId.Body_LeftHandWristTwist:
                    return "Body_LeftHandWristTwist";
                case BoneId.Body_RightShoulder:
                    return "Body_RightShoulder";
                case BoneId.Body_RightScapula:
                    return "Body_RightScapula";
                case BoneId.Body_RightArmUpper:
                    return "Body_RightArmUpper";
                case BoneId.Body_RightArmLower:
                    return "Body_RightArmLower";
                case BoneId.Body_RightHandWristTwist:
                    return "Body_RightHandWristTwist";
                case BoneId.Body_LeftHandPalm:
                    return "Body_LeftHandPalm";
                case BoneId.Body_LeftHandWrist:
                    return "Body_LeftHandWrist";
                case BoneId.Body_LeftHandThumbMetacarpal:
                    return "Body_LeftHandThumbMetacarpal";
                case BoneId.Body_LeftHandThumbProximal:
                    return "Body_LeftHandThumbProximal";
                case BoneId.Body_LeftHandThumbDistal:
                    return "Body_LeftHandThumbDistal";
                case BoneId.Body_LeftHandThumbTip:
                    return "Body_LeftHandThumbTip";
                case BoneId.Body_LeftHandIndexMetacarpal:
                    return "Body_LeftHandIndexMetacarpal";
                case BoneId.Body_LeftHandIndexProximal:
                    return "Body_LeftHandIndexProximal";
                case BoneId.Body_LeftHandIndexIntermediate:
                    return "Body_LeftHandIndexIntermediate";
                case BoneId.Body_LeftHandIndexDistal:
                    return "Body_LeftHandIndexDistal";
                case BoneId.Body_LeftHandIndexTip:
                    return "Body_LeftHandIndexTip";
                case BoneId.Body_LeftHandMiddleMetacarpal:
                    return "Body_LeftHandMiddleMetacarpal";
                case BoneId.Body_LeftHandMiddleProximal:
                    return "Body_LeftHandMiddleProximal";
                case BoneId.Body_LeftHandMiddleIntermediate:
                    return "Body_LeftHandMiddleIntermediate";
                case BoneId.Body_LeftHandMiddleDistal:
                    return "Body_LeftHandMiddleDistal";
                case BoneId.Body_LeftHandMiddleTip:
                    return "Body_LeftHandMiddleTip";
                case BoneId.Body_LeftHandRingMetacarpal:
                    return "Body_LeftHandRingMetacarpal";
                case BoneId.Body_LeftHandRingProximal:
                    return "Body_LeftHandRingProximal";
                case BoneId.Body_LeftHandRingIntermediate:
                    return "Body_LeftHandRingIntermediate";
                case BoneId.Body_LeftHandRingDistal:
                    return "Body_LeftHandRingDistal";
                case BoneId.Body_LeftHandRingTip:
                    return "Body_LeftHandRingTip";
                case BoneId.Body_LeftHandLittleMetacarpal:
                    return "Body_LeftHandLittleMetacarpal";
                case BoneId.Body_LeftHandLittleProximal:
                    return "Body_LeftHandLittleProximal";
                case BoneId.Body_LeftHandLittleIntermediate:
                    return "Body_LeftHandLittleIntermediate";
                case BoneId.Body_LeftHandLittleDistal:
                    return "Body_LeftHandLittleDistal";
                case BoneId.Body_LeftHandLittleTip:
                    return "Body_LeftHandLittleTip";
                case BoneId.Body_RightHandPalm:
                    return "Body_RightHandPalm";
                case BoneId.Body_RightHandWrist:
                    return "Body_RightHandWrist";
                case BoneId.Body_RightHandThumbMetacarpal:
                    return "Body_RightHandThumbMetacarpal";
                case BoneId.Body_RightHandThumbProximal:
                    return "Body_RightHandThumbProximal";
                case BoneId.Body_RightHandThumbDistal:
                    return "Body_RightHandThumbDistal";
                case BoneId.Body_RightHandThumbTip:
                    return "Body_RightHandThumbTip";
                case BoneId.Body_RightHandIndexMetacarpal:
                    return "Body_RightHandIndexMetacarpal";
                case BoneId.Body_RightHandIndexProximal:
                    return "Body_RightHandIndexProximal";
                case BoneId.Body_RightHandIndexIntermediate:
                    return "Body_RightHandIndexIntermediate";
                case BoneId.Body_RightHandIndexDistal:
                    return "Body_RightHandIndexDistal";
                case BoneId.Body_RightHandIndexTip:
                    return "Body_RightHandIndexTip";
                case BoneId.Body_RightHandMiddleMetacarpal:
                    return "Body_RightHandMiddleMetacarpal";
                case BoneId.Body_RightHandMiddleProximal:
                    return "Body_RightHandMiddleProximal";
                case BoneId.Body_RightHandMiddleIntermediate:
                    return "Body_RightHandMiddleIntermediate";
                case BoneId.Body_RightHandMiddleDistal:
                    return "Body_RightHandMiddleDistal";
                case BoneId.Body_RightHandMiddleTip:
                    return "Body_RightHandMiddleTip";
                case BoneId.Body_RightHandRingMetacarpal:
                    return "Body_RightHandRingMetacarpal";
                case BoneId.Body_RightHandRingProximal:
                    return "Body_RightHandRingProximal";
                case BoneId.Body_RightHandRingIntermediate:
                    return "Body_RightHandRingIntermediate";
                case BoneId.Body_RightHandRingDistal:
                    return "Body_RightHandRingDistal";
                case BoneId.Body_RightHandRingTip:
                    return "Body_RightHandRingTip";
                case BoneId.Body_RightHandLittleMetacarpal:
                    return "Body_RightHandLittleMetacarpal";
                case BoneId.Body_RightHandLittleProximal:
                    return "Body_RightHandLittleProximal";
                case BoneId.Body_RightHandLittleIntermediate:
                    return "Body_RightHandLittleIntermediate";
                case BoneId.Body_RightHandLittleDistal:
                    return "Body_RightHandLittleDistal";
                case BoneId.Body_RightHandLittleTip:
                    return "Body_RightHandLittleTip";
                default:
                    return "Body_Unknown";
            }
        }
        else if (skeletonType == OVRSkeleton.SkeletonType.HandLeft || skeletonType == OVRSkeleton.SkeletonType.HandRight)
        {
            switch (boneId)
            {
                case OVRSkeleton.BoneId.Hand_WristRoot:
                    return "Hand_WristRoot";
                case OVRSkeleton.BoneId.Hand_ForearmStub:
                    return "Hand_ForearmStub";
                case OVRSkeleton.BoneId.Hand_Thumb0:
                    return "Hand_Thumb0";
                case OVRSkeleton.BoneId.Hand_Thumb1:
                    return "Hand_Thumb1";
                case OVRSkeleton.BoneId.Hand_Thumb2:
                    return "Hand_Thumb2";
                case OVRSkeleton.BoneId.Hand_Thumb3:
                    return "Hand_Thumb3";
                case OVRSkeleton.BoneId.Hand_Index1:
                    return "Hand_Index1";
                case OVRSkeleton.BoneId.Hand_Index2:
                    return "Hand_Index2";
                case OVRSkeleton.BoneId.Hand_Index3:
                    return "Hand_Index3";
                case OVRSkeleton.BoneId.Hand_Middle1:
                    return "Hand_Middle1";
                case OVRSkeleton.BoneId.Hand_Middle2:
                    return "Hand_Middle2";
                case OVRSkeleton.BoneId.Hand_Middle3:
                    return "Hand_Middle3";
                case OVRSkeleton.BoneId.Hand_Ring1:
                    return "Hand_Ring1";
                case OVRSkeleton.BoneId.Hand_Ring2:
                    return "Hand_Ring2";
                case OVRSkeleton.BoneId.Hand_Ring3:
                    return "Hand_Ring3";
                case OVRSkeleton.BoneId.Hand_Pinky0:
                    return "Hand_Pinky0";
                case OVRSkeleton.BoneId.Hand_Pinky1:
                    return "Hand_Pinky1";
                case OVRSkeleton.BoneId.Hand_Pinky2:
                    return "Hand_Pinky2";
                case OVRSkeleton.BoneId.Hand_Pinky3:
                    return "Hand_Pinky3";
                case OVRSkeleton.BoneId.Hand_ThumbTip:
                    return "Hand_ThumbTip";
                case OVRSkeleton.BoneId.Hand_IndexTip:
                    return "Hand_IndexTip";
                case OVRSkeleton.BoneId.Hand_MiddleTip:
                    return "Hand_MiddleTip";
                case OVRSkeleton.BoneId.Hand_RingTip:
                    return "Hand_RingTip";
                case OVRSkeleton.BoneId.Hand_PinkyTip:
                    return "Hand_PinkyTip";
                default:
                    return "Hand_Unknown";
            }
        }
        else
        {
            return "Skeleton_Unknown";
        }
    }
}

public class OVRBone
{
    public OVRSkeleton.BoneId Id { get; set; }
    public short ParentBoneIndex { get; set; }
    public Transform Transform { get; set; }

    public OVRBone() { }

    public OVRBone(OVRSkeleton.BoneId id, short parentBoneIndex, Transform trans)
    {
        Id = id;
        ParentBoneIndex = parentBoneIndex;
        Transform = trans;
    }
}

public class OVRBoneCapsule
{
    public short BoneIndex { get; set; }
    public Rigidbody CapsuleRigidbody { get; set; }
    public CapsuleCollider CapsuleCollider { get; set; }

    public OVRBoneCapsule() { }

    public OVRBoneCapsule(short boneIndex, Rigidbody capsuleRigidBody, CapsuleCollider capsuleCollider)
    {
        BoneIndex = boneIndex;
        CapsuleRigidbody = capsuleRigidBody;
        CapsuleCollider = capsuleCollider;
    }
}
