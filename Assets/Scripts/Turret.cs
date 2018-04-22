using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    public Transform[] muffles;
    public Transform bucket;

    public float shootDelay = 1.1f;

    public float steering = 45.0f;
    public float anglePerStep = 45.0f;

    float timer = 0;
    float angleCount = 0;
    bool rotate = true;
	// Use this for initialization
	void Start () {
        timer = shootDelay;
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;

        float angle = steering * Time.deltaTime;

        if (rotate)
        {
            if (angleCount + angle > anglePerStep)
            {
                angle = anglePerStep - angleCount;
                rotate = false;
                angleCount = 0;
            }
            else angleCount += angle;

            this.transform.Rotate(0, 0, angle);
        }


        if (timer < 0)
        {
            Shoot();
            timer = shootDelay;
            rotate = true;
        }
	}

    void Shoot()
    {
        // Check if we are running out of projectile instances in the bucket
        // and create new ones as needed (we need, at least "number of muffles + 1"
        // so we have a spare example for other iterations if needed)
        int count = muffles.Length - bucket.childCount;
        while (count >= -1)
        {
            GameObject go = (GameObject)Instantiate(bucket.GetChild(0).gameObject, bucket.position, bucket.rotation, bucket);
            count--;
        }

        for (int i = 0; i < muffles.Length; i++)
        {
            Transform t = bucket.GetChild(0);

            t.position = muffles[i].position;
            t.rotation = muffles[i].rotation;
            t.SetParent(null);
            t.gameObject.SetActive(true);
        }
    }
}
