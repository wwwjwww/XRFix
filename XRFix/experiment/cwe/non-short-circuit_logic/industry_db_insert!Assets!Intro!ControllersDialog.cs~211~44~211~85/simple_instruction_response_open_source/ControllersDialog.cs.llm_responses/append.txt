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
