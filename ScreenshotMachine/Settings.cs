using System;
using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;
using Modding.Converters;
using InControl;
namespace ScreenshotMachine
{
    [Serializable]
    public class GlobalSettings
    {
        [JsonConverter(typeof(PlayerActionSetConverter))]
        public KeyBinds keyBinds = new KeyBinds();
            [FormerlySerializedAs("displayTime")]
            public float DisplayTime = 30f;
            [FormerlySerializedAs("stoppedTime")]
            public float StoppedTime = 10f;

            [FormerlySerializedAs("alwaysUseSameSeedForParticles")]
            public bool AlwaysUseSameSeedForParticles;

            public int CaptureFactor = 2;

            public int RenderW = 7680;
            public int RenderH = 4320;
    }
    public class KeyBinds : PlayerActionSet
    {
        public PlayerAction CameraLeftKey;
        public PlayerAction CameraRightKey;
        public PlayerAction CameraUpKey;
        public PlayerAction CameraDownKey;
        public PlayerAction CameraInKey;
        public PlayerAction CameraOutKey;
        public PlayerAction ToggleScreenshotModeKey;
        public PlayerAction BlurPlaneToggleKey;
        public PlayerAction ToggleCenterLineKey;
        public PlayerAction RestoreCameraKey;
        public PlayerAction CaptureButton;

        public KeyBinds()
        {
            CameraLeftKey = CreatePlayerAction("Camera Left");
            CameraRightKey = CreatePlayerAction("Camera Right");
            CameraUpKey = CreatePlayerAction("Camera Up");
            CameraDownKey = CreatePlayerAction("Camera Down");
            CameraInKey = CreatePlayerAction("Camera In");
            CameraOutKey = CreatePlayerAction("Camera Out");
            ToggleScreenshotModeKey = CreatePlayerAction("Toggle Screenshot Mode");
            BlurPlaneToggleKey = CreatePlayerAction("BlurPlane Toggle");
            ToggleCenterLineKey = CreatePlayerAction("Toggle CenterLine");
            RestoreCameraKey = CreatePlayerAction("Restore Camera");
            CaptureButton = CreatePlayerAction("Capture Button");
        }
    }
}