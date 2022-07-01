using System.Collections;
using System.Linq;
using GlobalEnums;
using Modding;
using UnityEngine;

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

        private static IEnumerator MoveCamera(Vector3 movement, KeyCode keyPressed)
        {
            if (!_frozenCamera)
            {
                GameCameras.instance.cameraController.FreezeInPlace();
                _frozenCamera = true;
            }

            while (Input.GetKey(keyPressed))
            {
                GameCameras.instance.cameraController.transform.position += movement;
                yield return new WaitForSeconds(0.1f);
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
            if (Input.GetKeyDown(ScreenshotMachine.Settings.ToggleCenterLineKey))
            {
                _lineDrawn = !_lineDrawn;
                
                if (_lineDrawn)
                    GameManager.instance.StartCoroutine(DrawCenterLines());
            }

            var settings = ScreenshotMachine.Settings;

            (Vector3 offset, KeyCode key)[] pairs =
            {
                (Vector3.zero with { x = 0.1f }, settings.CameraRightKey),
                (Vector3.zero with { x = -0.1f }, settings.CameraLeftKey),
                (Vector3.zero with { y = 0.1f }, settings.CameraUpKey),
                (Vector3.zero with { y = -0.1f }, settings.CameraDownKey),
                (Vector3.zero with { z = 0.1f }, settings.CameraOutKey),
                (Vector3.zero with { z = -0.1f }, settings.CameraInKey)
            };

            foreach ((Vector3 offset, KeyCode key) in pairs.Where(x => Input.GetKeyDown(x.key)))
                GameManager.instance.StartCoroutine(MoveCamera(offset, key));

            if (Input.GetKeyDown(ScreenshotMachine.Settings.RestoreCameraKey))
                RestoreCameraBehaviour();


            if (Input.GetKeyDown(ScreenshotMachine.Settings.ToggleScreenshotModeKey))
            {
                if (!_toggled)
                    ToggleOn();
                else
                    ToggleOff();
            }

            if (Input.GetKeyDown(ScreenshotMachine.Settings.BlurPlaneToggleKey))
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