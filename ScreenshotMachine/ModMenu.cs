using Satchel.BetterMenus;
namespace ScreenshotMachine
{
    public class ModMenu
    {
        private static Menu MenuRef;
        public static MenuScreen GetMenu(MenuScreen lastmenu)
        {
            if (MenuRef == null)
                MenuRef = Prepare();
            return MenuRef.GetMenuScreen(lastmenu);
        }
        public static Menu Prepare()
        {
            return new Menu("ScreenShotMachine", new Element[]
            {
                new KeyBind("Camera Left",ScreenshotMachine.Settings.KeyBinds.CameraLeftKey,Id:"CameraLeft"),
                new KeyBind("Camera Right",ScreenshotMachine.Settings.KeyBinds.CameraRightKey,Id:"CameraRight"),
                new KeyBind("Camera Up",ScreenshotMachine.Settings.KeyBinds.CameraUpKey,Id:"CameraUp"),
                new KeyBind("Camera Down",ScreenshotMachine.Settings.KeyBinds.CameraDownKey,Id:"CameraDown"),
                new KeyBind("Camera In",ScreenshotMachine.Settings.KeyBinds.CameraInKey,Id:"CameraIn"),
                new KeyBind("Camera Out",ScreenshotMachine.Settings.KeyBinds.CameraOutKey,Id:"CameraOut"),
                new KeyBind("Toggle Mode",ScreenshotMachine.Settings.KeyBinds.ToggleScreenshotModeKey,Id:"ToggleMode"),
                new KeyBind("Toggle BlurPlane",ScreenshotMachine.Settings.KeyBinds.BlurPlaneToggleKey,Id:"Toggle BlurPlane"),
                new KeyBind("Toggle CenterLine",ScreenshotMachine.Settings.KeyBinds.ToggleCenterLineKey,Id:"Toggle CenterLine"),
                new KeyBind("Reset Camera",ScreenshotMachine.Settings.KeyBinds.RestoreCameraKey,Id:"Reset Camera"),
                new KeyBind("Capture Screen",ScreenshotMachine.Settings.KeyBinds.CaptureButton,Id:"Capture Screen"),
                new KeyBind("Camera Speed Up",ScreenshotMachine.Settings.KeyBinds.CameraSpeedUp,Id:"Camera Speed Up"),
            });
        }
    }
}
