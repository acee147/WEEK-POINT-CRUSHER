using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーが壁に触れた場合の処理（必要に応じて実装）
        if (collision.gameObject.CompareTag("Player"))
        {
         if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
        //Bulletとの衝突処理（必要に応じて実装）
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            // 弾が壁に当たった場合の処理
            Destroy(collision.gameObject); // 例: 弾を破壊する
        }

    }
}
