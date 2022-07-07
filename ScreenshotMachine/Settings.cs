using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using Modding.Converters;
using InControl;

namespace ScreenshotMachine
{
    [Serializable]
    public class GlobalSettings
    {
            [JsonConverter(typeof(PlayerActionSetConverter))]
            public KeyBinds KeyBinds = new();
            
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
        public PlayerAction CameraSpeedUp;
        
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
            CameraSpeedUp = CreatePlayerAction("Camera Speed Up");
            
            CameraLeftKey.AddDefaultBinding(Key.LeftArrow);
            CameraRightKey.AddDefaultBinding(Key.RightArrow);
            CameraUpKey.AddDefaultBinding(Key.UpArrow);
            CameraDownKey.AddDefaultBinding(Key.DownArrow);
            
            CameraInKey.AddDefaultBinding(Key.Key6);
            CameraOutKey.AddDefaultBinding(Key.Key7);
            
            ToggleScreenshotModeKey.AddDefaultBinding(Key.P);
            BlurPlaneToggleKey.AddDefaultBinding(Key.R);
            ToggleCenterLineKey.AddDefaultBinding(Key.E);
            
            RestoreCameraKey.AddDefaultBinding(Key.Y);
            
            CaptureButton.AddDefaultBinding(Key.F11);
            
            CameraSpeedUp.AddDefaultBinding(Key.Backspace);
        }
    }
}