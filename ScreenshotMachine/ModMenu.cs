using Satchel.BetterMenus;
namespace ScreenshotMachine
{
    public class ModMenu
    {
        private static Menu MenuRef;
        public static MenuScreen GetMenu(MenuScreen lastmenu)
        {
            if (MenuRef == null)
            {
                MenuRef = Prepare();
            }
            return MenuRef.GetMenuScreen(lastmenu);
        }
        public static Menu Prepare()
        {
            return new Menu("ScreenShotMachine", new Element[]
            {
                new KeyBind("Camera Left",ScreenshotMachine.Settings.keyBinds.CameraLeftKey,Id:"CameraLeft"),
                new KeyBind("Camera Right",ScreenshotMachine.Settings.keyBinds.CameraRightKey,Id:"CameraRight"),
                new KeyBind("Camera Up",ScreenshotMachine.Settings.keyBinds.CameraUpKey,Id:"CameraUp"),
                new KeyBind("Camera Down",ScreenshotMachine.Settings.keyBinds.CameraDownKey,Id:"CameraDown"),
                new KeyBind("Camera In",ScreenshotMachine.Settings.keyBinds.CameraInKey,Id:"CameraIn"),
                new KeyBind("Camera Out",ScreenshotMachine.Settings.keyBinds.CameraOutKey,Id:"CameraOut"),
                new KeyBind("Toggle Mode",ScreenshotMachine.Settings.keyBinds.ToggleScreenshotModeKey,Id:"ToggleMode"),
                new KeyBind("Toggle BlurPlane",ScreenshotMachine.Settings.keyBinds.BlurPlaneToggleKey,Id:"Toggle BlurPlane"),
                new KeyBind("Toggle CenterLine",ScreenshotMachine.Settings.keyBinds.ToggleCenterLineKey,Id:"Toggle CenterLine"),
                new KeyBind("Reset Camera",ScreenshotMachine.Settings.keyBinds.RestoreCameraKey,Id:"Reset Camera"),
                new KeyBind("Capture Screen",ScreenshotMachine.Settings.keyBinds.CaptureButton,Id:"Capture Screen"),
            });
        }
    }
}
