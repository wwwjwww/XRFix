
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
			return GetCurrentMaxSkinnableBoneId() - GetCurrentStartBoneId();
		case SkeletonType.None:
		default:
			return 0;
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

