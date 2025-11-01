using UnityEngine;
using System.IO;
using System.Threading.Tasks;

// 画像ファイル保存
public class ImageSaver : MonoBehaviour
{
    public async Task<string> SaveImageAsync(byte[] imageData, string fileName = null)
    {
        if (imageData == null || imageData.Length == 0)
        {
            Debug.LogError("保存するデータがありません");
            return null;
        }

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = $"screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg";
        }

        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            await File.WriteAllBytesAsync(filePath, imageData);
            Debug.Log($"画像を保存しました: {filePath}");
            return filePath;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"保存に失敗しました: {ex.Message}");
            return null;
        }
    }
}