using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEditor;
using UnityEngine;

        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private List<FoldoutHierarchyItem<T>> _data = new List<FoldoutHierarchyItem<T>>();

        //Remove or Commented-out the collection if it is no longer needed
        // FIXED CODE:
