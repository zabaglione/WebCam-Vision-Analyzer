using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

// 画像分析UI
public class ImageAnalysisUIController : MonoBehaviour
{
    [SerializeField] private Button analyzeButton;
    [SerializeField] private ImageCapture imageCapture;
    [SerializeField] private OpenAIVisionAnalyzer analyzer;
    [SerializeField] private TextMeshProUGUI resultText;

    private bool isAnalyzing = false;

    void Start()
    {
        if (analyzeButton != null)
        {
            analyzeButton.onClick.AddListener(() => _ = OnAnalyzeButtonClicked());
        }
    }

    async Task OnAnalyzeButtonClicked()
    {
        if (isAnalyzing)
        {
            Debug.LogWarning("[ImageAnalysisUIController] 既に分析中です");
            return;
        }

        isAnalyzing = true;
        analyzeButton.interactable = false;

        try
        {
            // 画像をキャプチャ
            byte[] imageData = imageCapture.CaptureAsJPG(85);
            if (imageData == null)
            {
                Debug.LogError("[ImageAnalysisUIController] 画像取得失敗");
                return;
            }

            Debug.Log("[ImageAnalysisUIController] 分析開始");

            // OpenAI APIで分析
            string prompt = GetAnalysisPrompt();
            string result = await analyzer.AnalyzeImageAsync(imageData, prompt);

            if (result != null)
            {
                Debug.Log($"[ImageAnalysisUIController] 分析結果: {result}");
                if (resultText != null)
                {
                    resultText.text = result;
                }
            }
            else
            {
                Debug.LogError("[ImageAnalysisUIController] 分析失敗");
            }
        }
        finally
        {
            isAnalyzing = false;
            analyzeButton.interactable = true;
        }
    }

    private string GetAnalysisPrompt()
    {
        return @"この画像を分析して、以下の項目について答えてください。JSON形式で回答してください：

{
  ""contains_insect"": true/false,  // 虫が写っているか
  ""contains_cat"": true/false,     // 猫が写っているか
  ""contains_dog"": true/false,     // 犬が写っているか
  ""contains_plant"": true/false,   // 植物が写っているか
  ""description"": ""画像の簡潔な説明"",
  ""confidence"": 0.0-1.0           // 判定の確信度
}

分析結果のみを出力し、その他のテキストは含めないでください。";
    }
}