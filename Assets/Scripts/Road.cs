using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour {
    public int damage = 1;
    public GameObject[] hazards;
    public float chanceSafe = 0.1f;

    private void Start()
    {
        if (Random.value > chanceSafe && hazards.Length > 0)
        {
            int idx = Random.Range(0, hazards.Length);

            GameObject go = (GameObject)Instantiate(hazards[idx], this.transform.position, this.transform.rotation, this.transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 dir = (collision.contacts[0].point - (Vector2)collision.gameObject.transform.position);
            collision.gameObject.GetComponent<Player>().Hit(damage, true, dir.x, dir.y);
        }
    }
}
