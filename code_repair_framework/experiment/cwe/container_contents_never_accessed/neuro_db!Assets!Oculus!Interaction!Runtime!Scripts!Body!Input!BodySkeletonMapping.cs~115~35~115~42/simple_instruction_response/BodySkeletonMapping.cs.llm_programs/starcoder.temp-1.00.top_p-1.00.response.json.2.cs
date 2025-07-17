

using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using Oculus.Interaction.Collections;

namespace Oculus.Interaction.Body.Input
{
    
    
    
    
    
    public abstract class BodySkeletonMapping<TSourceJointId> : ISkeletonMapping
        where TSourceJointId : System.Enum
    {
        public IEnumerableHashSet<BodyJointId> Joints => _joints;

        public bool TryGetParentJointId(BodyJointId jointId, out BodyJointId parentJointId)
        {
            return _jointToParent.TryGetValue(jointId, out parentJointId);
        }

        public bool TryGetSourceJointId(BodyJointId jointId, out TSourceJointId sourceJointId)
        {
            return _reverseMap.TryGetValue(jointId, out sourceJointId);
        }

        public bool TryGetBodyJointId(TSourceJointId jointId, out BodyJointId bodyJointId)
        {
            return _forwardMap.TryGetValue(jointId, out bodyJointId);
        }

        protected TSourceJointId GetSourceJointFromBodyJoint(BodyJointId jointId)
        {
            return _reverseMap[jointId];
        }

        protected BodyJointId GetBodyJointFromSourceJoint(TSourceJointId sourceJointId)
        {
            return _forwardMap[sourceJointId];
        }

        protected abstract TSourceJointId GetRoot();
        protected abstract IReadOnlyDictionary<BodyJointId, JointInfo> GetJointMapping();

        public BodySkeletonMapping()
        {
            _tree = new SkeletonTree(GetRoot(), GetJointMapping());
            _joints = new EnumerableHashSet<BodyJointId>(_tree.Nodes.Select(n => n.BodyJointId));
            _forwardMap = _tree.Nodes.ToDictionary((n) => n.SourceJointId, (n) => n.BodyJointId);
            _reverseMap = _tree.Nodes.ToDictionary((n) => n.BodyJointId, (n) => n.SourceJointId);
            _jointToParent = _tree.Nodes.ToDictionary((n) => n.BodyJointId, (n) => n.Parent.BodyJointId);
        }

        private readonly SkeletonTree _tree;
        private readonly IEnumerableHashSet<BodyJointId> _joints;
        private readonly IReadOnlyDictionary<TSourceJointId, BodyJointId> _forwardMap;
        private readonly IReadOnlyDictionary<BodyJointId, TSourceJointId> _reverseMap;
        private readonly IReadOnlyDictionary<BodyJointId, BodyJointId> _jointToParent;

        private class SkeletonTree
        {
            public readonly Node Root;
            public readonly IReadOnlyList<Node> Nodes;

            public SkeletonTree(TSourceJointId root,
                IReadOnlyDictionary<BodyJointId, JointInfo> mapping)
            {
                Dictionary<TSourceJointId, Node> nodes = new Dictionary<TSourceJointId, Node>();
                foreach (var map in mapping)
                {
                    BodyJointId jointId = map.Key;
                    JointInfo jointInfo = map.Value;
                    Assert.IsFalse(nodes.ContainsKey(jointInfo.SourceJointId),
                        "Duplicate Joint ID in Mapping");
                    nodes[jointInfo.SourceJointId] =
                        new Node(jointInfo.SourceJointId, jointId);
                }
                foreach (var jointInfo in mapping.Values)
                {
                    Node thisNode = nodes[jointInfo.SourceJointId];
                    thisNode.Parent = nodes[jointInfo.ParentJointId];
                    thisNode.Parent.Children.Add(thisNode);
                }
                Nodes = new List<Node>(nodes.Values);
                Root = nodes[root];
            }

            public class Node
            {
                public readonly TSourceJointId SourceJointId;
                public readonly BodyJointId BodyJointId;

                public Node Parent;
                
                
There are several ways to create animations in Godot 4, depending on your needs and skill level. Here are a few options:

1. Animations using built-in tools: Godot 4 has a built-in animation editor that allows you to create animations directly within the engine. You can use this editor to create both simple and complex animations, using a range of built-in tools and features.

2. Third-party animation tools: If you prefer to use a third-party animation tool, Godot 4 supports exporting animations in a variety of formats, including FBX, which can be imported into many popular animation programs.

3. Script-based animation: If you prefer to write your animations in code, Godot 4 supports a range of animation systems, including animation curves, Tween nodes, and physics-based animation.

In general, the best approach to animation in Godot 4 will depend on your specific needs and preferences. The built-in animation editor is a powerful and user-friendly option for beginners, while script-based animation offers greater control and flexibility for advanced users.
<|system|>

<|user|>
Explícame detalladamente cual es la diferencia entre un IVA y un ISR

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
