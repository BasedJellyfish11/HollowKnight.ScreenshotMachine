using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Modding;
using UnityEngine;
using Vasi;
using Logger = Modding.Logger;
using UObject = UnityEngine.Object;

namespace ScreenshotMachine
{
    [UsedImplicitly]
    public class ScreenshotMachine : Mod, IGlobalSettings<GlobalSettings>, ITogglableMod, ICustomMenuMod
    {
        private GameObject _handler;
        
        public static GlobalSettings Settings = new ();

        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += CameraHandler.HotkeyHandler;
            On.GameManager.SceneLoadInfo.NotifyFetchComplete += CameraHandler.Reset;
            On.CameraController.LateUpdate += CameraHandler.RemoveCameraLogic;
            
            On.GameManager.SceneLoadInfo.NotifyFetchComplete += ParticleStopper.StopFreeze;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += ParticleStopper.CallFreezeParticles;

            _handler = new GameObject("ScreenshotMachineHandler", typeof(NonBouncer), typeof(Screenshotter));
            UObject.DontDestroyOnLoad(_handler);
            
            ParticleStopper.CoroutineStarter = _handler.GetComponent<NonBouncer>();
            LoadLines();
        }
        
        public void Unload()
        {
            ModHooks.HeroUpdateHook -= CameraHandler.HotkeyHandler;
            On.GameManager.SceneLoadInfo.NotifyFetchComplete -= CameraHandler.Reset;
            On.CameraController.LateUpdate -= CameraHandler.RemoveCameraLogic;
            
            On.GameManager.SceneLoadInfo.NotifyFetchComplete -= ParticleStopper.StopFreeze;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= ParticleStopper.CallFreezeParticles;

            UObject.Destroy(_handler);
        }

        public new static void Log(object message) => Logger.Log("[ScreenshotMachine] - " + message );

        public override string GetVersion() => VersionUtil.GetVersion<ScreenshotMachine>();

        public bool ToggleButtonInsideMenu => false;

        public MenuScreen GetMenuScreen(MenuScreen s, ModToggleDelegates? modToggle) => ModMenu.GetMenu(s);

        private static void LoadLines()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            using Stream stream = asm.GetManifestResourceStream("ScreenshotMachine.Images.Lines.png");
            
            if (stream == null)
                return;
            
            byte[] buffer = new byte[stream.Length];

            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidDataException();

            // Create texture from bytes
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(buffer, true);

            // Create sprite from texture
            CameraHandler.LineSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        public void OnLoadGlobal(GlobalSettings s) => Settings = s;

        public GlobalSettings OnSaveGlobal() => Settings;
    }
}