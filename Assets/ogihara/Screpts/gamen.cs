using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    // ★★★ 16:9に設定 ★★★
    [SerializeField]
    private float targetAspectWidth = 16f;
    [SerializeField]
    private float targetAspectHeight = 9f;
    // ★★★★★★★★★★★★

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("メインカメラが見つかりません。");
            return;
        }

        AdjustCameraAspect();
    }

    void Update()
    {
        // 画面サイズが変更されるたびに調整
        AdjustCameraAspect();
    }

    void AdjustCameraAspect()
    {
        // ターゲットアスペクト比 (16 / 9)
        float targetAspect = targetAspectWidth / targetAspectHeight;

        // 現在のウィンドウのアスペクト比
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // ターゲットアスペクト比とウィンドウアスペクト比の比率を計算
        float scaleHeight = windowAspect / targetAspect;

        // カメラの描画領域（Viewport Rect）を設定
        if (scaleHeight < 1.0f)
        {
            // ウィンドウが目標より縦長の場合 (上下に黒帯)
            Rect rect = new Rect(0.0f, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
            mainCamera.rect = rect;
        }
        else
        {
            // ウィンドウが目標より横長の場合 (左右に黒帯)
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = new Rect((1.0f - scaleWidth) / 2.0f, 0.0f, scaleWidth, 1.0f);
            mainCamera.rect = rect;
        }
    }
}