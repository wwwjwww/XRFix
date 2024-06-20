
        protected override void OnBeforeInspector()
        {
            base.OnBeforeInspector();

            bool isEmpty;

            AutoFix(AutoFixIsUsingScreenSpaceCanvas(), AutoFixSetToWorldSpaceCamera, "The OverlayRenderer only supports Canvases that are set to World Space.");

            AutoFix(isEmpty = AutoFixIsMaskEmpty(), AutoFixAssignUIToMask, "The rendering Mask is empty, it needs to contain at least one layer for rendering to function.");

            if (!isEmpty)
            {
                AutoFix(AutoFixAnyCamerasRenderingTargetLayers(), AutoFixRemoveRenderingMaskFromCameras, "Some cameras are rendering using a layer that is specified here as a Rendering layer. This can cause the UI to be rendered twice.");
                AutoFix(AutoFixAnyRenderersOnUnrenderedLayers(), AutoFixMoveRenderersToMaskedLayers, "Some CanvasRenderers are using a layer that is not included in the rendered LayerMask.");
            }
        }

        #region AutoFix

        private bool AutoFix(bool needsFix, Action fixAction, string message)
        {
            if (needsFix)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.HelpBox(message, MessageType.Warning);
                    if (GUILayout.Button("Auto-Fix", GUILayout.ExpandHeight(true)))
                    {
                        fixAction();
                    }
                }
            }

            return needsFix;
        }

        private bool AutoFixIsUsingScreenSpaceCanvas()
        {
            Canvas canvas = target.GetComponent<Canvas>();
            if (canvas == null)
            {
                return false;
            }

            return canvas.renderMode != RenderMode.WorldSpace;
        }

        private void AutoFixSetToWorldSpaceCamera()
        {
            Canvas canvas = target.GetComponent<Canvas>();
            if (canvas != null)
            {
                Undo.RecordObject(canvas, "Set Canvas To World Space");
                canvas.renderMode = RenderMode.WorldSpace;
            }
        }

        private bool AutoFixIsMaskEmpty()
        {
            var layerProp = serializedObject.FindProperty(props.RenderLayers);
            return layerProp.intValue == 0;
        }

        public void AutoFixAssignUIToMask()
        {
            Undo.RecordObject(target, "Set Overlay Mask");
            var layerProp = serializedObject.FindProperty(props.RenderLayers);
            layerProp.intValue = CanvasRenderTexture.DEFAULT_UI_LAYERMASK;
            serializedObject.ApplyModifiedProperties();
        }

        private bool AutoFixAnyRenderersOnUnrenderedLayers()
        {
            var canvasProp = serializedObject.FindProperty(props.Canvas);
            Canvas canvas = canvasProp.objectReferenceValue as Canvas;

            if (canvas == null)
            {
                return false;
            }

            canvas.gameObject.GetComponentsInChildren(_tmpRenderers);
            foreach (var renderer in _tmpRenderers)
            {
                int layer = renderer.gameObject.layer;
                if (((1 << layer) & target.RenderingLayers) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void AutoFixMoveRenderersToMaskedLayers()
        {
            var canvasProp = serializedObject.FindProperty(props.Canvas);
            Canvas canvas = canvasProp.objectReferenceValue as Canvas;

            if (canvas == null)
            {
                return;
            }

            var maskedLayers = AutoFixGetMaskedLayers();
            var targetLayer = maskedLayers.FirstOrDefault();

            canvas.gameObject.GetComponentsInChildren(_tmpRenderers);
            foreach (var renderer in _tmpRenderers)
            {
                int layer = renderer.gameObject.layer;
                if ((layer & target.RenderingLayers) == 0)
                {
                    Undo.RecordObject(renderer.gameObject, "Set Overlay Layer");
                    renderer.gameObject.layer = targetLayer;
                }
            }
        }

        private bool AutoFixAnyCamerasRenderingTargetLayers()
        {
            var cameras = FindObjectsOfType<Camera>();
            foreach (var camera in cameras)
            {
                //Ignore the special camera we create to render to the overlay
                if (camera == target.OverlayCamera)
                {
                    continue;
                }

                if ((camera.cullingMask & target.RenderingLayers) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void AutoFixRemoveRenderingMaskFromCameras()
        {
            var cameras = FindObjectsOfType<Camera>();
            foreach (var camera in cameras)
            {
                Undo.RecordObject(camera, "Set Camera Culling Mask");
                camera.cullingMask = camera.cullingMask & ~target.RenderingLayers;
            }
        }

        private List<int> AutoFixGetMaskedLayers()
        {
            List<int> maskedLayers = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                if ((target.RenderingLayers & (1 << i)) != 0)
                {
                    maskedLayers.Add(i);
                }
            }
            return maskedLayers;
        }
        #endregion
    }
}
