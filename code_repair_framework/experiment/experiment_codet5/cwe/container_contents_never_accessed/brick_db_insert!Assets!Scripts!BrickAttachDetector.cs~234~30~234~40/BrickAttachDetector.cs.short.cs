using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using System.Linq;
using System;

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private List<GameObject> testObjects = new List<GameObject>();

    //Remove or Commented-out the collection if it is no longer needed
    // FIXED CODE:
