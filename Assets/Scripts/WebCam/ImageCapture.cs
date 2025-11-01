using UnityEngine;

// 画像キャプチャ
public class ImageCapture : MonoBehaviour
{
    [SerializeField] private WebCameraManager cameraManager;

    public byte[] CaptureAsJPG(int quality = 85)
    {
        Texture2D screenshot = cameraManager.GetCurrentFrameAsTexture2D();

        if (screenshot == null)
        {
            Debug.LogError("[ImageCapture] フレーム取得に失敗しました");
            return null;
        }

        // JPGエンコードはメインスレッド必須（Unity制限）
        byte[] bytes = ImageConversion.EncodeToJPG(screenshot, quality);

        Destroy(screenshot);

        return bytes;
    }
}