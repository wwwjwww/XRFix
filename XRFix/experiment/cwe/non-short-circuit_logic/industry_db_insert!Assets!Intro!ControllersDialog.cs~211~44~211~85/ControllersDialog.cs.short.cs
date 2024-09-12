using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BaroqueUI;
using System;

        private void Gt_onControllersUpdate(Controller[] controllers)
        {
                    // BUG: Potentially dangerous use of non-short-circuit logic
                    // MESSAGE: The & and | operators do not use short-circuit evaluation and can be dangerous when applied to boolean operands. In particular, their use can result in errors if the left-hand operand checks for cases in which it is not safe to evaluate the right-hand one.
                    //                     bool trigger_pressed = ctrl.triggerPressed | ctrl.touchpadPressed;

                    // FIXED CODE:
