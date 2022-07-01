using System;
using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ScreenshotMachine
{
    [Serializable]
    public class GlobalSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyMoveCameraLeft = KeyCode.LeftArrow;
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyMoveCameraRight = KeyCode.RightArrow;
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyMoveCameraUp = KeyCode.UpArrow;
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyMoveCameraDown = KeyCode.DownArrow;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyMoveCameraIn = KeyCode.Keypad6;
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyMoveCameraOut = KeyCode.Keypad4;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyToggleScreenshotMode = KeyCode.Pause;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyToggleBlurPlane = KeyCode.R;
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyToggleCenterLine = KeyCode.E;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode keyRestoreCamera = KeyCode.Q;

        [JsonConverter(typeof(StringEnumConverter))]
        public KeyCode CaptureButton = KeyCode.F11;
        
        public float displayTime = 30f;
        public float stoppedTime = 10f;
        public bool alwaysUseSameSeedForParticles;

        public int CaptureFactor = 2;
        
        public int RenderW = 7680;
        public int RenderH = 4320;
    }
}