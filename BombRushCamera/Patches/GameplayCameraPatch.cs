using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BombRushCamera.Patches;

[HarmonyPatch(typeof(GameplayCamera))]
public class GameplayCameraPatch {
    [HarmonyPrefix]
    [HarmonyPatch("UpdateCamera")]
    public static bool UpdateCamera(GameplayCamera __instance) {
        if (Plugin.Active) {
            var tf = __instance.transform;
            var playerTf = __instance.player.tf;

            var anchorPressed = Input.GetKeyDown(Plugin.PluginConfig.Input.ToggleAnchorKey.Value);
            if (Plugin.State is null || anchorPressed) {
                Plugin.State ??= new FreecamState {
                    Transform = tf.position,
                    AnchoredTransform = false
                };

                if (anchorPressed) {
                    Plugin.State.AnchoredTransform = !Plugin.State.AnchoredTransform;

                    if (Plugin.State.AnchoredTransform) {
                        Plugin.State.Transform = tf.position - playerTf.position;
                    } else {
                        Plugin.State.Transform = tf.position;
                    }
                }
            }

            var pos = Plugin.State.Transform;

            // Only control the camera when the game's inputs are disabled
            if (Plugin.InputsDisabled) {
                {
                    var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
                    var sensitivity = Plugin.PluginConfig.Freecam.MouseSensitivity.Value;

                    var newRot = tf.rotation;
                    newRot *= Quaternion.Euler(mouseDelta.y * sensitivity, mouseDelta.x * sensitivity, 0f);

                    var angles = newRot.eulerAngles;
                    angles.z = 0;

                    // Horizon is 360, up is 270, down is 90... what?
                    var wtf = angles.x;
                    wtf = wtf > 180 ? wtf - 360 : wtf;
                    wtf = Mathf.Clamp(wtf, -89, 89);
                    angles.x = (wtf + 360) % 360;

                    newRot.eulerAngles = angles;
                    tf.rotation = newRot;
                }

                {
                    var moveSpeed = Time.deltaTime * Plugin.PluginConfig.Freecam.BaseSpeed.Value;
                    if (Input.GetKey(KeyCode.LeftShift)) moveSpeed *= Plugin.PluginConfig.Freecam.ShiftMultiplier.Value;
                    if (Input.GetKey(KeyCode.LeftControl))
                        moveSpeed *= Plugin.PluginConfig.Freecam.CtrlMultiplier.Value;

                    if (Input.GetKey(KeyCode.W)) pos += tf.forward * moveSpeed;
                    if (Input.GetKey(KeyCode.S)) pos -= tf.forward * moveSpeed;
                    if (Input.GetKey(KeyCode.A)) pos -= tf.right * moveSpeed;
                    if (Input.GetKey(KeyCode.D)) pos += tf.right * moveSpeed;
                }
            }

            var additivePos = Plugin.State.AnchoredTransform
                                  ? __instance.player.tf.position
                                  : Vector3.zero;
            tf.position = pos + additivePos;
            Plugin.State.Transform = pos;

            return false;
        }

        return true;
    }
}
