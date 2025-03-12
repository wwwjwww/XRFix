#if AR_FOUNDATION_PRESENT
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets
{



    public class ARInteractorSpawnTrigger : MonoBehaviour
    {



        public enum SpawnTriggerType
        {




            SelectAttempt,




            InputAction,
        }

        [SerializeField]
        [Tooltip("When enabled, spawn will not be triggered if an object is currently selected.")]
        bool m_BlockSpawnWhenInteractorHasSelection = true;




        public XRRayInteractor arInteractor
        {
            get => m_ARInteractor;
            set => m_ARInteractor = value;
        }

        [SerializeField]
        [Tooltip("When enabled, spawn will not be triggered if an object is currently selected.")]
        bool m_BlockSpawnWhenInteractorHasSelection = true;




        public ObjectSpawner objectSpawner
        {
            get => m_ObjectSpawner;
            set => m_ObjectSpawner = value;
        }

        [SerializeField]
        [Tooltip("When enabled, spawn will not be triggered if an object is currently selected.")]
        bool m_BlockSpawnWhenInteractorHasSelection = true;





        public bool requireHorizontalUpSurface
        {
            get => m_RequireHorizontalUpSurface;
            set => m_RequireHorizontalUpSurface = value;
        }

        [SerializeField]
        [Tooltip("When enabled, spawn will not be triggered if an object is currently selected.")]
        bool m_BlockSpawnWhenInteractorHasSelection = true;




        public SpawnTriggerType spawnTriggerType
        {
            get => m_SpawnTriggerType;
            set => m_SpawnTriggerType = value;
        }

        [SerializeField]
        [Tooltip("When enabled, spawn will not be triggered if an object is currently selected.")]
        bool m_BlockSpawnWhenInteractorHasSelection = true;




        public XRInputButtonReader spawnObjectInput
        {
            get => m_SpawnObjectInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_SpawnObjectInput, value, this);
        }

        [SerializeField]
        [Tooltip("When enabled, spawn will not be triggered if an object is currently selected.")]
        bool m_BlockSpawnWhenInteractorHasSelection = true;




        public bool blockSpawnWhenInteractorHasSelection
        {
            get => m_BlockSpawnWhenInteractorHasSelection;
            set => m_BlockSpawnWhenInteractorHasSelection = value;
        }

        bool m_AttemptSpawn;
        bool m_EverHadSelection;




        void OnEnable()
        {
            m_SpawnObjectInput.EnableDirectActionIfModeUsed();
        }




        void OnDisable()
        {
            m_SpawnObjectInput.DisableDirectActionIfModeUsed();
        }




        void Start()
        {
            if (m_ObjectSpawner == null)
                m_ObjectSpawner = FindObjectOfType<ObjectSpawner>();

            if (m_ARInteractor == null)
            {
                Debug.LogError("Missing AR Interactor reference, disabling component.", this);
                enabled = false;
            }
        }





/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*         void Update()
*         {
* 
* 
* 
*             if (m_AttemptSpawn)
*             {
*                 m_AttemptSpawn = false;
* 
* 
*                 var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
*                 if (!isPointerOverUI && m_ARInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
*                 {
*                     if (!(arRaycastHit.trackable is ARPlane arPlane))
*                         return;
* 
*                     if (m_RequireHorizontalUpSurface && arPlane.alignment != PlaneAlignment.HorizontalUp)
*                         return;
* 
*                     m_ObjectSpawner.TrySpawnObject(arRaycastHit.pose.position, arPlane.normal);
*                 }
* 
*                 return;
*             }
* 
*             var selectState = m_ARInteractor.logicalSelectState;
* 
*             if (m_BlockSpawnWhenInteractorHasSelection)
*             {
*                 if (selectState.wasPerformedThisFrame)
*                     m_EverHadSelection = m_ARInteractor.hasSelection;
*                 else if (selectState.active)
*                     m_EverHadSelection |= m_ARInteractor.hasSelection;
*             }
* 
*             m_AttemptSpawn = false;
*             switch (m_SpawnTriggerType)
*             {
*                 case SpawnTriggerType.SelectAttempt:
*                     if (selectState.wasCompletedThisFrame)
*                         m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
*                     break;
* 
*                 case SpawnTriggerType.InputAction:
*                     if (m_SpawnObjectInput.ReadWasPerformedThisFrame())
*                         m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
*                     break;
*             }
*         }



        static List<XRBaseInputDevice> s_InputDevices = new List<XRBaseInputDevice>();

        void Update()
        {
            if (!enabled)
                return;

            UpdateInput();
            if (m_AttemptSpawn)
            {
                m_AttemptSpawn = false;
                var arRaycastHit = default(RaycastHit);
                if (m_ARInteractor.TryGetCurrentARRaycastHit(out arRaycastHit))
                {
                    if (!(arRaycastHit.trackable is ARPlane arPlane))
                        return;
                    if (m_RequireHorizontalUpSurface && arPlane.alignment!= PlaneAlignment.HorizontalUp)
                        return;
                    m_ObjectSpawner.TrySpawnObject(arRaycastHit.pose.position, arPlane.normal);
                }
            }

            if (m_EverHadSelection)
                m_AttemptSpawn = false;
        }

        void UpdateInput()
        {
            s_InputDevices.Clear();
            XRInputSubsystem.InputSubsystem.GetConnectedDevices(s_InputDevices);
            foreach (var device in s_InputDevices)
                device.Update();

            var selectState = m_ARInteractor.logicalSelectState;
            if (m_BlockSpawnWhenInteractorHasSelection)
            {
                if (selectState.wasPerformedThisFrame)
                    m_EverHadSelection = m_ARInteractor.hasSelection;
                else if (selectState.active)
                    m_EverHadSelection |= m_ARInteractor.hasSelection;
            }

            m_AttemptSpawn = false;
            switch (m_SpawnTriggerType)
            {
                case SpawnTriggerType.SelectAttempt:
                    if (selectState.wasCompletedThisFrame)
                        m_AttemptSpawn =!m_ARInteractor.hasSelection &&!m_EverHadSelection;
                    break;
                case SpawnTriggerType.InputAction:
                    if (m_SpawnObjectInput.ReadWasPerformedThisFrame())
                        m_AttemptSpawn =!m_ARInteractor.hasSelection &&!m_EverHadSelection;
                    break;
            }
        }

        public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
        {
            if (!enabled)
                return false;

            var result = false;
            if (m_ObjectSpawner!= null)
            {
                var inView = m_OnlySpawnInView? m_CameraToFace.WorldToViewportPoint(spawnPoint) : new Vector3(0, 0, 1);
                if (inView.z >= 0 && inView.x >= 0 && inView.x <= 1 && inView.y >= 0 && inView.y <= 1)
                    result = m_ObjectSpawner.TrySpawnObject(spawnPoint, spawnNormal);
            }
            return result;
        }


    }
}
#endif
