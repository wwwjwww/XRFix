/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Meta.Voice;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using Meta.WitAi.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.WitAi
{
    /// <summary>
    /// Manages a single request lifecycle when sending/receiving data from Wit.ai.
    ///
    /// Note: This is not intended to be instantiated directly. Requests should be created with the
    /// WitRequestFactory
    /// </summary>
    public class WitRequest : VoiceServiceRequest
    {
        #region PARAMETERS
        /// <summary>
        /// The wit Configuration to be used with this request
        /// </summary>
        public WitConfiguration Configuration { get; private set; }
        /// <summary>
        /// The request timeout in ms
        /// </summary>
        public int TimeoutMs { get; private set; } = 1000;
        /// <summary>
        /// Encoding settings for audio based requests
        /// </summary>
        public AudioEncoding AudioEncoding { get; set; }
        [Obsolete("Deprecated for AudioEncoding")]
        public AudioEncoding audioEncoding
        {
            get => AudioEncoding;
            set => AudioEncoding = value;
        }

        /// <summary>
        /// Endpoint to be used for this request
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// Final portion of the endpoint Path
        /// </summary>
        public string Command { get; private set; }
        /// <summary>
        /// Whether a post command should be called
        /// </summary>
        public bool IsPost { get; private set; }
        /// <summary>
        /// Key value pair that is sent as a query param in the Wit.ai uri
        /// </summary>
        [Obsolete("Deprecated for Options.QueryParams")]
        public VoiceServiceRequestOptions.QueryParam[] queryParams
        {
            get
            {
                List<VoiceServiceRequestOptions.QueryParam> results = new List<VoiceServiceRequestOptions.QueryParam>();
                foreach (var key in Options?.QueryParams?.Keys)
                {
                    VoiceServiceRequestOptions.QueryParam p = new VoiceServiceRequestOptions.QueryParam()
                    {
                        key = key,
                        value = Options?.QueryParams[key]
                    };
                    results.Add(p);
                }
                return results.ToArray();
            }
        }

        public byte[] postData;
        public string postContentType;
        public string forcedHttpMethodType = null;
        #endregion PARAMETERS

        #region REQUEST
        /// <summary>
        /// Returns true if the request is being performed
        /// </summary>
        public bool IsRequestStreamActive => IsActive || IsInputStreamReady;
        /// <summary>
        /// Returns true if the response had begun
        /// </summary>
        public bool HasResponseStarted { get; private set; }
        /// <summary>
        /// Returns true if the response had begun
        /// </summary>
        public bool IsInputStreamReady { get; private set; }

        public AudioDurationTracker audioDurationTracker;
        private HttpWebRequest _request;
        private Stream _writeStream;
        private object _streamLock = new object();
        private int _bytesWritten;
        private string _stackTrace;
        private DateTime _requestStartTime;
        private ConcurrentQueue<byte[]> _writeBuffer = new ConcurrentQueue<byte[]>();
        #endregion REQUEST

        #region RESULTS
        /// <summary>
        /// The current status of the request
        /// </summary>
        public string StatusDescription { get; private set; }

        /// <summary>
        /// Simply return the Path to be called
        /// </summary>
        public override string ToString() => Path;

        /// <summary>
        /// Last response data parsed
        /// </summary>
        private WitResponseNode _lastResponseData;
        #endregion RESULTS

        #region EVENTS
        /// <summary>
        /// Provides an opportunity to provide custom headers for the request just before it is
        /// executed.
        /// </summary>
        public event OnProvideCustomHeadersEvent onProvideCustomHeaders;
        public delegate Dictionary<string, string> OnProvideCustomHeadersEvent();
        /// <summary>
        /// Callback called when the server is ready to receive data from the WitRequest's input
        /// stream. See WitRequest.Write()
        /// </summary>
        public event Action<WitRequest> onInputStreamReady;
        /// <summary>
        /// Returns the raw string response that was received before converting it to a JSON object.
        ///
        /// NOTE: This response comes back on a different thread. Do not attempt ot set UI control
        /// values or other interactions from this callback. This is intended to be used for demo
        /// and test UI, not for regular use.
        /// </summary>
        public Action<string> onRawResponse;

        /// <summary>
        /// Provides an opportunity to customize the url just before a request executed
        /// </summary>
        [Obsolete("Deprecated for WitVRequest.OnProvideCustomUri")]
        public OnCustomizeUriEvent onCustomizeUri;
        public delegate Uri OnCustomizeUriEvent(UriBuilder uriBuilder);
        /// <summary>
        /// Allows customization of the request before it is sent out.
        ///
        /// Note: This is for devs who are routing requests to their servers
        /// before sending data to Wit.ai. This allows adding any additional
        /// headers, url modifications, or customization of the request.
        /// </summary>
        public static PreSendRequestDelegate onPreSendRequest;
        public delegate void PreSendRequestDelegate(ref Uri src_uri, out Dictionary<string,string> headers);
        /// <summary>
        /// Returns a partial utterance from an in process request
        ///
        /// NOTE: This response comes back on a different thread.
        /// </summary>
        [Obsolete("Deprecated for Events.OnPartialTranscription")]
        public event Action<string> onPartialTranscription;
        /// <summary>
        /// Returns a full utterance from a completed request
        ///
        /// NOTE: This response comes back on a different thread.
        /// </summary>
        [Obsolete("Deprecated for Events.OnFullTranscription")]
        public event Action<string> onFullTranscription;

        /// <summary>
        /// Callback called when a response is received from the server off a partial transcription
        /// </summary>
        [Obsolete("Deprecated for Events.OnPartialResponse")]
        public event Action<WitRequest> onPartialResponse;
        /// <summary>
        /// Callback called when a response is received from the server
        /// </summary>
        [Obsolete("Deprecated for Events.OnComplete")]
        public event Action<WitRequest> onResponse;
        #endregion EVENTS

        #region INITIALIZATION
        /// <summary>
        /// Initialize wit request with configuration & path to endpoint
        /// </summary>
        /// <param name="newConfiguration"></param>
        /// <param name="newOptions"></param>
        /// <param name="newEvents"></param>
        public WitRequest(WitConfiguration newConfiguration, string newPath,
            WitRequestOptions newOptions, VoiceServiceRequestEvents newEvents)
            : base(NLPRequestInputType.Audio, newOptions, newEvents)
        {
            // Set Configuration & path
            Configuration = newConfiguration;
            Path = newPath;

            // Finalize
            _initialized = true;
            SetState(VoiceRequestState.Initialized);
        }
        /// <summary>
        /// Only set state if initialized
        /// </summary>
        private bool _initialized = false;
        protected override void SetState(VoiceRequestState newState)
        {
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private ConcurrentQueue<byte[]> _writeBuffer = new ConcurrentQueue<byte[]>();
        //         #endregion REQUEST
        // 
        //         #region RESULTS
        //         /// <summary>
        //         /// The current status of the request
        //         /// </summary>
        //         public string StatusDescription { get; private set; }
        // 
        //         /// <summary>
        //         /// Simply return the Path to be called
        //         /// </summary>
        //         public override string ToString() => Path;
        // 
        //         /// <summary>
        //         /// Last response data parsed
        //         /// </summary>
        //         private WitResponseNode _lastResponseData;
        //         #endregion RESULTS
        // 
        //         #region EVENTS
        //         /// <summary>
        //         /// Provides an opportunity to provide custom headers for the request just before it is
        //         /// executed.
        //         /// </summary>
        //         public event OnProvideCustomHeadersEvent onProvideCustomHeaders;
        //         public delegate Dictionary<string, string> OnProvideCustomHeadersEvent();
        //         /// <summary>
        //         /// Callback called when the server is ready to receive data from the WitRequest's input
        //         /// stream. See WitRequest.Write()
        //         /// </summary>
        //         public event Action<WitRequest> onInputStreamReady;
        //         /// <summary>
        //         /// Returns the raw string response that was received before converting it to a JSON object.
        //         ///
        //         /// NOTE: This response comes back on a different thread. Do not attempt ot set UI control
        //         /// values or other interactions from this callback. This is intended to be used for demo
        //         /// and test UI, not for regular use.
        //         /// </summary>
        //         public Action<string> onRawResponse;
        // 
        //         /// <summary>
        //         /// Provides an opportunity to customize the url just before a request executed
        //         /// </summary>
        //         [Obsolete("Deprecated for WitVRequest.OnProvideCustomUri")]
        //         public OnCustomizeUriEvent onCustomizeUri;
        //         public delegate Uri OnCustomizeUriEvent(UriBuilder uriBuilder);
        //         /// <summary>
        //         /// Allows customization of the request before it is sent out.
        //         ///
        //         /// Note: This is for devs who are routing requests to their servers
        //         /// before sending data to Wit.ai. This allows adding any additional
        //         /// headers, url modifications, or customization of the request.
        //         /// </summary>
        //         public static PreSendRequestDelegate onPreSendRequest;
        //         public delegate void PreSendRequestDelegate(ref Uri src_uri, out Dictionary<string,string> headers);
        //         /// <summary>
        //         /// Returns a partial utterance from an in process request
        //         ///
        //         /// NOTE: This response comes back on a different thread.
        //         /// </summary>
        //         [Obsolete("Deprecated for Events.OnPartialTranscription")]
        //         public event Action<string> onPartialTranscription;
        //         /// <summary>
        //         /// Returns a full utterance from a completed request
        //         ///
        //         /// NOTE: This response comes back on a different thread.
        //         /// </summary>
        //         [Obsolete("Deprecated for Events.OnFullTranscription")]
        //         public event Action<string> onFullTranscription;
        // 
        //         /// <summary>
        //         /// Callback called when a response is received from the server off a partial transcription
        //         /// </summary>
        //         [Obsolete("Deprecated for Events.OnPartialResponse")]
        //         public event Action<WitRequest> onPartialResponse;
        //         /// <summary>
        //         /// Callback called when a response is received from the server
        //         /// </summary>
        //         [Obsolete("Deprecated for Events.OnComplete")]
        //         public event Action<WitRequest> onResponse;
        //         #endregion EVENTS
        // 
        //         #region INITIALIZATION
        //         /// <summary>
        //         /// Initialize wit request with configuration & path to endpoint
        //         /// </summary>
        //         /// <param name="newConfiguration"></param>
        //         /// <param name="newOptions"></param>
        //         /// <param name="newEvents"></param>
        //         public WitRequest(WitConfiguration newConfiguration, string newPath,
        //             WitRequestOptions newOptions, VoiceServiceRequestEvents newEvents)
        //             : base(NLPRequestInputType.Audio, newOptions, newEvents)
        //         {
        //             // Set Configuration & path
        //             Configuration = newConfiguration;
        //             Path = newPath;
        // 
        //             // Finalize
        //             _initialized = true;
        //             SetState(VoiceRequestState.Initialized);
        //         }
        //         /// <summary>
        //         /// Only set state if initialized
        //         /// </summary>
        //         private bool _initialized = false;
        //         protected override void SetState(VoiceRequestState newState)
        //         {
        //             if (_initialized)
        //             {
        //                 base.SetState(newState);
        //             }
        //         }

        // FIXED VERSION:
