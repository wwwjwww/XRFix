using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.UI
{



    [Serializable]
    public class DebugUIPrefabBundle
    {

        public string type;

        public RectTransform prefab;
    }




    public class DebugUIHandlerCanvas : MonoBehaviour
    {
        int m_DebugTreeState;
        Dictionary<Type, Transform> m_PrefabsMap;


        public Transform panelPrefab;

        public List<DebugUIPrefabBundle> prefabs;

        List<DebugUIHandlerPanel> m_UIPanels;
        int m_SelectedPanel;
        DebugUIHandlerWidget m_SelectedWidget;
        string m_CurrentQueryPath;

        void OnEnable()
        {
            if (prefabs == null)
                prefabs = new List<DebugUIPrefabBundle>();

            if (m_PrefabsMap == null)
                m_PrefabsMap = new Dictionary<Type, Transform>();

            if (m_UIPanels == null)
                m_UIPanels = new List<DebugUIHandlerPanel>();

            DebugManager.instance.RegisterRootCanvas(this);
        }

        void Update()
        {
            int state = DebugManager.instance.GetState();
            if (m_DebugTreeState != state)
            {
                ResetAllHierarchy();
            }

            HandleInput();


            if (m_UIPanels != null && m_SelectedPanel < m_UIPanels.Count && m_UIPanels[m_SelectedPanel] != null)
                m_UIPanels[m_SelectedPanel].UpdateScroll();
        }

        internal void RequestHierarchyReset()
        {
            m_DebugTreeState = -1;
        }

        void ResetAllHierarchy()
        {
            foreach (Transform t in transform)
                CoreUtils.Destroy(t.gameObject);

            Rebuild();
        }

