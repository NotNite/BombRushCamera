using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BombRushCamera;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Bomb Rush Cyberfunk.exe")]
public class Plugin : BaseUnityPlugin {
    public static ManualLogSource Log = null!;
    public static Config PluginConfig = null!;
    public static Harmony Harmony = null!;

    public static bool Active = false;
    public static bool InputsDisabled = false;
    public static List<int> RewiredMaps = new();
    public static FreecamState? State = null;

    private void Awake() {
        Log = this.Logger;
        PluginConfig = new Config(this.Config);

        Harmony = new Harmony("BombRushCamera.Harmony");
        Harmony.PatchAll();
    }

    private void Update() {
        if (Input.GetKeyDown(PluginConfig.Input.Key.Value)) {
            Active = !Active;
            InputsDisabled = Active;

            this.ApplyInputs();
        }

        if (Active && Input.GetKeyDown(PluginConfig.Input.ToggleInputsKey.Value)) {
            InputsDisabled = !InputsDisabled;
            this.ApplyInputs();
        }
    }

    private void ApplyInputs() {
        var gameInput = Core.Instance.GameInput;
        var player = gameInput.rewiredMappingHandler.GetRewiredPlayer();
        var maps = player.controllers.maps.GetAllMaps()
            .Where(x => x.enabled)
            .Select(x => x.id);

        // Save and restore Rewired maps so we don't have input colliding
        if (InputsDisabled) {
            if (RewiredMaps.Count == 0) RewiredMaps = maps.ToList();
            gameInput.DisableAllControllerMaps();
        } else {
            foreach (var map in RewiredMaps) gameInput.EnableControllerMap(map);
        }
    }
}
