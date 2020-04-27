using Modding;
using UnityEngine;

namespace ScreenshotMachine
{
    public class ScreenshotMachine : Mod
    {
        private bool _toggled;


        public override void Initialize()
        {
            ModHooks.Instance.HeroUpdateHook += ScreenshotThings;
            On.GameManager.BeginSceneTransition += ResetCamera;
        }

        private void ResetCamera(On.GameManager.orig_BeginSceneTransition orig, GameManager self,
            GameManager.SceneLoadInfo info)
        {
            GameCameras.instance.cameraController.camTarget.mode = CameraTarget.TargetMode.FOLLOW_HERO;
            orig(self, info);
        }

        private void MoveCamera(Vector3 movement)
        {
            GameCameras.instance.cameraController.camTarget.transform.position += movement;
            GameCameras.instance.cameraController.transform.position += movement;
            GameCameras.instance.cameraController.camTarget.FreezeInPlace();
        }

        private void ScreenshotThings()
        {

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveCamera(new Vector3(0.1f, 0f, 0f));
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveCamera(new Vector3(-0.1f, 0f, 0f));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveCamera(new Vector3(0f, 0.1f, 0f));
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveCamera(new Vector3(0f, -0.1f, 0f));
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                GameCameras.instance.cameraController.camTarget.mode = CameraTarget.TargetMode.FOLLOW_HERO;

            }

            if (Input.GetKeyDown(KeyCode.Pause))
            {
                if (!_toggled)
                {
                    HeroController.instance.vignette.enabled = false;

                    Color a = HeroController.instance.gameObject.transform.Find("HeroLight").gameObject
                        .GetComponent<SpriteRenderer>().color;
                    a.a = 0f;
                    HeroController.instance.gameObject.transform.Find("HeroLight").gameObject
                        .GetComponent<SpriteRenderer>().color = a;
                    GameCameras.instance.hudCanvas.gameObject.SetActive(false);

                }

                if (_toggled)
                {
                    HeroController.instance.vignette.enabled = true;

                    Color a = HeroController.instance.gameObject.transform.Find("HeroLight").gameObject
                        .GetComponent<SpriteRenderer>().color;
                    a.a = 0.7f;
                    HeroController.instance.gameObject.transform.Find("HeroLight").gameObject
                        .GetComponent<SpriteRenderer>().color = a;
                    GameCameras.instance.hudCanvas.gameObject.SetActive(true);
                    
                }
                
                _toggled = !_toggled;

            }
        }
    }
}