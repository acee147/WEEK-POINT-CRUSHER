using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 重み計算とLinqメソッドのために必要

public class UpgradeManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static UpgradeManager Instance { get; private set; }

    // ★★★ アップグレードの定義 (構造体) ★★★
    [System.Serializable]
    public struct UpgradeOption
    {
        public string type;   // UpgradeManager.ApplyUpgrade()に渡す文字列 (例: "FireRate")
        public int weight;    // 抽選における重み (高いほど出やすい)
    }

    [Header("アップグレード設定")]
    [SerializeField]
    private int upgradeThreshold = 500;
    [Tooltip("抽選対象のアップグレードと重みを設定")]
    [SerializeField]
    private List<UpgradeOption> availableUpgrades; // 抽選対象のアップグレードリスト

    [Header("参照")]
    [SerializeField]
    private PlayerStats playerStats;
    [SerializeField]
    private GameObject upgradePanel;
    [Tooltip("アップグレードパネルのUI制御スクリプトを割り当て")]
    [SerializeField]
    private UpgradeUIPanel upgradeUIPanel; // UI制御スクリプトへの参照

    private int currentScore = 0;
    private bool waitingForUpgrade = false;

    // 抽選された選択肢を保持し、外部（UpgradeUIPanel）に公開するプロパティ
    private List<UpgradeOption> currentChoices;
    public List<UpgradeOption> CurrentChoices => currentChoices;


    void Awake()
    {
        // シングルトンインスタンスの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 外部（TargetSpotなど）からスコア加算時に呼び出されるメソッド
    public void AddScore(int points)
    {
        if (waitingForUpgrade) return;

        currentScore += points;

        if (currentScore >= upgradeThreshold)
        {
            ShowUpgradeChoice();
            // 次の閾値を設定
            upgradeThreshold += 500;
        }
    }

    // アップグレード選択肢を提示し、ゲームを一時停止
    private void ShowUpgradeChoice()
    {
        if (playerStats == null)
        {
            Debug.LogError("Player Stats Script (PlayerStats) is not assigned to the UpgradeManager!");
            return;
        }

        waitingForUpgrade = true;
        Time.timeScale = 0f; // ゲームを一時停止

        // ★★★ 抽選ロジックの実行 ★★★
        currentChoices = GetRandomUpgrades(3); // 3つをランダムに抽選

        // UIスクリプトを呼び出し、抽選結果を渡す
        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetChoices(currentChoices);
        }
        else
        {
            Debug.LogError("UpgradeUIPanelが設定されていません！");
        }

        // UIパネルを表示
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }

        Debug.Log("--- UPGRADE TIME! ---");
    }

    // 重み付け抽選ロジック
    private List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> candidates = new List<UpgradeOption>(availableUpgrades);
        List<UpgradeOption> results = new List<UpgradeOption>();

        for (int i = 0; i < count; i++)
        {
            if (candidates.Count == 0) break;

            // 抽選候補の重み合計を計算
            int totalWeight = candidates.Sum(u => u.weight);
            if (totalWeight <= 0) break;

            int randomNumber = Random.Range(0, totalWeight);
            int cumulativeWeight = 0;
            UpgradeOption selected = new UpgradeOption();

            // 抽選 (重みに基づいて選択)
            foreach (var upgrade in candidates)
            {
                cumulativeWeight += upgrade.weight;
                if (randomNumber < cumulativeWeight)
                {
                    selected = upgrade;
                    break;
                }
            }

            // 選ばれたアップグレードを結果に追加し、候補リストから削除（重複を防ぐ）
            if (selected.type != null)
            {
                results.Add(selected);
                candidates.Remove(selected);
            }
        }

        return results;
    }

    // 外部（UIボタン）から選択されたときに呼ばれるメソッド
    public void ApplyUpgrade(string upgradeType)
    {
        if (!waitingForUpgrade || playerStats == null) return;

        // プレイヤーの能力値に適用
        if (upgradeType == "FireRate")
        {
            playerStats.IncreaseFireRate(0.05f);
        }
        else if (upgradeType == "MoveSpeed")
        {
            playerStats.IncreaseMoveSpeed(0.5f);
        }
        else if (upgradeType == "SlowMotionUses")
        {
            playerStats.IncreaseSlowMotionUses(1);
            // GameManagerに現在の使用回数をリセットさせる
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetCurrentSlowMotionUses();
            }
        }
        else if (upgradeType == "WideShot")
        {
            playerStats.EnableWideShot(3); // 3発発射を有効化
        }

        // UIパネルを非表示にする
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        // ゲーム再開
        waitingForUpgrade = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        Debug.Log($"'{upgradeType}' アップグレードを適用し、ゲームを再開しました。");
    }

    // スコア取得用プロパティ
    public int CurrentScore
    {
        get { return currentScore; }
    }
}