
        void LoadLocalStoreLevels()
        {
            var filename = Level.GetLevelsStoreFileName();
            Level.Store store1 = Level.LoadJson<Level.Store>(filename);
            if (store1 == null || store1.levels == null || store1.levels.Length == 0 ||
                store1.version != Level.Store.STORE_VERSION)
            {
                /* sanity check */
                store = new Level.Store
                {
                    version = Level.Store.STORE_VERSION,
                    levels = Array.Empty<Level.LevelFile>(),
                };
            }
            else
            {
                store = store1;
            }
        }

        public static void StartRefreshingLevels(float delay=0.1f)
        {
            SceneLoader.StartCustomCoroutine(RefreshLevels());

            IEnumerator RefreshLevels()
            {
                yield return new WaitForSeconds(delay);

                Level.Store current_store = store;
                Level.Store updated_store = null;
                string storefilename = Level.GetLevelsStoreFileName();

                var th = new System.Threading.Thread(() =>
                {
                    string url = BASE_URL + "s/update/" + current_store.latest_revision;
                    url += "?v=" + Level.LevelFile.VERSION;

                    var request = System.Net.WebRequest.Create(url);
                    var response = request.GetResponse();
                    string wwwtext;
                    using (var reader = new StreamReader(response.GetResponseStream()))
                        wwwtext = reader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(wwwtext))
                    {
                        Debug.LogError(url + " => empty response");
                        return;
                    }

                    var store_update = JsonUtility.FromJson<Level.StoreUpdate>(wwwtext);
                    if (store_update.revision == 0)
                    {
                        store_update.revision = current_store.latest_revision;
                        store_update.mod_levels = null;
                    }
                    if (store_update.mod_levels == null)
                        store_update.mod_levels = Array.Empty<Level.LevelFile>();
                    Debug.Log(url + " => " + store_update.mod_levels.Length + " new level mods");

                    var codes_seen = new Dictionary<string, int>();
                    var lvls = new List<Level.LevelFile>();

                    foreach (var lvl in current_store.levels)
                    {
                        codes_seen[lvl.codename] = lvls.Count;
                        lvls.Add(lvl);
                    }
                    foreach (var lvl in store_update.mod_levels)
                    {
                        Debug.Assert(!string.IsNullOrEmpty(lvl.codename));
                        if (!lvl.IsEmpty)
                        {
                            codes_seen[lvl.codename] = lvls.Count;
                            lvls.Add(lvl);
                        }
                        else
                            codes_seen[lvl.codename] = -1;   /* to remove */
                    }

                    if (!string.IsNullOrEmpty(store_update.scores))
                    {
                        string scores = store_update.scores;
                        int start_index = 0;
                        while (start_index < scores.Length)
                        {
                            int end_index = scores.IndexOf(" ", start_index);
                            if (end_index < 0)
                                end_index = scores.Length;
                            Debug.Assert(end_index > start_index + 13);
                            string codename = scores.Substring(start_index, 13);
                            start_index += 13;
                            if (codes_seen.TryGetValue(codename, out int i) && i >= 0)
                                lvls[i].score_stats = scores.Substring(start_index, end_index - start_index);
                            start_index = end_index + 1;
                        }
                    }

                    var lvls2 = new List<Level.LevelFile>();
                    for (int i = 0; i < lvls.Count; i++)
                        if (codes_seen[lvls[i].codename] == i)
                            lvls2.Add(lvls[i]);

                    updated_store = new Level.Store
                    {
                        version = Level.Store.STORE_VERSION,
                        latest_revision = store_update.revision,
                        levels = lvls2.ToArray(),
                    };
                    Level.SaveJson(storefilename, updated_store);
                });
                th.Start();
                while (th.IsAlive)
                    yield return null;
                th.Join();

                if (updated_store != null)
                {
                    store = updated_store;
                    var self = FindObjectOfType<PickLevelGroup>();
                    if (self)
                        self.StoreUpdated();
                }
            }
        }

        public static void StartUploadingLevel(Level.LevelFile level, Action<string> on_response)
        {
            SceneLoader.StartCustomCoroutine(UploadLevel());

            IEnumerator UploadLevel()
            {
                level.ident = Level.GetLocalFile().machine_ident;
                level.version = Level.LevelFile.VERSION;
                string upload_data = JsonUtility.ToJson(level);
                string response = "✖ Upload failed";

                var th = new System.Threading.Thread(() =>
                {
                    string url = BASE_URL + "s/upload";

                    using (var wb = new System.Net.WebClient())
                    {
                        response = wb.UploadString(url, "POST", upload_data);
                    }
                    Debug.Log(url + " => " + response);
                });
                th.Start();
                while (th.IsAlive)
                    yield return null;
                th.Join();

                on_response(response);
            }
        }

        public class FinishToken { public bool finished; }

        public static FinishToken StartSendingScore(Level.LevelFile level, bool full_refresh = false)
        {
            var token = new FinishToken();
            SceneLoader.StartCustomCoroutine(SendScore());
            return token;

            IEnumerator SendScore()
            {
                string response = "";
                if (Level.WasSolved(level, out int ncycles, out int nblocks)
                        && ncycles < Level.CYCLES_INF && nblocks < Level.BLOCKS_INF)
                {
                    var add_score = new Level.AddScore
                    {
                        codename = level.codename,
                        ident = Level.GetLocalFile().machine_ident,
                        ncycles = ncycles,
                        nblocks = nblocks,
                        version = Level.LevelFile.VERSION,
                    };
                    string upload_data = JsonUtility.ToJson(add_score);

                    var th = new System.Threading.Thread(() =>
                    {
                        string url = BASE_URL + "s/addscore";

                        using (var wb = new System.Net.WebClient())
                        {
                            response = wb.UploadString(url, "POST", upload_data);
                        }
                        Debug.Log(url + " => " + response);
                    });
                    th.Start();
                    while (th.IsAlive)
                        yield return null;
                    th.Join();
                }

                if (response.StartsWith("OK:"))
                    level.score_stats = response.Substring(3);
                token.finished = true;
                if (full_refresh)
                    StartRefreshingLevels(delay: 1f);
            }
        }


        void StoreUpdated()
        {
            for (int i = groupsContent.childCount - 1; i >= 0; i--)
                Destroy(groupsContent.GetChild(i).gameObject);

            float h = (categoryButtonPrefab.transform as RectTransform).rect.height;
            float y = h * -0.5f;

            if (store == null || store.levels.Length == 0)
            {
                var btn = Instantiate(extraButtonPrefab, groupsContent);
                btn.GetComponentInChildren<Text>().text = "downloading levels...";
                btn.interactable = false;
                var pos = btn.transform.localPosition;
                pos.y = y;
                btn.transform.localPosition = pos;
                return;
            }

            /* compute the categories */
            var cat_dict = new Dictionary<string, float>();
            foreach (var level in store.levels)
            {
                Debug.Assert(!level.IsCustom);
                if (level.cat_sort != 0 || !cat_dict.ContainsKey(level.category))
                    cat_dict[level.category] = level.cat_sort;
            }

            var categories = cat_dict.Keys.ToList();
            categories.Sort((c1, c2) => cat_dict[c1].CompareTo(cat_dict[c2]));
            categories.Add(Level.LevelFile.CATEGORY_CUSTOM);

            if (!categories.Contains(current_category))
                current_category = categories[0];
            if (!categories.Contains(target_category))
                target_category = current_category;

            /* show them */
            for (int i = 0; i < categories.Count; i++)
            {
                string cat = categories[i];
                var btn = Instantiate(categoryButtonPrefab, groupsContent);
                btn.GetComponentInChildren<Text>().text = cat;

                var cc = btn.colors;
                if (cat == current_category)
                {
                    cc.normalColor = cc.disabledColor;
                    cc.highlightedColor = cc.pressedColor;
                    var cc1 = cc.normalColor;
                    cc1.a *= 0.5f;
                    cc.pressedColor = cc1;
                }
                cc.selectedColor = cc.normalColor;
                btn.colors = cc;

                var pos = btn.transform.localPosition;
                pos.y = y;
                btn.transform.localPosition = pos;
                y -= h;

                btn.onClick.AddListener(() =>
                {
                    if (cat == current_category)
                        return;
                    target_category = cat;
                    btn.interactable = false;
                    var cblock = btn.colors;
                    cblock.disabledColor *= 2;
                    btn.colors = cblock;
                    SceneLoader.Play2D("Pick Category");
                    FindObjectOfType<ControllersDialog>().enabled = false;
                });
            }

            if (current_category == Level.LevelFile.CATEGORY_CUSTOM)
            {
                var btn = Instantiate(extraButtonPrefab, groupsContent);
                btn.GetComponentInChildren<Text>().text = "\t<i>new level...</i>";
                var pos = btn.transform.localPosition;
                pos.y = y;
                btn.transform.localPosition = pos;

                btn.onClick.AddListener(band.MakeNewLevel);
            }
            band.UpdateContents();

            FindObjectOfType<ControllersDialog>().enabled = true;

#if UNITY_EDITOR && false
            var current_levels = GetCurrentLevels();
            SceneLoader.OneShotEvent(() =>
            {
                var lvl = current_levels[10];
                Debug.Log("LEVEL: " + lvl.codename);
                SceneSetup.LoadLevel(lvl, 10);
            });
#endif
        }

        public string GetCurrentCategory() => current_category;

        public List<Level.LevelFile> GetCurrentLevels()
        {
            if (store == null || store.levels.Length == 0)
                return new List<Level.LevelFile>();

            List<Level.LevelFile> lst;
            if (current_category != Level.LevelFile.CATEGORY_CUSTOM)
            {
                lst = store.levels.Where(lvl => lvl.category == current_category).ToList();
            }
            else
            {
                lst = Level.LoadCustomLevels();
            }
            lst.Sort((lvl1, lvl2) =>
            {
                int result = lvl1.title_sort.CompareTo(lvl2.title_sort);
                if (result == 0)
                    result = lvl1.codename.CompareTo(lvl2.codename);
                return result;
            });
            return lst;
        }

        public static bool IsCodeOfNonCustomLevel(string codename)
        {
            if (store == null || store.levels.Length == 0)
                return false;

            foreach (var lvl in store.levels)
                if (lvl.codename == codename)
                    return true;
            return false;
        }

        public void ShowHelp()
        {
            if (canvasHelp.activeSelf)
            {
                canvasHelp.SetActive(false);
                helpText.text = "Help";
            }
            else
            {
                canvasHelp.SetActive(true);
                helpText.text = "Close help";

                bool is_oculus = Baroque.GetControllers().Any(ctrl => ctrl.controllerModel == Controller.Model.OculusTouch);
                bool is_index = Baroque.GetControllers().Any(ctrl => ctrl.controllerModel == Controller.Model.Knuckles);
                var touchpad = is_oculus ? "A/X button" : is_index ? "B button" : "Touchpad";
                var tt = canvasHelp.transform.Find("Text Touchpad").GetComponent<Text>();
                tt.text = tt.text.Replace("XXX", touchpad);
            }
        }
    }
}
