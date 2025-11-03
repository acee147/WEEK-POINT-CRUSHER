using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使用するために必要
using TMPro;                       // TextMeshProUGUIを使用するために必要

public class ResultSceneManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField]
    private TextMeshProUGUI finalScoreText; // 最終スコアを表示するUIテキスト

    void Start()
    {
        // GameManagerからスコアを取得し表示
        if (GameManager.Instance != null)
        {
            // GameManagerのGetFinalScoreメソッドを使用してスコアを取得
            int finalScore = GameManager.Instance.GetFinalScore();

            if (finalScoreText != null)
            {
                finalScoreText.text = "FINAL SCORE: " + finalScore.ToString();
            }
            else
            {
                Debug.LogError("Final Score Text is not assigned in ResultSceneManager.");
            }
        }
        else
        {
            // GameManagerが存在しない場合はエラー表示
            if (finalScoreText != null)
            {
                finalScoreText.text = "FINAL SCORE: 0 (Error: No Manager)";
            }
            Debug.LogError("GameManager instance not found. Cannot retrieve score.");
        }
    }

    // UIボタンから呼び出すメソッド (タイトルへ戻る)
    public void OnClickReturnToTitle()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadTitleScene();
        }
        else
        {
            // GameManagerが破棄されていた場合のフォールバック（緊急用）
            SceneManager.LoadScene("TitleScene");
        }
    }

    // UIボタンから呼び出すメソッド (ゲームをリスタート)
    public void OnClickRestartGame()
    {
        if (GameManager.Instance != null)
        {
            // ここでは「GameScene」という名前を仮定して直接読み込む
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}