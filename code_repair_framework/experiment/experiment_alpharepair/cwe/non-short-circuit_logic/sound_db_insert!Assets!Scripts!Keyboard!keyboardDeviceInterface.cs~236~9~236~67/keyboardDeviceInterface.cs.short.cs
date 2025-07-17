using UnityEngine;
using System.Collections;
using System.Collections.Generic;

  public override void Load(InstrumentData d) {
    // BUG: Potentially dangerous use of non-short-circuit logic
    // MESSAGE: The & and | operators do not use short-circuit evaluation and can be dangerous when applied to boolean operands. In particular, their use can result in errors if the left-hand operand checks for cases in which it is not safe to evaluate the right-hand one.
    //     if (data.midiInConnection != null & data.midiInConnection != "") {

    //Replace the operator with the short circuit equivalent. 
    // FIXED CODE:
