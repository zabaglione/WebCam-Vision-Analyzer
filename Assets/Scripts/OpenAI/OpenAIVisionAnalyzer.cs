using UnityEngine;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

// OpenAI API通信
public class OpenAIVisionAnalyzer : MonoBehaviour
{
    private string apiKey;
    private const string API_ENDPOINT = "https://api.openai.com/v1/chat/completions";
    private const string MODEL_NAME = "gpt-4o";

    void Start()
    {
        apiKey = System.Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("[OpenAIVisionAnalyzer] OPENAI_API_KEYが設定されていません");
            Debug.LogError("[OpenAIVisionAnalyzer] セットアップ手順：");
            Debug.LogError("[OpenAIVisionAnalyzer] 1. OpenAIのAPIキーを取得（https://platform.openai.com/api-keys）");
            Debug.LogError("[OpenAIVisionAnalyzer] 2. 環境変数に設定：");
            Debug.LogError("[OpenAIVisionAnalyzer]    Windows: setx OPENAI_API_KEY \"your-key-here\"");
            Debug.LogError("[OpenAIVisionAnalyzer]    Mac/Linux: export OPENAI_API_KEY=\"your-key-here\"");
            Debug.LogError("[OpenAIVisionAnalyzer] 3. UnityおよびUnity Hubを再起動して反映");
        }
    }

    public async Task<string> AnalyzeImageAsync(byte[] imageData, string prompt)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("[OpenAIVisionAnalyzer] APIキーが未設定です");
            return null;
        }

        string base64Image = System.Convert.ToBase64String(imageData);
        string jsonRequest = BuildRequestJson(base64Image, prompt);

        using (var www = new UnityEngine.Networking.UnityWebRequest(API_ENDPOINT, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
            www.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            var request = www.SendWebRequest();

            while (!request.isDone)
            {
                await Task.Delay(10);
            }

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[OpenAIVisionAnalyzer] API呼び出し失敗: {www.error}");
                Debug.LogError($"[OpenAIVisionAnalyzer] レスポンス: {www.downloadHandler.text}");
                return null;
            }

            string responseJson = www.downloadHandler.text;
            var response = JsonUtility.FromJson<ImageAnalysisResponse>(responseJson);

            if (response.choices != null && response.choices.Length > 0)
            {
                return response.choices[0].message.content;
            }

            return null;
        }
    }

    private string BuildRequestJson(string base64Image, string prompt)
    {
        var request = new JObject
        {
            ["model"] = MODEL_NAME,
            ["messages"] = new JArray
            {
                new JObject
                {
                    ["role"] = "user",
                    ["content"] = new JArray
                    {
                        new JObject { ["type"] = "text", ["text"] = prompt },
                        new JObject
                        {
                            ["type"] = "image_url",
                            ["image_url"] = new JObject { ["url"] = $"data:image/jpeg;base64,{base64Image}" }
                        }
                    }
                }
            },
            ["max_tokens"] = 1024
        };

        return request.ToString(Newtonsoft.Json.Formatting.None);
    }
}

[System.Serializable]
public class ImageAnalysisResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public MessageContent message;
}

[System.Serializable]
public class MessageContent
{
    public string content;
}