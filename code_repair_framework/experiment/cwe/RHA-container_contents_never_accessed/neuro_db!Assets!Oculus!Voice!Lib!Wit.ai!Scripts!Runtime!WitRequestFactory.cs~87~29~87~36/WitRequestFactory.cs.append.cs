            var existingKeywords = mergedArray.AsArray;
            for (int i = 0; i < existingKeywords.Count; i++)
            {
                var keyword = existingKeywords[i].AsObject;
                var key = keyword["keyword"].Value;
                if(!map.ContainsKey(key))
                {
                    map[key] = keyword;
                }
            }
            foreach (var keyword in providerEntity.keywords)
            {
                if (map.TryGetValue(keyword.keyword, out var keywordObject))
                {
                    foreach (var synonym in keyword.synonyms)
                    {
                        keywordObject["synonyms"].Add(synonym);
                    }
                }
                else
                {
                    keywordObject = JsonConvert.SerializeToken(keyword).AsObject;
                    map[keyword.keyword] = keywordObject;
                    mergedArray.Add(keywordObject);
                }
            }
        }

        private static WitRequestOptions GetSetupOptions(WitRequestOptions newOptions,
            IDynamicEntitiesProvider[] additionalDynamicEntities)
        {
            // Generate options exist
            WitRequestOptions options = newOptions ?? new WitRequestOptions();
            // Set intents
            if (-1 != options.nBestIntents)
            {
                options.QueryParams["n"] = options.nBestIntents.ToString();
            }
            // Set dynamic entities
            HandleWitRequestOptions(options, additionalDynamicEntities);
            // Set tag
            if (!string.IsNullOrEmpty(options.tag))
            {
                options.QueryParams["tag"] = options.tag;
            }
            return options;
        }

        /// <summary>
        /// Creates a message request that will process a query string with NLU
        /// </summary>
        /// <param name="config"></param>
        /// <param name="query">Text string to process with the NLU</param>
        /// <returns></returns>
        public static VoiceServiceRequest CreateMessageRequest(this WitConfiguration config, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents, IDynamicEntitiesProvider[] additionalEntityProviders = null)
        {
            var options = GetSetupOptions(requestOptions, additionalEntityProviders);
            return new WitUnityRequest(config, NLPRequestInputType.Text, options, requestEvents);
        }

        /// <summary>
        /// Creates a request for nlu processing that includes a data stream for mic data
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static WitRequest CreateSpeechRequest(this WitConfiguration config, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents, IDynamicEntitiesProvider[] additionalEntityProviders = null)
        {
            var options = GetSetupOptions(requestOptions, additionalEntityProviders);
            var path = config.GetEndpointInfo().Speech;
            return new WitRequest(config, path, options, requestEvents);
        }

        /// <summary>
        /// Creates a request for getting the transcription from the mic data
        /// </summary>
        ///<param name="config"></param>
        /// <param name="requestOptions"></param>
        /// <returns>WitRequest</returns>
        public static WitRequest CreateDictationRequest(this WitConfiguration config, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents = null)
        {
            var options = GetSetupOptions(requestOptions, null);
            var path = config.GetEndpointInfo().Dictation;
            return new WitRequest(config, path, options, requestEvents);
        }
    }
}
