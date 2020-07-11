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
        
        private static bool _lineDrawn;
        private static bool _blurPlaneOff;
        
        
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

            GameCameras.instance.cameraController.SetMode(CameraController.CameraMode.FOLLOWING);
            GameCameras.instance.cameraController.PositionToHero(false);
            Transform transform = GameCameras.instance.cameraController.transform;
            Vector3 position = transform.position;
            position += new Vector3(0,0, -38.1f - position.z);
            transform.position = position;

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
            //heroControllerGO.layer = (int)PhysLayers.CORPSE; //Make enemies not aggro and disable transitions
            
            
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
            //heroControllerGO.layer = (int)PhysLayers.PLAYER;
            

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

            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyToggleCenterLine))
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
            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyMoveCameraRight))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0.1f, 0f, 0f), ScreenshotMachine.Settings.keyMoveCameraRight));
            }

            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyMoveCameraLeft))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(-0.1f, 0f, 0f), ScreenshotMachine.Settings.keyMoveCameraLeft));
            }

            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyMoveCameraUp))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, 0.1f, 0f), ScreenshotMachine.Settings.keyMoveCameraUp));
            }

            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyMoveCameraDown))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, -0.1f, 0f), ScreenshotMachine.Settings.keyMoveCameraDown));
            }
            
            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyMoveCameraOut))
            {
                
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, 0f, 0.1f), ScreenshotMachine.Settings.keyMoveCameraOut));

            }
            
            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyMoveCameraIn))
            {
                GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0f, 0f, -0.1f), ScreenshotMachine.Settings.keyMoveCameraIn));

            }

            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyRestoreCamera))
            {
                RestoreCameraBehaviour();

            }


            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyToggleScreenshotMode))
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

            if (Input.GetKeyDown(ScreenshotMachine.Settings.keyToggleBlurPlane))
            {
                ToggleBlurPlane();
            }
        }

        public static void RemoveCameraLogic(On.CameraController.orig_LateUpdate orig, CameraController self)
        {
            if (!_frozenCamera)
                orig(self);
        }
        
        public static void Reset(On.GameManager.SceneLoadInfo.orig_NotifyFetchComplete origNotifyFetchComplete, GameManager.SceneLoadInfo sceneLoadInfo)
        {
            RestoreCameraBehaviour();            
            ToggleOff();
            if(_blurPlaneOff)
                ToggleBlurPlane();

            origNotifyFetchComplete(sceneLoadInfo);
        }
        
    }
}