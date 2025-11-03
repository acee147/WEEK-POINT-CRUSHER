using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ゲーム状態")]
    private bool isGameOver = false;

    [Header("シーン設定")]
    [SerializeField]
    private string titleSceneName = "TitleScene";
    [SerializeField]
    private string resultSceneName = "ResultScene";

    [Header("UI設定")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI slowMoUsesText; // スローモーション回数表示用UI

    // --- スローモーション関連 ---
    [Header("スローモーション設定")]
    private int currentSlowMotionUses = 0; // 現在の使用済み回数 (内部で管理)
    [SerializeField]
    private float slowMotionScale = 0.3f;
    [SerializeField]
    private float slowMotionDuration = 3.0f;
    private bool isSlowMotionActive = false;
    private Coroutine slowMotionCoroutine;


    void Awake()
    {
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

    void Start()
    {
        if (SceneManager.GetActiveScene().name != resultSceneName)
        {
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f && !isSlowMotionActive) return;

        UpdateScoreDisplay();
        // ★UI更新メソッドを呼び出す★
        UpdateSlowMoUsesDisplay();

        // Rキーが押された瞬間にチェック
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isGameOver && !isSlowMotionActive)
            {
                // PlayerStatsから現在の最大回数を動的に取得
                int maxUses = 2; // デフォルト値
                PlayerStats stats = FindObjectOfType<PlayerStats>();
                if (stats != null)
                {
                    maxUses = stats.maxSlowMotionUses;
                }

                if (currentSlowMotionUses < maxUses)
                {
                    ActivateSlowMotion();
                }
                else
                {
                    Debug.Log("スローモーションの使用回数制限に達しました。");
                }
            }
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null && UpgradeManager.Instance != null)
        {
            int currentScore = UpgradeManager.Instance.CurrentScore;
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    // ★★★ スローモーションの「残り回数」を表示するメソッドに修正 ★★★
    private void UpdateSlowMoUsesDisplay()
    {
        if (slowMoUsesText == null) return;

        // PlayerStatsから現在の最大回数を取得
        int maxUses = 2; // デフォルト値
        PlayerStats stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
        {
            maxUses = stats.maxSlowMotionUses;
        }

        // ★残り回数を計算★
        int remainingUses = maxUses - currentSlowMotionUses;

        // 残り回数と最大回数を表示（例: "SlowMo: 2/3"）
        slowMoUsesText.text = $"SlowMo: {remainingUses}/{maxUses}";

        // 残り回数が0の場合に色を変えるなど、視覚的な調整を加えても良い
        if (remainingUses <= 0)
        {
            slowMoUsesText.color = Color.red;
        }
        else
        {
            slowMoUsesText.color = Color.white;
        }
    }
    // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        DeactivateSlowMotion();
        Time.timeScale = 1f;
        LoadResultScene();
    }

    public void LoadResultScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    public void LoadTitleScene()
    {
        ResetGameManager();
        SceneManager.LoadScene(titleSceneName);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetFinalScore()
    {
        if (UpgradeManager.Instance != null)
        {
            return UpgradeManager.Instance.CurrentScore;
        }
        return 0;
    }

    // --- スローモーション制御 ---

    public void ActivateSlowMotion()
    {
        if (isSlowMotionActive) return;

        Time.timeScale = slowMotionScale;
        isSlowMotionActive = true;
        currentSlowMotionUses++; // 使用済み回数をインクリメント
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        PlayerStats stats = FindObjectOfType<PlayerStats>();
        int maxUses = (stats != null) ? stats.maxSlowMotionUses : 2;
        Debug.Log($"スローモーション開始。残り回数: {maxUses - currentSlowMotionUses}");

        slowMotionCoroutine = StartCoroutine(SlowMotionTimer());
    }

    public void DeactivateSlowMotion()
    {
        if (!isSlowMotionActive) return;

        if (slowMotionCoroutine != null)
        {
            StopCoroutine(slowMotionCoroutine);
        }

        Time.timeScale = 1f;
        isSlowMotionActive = false;
        Time.fixedDeltaTime = 0.02f;
        Debug.Log("スローモーション解除。");
    }

    private IEnumerator SlowMotionTimer()
    {
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        DeactivateSlowMotion();
    }

    // UpgradeManagerから呼ばれ、現在の使用済み回数をリセットするメソッド
    public void ResetCurrentSlowMotionUses()
    {
        currentSlowMotionUses = 0;
    }

    // --- マネージャーのリセット ---

    private void ResetGameManager()
    {
        if (UpgradeManager.Instance != null)
        {
            Destroy(UpgradeManager.Instance.gameObject);
        }

        isGameOver = false;
        isSlowMotionActive = false;
        currentSlowMotionUses = 0;

        Instance = null;
        Destroy(gameObject);
    }
}