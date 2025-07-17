using UnityEngine;
using System.Collections;

  public override void setState(manipState state) {
        // BUG: Constant condition
        // MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
        //         if (true) highlightMat.SetFloat("_EmissionGain", .9f);

        //Avoid constant conditions where possible, and either eliminate the conditions or replace them.
        // FIXED CODE:
