using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour {
    public int damage = 1;
    public bool destroySelf = false;
    public Transform bucket = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character ch = collision.gameObject.GetComponentInParent<Character>();
        Debug.Log(collision.tag);

        if (ch != null)
        {
            ch.Hit(damage);
            Die();
        }

        if (collision.gameObject.tag == "Road") Die();
    }

    void Die()
    {
        if (destroySelf)
        {
            if (bucket == null) Destroy(this.gameObject);
            else
            {
                this.transform.position = bucket.position;
                this.transform.SetParent(bucket);
                this.gameObject.SetActive(false);
            }
        }

    }
}
