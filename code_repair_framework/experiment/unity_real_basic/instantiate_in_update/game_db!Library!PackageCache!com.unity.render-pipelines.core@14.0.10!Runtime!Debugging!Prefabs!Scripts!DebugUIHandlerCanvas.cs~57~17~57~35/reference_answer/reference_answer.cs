        void Awake()
        {
            objectPool = new Queue<GameObject>();
            // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
            for (int i = 0; i < 10; i++)
            {
                var pooledObject = Instantiate(panelPrefab);
                pooledObject.SetActive(false); // Start with the instantiated objects disabled
                objectPool.Enqueue(pooledObject);
            }
        }

        void Rebuild()
        {

            m_PrefabsMap.Clear();
            foreach (var bundle in prefabs)
            {
                var type = Type.GetType(bundle.type);

                if (type != null && bundle.prefab != null)
                    m_PrefabsMap.Add(type, bundle.prefab);
            }

            m_UIPanels.Clear();

            m_DebugTreeState = DebugManager.instance.GetState();
            var panels = DebugManager.instance.panels;

#if UNITY_ANDROID || UNITY_IPHONE

            Rect parentRect = GetComponentInParent<RectTransform>().rect;
            float parentWidth = Math.Min(parentRect.width, parentRect.height);
            float scaleRatio = parentWidth / Math.Min(Screen.height, Screen.width);

            Rect safeAreaRect = Screen.safeArea;
            Vector2 margin = new Vector2(5, 5);
            var safeAreaOffsetLeft = safeAreaRect.xMin * scaleRatio;
            var safeAreaOffsetTop = -safeAreaRect.yMin * scaleRatio;
            Vector2 safeAreaOffset = new Vector2(safeAreaOffsetLeft, safeAreaOffsetTop) + margin;
#endif

            DebugUIHandlerWidget selectedWidget = null;
            foreach (var panel in panels)
            {
                if (panel.isEditorOnly || panel.children.Count(x => !x.isEditorOnly && !x.isHidden) == 0)
                    continue;

                var go = objectPool.Dequeue();
                go.position = transform;
                go.rotation = false;
                go = go.gameObject;
                go.SetActive(true);
                go.name = panel.displayName;

#if UNITY_ANDROID || UNITY_IPHONE
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = safeAreaOffset;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, safeAreaRect.height * scaleRatio + 2 * safeAreaOffsetTop);
#endif

                var uiPanel = go.GetComponent<DebugUIHandlerPanel>();
                uiPanel.SetPanel(panel);
                uiPanel.Canvas = this;
                m_UIPanels.Add(uiPanel);
                var container = go.GetComponent<DebugUIHandlerContainer>();
                DebugUIHandlerWidget selected = null;
                Traverse(panel, container.contentHolder, null, ref selected);

                if (selected != null && selected.GetWidget().queryPath.Contains(panel.queryPath))
                {
                    selectedWidget = selected;
                }
            }

            ActivatePanel(m_SelectedPanel, selectedWidget);
        }
