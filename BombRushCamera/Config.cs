using BepInEx.Configuration;
using UnityEngine;

namespace BombRushCamera;

public class Config(ConfigFile config) {
    public ConfigFreecam Freecam = new(config);
    public ConfigInput Input = new(config);

    public class ConfigFreecam(ConfigFile config) {
        public ConfigEntry<float> BaseSpeed = config.Bind(
            "Freecam",
            "BaseSpeed",
            5f,
            "The speed you move at in freecam by default."
        );

        public ConfigEntry<float> ShiftMultiplier = config.Bind(
            "Freecam",
            "ShiftMultiplier",
            10f,
            "The speed multiplier when holding shift."
        );

        public ConfigEntry<float> CtrlMultiplier = config.Bind(
            "Freecam",
            "CtrlMultiplier",
            0.1f,
            "The speed multiplier when holding ctrl."
        );

        public ConfigEntry<float> MouseSensitivity = config.Bind(
            "Freecam",
            "MouseSensitivity",
            5f,
            "The mouse sensitivity when in freecam."
        );
    }

    public class ConfigInput(ConfigFile config) {
        public ConfigEntry<KeyCode> Key = config.Bind(
            "Input",
            "Key",
            KeyCode.RightAlt,
            "Toggles freecam. Press to start, press to stop."
        );

        public ConfigEntry<KeyCode> ToggleAnchorKey = config.Bind(
            "Input",
            "ToggleAnchorKey",
            KeyCode.Period,
            "Toggles between free movement and anchoring the camera to the player."
        );

        public ConfigEntry<KeyCode> ToggleInputsKey = config.Bind(
            "Input",
            "ToggleInputsKey",
            KeyCode.Slash,
            "Re-enables inputs when pressed in freecam."
        );
    }
}
