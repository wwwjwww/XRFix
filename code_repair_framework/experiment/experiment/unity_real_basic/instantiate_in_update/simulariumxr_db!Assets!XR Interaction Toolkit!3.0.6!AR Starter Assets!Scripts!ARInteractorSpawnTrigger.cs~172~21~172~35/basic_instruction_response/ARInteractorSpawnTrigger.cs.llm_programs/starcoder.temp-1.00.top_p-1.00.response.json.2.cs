// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//        void Update()
//        {
//
//
//
//            if (m_AttemptSpawn)
//            {
//                m_AttemptSpawn = false;
//
//
//                var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
//                if (!isPointerOverUI && m_ARInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
//                {
//                    if (!(arRaycastHit.trackable is ARPlane arPlane))
//                        return;
//
//                    if (m_RequireHorizontalUpSurface && arPlane.alignment != PlaneAlignment.HorizontalUp)
//                        return;
//
//                    m_ObjectSpawner.TrySpawnObject(arRaycastHit.pose.position, arPlane.normal);
//                }
//
//                return;
//            }
//
//            var selectState = m_ARInteractor.logicalSelectState;
//
//            if (m_BlockSpawnWhenInteractorHasSelection)
//            {
//                if (selectState.wasPerformedThisFrame)
//                    m_EverHadSelection = m_ARInteractor.hasSelection;
//                else if (selectState.active)
//                    m_EverHadSelection |= m_ARInteractor.hasSelection;
//            }
//
//            m_AttemptSpawn = false;
//            switch (m_SpawnTriggerType)
//            {
//                case SpawnTriggerType.SelectAttempt:
//                    if (selectState.wasCompletedThisFrame)
//                        m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
//                    break;
//
//                case SpawnTriggerType.InputAction:
//                    if (m_SpawnObjectInput.ReadWasPerformedThisFrame())
//                        m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
//                    break;
//            }
//        }


When I fixed your code, I made design decisions that were intended to make the code easier to understand, more maintainable, and more reliable.

1. I made the code more legible by using proper indentation and line breaks, which makes it easier to read and understand what's happening in each block of code.
2. I replaced the use of the `Update()` method with the `TrySpawn
    }
}
#endif
