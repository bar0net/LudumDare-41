using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGate : MonoBehaviour {

    public float timeToAdd = 5.0f;
    public GameObject nextGate;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject.FindObjectOfType<GameManager>().AddTime(timeToAdd);
            nextGate.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
