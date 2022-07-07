using Satchel.BetterMenus;

namespace ScreenshotMachine
{
    public static class ModMenu
    {
        private static Menu MenuRef;

        public static MenuScreen GetMenu(MenuScreen lastmenu)
        {
            MenuRef ??= Prepare();
            
            return MenuRef.GetMenuScreen(lastmenu);
        }

        private static Menu Prepare()
        {
            var binds = ScreenshotMachine.Settings.KeyBinds;

            return new Menu
            (
                "ScreenShotMachine",
                
                new Element[]
                {
                    new KeyBind("Camera Left", binds.CameraLeftKey, Id: "CameraLeft"),
                    new KeyBind("Camera Right", binds.CameraRightKey, Id: "CameraRight"),
                    new KeyBind("Camera Up", binds.CameraUpKey, Id: "CameraUp"),
                    new KeyBind("Camera Down", binds.CameraDownKey, Id: "CameraDown"),
                    new KeyBind("Camera In", binds.CameraInKey, Id: "CameraIn"),
                    new KeyBind("Camera Out", binds.CameraOutKey, Id: "CameraOut"),
                    new KeyBind("Toggle Mode", binds.ToggleScreenshotModeKey, Id: "ToggleMode"),
                    new KeyBind("Toggle BlurPlane", binds.BlurPlaneToggleKey, Id: "Toggle BlurPlane"),
                    new KeyBind("Toggle CenterLine", binds.ToggleCenterLineKey, Id: "Toggle CenterLine"),
                    new KeyBind("Reset Camera", binds.RestoreCameraKey, Id: "Reset Camera"),
                    new KeyBind("Capture Screen", binds.CaptureButton, Id: "Capture Screen"),
                    new KeyBind("Camera Speed Up", binds.CameraSpeedUp, Id: "Camera Speed Up"),
                }
            );
        }
    }
}