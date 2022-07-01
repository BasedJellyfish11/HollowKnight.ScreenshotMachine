using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace ScreenshotMachine;

public class Screenshotter : MonoBehaviour
{
    private static string _dir;
    
    private const string _folder= "Screenshots";

    public Screenshotter()
    {
        _dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, _folder);

        if (!Directory.Exists(_dir)) 
            Directory.CreateDirectory(_dir);

        StartCoroutine(Listen());
    }

    private static IEnumerator Listen()
    {
        while (true)
        {
            if (!Input.GetKeyDown(ScreenshotMachine.Settings.CaptureButton))
            {
                yield return null;
                
                continue;
            }
            
            /*
             * Note the WaitForEndOfFrame before each capture,
             * this is necessary in order to make sure the rendering
             * has finished, otherwise capture behaviour is undefined.
             */

            // tk2dCamera with W/H as specified
            yield return new WaitForEndOfFrame();
            CaptureScreen(GameManager.instance.cameraCtrl.cam);
            
            // Normal resolution
            yield return new WaitForEndOfFrame();
            var x1 = ScreenCapture.CaptureScreenshotAsTexture();
            
            // Capture with CaptureFactor
            yield return new WaitForEndOfFrame();
            var x4 = ScreenCapture.CaptureScreenshotAsTexture(ScreenshotMachine.Settings.CaptureFactor);

            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Write the Unity ones to disk.
            foreach (var (tex, name) in new[] { (x1, "1x"), (x4, "4x") })
            {
                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(GetPath(sceneName, name), bytes);
            }

            yield return null;
        }
    }

    private static void CaptureScreen(Camera cam)
    {
        cam.targetTexture = new RenderTexture
        (
            ScreenshotMachine.Settings.RenderW,
            ScreenshotMachine.Settings.RenderH,
            24,
            RenderTextureFormat.ARGB32
        );

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();

        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        
        RenderTexture.active = prev;

        byte[] bytes = image.EncodeToPNG();
        
        DestroyImmediate(image);

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        File.WriteAllBytes(GetPath(sceneName, cam.name), bytes);

        cam.targetTexture = null;
    }

    private static string GetPath(string sceneName, string name) => 
        Path.Combine(_dir, $"{sceneName}-{UniqueId()}-{name}.png");

    private static string UniqueId() => Guid.NewGuid().ToString();
}