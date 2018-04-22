using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour {
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 dir = (collision.contacts[0].point - (Vector2)collision.gameObject.transform.position);
            collision.gameObject.GetComponent<Player>().Hit(damage, true, dir.x, dir.y);
        }
    }
}
