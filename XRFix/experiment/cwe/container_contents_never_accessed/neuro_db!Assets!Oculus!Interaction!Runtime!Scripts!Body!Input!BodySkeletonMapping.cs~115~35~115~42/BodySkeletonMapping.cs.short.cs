using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using Oculus.Interaction.Collections;

                // BUG: Container contents are never accessed
                // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
                //                 public List<Node> Children = new List<Node>();

                // FIXED CODE:
