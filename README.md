# WebCam Vision Analyzer

Unityで構築されたウェブカメラ画像キャプチャ・分析アプリケーション。OpenAI GPT-4oのビジョンAPIを使用して、撮影した画像から虫、猫、犬、植物などの物体を検出します。

## 概要

このプロジェクトは、以下の機能を提供します：

- ウェブカメラからのリアルタイム画像表示
- JPG形式での画像キャプチャと保存
- OpenAI GPT-4oビジョンAPIを使用した画像分析
- 物体検出（虫、猫、犬、植物など）
- 非同期ファイルI/Oによるノンブロッキング処理
- クリーンなアーキテクチャ設計（責務分離）

## 環境要件

- **Unity**: 6.0以上
- **.NET Framework**: API Compatibility Level設定
- **OpenAI APIキー**: https://platform.openai.com/api-keys より取得
- **Newtonsoft.Json**: Unityパッケージマネージャーで自動インストール

## インストール手順

### 1. リポジトリのクローン
```bash
git clone https://github.com/yourusername/WebCam-Vision-Analyzer.git
cd WebCam-Vision-Analyzer
```

### 2. Unityプロジェクトを開く

- Unity Hubでプロジェクトを追加
- または、Unityエディタで`File > Open Project`でこのディレクトリを選択

### 3. 依存パッケージの インストール

Unity起動後、Package Manager からインストール：
```
Window > TextMesh Pro > Import TMP Essential Resources
```

パッケージ名で追加：
- `com.unity.nuget.newtonsoft-json`

### 4. OpenAI APIキーの設定

OpenAIのサイトからAPIキーを取得し、環境変数に設定します。

**Windows（PowerShell）:**
```powershell
[Environment]::SetEnvironmentVariable("OPENAI_API_KEY", "your-api-key-here", "User")
```

**Windows（コマンドプロンプト）:**
```cmd
setx OPENAI_API_KEY "your-api-key-here"
```

**Mac/Linux:**
```bash
export OPENAI_API_KEY="your-api-key-here"
```

> 環境変数設定後、Unityを再起動してください。

## プロジェクト構成
```
WebCam-Vision-Analyzer/
├── Assets/
│   ├── Scenes/
│   │   └── MainScene.unity          # メインシーン
│   ├── Scripts/
│   │   ├── WebCameraManager.cs      # ウェブカメラ管理
│   │   ├── ImageCapture.cs          # 画像キャプチャ（JPGエンコード）
│   │   ├── ImageSaver.cs            # 非同期ファイル保存
│   │   ├── OpenAIVisionAnalyzer.cs  # OpenAI API連携
│   │   ├── CameraUIController.cs    # キャプチャUI制御
│   │   └── ImageAnalysisUIController.cs  # 分析UI制御
│   └── Prefabs/
│       └── (プリファブがあれば配置)
├── ProjectSettings/           # Unity設定ファイル
├── Packages/                  # パッケージ設定
├── README.md                  # このファイル
├── .gitignore                 # Git除外設定
└── LICENSE                    # ライセンス
```

## 使用方法

### 基本的な操作

1. **Captureボタンを押す**
   - ウェブカメラから画像をキャプチャし、JPGで保存
   - 保存先: `Application.persistentDataPath/screenshot_YYYY-MM-DD_HH-mm-ss.jpg`

2. **Analyzeボタンを押す**
   - キャプチャした画像をOpenAI GPT-4oに送信
   - 分析結果がUIに表示

### シーンのセットアップ

1. **Canvas作成**
   - Hierarchy右クリック > UI > Canvas

2. **UI要素配置**
   - RawImage: ウェブカメラ表示用
   - Button: Captureボタン
   - Button: Analyzeボタン
   - Text: 分析結果表示用

3. **スクリプトアタッチ**
   - 空のGameObject作成（例：CameraSystem）
   - 以下のスクリプトをアタッチ：
     - WebCameraManager
     - ImageCapture
     - ImageSaver
     - OpenAIVisionAnalyzer
     - CameraUIController
     - ImageAnalysisUIController

4. **Inspectorで設定**
   - **WebCameraManager**: RawImageを指定
   - **CameraUIController**: 
     - Capture Button
     - Image Capture
     - Image Saver
   - **ImageAnalysisUIController**:
     - Analyze Button
     - Image Capture
     - OpenAI Vision Analyzer
     - Result Text

## API分析結果形式

