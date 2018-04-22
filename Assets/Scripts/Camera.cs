using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    const float _THRESHOLD_ = 0.05f;

    Vector3 targetPosition;
    float speed = 20f;

	// Use this for initialization
	void Start () {
        targetPosition = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
    { 
        Vector3 dir = targetPosition - this.transform.localPosition;

		if (dir.magnitude > _THRESHOLD_)
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, targetPosition, speed * Time.deltaTime);
        }
        else this.transform.localPosition = targetPosition;
    }
}
