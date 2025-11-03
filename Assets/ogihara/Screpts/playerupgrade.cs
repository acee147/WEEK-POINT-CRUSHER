using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("現在のステータス")]
    public float currentMoveSpeed = 5.0f;
    public float currentFireRateInterval = 0.5f;
    public int maxSlowMotionUses = 2;

    // ★★★ ワイドショットのフラグと設定を追加 ★★★
    public bool isWideShotEnabled = false; // ワイドショットが有効かどうか
    [Range(1, 5)]
    public int wideShotCount = 1; // 発射する弾の数（初期値は1）
    [Range(0f, 15f)]
    public float spreadAngle = 10f; // 弾が広がる角度
    // ★★★★★★★★★★★★★★★★★★★★★★★★★

    // 連射速度を上げるアップグレード (FireRateIntervalを下げる)
    public void IncreaseFireRate(float amount = 0.05f)
    {
        currentFireRateInterval = Mathf.Max(0.1f, currentFireRateInterval - amount);
        Debug.Log($"連射間隔が {currentFireRateInterval} になりました。");
    }

    // 移動速度を上げるアップグレード
    public void IncreaseMoveSpeed(float amount = 0.5f)
    {
        currentMoveSpeed += amount;
        Debug.Log($"移動速度が {currentMoveSpeed} になりました。");
    }

    // スローモーション回数を増やすアップグレード
    public void IncreaseSlowMotionUses(int amount = 1)
    {
        maxSlowMotionUses += amount;
        Debug.Log($"スローモーション使用回数が {maxSlowMotionUses} になりました。");
    }

    // ★★★ ワイドショットアップグレードの適用メソッドを追加 ★★★
    public void EnableWideShot(int count = 3)
    {
        isWideShotEnabled = true;
        wideShotCount = count; // 1回アップグレードしたら3発発射に設定
        Debug.Log($"ワイドショットが有効化されました。発射数: {wideShotCount}");
    }
}