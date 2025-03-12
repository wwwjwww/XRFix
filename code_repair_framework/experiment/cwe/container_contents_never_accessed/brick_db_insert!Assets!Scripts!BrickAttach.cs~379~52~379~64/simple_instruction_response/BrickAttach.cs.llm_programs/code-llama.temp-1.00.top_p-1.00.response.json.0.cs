
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
    
    
This is a  script used in Unity to represent a brick in the game "Lego Brick Breaker". The script is part of the Unity framework for building video games. This code appears over the course of the program. The code is written in  (Unity Script); therefore, it has numerous references to other elements of the code.

The first block of code that appears in the script is a 'brick color' and a public color32 for the brick.
public Color32 Color
{
get => _color;
set => SetColor(value);     
}

The Color32 value used in the Color property of this script is a three-component color value comprised of red ( r), green (g), blue (b) and alpha values (a).
public Color32 HoverColor
{
get {
int differences = (Convert.ToInt32(_color.r) + Convert.ToInt32(_color.g) + Convert.ToInt32(color.b) > 300 ? -20 : 20);
byte R = (byte)(Mathf.Clamp(color.r + differences, 0, 255));
byte G = (byte)(Mathf.Clamp(color.g + differences, 0, 255));
byte B = (byte)(Mathf.Clamp(color.b + differences, 0, 255));

byte a = (byte)(Mathf.Clamp(color.a, 0, 255));

return (new Color32(R, G, B, a));
}
}

In the code, the public Color32 value is assigned and then returned in a new color 32 variable. A function called ConnectorScripts is used, which requires parameters of a transform and a List of connector scripts.The connector is a game object that has a transform value associated with it, a connector script value associated with the game object, and an enabled value that is a Boolean value. It is an array containing all connector scripts of the targeted brick object, which is generated from the male and female connector parents of the targeted brick object. The connector scripts are enabled values that determine if the brick object can attach to any of the connected game objects.

The Color function is public, and it returns the Color property.
///   ----- Get and Set ----
/// <summary>
///  The brick's color.
/// </summary>

/// The Color32 value used in the _color field of this script.
/// <summary>
///  The material assigned to the model of this brick.
/// </summary>

/// The material assigned to the model of this brick.
/// <summary>
///  The property block assigned to the mesh renderer of this brick.
/// </summary>

/// The property block assigned to the mesh renderer of this brick.
/// <summary>
///  Get and set the material of the brick.
/// </summary>
  Get or set method for the brick's material.
 /// <param name="color32">
 ///  The new color to assign.
 /// </param>

/// <returns>
///  The new color32 being assigned.
/// </returns>

The brick's color is a public property that stores the color32 value of this script. The Color method is a public function that assigns (_Color) the Color32 value of this script. The Color method returns a Color32 value representing the brick's color.
/// public Color32 HoverColor {
///  get {

In the code, the 'get' statement is used to get the return value of this public value.This is done by creating an integer value for the sum of the color property r, g, and b, which are created by using Converter.ItoInt32().The 'difference' element is then set equal to (R + G + B) / 2.Then a Byte (byte) value is created by converting the differences integer value to a byte value by using Convert.ToByte().The 'R', 'G', and 'B' character variables are then set equal to the Color32's new values, which have a range between zero and 255. The difference between these numbers and zero is taken to generate a byte value. If the number is non-negative, a value between zero and 255 is generated by taking the difference and converting it to a Byte(byte) value.If the number is negative, a value between -255 and 0 is generated by adding 256 to its absolute value.

The _color field of this script is assigned a new Color32 value by assigning the Color32 value of the _color field value. Then, the R, G, and B values of the Color32 value of the _color field value are clamped between the values of 0 and 255 using Mathf.Clamp. The field values of the brick object that contain these values are clamped between the values of 0 and 255 using Mathf.Clamp. This is used to clamp the values of the brick between the values of 0 and 255. If the value is below zero, a value between -255 and 0 is generated by adding 256 to its absolute value.If the value is above 255, a value between 0 and 255 is generated by taking the value modulo 256.

The Hover Color field value of this script is also assigned a new color32 value equal to 'R', 'G', and 'B' of the Color32 value.The value of the 'R', 'G', and 'B' values is taken and divided by two to obtain the mean value. Afterward, the 'R', 'G', and 'B' values of the Color32 value are clamped to be between zero and 255 by calling Mathf.Clamp. Then, the field Color32 values of the brick object that contain these values are clamped between the values of 0 and 255 by calling Mathf.Clamp.The Brick's hover color property is a public value that stores the hover color32 value, which is used as the color of this brick when it is being hovered over. This is called the brick's hover color.

The Brick Uuid component is used to store the brick's UUID, which is a unique string identifier for this brick. The Set Color method is a public method that assigns the _color field of the brick a new Color32 value by calling SetColor. The _color field of the brick's Color32 value is assigned the color32 value of this BrickUuid component if no color is assigned to it. Otherwise, the _color field of the brick will be the color field value assigned.The _color field of this script will hold the _Color32 value assigned to this script.

The Held field is used by set and get. To get the Held value, get returns Held. Otherwise, the value of Held field value is set to false first.

public bool Held {
get {    return Held;

set {
    Held = value;
    UpdateBrickColor();
}
/// <summary>
///  The mesh filter of the brick model.
/// </summary>

/// The mesh filter of the brick model.

public MeshRenderer meshRenderer;

/// <summary>
///  The mesh renderer of the brick model.
/// </summary>

/// The mesh renderer of the brick model.

/// <summary>
///  Get and set the color of the brick.
/// </summary>

/// The Color32 value used in the material of this brick.

/// <summary>
///  The material assigned to the brick model.
/// </summary>

private MaterialPropertyBlock _props;

/// <summary>
/// Create a new material that clamps the color of the material property block used by the brick model.
/// </summary>

public void Start()
{
/// <summary>
/// Create a new material by calling ClampByColor with the color value of the _color field.
/// </summary>

OwnedPhysicsBricksStore.GetInstance().AddBrick(gameObject);
_avatarManager = AvatarManager.GetInstance();
 session = session ?? Session.GetInstance();
 headClientId = session.ClientID;

 /// <summary>
///  Configure the given brick's color property and set brickColor to the current Color32
/// </summary>

public void RecalculateRenderedGeometry()
{
/// <summary>
/// The cache key for the mesh property.
/// </summary>
string cacheKey = "";

cacheKey += normalPrefabName;

/// <summary>
/// If the brick mesh has been cached, use it. Otherwise cache the current mesh
/// using the cache key as the cache key.
/// </summary>
MeshFilter combinedMeshFilter = combinedModel.GetComponent<MeshFilter>();

Mesh cachedMesh =
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
