using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BaroqueUI;
using System;

namespace NanIndustryVR
{
    public interface IControllerDialog
    {
        float FindDistance(Camera cam, bool force = false);
        void Enter(ControllersDialog.Track track);
        void Leave(ControllersDialog.Track track);
        void Hover(ControllersDialog.Track track, Controller controller, Camera cam);
        void TriggerDown(ControllersDialog.Track track, Camera cam);
        void TriggerUp(ControllersDialog.Track track, Camera cam);
    }

    public class ControllersDialog : MonoBehaviour
    {
        public Material lineMaterial, lineMissMaterial;
        public float defaultDistance;

        Mesh lineMesh;
        new Camera camera;

        private void Start()
        {
            lineMesh = new Mesh();
            lineMesh.vertices = new Vector3[] { Vector3.zero, Vector3.forward };
            lineMesh.SetIndices(new int[] { 0, 1 }, MeshTopology.Lines, 0);
            lineMesh.RecalculateBounds();
            lineMesh.UploadMeshData(true);

            camera = GetComponent<Camera>();

            var gt = Controller.GlobalTracker(this);
            gt.onControllersUpdate += Gt_onControllersUpdate;
        }


        internal class TouchPadScroll
        {
            enum State { WaitTouch, WaitMoveFarEnough, Moving, FreeWheeling };
            State state = State.WaitTouch;
            Vector2 original_touch;
            Vector2 speed_estimate;
            float previous_time;

            internal Vector2 Handle(Controller ctrl)
            {
                if (ctrl.haveRealTouchpad)
                    return HandleRealTouchpad(ctrl);
                if (ctrl.haveJoystick)
                    return HandleJoystick(ctrl);
                return Vector2.zero;
            }

            Vector2 HandleRealTouchpad(Controller ctrl)
            {
                const float SCROLL_SPEED = 2.5f;
                bool touched = ctrl.touchpadTouched && !ctrl.touchpadPressed;

                if (state == State.WaitTouch || (state == State.FreeWheeling && touched))
                {
                    if (touched)
                    {
                        original_touch = ctrl.touchpadPosition;
                        state = State.WaitMoveFarEnough;
                    }
                    return Vector2.zero;
                }
                if (state == State.WaitMoveFarEnough)
                {
                    if (!touched)
                    {
                        state = State.WaitTouch;
                        return Vector2.zero;
                    }
                    if ((ctrl.touchpadPosition - original_touch).sqrMagnitude < 0.01f)
                        return Vector2.zero;   
                    state = State.Moving;
                    previous_time = float.NegativeInfinity;
                    speed_estimate = Vector2.zero;
                }
                
                

                
                float delta_time = Time.time - previous_time;
                previous_time = Time.time;
                if (state == State.Moving)
                {
                    if (touched)
                    {
                        Vector2 delta = (ctrl.touchpadPosition - original_touch) * SCROLL_SPEED;
                        original_touch = ctrl.touchpadPosition;
                        Vector2 instant_speed = delta / delta_time;
                        speed_estimate = Vector2.Lerp(
                            instant_speed, speed_estimate, Mathf.Exp(delta_time * -13f));
                        return delta;
                    }
                    if (speed_estimate.sqrMagnitude < 2f)
                    {
                        state = State.WaitTouch;
                        return Vector2.zero;
                    }
                    state = State.FreeWheeling;

                }
                
                speed_estimate *= Mathf.Exp(delta_time * -3f);
                if (ctrl.touchpadPressed || speed_estimate.sqrMagnitude < 0.4f)
                {
                    state = State.WaitTouch;
                    return Vector2.zero;
                }
                return speed_estimate * delta_time;
            }

            Vector2 HandleJoystick(Controller ctrl)
            {
                const float SCROLL_SPEED = 4.5f;

                if (ctrl.touchpadPosition.sqrMagnitude > 0.01f)
                    return ctrl.touchpadPosition * Time.deltaTime * SCROLL_SPEED;
                else
                    return Vector2.zero;    
            }
        }

        public class Track
        {
            internal IControllerDialog hover;
            internal PointerEventData pevent;
            internal Vector3? grab_position;
            internal bool grab_move;
            internal GameObject current_pressed;
            internal bool trigger_down;
            internal TouchPadScroll touch_pad_scroll;
            internal Track[] all_tracks;
        }
        Track[] tracks;

