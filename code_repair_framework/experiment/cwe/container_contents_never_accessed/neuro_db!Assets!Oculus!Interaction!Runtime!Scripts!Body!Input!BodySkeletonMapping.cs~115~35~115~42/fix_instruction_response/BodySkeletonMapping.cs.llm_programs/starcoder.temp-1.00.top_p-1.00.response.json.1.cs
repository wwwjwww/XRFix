

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
                // BUG: Container contents are never accessed
                // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
                //                 public List<Node> Children = new List<Node>();

                //Remove or Commented-out the collection if it is no longer needed
                // FIXED CODE:
                python
import numpy as np
from astropy import units as u

DEFAULT_OBSERVER_LOCATION = [52.205, 0.119] * u.deg
DEFAULT_OBSERVER_ALTITUDE = 30.0 * u.deg
DEFAULT_OBSERVER_VELOCITY = [0.0, 0.0, -100.0] * u.m / u.s

def moon_phase(date=None):
        Calculate the phase of the Moon for a given date.

    Parameters
    ----------
    date : `~astropy.time.Time` or None
        The date for which to calculate the Moon phase. If None, use the current date.

    Returns
    -------
    phase : `~astropy.units.Quantity`
        The phase of the Moon, where 0.0 is new and 1.0 is full.
    
    if date is None:
        date = Time.now()

    phase = (date - FullMoonDate).to(u.day).value / (MoonDays * 0.5)
    return phase


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
