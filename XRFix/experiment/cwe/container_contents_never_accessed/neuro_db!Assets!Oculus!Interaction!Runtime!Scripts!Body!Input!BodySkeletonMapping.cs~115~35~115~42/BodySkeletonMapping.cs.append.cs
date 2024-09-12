
                public Node(TSourceJointId sourceJointId, BodyJointId bodyJointId)
                {
                    SourceJointId = sourceJointId;
                    BodyJointId = bodyJointId;
                }
            }
        }

        protected readonly struct JointInfo
        {
            public readonly TSourceJointId SourceJointId;
            public readonly TSourceJointId ParentJointId;

            public JointInfo(
                TSourceJointId sourceJointId,
                TSourceJointId parentJointId)
            {
                SourceJointId = sourceJointId;
                ParentJointId = parentJointId;
            }
        }
    }
}
