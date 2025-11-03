using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private PlayerStats stats; // PlayerStatsから値を取得するための参照

    [Header("発射設定")]
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private float launchSpeed = 10f;

    private float nextFireTime = 0f;
    private Rigidbody2D rb;

    [Header("効果音設定")]
    [SerializeField]
    private AudioClip shootSound;
    private AudioSource audioSource;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // PlayerStatsコンポーネントを取得
        stats = GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats component not found! Please add it to the player.");
        }

        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            return;
        }

        // 発射間隔に PlayerStats の値を使用
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            // ★Statsを参照して連射間隔を決定★
            nextFireTime = Time.time + stats.currentFireRateInterval;
            ShootBall();
            PlayShootSound();
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float moveVertical = Input.GetAxisRaw("Vertical");

        // 移動速度に PlayerStats の値を使用
        float currentSpeed = stats.currentMoveSpeed; // ★Statsを参照★

        // Dash機能
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // ダッシュ倍率 (定数として仮に2.0を使用)
            currentSpeed *= 2.0f;
        }

        rb.linearVelocity = new Vector2(0f, moveVertical * currentSpeed);
    }

    // ★★★ ShootBall() メソッドの完全修正 ★★★
    void ShootBall()
    {
        if (stats == null || ballPrefab == null || firePoint == null)
        {
            Debug.LogError("Missing references in PlayerController.");
            return;
        }

        // ワイドショットが有効でなければ、通常の1発を発射
        if (!stats.isWideShotEnabled || stats.wideShotCount <= 1)
        {
            GameObject newBall = Instantiate(ballPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D ballRb = newBall.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = firePoint.right * launchSpeed;
            }
            Destroy(newBall, 3f);
            return;
        }

        // ワイドショット発射ロジック
        int shotCount = stats.wideShotCount;
        float angle = stats.spreadAngle;

        // 複数の弾を発射するための中心角を計算
        float startAngle = -angle * (shotCount - 1) / 2f;

        for (int i = 0; i < shotCount; i++)
        {
            // 現在の弾の回転角度を計算 (例: 3発、10度拡散なら -10度, 0度, +10度)
            float currentAngle = startAngle + i * angle;

            // 弾の回転をFirePointから計算
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, currentAngle);

            // 弾を生成
            GameObject newBall = Instantiate(ballPrefab, firePoint.position, rotation);
            Rigidbody2D ballRb = newBall.GetComponent<Rigidbody2D>();

            if (ballRb != null)
            {
                // 回転後の前方ベクトルに速度を適用
                Vector3 direction = rotation * Vector3.right;
                ballRb.linearVelocity = direction.normalized * launchSpeed;
            }
            Destroy(newBall, 3f);
        }
    }
    // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★

    void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}