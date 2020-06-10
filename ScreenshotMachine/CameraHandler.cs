using System.Collections;
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
        private static float _xOffset;
        private static float _xLookAhead;
        private static bool _lineDrawn;
        private static bool _blurPlaneOff;

        public const KeyCode KeyMoveCameraLeft = KeyCode.LeftArrow;
        public const KeyCode KeyMoveCameraRight = KeyCode.RightArrow;
        public const KeyCode KeyMoveCameraUp = KeyCode.UpArrow;
        public const KeyCode KeyMoveCameraDown = KeyCode.DownArrow;
        public const KeyCode KeyMoveCameraIn = KeyCode.Keypad6;
        public const KeyCode KeyMoveCameraOut = KeyCode.Keypad4;
        public const KeyCode KeyToggleScreenshotMode = KeyCode.Pause;
        public const KeyCode KeyToggleBlurPlane = KeyCode.R;
        public const KeyCode KeyToggleCenterLine = KeyCode.E;
        public const KeyCode KeyRestoreCamera = KeyCode.Q;

        
        public static Sprite LineSprite { get; set; }


        private static IEnumerator MoveCamera(Vector3 movement, KeyCode keyPressed)
        {

            if (!_frozenCamera)
            {
                _xOffset = GameCameras.instance.cameraController.camTarget.xOffset;
                GameCameras.instance.cameraController.camTarget.xOffset = 0;
                _xLookAhead = GameCameras.instance.cameraController.camTarget.xLookAhead;
                GameCameras.instance.cameraController.camTarget.xLookAhead = 0;
                GameCameras.instance.cameraController.camTarget.stickToHeroY = false;
                GameCameras.instance.cameraController.camTarget.stickToHeroX = false;


                GameCameras.instance.cameraController.camTarget.FreezeInPlace();
                _frozenCamera = true;
            }

            while (Input.GetKey(keyPressed))
            {
                GameCameras.instance.cameraController.camTarget.transform.position += movement;
                GameCameras.instance.cameraController.transform.position += movement;
                yield return new WaitForSeconds(0.25f);
            }

        }

        private static void RestoreCameraBehaviour()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_xOffset != 0f)
            {
                GameCameras.instance.cameraController.camTarget.xOffset = _xOffset;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_xLookAhead != 0f)
            {
                GameCameras.instance.cameraController.camTarget.xLookAhead = _xLookAhead;

            }
            GameCameras.instance.cameraController.camTarget.mode = CameraTarget.TargetMode.FOLLOW_HERO;
            GameCameras.instance.cameraController.camTarget.stickToHeroY = true;
            GameCameras.instance.cameraController.camTarget.stickToHeroX = true;
            _frozenCamera = false;
        }

        private static void ToggleOn()
        {
            HeroController.instance.vignette.enabled = false;
            GameObject heroControllerGO = HeroController.instance.gameObject;

            Color transparent = heroControllerGO.transform.Find("HeroLight").gameObject.GetComponent<SpriteRenderer>().color;
            transparent.a = 0f;
            heroControllerGO.transform.Find("HeroLight").gameObject.GetComponent<SpriteRenderer>().color = transparent;
            
            GameCameras.instance.hudCanvas.gameObject.SetActive(false);
            
            heroControllerGO.GetComponent<tk2dSprite>().color = transparent;
            
            HeroController.instance.damageMode = DamageMode.NO_DAMAGE;
            heroControllerGO.layer = (int)PhysLayers.CORPSE; //Make enemies not aggro and disable transitions
            
            
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
            heroControllerGO.layer = (int)PhysLayers.PLAYER;
            

            _toggled = false;
        }

        private static void ToggleBlurPlane()
        {
            if (!_blurPlaneOff)
            {
                _blurPlane = GameObject.Find("BlurPlane");
                if (_blurPlane != null)
                    _blurPlane.SetActive(false);
                _blurPlaneOff = true;
            }

            else
            {
                if(_blurPlane!=null)
                    _blurPlane.SetActive(true);
                _blurPlaneOff = false;
            }
        }

        private static IEnumerator DrawCenterLines()
        {

            ScreenshotMachine.Log("Drawing middle intersection lines...");

            GameObject lineCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceCamera, new Vector2(1920, 1080));


            GameObject lines = CanvasUtil.CreateImagePanel(lineCanvas, LineSprite,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one));

            lines.gameObject.AddComponent<CanvasGroup>();
            CanvasGroup group = lines.gameObject.GetComponent<CanvasGroup>();
            group.interactable = false;
            group.blocksRaycasts = false;
            
            

            yield return new WaitUntil(()=> !_lineDrawn);
            ScreenshotMachine.Log("Destroying middle intersection lines...");
            Object.Destroy(lines);

        }

        public static void HotkeyHandler()
        {

            if (Input.GetKeyDown(KeyToggleCenterLine))
            {
                if (!_lineDrawn)
                {
                    _lineDrawn = true;
                    GameManager.instance.StartCoroutine(DrawCenterLines());
                }
                else
                {
                    _lineDrawn = false;
                }
                
            }
            if (Input.GetKeyDown(KeyMoveCameraRight))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0.1f, 0f, 0f), KeyMoveCameraRight));
            }

            if (Input.GetKeyDown(KeyMoveCameraLeft))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(-0.1f, 0f, 0f), KeyMoveCameraLeft));
            }

            if (Input.GetKeyDown(KeyMoveCameraUp))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, 0.1f, 0f), KeyMoveCameraUp));
            }

            if (Input.GetKeyDown(KeyMoveCameraDown))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, -0.1f, 0f), KeyMoveCameraDown));
            }
            
            if (Input.GetKeyDown(KeyMoveCameraOut))
            {
                
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, 0f, 0.1f), KeyMoveCameraOut));

            }
            
            if (Input.GetKeyDown(KeyMoveCameraIn))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, 0f, -0.1f), KeyMoveCameraIn));

            }

            if (Input.GetKeyDown(KeyRestoreCamera))
            {
                RestoreCameraBehaviour();

            }


            if (Input.GetKeyDown(KeyToggleScreenshotMode))
            {
                if (!_toggled)
                {
                    ToggleOn();
                }

                else
                {
                    ToggleOff();
                }

            }

            if (Input.GetKeyDown(KeyToggleBlurPlane))
            {
                ToggleBlurPlane();
            }
        }

        public static void RemoveCameraLogic(On.CameraController.orig_LateUpdate orig, CameraController self)
        {
            if (!_frozenCamera)
                orig(self);
        }
        
        public static void Reset(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            RestoreCameraBehaviour();            
            ToggleOff();
            if(_blurPlaneOff)
                ToggleBlurPlane();
            orig(self, info);
        }
    }
}