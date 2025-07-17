
using System.Diagnostics.CodeAnalysis;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;




[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Local")]
public class BrickAttach : MonoBehaviour {
    private Color32 _color;
    public Color32 Color {
        get => _color;
        set => SetColor(value);
    }

    public Color32 HoverColor {
        get {
            int difference = (Convert.ToInt32(_color.r) + Convert.ToInt32(_color.g) + Convert.ToInt32(_color.b)) > 300
                ? -20
                : 20;
            byte r = (byte)(Mathf.Clamp(_color.r + difference, 0, 255));
            byte g = (byte)(Mathf.Clamp(_color.g + difference, 0, 255));
            byte b = (byte)(Mathf.Clamp(_color.b + difference, 0, 255));
            byte a = (byte)(Mathf.Clamp(_color.a, 0, 255));

            return new Color32(r, g, b, a);
        }
    }

    private Session session;
    private bool _hoveredLeft;
    private bool _hoveredRight;
    public bool Held { get; private set; }

    public int renderQueue;

    public GameObject maleConnectorParent;
    public GameObject femaleConnectorParent;

    public GameObject model;
    public GameObject combinedModel;

    private BrickUuid _brickUuid;
    private GameObject _modularModel;

    private BulkGrabFollow[] _attachedBulkGrabFollows;

    
    public string swapPrefab;

    public string normalPrefabName;

    public List<GameObject> maleConnectors;
    public List<GameObject> femaleConnectors;

    public List<LegoConnectorScript> maleConnectorScripts;
    public List<LegoConnectorScript> femaleConnectorScripts;

    private readonly Dictionary<string, LegoConnectorScript> maleConnectorScriptsByName =
        new Dictionary<string, LegoConnectorScript>();

    public Mesh originalMesh;
    public Material originalMaterial;

    private UserSettings _userSettings;
    private static readonly int ShaderColorProperty = Shader.PropertyToID("_Color");
    private static readonly int ShaderTexOffsetProperty = Shader.PropertyToID("_TexOffset");
    private MaterialPropertyBlock _props;

    public Mesh solidMesh;
    public Mesh hollowMesh;
    public Mesh studMesh;

    public PlacedBrickRenderer placedBrickRenderer;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public bool renderHollowMesh;

    public string headClientId;
    public bool isPlayerHead;

    private AvatarManager _avatarManager;

    public float texOffset;

    private void Awake() {
        texOffset = Random.Range(0f, 1f);
        _props = new MaterialPropertyBlock();
        _brickUuid = GetComponent<BrickUuid>();
        _userSettings = UserSettings.GetInstance();
        placedBrickRenderer = GetComponent<PlacedBrickRenderer>();
        meshFilter = model.GetComponent<MeshFilter>();
        meshRenderer = model.GetComponent<MeshRenderer>();

        foreach (Transform child in maleConnectorParent.transform) {
            maleConnectorScriptsByName.Add(child.gameObject.name, child.gameObject.GetComponent<LegoConnectorScript>());
        }

        ModularBrickObjects modularBrickObject = ModularBrickObjects.GetInstance();
        _modularModel = modularBrickObject.GetModularModel(normalPrefabName);
        solidMesh = modularBrickObject.GetSolidMesh(normalPrefabName);
        hollowMesh = modularBrickObject.GetHollowMesh(normalPrefabName);
        studMesh = modularBrickObject.GetStudMesh(normalPrefabName);

        if (Application.isEditor) {
            originalMaterial = model.GetComponent<MeshRenderer>().sharedMaterial;
            originalMesh = _userSettings.SuperUltraPerformanceMode ? _modularModel.transform.Find("FlatBody").GetComponentInChildren<MeshFilter>().sharedMesh : model.GetComponent<MeshFilter>().sharedMesh;
        }

        SetSortedMaterial();
    }

    private void Start() {
        OwnedPhysicsBricksStore.GetInstance().AddBrick(gameObject);
        _avatarManager = AvatarManager.GetInstance();
        session = Session.GetInstance();
        headClientId = session.ClientID;

        if (!isPlayerHead && headClientId == session.ClientID) 
            ChunkedRenderer.GetInstance().AddBrickToRenderer(gameObject);
    }

    private void SetSortedMaterial(Mesh mesh = null)
    {
        MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();

        meshRenderer.material = BrickRenderQueueSorter.SortedMaterialFromMesh(mesh ? mesh : meshFilter.sharedMesh, meshRenderer.sharedMaterial);
        renderQueue = meshRenderer.sharedMaterial.renderQueue;
    }

    public void SetUuid(string uuid) {
        _brickUuid.uuid = uuid;
    }

    public string GetUuid()
    {
        return _brickUuid.uuid;
    }

    public void SetColor(Color32 color)
    {
        if (!model)
        {
            Debug.LogError("There should be a model on this object");
            return;
        }

        _color = color;

        UpdateBrickColor();
    }

    public void SetHovered(bool hovered, bool left)
    {
        if ((left && (_hoveredLeft == hovered)) || (!left && (_hoveredRight == hovered)))
            return;

        if (left)
            _hoveredLeft = hovered;
        else
            _hoveredRight = hovered;

        UpdateBrickColor();
    }

    public void SetHeld(bool held)
    {
        if (Held == held)
            return;

        Held = held;
        UpdateBrickColor();
    }

    private void UpdateBrickColor() {
        if (_props == null) return;

        _props.SetColor(ShaderColorProperty, (_hoveredLeft || _hoveredRight || Held) ? HoverColor : _color);
        _props.SetFloat(ShaderTexOffsetProperty, texOffset);

        MeshRenderer renderer = model.GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(_props);
    }

    public bool ConnectBricks(Vector3 newPos, Quaternion newRot, Vector3 connectionDirection, Session session = null) {
        session = session ?? Session.GetInstance();
        transform.position = newPos;
        transform.rotation = newRot;

        List<GameObject> bricksBelow = OverlappingBricksFromConnectors(femaleConnectors);
        List<BrickAttach> bricksBelowAttaches = bricksBelow.Select(b => b.GetComponent<BrickAttach>()).ToList();

        List<GameObject> bricksAbove = OverlappingBricksFromConnectors(maleConnectors);
        List<BrickAttach> bricksAboveAttaches = bricksAbove.Select(b => b.GetComponent<BrickAttach>()).ToList();

        string attachedToHeadClientId = session.ClientID;
        foreach(BrickAttach attach in bricksBelowAttaches)
            if (attach.headClientId != session.ClientID)
                attachedToHeadClientId = attach.headClientId;

        foreach(BrickAttach attach in bricksAboveAttaches)
            if (attach.headClientId != session.ClientID)
                attachedToHeadClientId = attach.headClientId;

        for (int i = 0; i < bricksBelowAttaches.Count; i++) {
            ConfigureNeighboringBrick(bricksBelowAttaches[i], bricksBelow[i], true);
        }

        for (int i = 0; i < bricksAboveAttaches.Count(); i++) {
            ConfigureNeighboringBrick(bricksAboveAttaches[i], bricksAbove[i], false);
        }

        BrickSwapper.SwapToFakeBrick(gameObject, attachedToHeadClientId, _avatarManager);

        return true;
    }

    private void ConfigureNeighboringBrick(BrickAttach attach, GameObject brick, bool below) {
        if (attach == null) return;
        if (attach.GetUuid() == GetUuid()) return;

        BrickSwapper.SwapToFakeBrick(brick);
    }

    private List<GameObject> OverlappingBricksFromConnectors(List<GameObject> connectors) {
        return connectors.Select(OverlappingBrickFromConnector).Where(x => x != null).Distinct().ToList();
    }

    
    
    private GameObject OverlappingBrickFromConnector(GameObject connector) {
        return connector.GetComponent<LegoConnectorScript>().BrickCollidingWith(null);
    }

    public void RecalculateEnabledConnectors() {
        foreach (LegoConnectorScript script in maleConnectorScripts) {
            script.RecalculateEnabled();
        }

        foreach (LegoConnectorScript script in femaleConnectorScripts) {
            script.RecalculateEnabled();
        }
    }

    public void RecalculateRenderedGeometry() {
        
        
        
        
        

        
        
        

        
        
        bool placed = !(placedBrickRenderer != null);
        bool performanceMode = _userSettings.SuperUltraPerformanceMode;

        string cacheKey = "";
        cacheKey += normalPrefabName;

        bool allFemalePegsCovered = femaleConnectorScripts.All(s => s.covered);

        bool isOnCarpet = IsOnCarpet();

        if (performanceMode)
        {
            cacheKey += "perfmode"; 
        }
        else
        {
            foreach (LegoConnectorScript s in maleConnectorScripts)
            {
                cacheKey += ((placed && s.covered) ? "Y" : "N");
            }

            if((allFemalePegsCovered || isOnCarpet) && placed) cacheKey += "-bottomcovered";
        }

        MeshFilter combinedMeshFilter = combinedModel.GetComponent<MeshFilter>();

        Mesh cachedMesh = BrickMeshCache.GetInstance().Get(cacheKey);
        if (cachedMesh != null)
        {
            combinedMeshFilter.mesh = cachedMesh;
            SetSortedMaterial(combinedMeshFilter.sharedMesh);
            return;
        }

        Vector3 originalPos = transform.position;
        transform.position = Vector3.zero;

        Quaternion originalRot = transform.rotation;
        transform.rotation = Quaternion.identity;

        List<CombineInstance> combineInstances = new List<CombineInstance>();

        _modularModel.transform.position = transform.position;

        foreach(MeshFilter meshFilter in _modularModel.GetComponentsInChildren<MeshFilter>())
        {
            GameObject obj = meshFilter.gameObject;
            string parentName = meshFilter.transform.parent.name;

            switch (parentName)
            {
                
                case "Studs" when (maleConnectorScriptsByName[obj.name].covered || performanceMode):
                
                case "Tubes" when (allFemalePegsCovered || performanceMode):
                
                case "Body" when (allFemalePegsCovered || performanceMode || isOnCarpet):
                
                case "FlatBody" when !performanceMode && !allFemalePegsCovered && !isOnCarpet:
                    continue;
            }

            CombineInstance instance = new CombineInstance();
            instance.mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            instance.transform = obj.GetComponent<MeshFilter>().transform.localToWorldMatrix;
            combineInstances.Add(instance);
        }

        Mesh newMesh = new Mesh();
        combinedMeshFilter.mesh = newMesh;
        combinedMeshFilter.mesh.CombineMeshes(combineInstances.ToArray());
        combinedMeshFilter.mesh.Optimize();

        BrickMeshCache.GetInstance().Put(cacheKey, combinedMeshFilter.sharedMesh);

        transform.position = originalPos;
        transform.rotation = originalRot;

        SetColor(_color);
        SetSortedMaterial(combinedMeshFilter.sharedMesh);
    }

    public void DelayedDestroy()
    {
        BrickDestroyer.GetInstance().DelayedDestroy(gameObject);
    }

    public bool IsOnCarpet()
    {
        return Math.Abs((transform.position.y + femaleConnectorParent.transform.localPosition.y) - (-0.065864f)) < 0.001f;
    }

    private void OnValidate()
    {
        if (!Application.isEditor) return;
        if (Application.isPlaying) return;
        
        maleConnectors = new List<GameObject>();
        maleConnectorScripts = new List<LegoConnectorScript>();
        foreach (Transform child in maleConnectorParent.transform)
        {
            maleConnectors.Add(child.gameObject);
            maleConnectorScripts.Add(child.gameObject.GetComponent<LegoConnectorScript>());
        }

        femaleConnectors = new List<GameObject>();
        femaleConnectorScripts = new List<LegoConnectorScript>();
        foreach (Transform child in femaleConnectorParent.transform)
        {
            femaleConnectors.Add(child.gameObject);
            femaleConnectorScripts.Add(child.gameObject.GetComponent<LegoConnectorScript>());
        }
    }

    private readonly Collider[] _colliderBuffer = new Collider[20];
    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private readonly Dictionary<BrickAttach, bool> _attachBuffer = new Dictionary<BrickAttach, bool>();

    //Remove or Commented-out the collection if it is no longer needed
    // FIXED CODE:
{
    public void NotifyNearbyBricksToRecalculateMesh()
    {
        BrickMeshRecalculator meshRecalculator = BrickMeshRecalculator.GetInstance();
        List<BrickAttach> objectsToRecalculate = new List<BrickAttach>();
        foreach (GameObject connectorObject in femaleConnectors.Concat(maleConnectors))
        {
            int hits = Physics.OverlapSphereNonAlloc(connectorObject.transform.position, 0.02f, _colliderBuffer);
            for (int i = 0; i < hits; i++)
            {
                BrickAttach attach = _colliderBuffer[i].GetComponentInParent<BrickAttach>();
                if (((object)attach) == null)
                    continue;

                if (!objectsToRecalculate.Contains(attach))
                {
                    objectsToRecalculate.Add(attach);
                    meshRecalculator.AddAttach(attach);
                }
            }
        }
    }


}
