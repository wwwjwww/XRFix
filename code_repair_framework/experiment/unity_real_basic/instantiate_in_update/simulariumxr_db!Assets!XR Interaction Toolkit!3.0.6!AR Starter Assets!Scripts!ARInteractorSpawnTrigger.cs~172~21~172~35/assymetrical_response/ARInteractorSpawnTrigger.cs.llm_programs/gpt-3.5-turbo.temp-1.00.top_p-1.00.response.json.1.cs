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
        [Tooltip("The AR ray interactor that determines where to spawn the object.")]
        XRRayInteractor m_ARInteractor;




        public XRRayInteractor arInteractor
        {
            get => m_ARInteractor;
            set => m_ARInteractor = value;
        }

        [SerializeField]
        [Tooltip("The behavior to use to spawn objects.")]
        ObjectSpawner m_ObjectSpawner;




        public ObjectSpawner objectSpawner
        {
            get => m_ObjectSpawner;
            set => m_ObjectSpawner = value;
        }

        [SerializeField]
        [Tooltip("Whether to require that the AR Interactor hits an AR Plane with a horizontal up alignment in order to spawn anything.")]
        bool m_RequireHorizontalUpSurface;





        public bool requireHorizontalUpSurface
        {
            get => m_RequireHorizontalUpSurface;
            set => m_RequireHorizontalUpSurface = value;
        }

        [SerializeField]
        [Tooltip("The type of trigger to use to spawn an object, either when the Interactor's select action occurs or " +
            "when a button input is performed.")]
        SpawnTriggerType m_SpawnTriggerType;




        public SpawnTriggerType spawnTriggerType
        {
            get => m_SpawnTriggerType;
            set => m_SpawnTriggerType = value;
        }

        [SerializeField]
        XRInputButtonReader m_SpawnObjectInput = new XRInputButtonReader("Spawn Object");




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
    for (int i = 0; i < 10; i++)
    {
        GameObject obj = Instantiate(m_ObjectSpawner.objectPrefabs[0]);
        obj.SetActive(false);
        objectPool.Add(obj);
    }

    if (m_ObjectSpawner == null)
#if UNITY_2023_1_OR_NEWER
        m_ObjectSpawner = FindAnyObjectByType<ObjectSpawner>();
#else
        m_ObjectSpawner = FindObjectOfType<ObjectSpawner>();
#endif

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



private List<GameObject> objectPool = new List<GameObject>();

void Update()
{
    if (m_AttemptSpawn)
    {
        m_AttemptSpawn = false;

        var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
        if (!isPointerOverUI && m_ARInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
        {
            if (!(arRaycastHit.trackable is ARPlane arPlane))
                return;

            if (m_RequireHorizontalUpSurface && arPlane.alignment != PlaneAlignment.HorizontalUp)
                return;

            //Use object pool to spawn object
            GameObject obj = GetPooledObject();
            if (obj != null)
            {
                obj.transform.position = arRaycastHit.pose.position;
                obj.transform.rotation = Quaternion.LookRotation(arPlane.normal, Vector3.up);
                obj.SetActive(true);
            }
        }

        return;
    }

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
                m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
            break;

        case SpawnTriggerType.InputAction:
            if (m_SpawnObjectInput.ReadWasPerformedThisFrame())
                m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
            break;
    }
}

private GameObject GetPooledObject()
{
    foreach (GameObject obj in objectPool)
    {
        if (!obj.activeInHierarchy)
        {
            return obj;
        }
    }

    return null;
}

    }
}
#endif
