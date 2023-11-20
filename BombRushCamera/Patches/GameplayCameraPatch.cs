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
            var moveSpeed = Time.deltaTime * Plugin.BaseSpeed.Value;
            if (Input.GetKey(KeyCode.LeftShift)) moveSpeed *= Plugin.ShiftMultiplier.Value;
            if (Input.GetKey(KeyCode.LeftControl)) moveSpeed *= Plugin.CtrlMultiplier.Value;

            if (Input.GetKey(KeyCode.W)) tf.position += tf.forward * moveSpeed;
            if (Input.GetKey(KeyCode.S)) tf.position -= tf.forward * moveSpeed;
            if (Input.GetKey(KeyCode.A)) tf.position -= tf.right * moveSpeed;
            if (Input.GetKey(KeyCode.D)) tf.position += tf.right * moveSpeed;

            var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            var sensitivity = Plugin.MouseSensitivity.Value;

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

            return false;
        }

        return true;
    }
}
