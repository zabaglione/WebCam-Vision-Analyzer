using UnityEngine;
using UnityEngine.UI;

// ウェブカメラ管理
public class WebCameraManager : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    private WebCamTexture webCamTexture;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (rawImage == null)
        {
            Debug.LogError("RawImageが指定されていません");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("ウェブカメラが見つかりません");
            return;
        }

        string deviceName = devices[0].name;
        webCamTexture = new WebCamTexture(deviceName, 1280, 720, 30);

        rawImage.texture = webCamTexture;
        webCamTexture.Play();

        Debug.Log("ウェブカメラを初期化しました");
    }

    public Texture2D GetCurrentFrameAsTexture2D()
    {
        if (webCamTexture == null || !webCamTexture.isPlaying)
            return null;

        Texture2D screenshot = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
        screenshot.SetPixels(webCamTexture.GetPixels());
        screenshot.Apply();
        return screenshot;
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}