/************************************************************************************
Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When attached to a GameObject with an OVROverlay component, OVROverlayMeshGenerator will use a mesh renderer
/// to preview the appearance of the OVROverlay as it would appear as a TimeWarp overlay on a headset.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class OVROverlayMeshGenerator : MonoBehaviour {

	private Mesh _Mesh;
	private List<Vector3> _Verts = new List<Vector3>();
	private List<Vector2> _UV = new List<Vector2>();
	private List<int> _Tris = new List<int>();
	private OVROverlay _Overlay;
	private MeshFilter _MeshFilter;
	private MeshCollider _MeshCollider;
	private MeshRenderer _MeshRenderer;
	private Transform _CameraRoot;
	private Transform _Transform;

	private OVROverlay.OverlayShape _LastShape;
	private Vector3 _LastPosition;
	private Quaternion _LastRotation;
	private Vector3 _LastScale;
	private Rect _LastDestRectLeft;
	private Rect _LastDestRectRight;
	private Rect _LastSrcRectLeft;
	private Texture _LastTexture;

	private bool _Awake = false;

	protected void Awake()
	{
		_MeshFilter = GetComponent<MeshFilter>();
		_MeshCollider = GetComponent<MeshCollider>();
		_MeshRenderer = GetComponent<MeshRenderer>();

		_Transform = transform;
		if (Camera.main && Camera.main.transform.parent)
		{
			_CameraRoot = Camera.main.transform.parent;
		}

		_Awake = true;
	}

	public void SetOverlay(OVROverlay overlay) {
		_Overlay = overlay;
	}

	private Rect GetBoundingRect(Rect a, Rect b)
	{
		float xMin = Mathf.Min(a.x, b.x);
		float xMax = Mathf.Max(a.x + a.width, b.x + b.width);
		float yMin = Mathf.Min(a.y, b.y);
		float yMax = Mathf.Max(a.y + a.height, b.y + b.height);

		return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
	}

	protected void OnEnable() {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.update += Update;
		#endif
	}

	protected void OnDisable() {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.update -= Update;
		#endif
	}

	private void Update()
	{
				// BUG: Using New() allocation in Update() method.
				// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
				// 		if (!Application.isEditor)
				// 		{
				// 			return;
				// 		}
				// 
				// 		if (!_Awake)
				// 		{
				// 			Awake();
				// 		}
				// 
				// 		if (_Overlay)
				// 		{
				// 			OVROverlay.OverlayShape shape = _Overlay.currentOverlayShape;
				// 			Vector3 position = _CameraRoot ? (_Transform.position - _CameraRoot.position) : _Transform.position;
				// 			Quaternion rotation = _Transform.rotation;
				// 			Vector3 scale = _Transform.lossyScale;
				// 			Rect destRectLeft = _Overlay.overrideTextureRectMatrix ? _Overlay.destRectLeft : new Rect(0, 0, 1, 1);
				// 			Rect destRectRight = _Overlay.overrideTextureRectMatrix ? _Overlay.destRectRight : new Rect(0, 0, 1, 1);
				// 			Rect srcRectLeft = _Overlay.overrideTextureRectMatrix ? _Overlay.srcRectLeft : new Rect(0, 0, 1, 1);
				// 			Texture texture = _Overlay.textures[0];
				// 
				// 			// Re-generate the mesh if necessary
				// 			if (_Mesh == null ||
				// 			    _LastShape != shape ||
				// 			    _LastPosition != position ||
				// 			    _LastRotation != rotation ||
				// 			    _LastScale != scale ||
				// 			    _LastDestRectLeft != destRectLeft ||
				// 			    _LastDestRectRight != destRectRight)
				// 			{
				// 				UpdateMesh(shape, position, rotation, scale, GetBoundingRect(destRectLeft, destRectRight));
				// 				_LastShape = shape;
				// 				_LastPosition = position;
				// 				_LastRotation = rotation;
				// 				_LastScale = scale;
				// 				_LastDestRectLeft = destRectLeft;
				// 				_LastDestRectRight = destRectRight;
				// 			}
				// 
				// 			// Generate the material and update textures if necessary
				// 			if (_MeshRenderer.sharedMaterial == null)
				// 			{
				// 				Material previewMat = new Material(Shader.Find("Unlit/Transparent"));
				// 				_MeshRenderer.sharedMaterial = previewMat;
				// 			}
				// 
				// 			if (_MeshRenderer.sharedMaterial.mainTexture != texture && !_Overlay.isExternalSurface)
				// 			{
				// 				_MeshRenderer.sharedMaterial.mainTexture = texture;
				// 			}
				// 
				// 			if (_LastSrcRectLeft != srcRectLeft)
				// 			{
				// 				_MeshRenderer.sharedMaterial.mainTextureOffset = srcRectLeft.position;
				// 				_MeshRenderer.sharedMaterial.mainTextureScale = srcRectLeft.size;
				// 				_LastSrcRectLeft = srcRectLeft;
				// 			}
				// 		}
				// 	}

				// FIXED VERSION:
