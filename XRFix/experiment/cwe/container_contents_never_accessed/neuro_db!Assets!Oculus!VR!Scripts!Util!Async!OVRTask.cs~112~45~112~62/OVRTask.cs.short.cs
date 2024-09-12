using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private static readonly HashSet<Action> SubscriberClearers = new HashSet<Action>();

    // FIXED CODE:
