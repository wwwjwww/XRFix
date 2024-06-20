using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using System.Linq;

// Note that the PlacedBrickRendererManager does not render pegs automatically.
// Pegs must be passed in using AddBrick, and removed using RemoveBrick.
public class PlacedBrickRendererManager : MonoBehaviour
{
    private static PlacedBrickRendererManager _instance;
    public static PlacedBrickRendererManager GetInstance()
    {
    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //         if (_instance == null)
    //         {
    //             _instance = GameObject.FindGameObjectWithTag("PlacedBrickRendererManager")
    //                 ?.GetComponent<PlacedBrickRendererManager>();
    //         }
    // 
    //         return _instance;
    //     }
    // 
    //     public Material brickMaterial;
    // 
    //     private readonly Dictionary<Mesh, MeshRenderBucket> _meshToRenderBuckets = new Dictionary<Mesh, MeshRenderBucket>();
    //     private readonly Dictionary<string, LinkedListNode<MeshToRender>> _brickUuidToNodes = new Dictionary<string, LinkedListNode<MeshToRender>>();
    //     private readonly Dictionary<string, Mesh> _brickUuidToMesh = new Dictionary<string, Mesh>();

    // FIXED VERSION:
