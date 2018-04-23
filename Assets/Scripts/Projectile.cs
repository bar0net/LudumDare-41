using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float lifeSpan = 1.1f;
    public float speed = 3.0f;

    public float timer = 0;

    public Transform bucket = null;
	// Use this for initialization
	void OnEnable () {
        timer = lifeSpan;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position += this.transform.up * speed * Time.deltaTime;
        timer -= Time.deltaTime;

        if (timer <= 0)
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
