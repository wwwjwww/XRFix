
            public class Node
            {
                public readonly TSourceJointId SourceJointId;
                public readonly BodyJointId BodyJointId;

                public Node Parent;
                public List<Node> Children = new List<Node>();

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
