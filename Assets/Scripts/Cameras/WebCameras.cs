using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WebCameras : MonoBehaviour
{
    /// <summary>
    /// This script is for create a webserver of a single webcamera not an array
    /// </summary>
    [Header("Camera & Capture")]
    public Camera cam;
    public int width = 640;
    public int heigth = 360;
    [Range(0, 100)] public int JPGQuality = 75;
    [Range(0, 60)] public float sendFPS = 10f;

    [Header("Server")]
    public string postURL = "http:||127.0.0.1:500/frame";

    private RenderTexture rt;
    private Texture2D tex;
    float timer;


    private void Start()
    {
        if (cam == null) cam = Camera.main;
        rt = new RenderTexture(width, heigth, 24, RenderTextureFormat.ARGB32);
        tex = new Texture2D(width, heigth, TextureFormat.RGB24, false);
    }

    private void OnDestroy()
    {
        if (rt != null) rt.Release();
        Destroy(tex);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f / Mathf.Max(sendFPS, 0.1f))
        {
            timer = 0f;
            StartCoroutine(CaptureAndSend());
        }
    }

    IEnumerator CaptureAndSend()
    {
        var prev = cam.targetTexture;
        cam.targetTexture = rt;
        cam.Render();
        cam.targetTexture = prev;

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] jpg = tex.EncodeToJPG(JPGQuality);

        using (UnityWebRequest req = new UnityWebRequest(postURL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(jpg);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/octet-stream");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogWarning("Upload error: " + req.error);
            else
                Debug.Log("Server: " + req.downloadHandler.text);
        }
    }

}