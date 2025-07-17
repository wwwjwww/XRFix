// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//        void Update()
//        {
//            int state = DebugManager.instance.GetState();
//            if (m_DebugTreeState != state)
//            {
//                ResetAllHierarchy();
//            }
//
//            HandleInput();
//
//
//            if (m_UIPanels != null && m_SelectedPanel < m_UIPanels.Count && m_UIPanels[m_SelectedPanel] != null)
//                m_UIPanels[m_SelectedPanel].UpdateScroll();
//        }
