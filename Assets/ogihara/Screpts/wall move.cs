using UnityEngine;
using UnityEngine.SceneManagement;
public class MovingWall : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField]
    private float initialMoveSpeed = 2f; // 初期移動速度
    private float currentMoveSpeed;       // 現在の移動速度
    [SerializeField]
    private float speedIncreaseRate = 0.5f; // ターゲット破壊（リスポーン）ごとに増える速度

    [Header("リスポーン設定")]
    [SerializeField]
    private float respawnXPosition = 15f; // 壁がリスポーンするX座標 (画面右端など)
    [SerializeField]
    private float destructionThresholdX = -10f; // 壁が画面外と判断するX座標 (画面左端など)

    [Header("ターゲット設定")]
    [SerializeField]
    private GameObject targetSpotPrefab;
    [SerializeField]
    private int numberOfSpots = 3;       // 生成するターゲットの総数
    [SerializeField]
    private float spawnYRange = 4f;      // Y軸の生成範囲（例: -4から4）
    [SerializeField]
    private float targetLocalXPosition = 0f; // 壁のローカル座標におけるターゲットのX位置

    private int activeTargetCount; // 現在残っているターゲットの数


    void Start()
    {
        // 初期設定
        currentMoveSpeed = initialMoveSpeed;

        // 最初のターゲットを生成
        SpawnTargets();
    }

    void Update()
    {
        // ゲームオーバー状態でないかチェック
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            return; // ゲームオーバーなら移動しない
        }

        // 壁を左（プレイヤーの方向）に移動させる
        transform.Translate(Vector2.left * currentMoveSpeed * Time.deltaTime);

        // 壁が画面外に出たらリスポーンさせる
        if (transform.position.x < destructionThresholdX)
        {
           SceneManager.LoadScene("ResultScene");
        }
    }

    // プレイヤーとの衝突検出 (壁の見た目の子オブジェクトにCollider2Dをアタッチすること)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーに"Player"タグが設定されていることを前提
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
            {
                GameManager.Instance.GameOver();
            }
        }
    }


    // ターゲットが破壊されたときに呼び出されるメソッド
    public void TargetDestroyed()
    {
        // ターゲットが1つ壊れた瞬間に速度を加速
        currentMoveSpeed += speedIncreaseRate;

        // 壁全体をリスポーンさせる
        Respawn();
    }

    // 壁をリスポーンさせる処理
    void Respawn()
    {
        // 現在の位置をリスポーン位置に移動
        // Y位置は変えない
        transform.position = new Vector2(respawnXPosition, transform.position.y);

        // 古いターゲットをすべて削除
        foreach (Transform child in transform)
        {
            // TargetSpotを持つ子オブジェクトのみを削除
            if (child.GetComponent<TargetSpot>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // 新しいターゲットを生成
        SpawnTargets();
    }

    // ターゲットをランダムな位置に生成する
    void SpawnTargets()
    {
        activeTargetCount = 0;

        // 設定された数のターゲットを生成
        for (int i = 0; i < numberOfSpots; i++)
        {
            // Y座標をランダムに決定
            float randomY = Random.Range(-spawnYRange, spawnYRange);

            // X座標は設定されたローカルX位置を使用
            Vector3 spawnPosition = new Vector3(targetLocalXPosition, randomY, 0);

            // ターゲットを生成し、この壁の子オブジェクトにする
            GameObject newTarget = Instantiate(targetSpotPrefab, transform);
            newTarget.transform.localPosition = spawnPosition;

            // TargetSpotスクリプトに親のMovingWallを設定
            TargetSpot spot = newTarget.GetComponent<TargetSpot>();
            if (spot != null)
            {
                spot.SetMovingWall(this);
                activeTargetCount++; // 実際に生成できたターゲットの数をカウント
            }
            else
            {
                Debug.LogWarning("TargetSpot component not found on the prefab!");
            }
        }
    }
}


