using UnityEngine;

public class Bullet : MonoBehaviour
{

    //‹…‚ª•Ç‚É“–‚½‚Á‚½‚çÁ‚¦‚é
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