デフォルトの分析結果（JSON形式）:
```json
{
  "contains_insect": true,
  "contains_cat": false,
  "contains_dog": false,
  "contains_plant": true,
  "description": "屋外の庭で撮影された画像。複数の植物と昆虫が映っている。",
  "confidence": 0.92
}
```

## スクリプトアーキテクチャ

### 責務分離設計

| クラス | 責務 |
|--------|------|
| `WebCameraManager` | ウェブカメラの初期化・フレーム取得 |
| `ImageCapture` | Texture2D→JPGエンコード処理 |
| `ImageSaver` | 非同期ファイルI/O処理 |
| `OpenAIVisionAnalyzer` | OpenAI API通信 |
| `CameraUIController` | キャプチャボタンUI制御 |
| `ImageAnalysisUIController` | 分析ボタン・結果表示UI制御 |

### 非同期処理フロー
```
ボタンクリック
    ↓
UIController (async)
    ├─ ImageCapture.CaptureAsJPG() [同期・メインスレッド]
    │   └─ JPGエンコード (Unity制限)
    ├─ ImageSaver.SaveImageAsync() [非同期・バックグラウンド]
    │   └─ ファイルI/O
    └─ OpenAIVisionAnalyzer.AnalyzeImageAsync() [非同期]
        └─ API通信
```

## トラブルシューティング

### APIキーが見つからないエラー
```
[OpenAIVisionAnalyzer] OPENAI_API_KEYが設定されていません
```

**対処法:**
- 環境変数を正しく設定したか確認
- Unityを完全に再起動（エディタを閉じて再度開く）
- `echo %OPENAI_API_KEY%` (Windows) または `echo $OPENAI_API_KEY` (Mac/Linux) で確認

### ウェブカメラが検出されない
```
[WebCameraManager] ウェブカメラが見つかりません
```

**対処法:**
- ウェブカメラが接続されているか確認
- 他のアプリでカメラが使用中でないか確認
- Unityの実行権限確認（macOS: System Preferences > Security）

### "Model gpt-4o does not exist" エラー
```
"message": "The model `gpt-4o` does not exist or you do not have access to it."
```

**対処法:**
- OpenAIアカウントがGPT-4oへのアクセス権を持っているか確認
- APIキーが有効か確認（https://platform.openai.com/account/api-keys）
- 十分なクレジットがあるか確認

### JSONパースエラー
```
"message": "We could not parse the JSON body of your request."
```

**対処法:**
- Newtonsoft.Jsonがインストール済みか確認
- `OpenAIVisionAnalyzer.cs`でエスケープ処理を確認
- Unityコンソールで送信されたJSONを確認

## パフォーマンス情報

| 項目 | 値 | 備考 |
|------|-----|------|
| ウェブカメラ解像度 | 1280x720 | `WebCameraManager`で変更可 |
| JPG圧縮品質 | 85 | `ImageCapture.CaptureAsJPG()`の引数で調整 |
| API最大トークン | 1024 | `OpenAIVisionAnalyzer`で変更可 |
| ファイルI/O | 非同期 | メインスレッドへのブロックなし |
| JPGエンコード | 同期・メインスレッド | Unity仕様による制限 |

## モバイル対応

### Android

`AndroidManifest.xml`に以下を追加：
```xml
<uses-permission android:name="android.permission.CAMERA" />
```

### iOS

`Info.plist`に以下を追加：
```xml
<key>NSCameraUsageDescription</key>
<string>ウェブカメラにアクセスするために必要です</string>
```

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。詳細は`LICENSE`ファイルを参照してください。

## 参考リンク

- [OpenAI API ドキュメント](https://platform.openai.com/docs/guides/vision)
- [Unity ネットワーキング](https://docs.unity3d.com/ja/current/Manual/UnityWebRequest.html)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)

## サポート

問題や質問がある場合：

1. このREADMEのトラブルシューティングを確認
2. GitHubのIssuesを確認
3. GitHubに新しいIssueを作成してください

## 貢献

改善提案やバグ報告は大歓迎です。

1. このリポジトリをフォーク
2. フィーチャーブランチを作成 (`git checkout -b feature/AmazingFeature`)
3. コミット (`git commit -m 'Add some AmazingFeature'`)
4. ブランチにプッシュ (`git push origin feature/AmazingFeature`)
5. Pull Requestを作成

---

**最終更新**: 2025年11月1日  
**メンテナ**: X: @z_zabaglione