/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https:
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal static class OVRTask
{
    internal static OVRTask<TResult> FromGuid<TResult>(Guid id) => Create<TResult>(id);
    internal static OVRTask<TResult> FromRequest<TResult>(ulong id) => Create<TResult>(GetId(id));

    internal static OVRTask<TResult> FromResult<TResult>(TResult result)
    {
        var task = Create<TResult>(Guid.NewGuid());
        task.SetResult(result);
        return task;
    }

    internal static OVRTask<TResult> GetExisting<TResult>(Guid id) => Get<TResult>(id);
    internal static OVRTask<TResult> GetExisting<TResult>(ulong id) => Get<TResult>(GetId(id));

    internal static void SetResult<TResult>(Guid id, TResult result) =>
        GetExisting<TResult>(id).SetResult(result);

    internal static void SetResult<TResult>(ulong id, TResult result) =>
        GetExisting<TResult>(id).SetResult(result);

    private static OVRTask<TResult> Get<TResult>(Guid id)
    {
        return new OVRTask<TResult>(id);
    }

    private static OVRTask<TResult> Create<TResult>(Guid id)
    {
        var task = Get<TResult>(id);
        task.AddToPending();
        return task;
    }

    internal static unsafe Guid GetId(ulong value)
    {
        const ulong hashModifier1 = 0x319642b2d24d8ec3;
        const ulong hashModifier2 = 0x96de1b173f119089;
        var guid = default(Guid);
        *(ulong*)&guid = unchecked(value + hashModifier1);
        *((ulong*)&guid + 1) = hashModifier2;
        return guid;
    }
}














public readonly struct OVRTask<TResult> : IEquatable<OVRTask<TResult>>, IDisposable
{
    #region static

    private static readonly HashSet<Guid> Pending = new HashSet<Guid>();
    private static readonly Dictionary<Guid, TResult> Results = new Dictionary<Guid, TResult>();
    private static readonly Dictionary<Guid, Action> Continuations = new Dictionary<Guid, Action>();

    private delegate void CallbackInvoker(Guid guid, TResult result);

    private delegate bool CallbackRemover(Guid guid);

    private static readonly Dictionary<Guid, CallbackInvoker>
        CallbackInvokers = new Dictionary<Guid, CallbackInvoker>();

    private static readonly Dictionary<Guid, CallbackRemover>
        CallbackRemovers = new Dictionary<Guid, CallbackRemover>();

    private static readonly HashSet<Action> CallbackClearers = new HashSet<Action>();

    private delegate bool InternalDataRemover(Guid guid);

    private static readonly Dictionary<Guid, InternalDataRemover> InternalDataRemovers =
        new Dictionary<Guid, InternalDataRemover>();

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private static readonly HashSet<Action> InternalDataClearers = new HashSet<Action>();

    //Remove or Commented-out the collection if it is no longer needed
    // FIXED CODE:
