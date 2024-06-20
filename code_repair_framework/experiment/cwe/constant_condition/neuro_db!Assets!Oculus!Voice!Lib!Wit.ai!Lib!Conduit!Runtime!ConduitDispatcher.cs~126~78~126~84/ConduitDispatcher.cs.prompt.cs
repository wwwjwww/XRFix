/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Meta.WitAi;

namespace Meta.Conduit
{
    /// <summary>
    /// The dispatcher is responsible for deciding which method to invoke when a request is received as well as parsing
    /// the parameters and passing them to the handling method.
    /// </summary>
    internal class ConduitDispatcher : IConduitDispatcher
    {
        /// <summary>
        /// The Conduit manifest which captures the structure of the voice-enabled methods.
        /// </summary>
        public Manifest Manifest { get; private set; }

        /// <summary>
        /// The manifest loader.
        /// </summary>
        private readonly IManifestLoader _manifestLoader;

        /// <summary>
        /// Resolves instances (objects) based on their type.
        /// </summary>
        private readonly IInstanceResolver _instanceResolver;

        /// <summary>
        /// Maps internal parameter names to fully qualified parameter names (roles/slots).
        /// </summary>
        private readonly Dictionary<string, string> _parameterToRoleMap = new Dictionary<string, string>();


        /// <summary>
        /// List of actions that we won't log warnings about any more to avoid spamming the logs.
        /// </summary>
        private readonly HashSet<string> _ignoredActionIds = new HashSet<string>();

        public ConduitDispatcher(IManifestLoader manifestLoader, IInstanceResolver instanceResolver)
        {
            _manifestLoader = manifestLoader;
            _instanceResolver = instanceResolver;
        }

        /// <summary>
        /// Parses the manifest provided and registers its callbacks for dispatching.
        /// </summary>
        /// <param name="manifestFilePath">The path to the manifest file.</param>
        public void Initialize(string manifestFilePath)
        {
            if (Manifest != null)
            {
                return;
            }

            Manifest = _manifestLoader.LoadManifest(manifestFilePath);

            if (Manifest == null)
            {
                return;
            }

            // Map fully qualified role names to internal parameters.
            foreach (var action in Manifest.Actions)
            {
                foreach (var parameter in action.Parameters)
                {
                    if (!_parameterToRoleMap.ContainsKey(parameter.InternalName))
                    {
                        _parameterToRoleMap.Add(parameter.InternalName, parameter.QualifiedName);
                    }
                }
            }
        }

        /// <summary>
        /// Invokes the method matching the specified action ID.
        /// This should NOT be called before the dispatcher is initialized.
        /// The parameters must be populated in the parameter provider before calling this method.
        /// </summary>
        /// <param name="parameterProvider">The parameter provider.</param>
        /// <param name="actionId">The action ID (which is also the intent name).</param>
        /// <param name="relaxed">When set to true, will allow matching parameters by type when the names mismatch.</param>
        /// <param name="confidence">The confidence level (between 0-1) of the intent that's invoking the action.</param>
        /// <param name="partial">Whether partial responses should be accepted or not</param>
        /// <returns>True if all invocations succeeded. False if at least one failed or no callbacks were found.</returns>
        public bool InvokeAction(IParameterProvider parameterProvider, string actionId, bool relaxed,
            float confidence = 1f, bool partial = false)
        {
                   // BUG: Constant condition
                   // MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
                   //             if (!Manifest.ContainsAction(actionId))
                   //             {
                   //                 var hasBeenHandledWithoutConduit = Manifest.WitResponseMatcherIntents.Contains(actionId);
                   //                 if (!_ignoredActionIds.Contains(actionId) && !hasBeenHandledWithoutConduit)
                   //                 {
                   //                     _ignoredActionIds.Add(actionId);
                   //                     InvokeError(actionId, new Exception($"Conduit did not find intent '{actionId}' in manifest."));
                   //                     VLog.W($"Conduit did not find intent '{actionId}' in manifest.");
                   //                 }
                   //                 return false;
                   //             }
                   // 
                   //             parameterProvider.PopulateRoles(_parameterToRoleMap);
                   // 
                   //             var filter =
                   //                 new InvocationContextFilter(parameterProvider, Manifest.GetInvocationContexts(actionId), relaxed);
                   // 
                   //             var invocationContexts = filter.ResolveInvocationContexts(actionId, confidence, partial);
                   //             if (invocationContexts.Count < 1)
                   //             {
                   //                 // Only log if this is non-partial and inverse does not contain any matches either
                   //                 if (!partial && filter.ResolveInvocationContexts(actionId, confidence, true).Count < 1)
                   //                 {
                   //                    VLog.W(
                   //                         $"Failed to resolve {(partial ? "partial" : "final")} method for {actionId} with supplied context");
                   //                    InvokeError(actionId, new Exception($"Failed to resolve {(partial ? "partial" : "final")} method for {actionId} with supplied context")
                   //                    );
                   //                 }
                   // 
                   //                 return false;
                   //             }
                   // 
                   //             var allSucceeded = true;
                   //             foreach (var invocationContext in invocationContexts)
                   //             {
                   //                 try
                   //                 {
                   //                     if (!InvokeMethod(invocationContext, parameterProvider, relaxed))
                   //                     {
                   //                         allSucceeded = false;
                   //                     }
                   //                 }
                   //                 catch (Exception e)
                   //                 {
                   //                     VLog.W($"Failed to invoke {invocationContext.MethodInfo.Name}. {e}");
                   //                     allSucceeded = false;
                   //                     InvokeError( invocationContext.MethodInfo.Name, e);
                   //                 }
                   //             }
                   // 
                   //             return allSucceeded;
                   //         }

                   // FIXED VERSION:
