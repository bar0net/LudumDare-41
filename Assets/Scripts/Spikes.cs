using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {
    [Range(0,1)]
    public float offset = 0.0f;

	// Use this for initialization
	void Start () {
        this.GetComponent<Animator>().Play("spikes", -1, offset);	
	}
}
