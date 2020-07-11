using System.Diagnostics;
using System.IO;
using System.Reflection;
using Modding;
using UnityEngine;
using Logger = Modding.Logger;

namespace ScreenshotMachine
{
    public class ScreenshotMachine : Mod
    {
        public override ModSettings GlobalSettings
        {
            get => Settings;
            set => Settings = value as GlobalSettings;
        }

        public static GlobalSettings Settings = new GlobalSettings();

        public override void Initialize()
        {
            ModHooks.Instance.HeroUpdateHook += CameraHandler.HotkeyHandler;
            On.GameManager.SceneLoadInfo.NotifyFetchComplete += CameraHandler.Reset;
            On.CameraController.LateUpdate += CameraHandler.RemoveCameraLogic;
            
            On.GameManager.SceneLoadInfo.NotifyFetchComplete += ParticleStopper.StopFreeze;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += ParticleStopper.CallFreezeParticles;

            GameObject go = new GameObject();
            ParticleStopper.CoroutineStarter = go.AddComponent<NonBouncer>();
            Object.DontDestroyOnLoad(go);
            LoadLines();
        }

        public new static void Log(object message)
        {
            Logger.Log("[ScreenshotMachine] - " + message );
        }

        public override string GetVersion()
        {
            return "1.3";
        }

        private void LoadLines()
        {
            Assembly callingAssembly = new StackFrame(1, false).GetMethod()?.DeclaringType?.Assembly;
            
            using (Stream stream = callingAssembly?.GetManifestResourceStream("ScreenshotMachine.Images.Lines.png"))
            {
                if(stream == null)
                    return;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                // Create texture from bytes
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(buffer, true);

                // Create sprite from texture
                CameraHandler.LineSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        
    }
}