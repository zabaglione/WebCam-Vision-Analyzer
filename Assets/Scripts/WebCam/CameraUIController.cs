using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

// UI制御（ボタンクリックの処理）
public class CameraUIController : MonoBehaviour
{
    [SerializeField] private Button captureButton;
    [SerializeField] private ImageCapture imageCapture;
    [SerializeField] private ImageSaver imageSaver;

    private bool isProcessing = false;

    void Start()
    {
        if (captureButton == null || imageCapture == null || imageSaver == null)
        {
            Debug.LogError("[CameraUIController] 必須コンポーネントが未設定です");
            return;
        }

        captureButton.onClick.AddListener(() => _ = OnCaptureButtonClicked());
    }

    async Task OnCaptureButtonClicked()
    {
        if (isProcessing)
        {
            Debug.LogWarning("[CameraUIController] 既に処理中です");
            return;
        }

        isProcessing = true;
        captureButton.interactable = false;

        try
        {
            // エンコードはメインスレッド（同期）
            byte[] imageData = imageCapture.CaptureAsJPG(85);

            if (imageData != null)
            {
                // ファイル保存はバックグラウンド（非同期）
                await imageSaver.SaveImageAsync(imageData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[CameraUIController] 例外発生: {ex.Message}");
        }
        finally
        {
            isProcessing = false;
            captureButton.interactable = true;
        }
    }
}