        private void Gt_onControllersUpdate(Controller[] controllers)
        {
            foreach (var ctrl in controllers)
            {
                transform.SetPositionAndRotation(ctrl.position, ctrl.rotation);

                var track = ctrl.GetAdditionalData(ref tracks);
                track.all_tracks = tracks;
                float closest_distance = float.PositiveInfinity;
                IControllerDialog closest = null;

                if (track.trigger_down)
                {
                    closest_distance = track.hover.FindDistance(camera, force: true);
                    if (closest_distance > 100)
                        closest_distance = 100;
                    closest = track.hover;
                }
                else
                {
                    foreach (var vr_dialog in FindObjectsOfType<VRDialog>())
                    {
                        float distance = vr_dialog.FindDistance(camera);
                        if (distance < closest_distance)
                        {
                            closest_distance = distance;
                            closest = vr_dialog;
                        }
                    }

                    float dist1 = LevelPlatform.FindDistanceTarget(camera, out var target);
                    if (dist1 < closest_distance)
                    {
                        closest_distance = dist1;
                        closest = target;
                    }
                }

                if (track.hover != closest)
                {
                    if (track.trigger_down)
                    {
                        track.trigger_down = false;
                        track.hover.TriggerUp(track, camera);
                    }
                    if (track.hover != null)
                        track.hover.Leave(track);
                    track.hover = closest;
                    if (closest != null)
                    {
                        closest.Enter(track);
                        ctrl.HapticPulse();
                    }
                }

                Material mat;
                if (closest == null)
                {
                    closest_distance = defaultDistance;
                    mat = lineMissMaterial;
                }
                else
                {
                    // BUG: Potentially dangerous use of non-short-circuit logic
                    // MESSAGE: The & and | operators do not use short-circuit evaluation and can be dangerous when applied to boolean operands. In particular, their use can result in errors if the left-hand operand checks for cases in which it is not safe to evaluate the right-hand one.
                    //                     bool trigger_pressed = ctrl.triggerPressed | ctrl.touchpadPressed;

                    // FIXED CODE:
                    var trigger_pressed = ctrl.triggerPressed || ctrl.touchpadPressed;

                    if (track.hover.TriggerDown(track, camera))
                    {
                        track.trigger_down = true;
                        ctrl.HapticFeedback(0.1f, 0.05f);
                    }
                }

                mat = closest == null ? lineMissMaterial : closest.LineMaterial(track);
                if (mat != null)
                    lineMaterial = mat;

                Vector3 end = camera.transform.position + camera.transform.forward * closest_distance;
                lineMesh.vertices = new Vector3[] { camera.transform.position, end };
                lineMesh.RecalculateBounds();
                lineMesh.UploadMeshData(true);
            }

            var tp = new TouchPadScroll();
            foreach (var track in tracks)
            {
                if (track.hover != null)
                {
                    Vector3 pos;
                    if (track.grab_position == null)
                        pos = track.hover.SuggestedGrabPosition(track, camera, out track.grab_move);
                    else
                        pos = track.grab_position.Value;
                    var pevent = track.pevent.Get(track.hover);
                    if (pevent == null)
                        pevent = track.hover.SuggestedPointerEvent(track, camera, out track.pevent);
                    track.hover.Hover(track, camera, pos, track.grab_move, pevent);
                    track.speed_estimate = tp.Handle(ctrl);
                }
            }
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NanIndustryVR
{
    public class VRDialog : MonoBehaviour
    {

        public float FindDistance(Camera cam)
        {
            return FindDistance(cam, false);
        }

        public float FindDistance(Camera cam, bool force)
        {
            return Vector3.Distance(cam.transform.position, transform.position);
        }

        public Material[] LineMaterials(ControllersDialog.Track track)
        {
            return new[] { track.current_pressed != null ? track.current_pressed.GetComponent<Renderer>().material : lineMaterial };
        }

        public Material SuggestedLineMaterial(ControllersDialog.Track track)
        {
            return track.current_pressed != null ? track.current_pressed.GetComponent<Renderer>().material : lineMaterial;
        }

        public virtual Vector3 SuggestedGrabPosition(ControllersDialog.Track track, Camera cam, out bool grab_move)
        {
            return transform.position;
        }

        // BUG: Redundant 'grab_position' parameter
        // MESSAGE: Parameter 'grab_position' is never used.
        //                     Vector3 SuggestedGrabPosition(ControllersDialog.Track track, Camera cam, out Vector3 grab_position)

        // FIXED CODE:
        { return transform.position; }

        public UnityEngine.EventSystems.BaseEventData SuggestedPointerEvent(ControllersDialog.Track track, Camera cam, out UnityEngine.EventSystems.BaseEventData pevent)
        {
            pevent = track.pevent;
            return pevent;
        }

        public void Enter(ControllersDialog.Track track)
        {
            track.hover = this;
        }

        public void Leave(ControllersDialog.Track track)
        {
            track.hover = null;
        }

        public virtual void Hover(ControllersDialog.Track track, Camera cam, Vector3 pos, bool mouse_move, UnityEngine.EventSystems.BaseEventData pevent)
        {

        }

        public virtual void TriggerDown(ControllersDialog.Track track, Camera cam)
        {

        }

        public virtual void TriggerUp(ControllersDialog.Track track, Camera cam)
        {

        }
    }
}`
                    if (trigger_pressed != track.trigger_down)
                    {
                        track.trigger_down = trigger_pressed;
                        if (trigger_pressed)
                            closest.TriggerDown(track, camera);
                        else
                            closest.TriggerUp(track, camera);
                    }
                    closest.Hover(track, ctrl, camera);
                    mat = lineMaterial;
                }

                if (closest == null || closest is LevelPlatform)
                {
                    if (track.trigger_down)
                        track.touch_pad_scroll = null;
                    else
                    {
                        if (track.touch_pad_scroll == null)
                            track.touch_pad_scroll = new TouchPadScroll();
                        LevelPlatform.HandleScroll(track.touch_pad_scroll.Handle(ctrl));
                    }
                }

                var matrix = Matrix4x4.TRS(ctrl.position, ctrl.rotation, closest_distance * Vector3.one);
                Graphics.DrawMesh(lineMesh, matrix, mat, 0);
            }
        }
    }
}
