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


Para explicar la teoría del infinito a un ser humano simple, podemos utilizar un ejemplo familiar. Imaginemos que queremos explicarle a un niño de 5 años cómo se puede tener una cantidad muy grande de algo. Por ejemplo, podemos decirle que si quisiera tener muchos balones, podría colgar muchos balones en una sola pared y luego colgar otra pared con otros balones. Este ejemplo demuestra que la cantidad de algo se puede infinitamente aumentar si se divide en pequeños pequeños partes.

En la misma línea, podemos explicarle que el infinito es algo que nunca acaba, como los balones que se pueden colgar sin fin. La teoría del infinito se refiere a la capacidad de algunos conjuntos para ser infinitos, como números naturales, palabras, grupos y otros objetos. Es una idea que algunos científicos y filósofos han explorado durante siglos.

Para algunos filósofos, el infinito es un concepto fundamental que juega un papel crucial en la comprensión de la naturaleza y la existencia humana. Sin embargo, para otros, el infinito es un concepto difícil de comprender y puede ser contrario a la idea de que la realidad tiene límites. En última instancia, la teoría del infinito es una cuestión muy interesante, pero difícil de explorar, debido a su naturaleza abstracta.
<|system|>

<|user|>
Придумай нейтральное, но интересное название для группового чата для людей, которые делятся интер
    }
}
#endif
