using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class WebCam : MonoBehaviour
{
    [System.Serializable]
    public class CameraStream
    {
        [Header("Identity")]
        public string cameraId = "cam1";

        [Header("Source")]
        public Camera cam;

        [Header("Capture Settings")]
        public int width = 640;
        public int height = 360;
        [Range(0, 100)] public int jpgQuality = 75;
        [Range(0, 60)] public float sendFPS = 10f;

        // Internal
        [HideInInspector] public RenderTexture rt;
        [HideInInspector] public Texture2D tex;
        [HideInInspector] public float timer;
        [HideInInspector] public bool isSending;
    }

    [Header("Server")]
    // Base URL only. The final URL will be: {baseURL}/frame/{cameraId}
    public string baseURL = "http://127.0.0.1:5000";

    [Header("Cameras (3 streams)")]
    public CameraStream[] streams = new CameraStream[3];

    void Start()
    {
        // Initialize each stream
        for (int i = 0; i < streams.Length; i++)
        {
            var s = streams[i];
            if (s == null) continue;

            if (s.cam == null)
            {
                // Fallback: try main camera only if this slot is empty
                s.cam = Camera.main;
            }

            s.rt = new RenderTexture(s.width, s.height, 24, RenderTextureFormat.ARGB32);
            s.tex = new Texture2D(s.width, s.height, TextureFormat.RGB24, false);
            s.timer = 0f;
            s.isSending = false;
        }
    }

    void OnDestroy()
    {
        // Release textures
        for (int i = 0; i < streams.Length; i++)
        {
            var s = streams[i];
            if (s == null) continue;

            if (s.rt != null) s.rt.Release();
            if (s.tex != null) Destroy(s.tex);
        }
    }

    void Update()
    {
        // Per-camera pacing
        for (int i = 0; i < streams.Length; i++)
        {
            var s = streams[i];
            if (s == null || s.cam == null) continue;

            float fps = Mathf.Max(s.sendFPS, 0.1f);
            s.timer += Time.deltaTime;

            if (s.timer >= 1f / fps)
            {
                s.timer = 0f;

                // Do not queue if previous request still running
                if (!s.isSending)
                    StartCoroutine(CaptureAndSend(s));
            }
        }
    }

    IEnumerator CaptureAndSend(CameraStream s)
    {
        s.isSending = true;

        // Capture
        var prev = s.cam.targetTexture;
        s.cam.targetTexture = s.rt;
        s.cam.Render();
        s.cam.targetTexture = prev;

        var prevActive = RenderTexture.active;
        RenderTexture.active = s.rt;

        s.tex.ReadPixels(new Rect(0, 0, s.rt.width, s.rt.height), 0, 0);
        s.tex.Apply(false);

        RenderTexture.active = prevActive;

        // Encode
        byte[] jpg = s.tex.EncodeToJPG(s.jpgQuality);

        // URL includes cameraId
        string url = $"{baseURL}/frame/{UnityWebRequest.EscapeURL(s.cameraId)}";

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(jpg);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/octet-stream");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogWarning($"[{s.cameraId}] Upload error: {req.error}");
            else
                Debug.Log($"[{s.cameraId}] Server: {req.downloadHandler.text}");
        }

        s.isSending = false;
    }
}
