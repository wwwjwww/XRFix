using System.Collections.Generic;
using System.Threading;
using UnityEngine;

///         public bool TryGetAnswer(out T answer)
//         {
//             if (got_answer)
//             {
                // BUG: Locking the 'this' object in a lock statement
                // MESSAGE: It is bad practice to lock the 'this' object because it might be locked elsewhere.
                //                 lock (this)
                //                     answer = result;
                //                 return true;
                //             }
                //             else
                //             {
                //                 answer = default(T);
                //                 return false;
                //             }
                //         }

                // Create a private readonly Object which is used exclusively for locking. This ensures that no other classes can use the same lock.
                // FIXED CODE:
