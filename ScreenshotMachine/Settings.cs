using System;
using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScreenshotMachine
{
    [Serializable]
    public class GlobalSettings
    {
        [FormerlySerializedAs("keyMoveCameraLeft")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CameraLeftKey = KeyCode.LeftArrow;
        
        [FormerlySerializedAs("keyMoveCameraRight")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CameraRightKey = KeyCode.RightArrow;
        
        [FormerlySerializedAs("keyMoveCameraUp")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CameraUpKey = KeyCode.UpArrow;
        
        [FormerlySerializedAs("keyMoveCameraDown")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CameraDownKey = KeyCode.DownArrow;
        
        [FormerlySerializedAs("keyMoveCameraIn")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CameraInKey = KeyCode.Keypad6;
        [FormerlySerializedAs("keyMoveCameraOut")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CameraOutKey = KeyCode.Keypad4;
        
        [FormerlySerializedAs("keyToggleScreenshotMode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode ToggleScreenshotModeKey = KeyCode.Pause;
        
        [FormerlySerializedAs("keyToggleBlurPlane")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode BlurPlaneToggleKey = KeyCode.R;
        [FormerlySerializedAs("keyToggleCenterLine")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode ToggleCenterLineKey = KeyCode.E;
        
        [FormerlySerializedAs("keyRestoreCamera")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode RestoreCameraKey = KeyCode.Q;

        [FormerlySerializedAs("CaptureButton")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CaptureScreenshotKey = KeyCode.F11;
        
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
}