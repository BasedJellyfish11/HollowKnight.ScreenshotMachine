using System.Collections;
using System.Linq;
using GlobalEnums;
using Modding;
using UnityEngine;
using InControl;
namespace ScreenshotMachine
{
    public static class CameraHandler
    {
        private static bool _toggled;
        private static bool _frozenCamera;
        private static GameObject _blurPlane;

        private static bool _lineDrawn;
        private static bool _blurPlaneOn;

        public static Sprite LineSprite { get; set; }

        private static IEnumerator MoveCamera(Vector3 movement, PlayerAction action)
        {
            if (!_frozenCamera)
            {
                GameCameras.instance.cameraController.FreezeInPlace();
                _frozenCamera = true;
            }

            while (action.IsPressed)
            {
                GameCameras.instance.cameraController.transform.position += movement;
                yield return new WaitForSeconds(ScreenshotMachine.Settings.KeyBinds.CameraSpeedUp.IsPressed?0.005f:0.1f);
            }
        }

        private static void RestoreCameraBehaviour()
        {
            var camCtrl = GameCameras.instance.cameraController;

            camCtrl.SetMode(CameraController.CameraMode.FOLLOWING);
            camCtrl.PositionToHero(false);
            camCtrl.transform.position += new Vector3(0, 0, -38.1f - camCtrl.transform.GetPositionZ());

            _frozenCamera = false;
        }

        private static void ToggleOn()
        {
            HeroController.instance.vignette.enabled = false;

            GameObject hero = HeroController.instance.gameObject;

            var light = hero.transform.Find("HeroLight");
            var sr = light.GetComponent<SpriteRenderer>();

            sr.color = sr.color with { a = 0f };

            GameCameras.instance.hudCanvas.gameObject.SetActive(false);

            hero.GetComponent<tk2dSprite>().color = sr.color;

            HeroController.instance.damageMode = DamageMode.NO_DAMAGE;

            _toggled = true;
        }

        private static void ToggleOff()
        {
            HeroController.instance.vignette.enabled = true;

            GameObject heroControllerGO = HeroController.instance.gameObject;

            Color a = heroControllerGO.transform.Find("HeroLight").gameObject.GetComponent<SpriteRenderer>().color;
            a.a = 0.7f;
            heroControllerGO.transform.Find("HeroLight").gameObject.GetComponent<SpriteRenderer>().color = a;

            GameCameras.instance.hudCanvas.gameObject.SetActive(true);

            a.a = 1f;
            heroControllerGO.GetComponent<tk2dSprite>().color = a;

            HeroController.instance.damageMode = DamageMode.FULL_DAMAGE;

            _toggled = false;
        }

        private static void ToggleBlurPlane()
        {
            _blurPlaneOn = !_blurPlaneOn;
            
            // Unity do not break (??) challenge (very hard)
            _blurPlane = _blurPlane ? _blurPlane : GameObject.Find("BlurPlane");
            
            if (_blurPlane != null)
                _blurPlane.SetActive(_blurPlaneOn);
        }

        private static IEnumerator DrawCenterLines()
        {
            ScreenshotMachine.Log("Drawing middle intersection lines...");

            GameObject lineCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceCamera, new Vector2(1920, 1080));

            GameObject lines = CanvasUtil.CreateImagePanel
            (
                lineCanvas,
                LineSprite,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
            );

            var group = lines.gameObject.AddComponent<CanvasGroup>();
            group.interactable = false;
            group.blocksRaycasts = false;

            yield return new WaitUntil(() => !_lineDrawn);

            ScreenshotMachine.Log("Destroying middle intersection lines...");
            Object.Destroy(lines);
        }

        public static void HotkeyHandler()
        {
            if (ScreenshotMachine.Settings.KeyBinds.ToggleCenterLineKey.WasPressed)
            {
                _lineDrawn = !_lineDrawn;
                
                if (_lineDrawn)
                    GameManager.instance.StartCoroutine(DrawCenterLines());
            }

            var settings = ScreenshotMachine.Settings;

            (Vector3 offset, PlayerAction action)[] pairs =
            {
                (Vector3.zero with { x = 0.1f }, settings.KeyBinds.CameraRightKey),
                (Vector3.zero with { x = -0.1f }, settings.KeyBinds.CameraLeftKey),
                (Vector3.zero with { y = 0.1f }, settings.KeyBinds.CameraUpKey),
                (Vector3.zero with { y = -0.1f }, settings.KeyBinds.CameraDownKey),
                (Vector3.zero with { z = 0.1f }, settings.KeyBinds.CameraOutKey),
                (Vector3.zero with { z = -0.1f }, settings.KeyBinds.CameraInKey)
            };

            foreach ((Vector3 offset, PlayerAction action) in pairs.Where(x => x.action.WasPressed))
                GameManager.instance.StartCoroutine(MoveCamera(offset, action));

            if (ScreenshotMachine.Settings.KeyBinds.RestoreCameraKey.WasPressed)
                RestoreCameraBehaviour();


            if (ScreenshotMachine.Settings.KeyBinds.ToggleScreenshotModeKey.WasPressed)
            {
                if (!_toggled)
                    ToggleOn();
                else
                    ToggleOff();
            }

            if (ScreenshotMachine.Settings.KeyBinds.BlurPlaneToggleKey.WasPressed)
                ToggleBlurPlane();
        }

        public static void RemoveCameraLogic(On.CameraController.orig_LateUpdate orig, CameraController self)
        {
            if (!_frozenCamera)
                orig(self);
        }

        public static void Reset
        (
            On.GameManager.SceneLoadInfo.orig_NotifyFetchComplete orig,
            GameManager.SceneLoadInfo sceneLoadInfo
        )
        {
            RestoreCameraBehaviour();
            ToggleOff();

            if (_blurPlaneOn)
                ToggleBlurPlane();

            orig(sceneLoadInfo);
        }
    }
}