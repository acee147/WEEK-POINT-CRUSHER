using UnityEngine;

public class TargetSpot : MonoBehaviour
{
    private MovingWall movingWall; // 親のMovingWallスクリプトへの参照

    [Header("スコア設定")]
    [SerializeField]
    private int scoreValue = 100; // このターゲットを破壊した時に加算されるスコア

    // ターゲットを初期化し、親のMovingWallを設定する
    public void SetMovingWall(MovingWall wall)
    {
        movingWall = wall;
    }

    // 弾が当たったときの処理 (弾にCollider2DとRigidbody2Dが必要)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 弾（Bullet）タグを持つオブジェクトに当たったか確認
        if (other.CompareTag("Bullet"))
        {
            // 弾を破壊
            Destroy(other.gameObject);

            // ★★★ 修正箇所: UpgradeManager.Instance.AddScore() を呼び出す ★★★
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.AddScore(scoreValue);
            }
            // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★

            // ターゲットが破壊されたことを親のMovingWallに伝える
            if (movingWall != null)
            {
                movingWall.TargetDestroyed();
            }

            // このターゲット自身を破壊
            Destroy(gameObject);
        }
    }
}