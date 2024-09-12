using Oculus.Interaction.Editor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using props = Oculus.Interaction.UnityCanvas.CanvasRenderTexture.Properties;

        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private static List<Graphic> _tmpGraphics = new List<Graphic>();

        // FIXED CODE:
