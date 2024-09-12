using UnityEngine;

        private float FindProjectileCollision(out RaycastHit hitInfo)
        {
                    // BUG: Constant condition
                    // MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
                    //                     if (true)

                    // FIXED CODE:
