using UnityEngine;
using System.Collections;
using System.Collections.Generic;

  public void setSelectAsynch(bool on) {
  // BUG: Container contents are never accessed
  // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
  //   Queue<bool> hits = new Queue<bool>();

  //Remove or Commented-out the collection if it is no longer needed
  // FIXED CODE:
