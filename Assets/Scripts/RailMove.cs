﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMove : MonoBehaviour {
    public float speed = 4.0f;
    public float waitingTime = 1.5f;

    public Transform[] checkpoints;

    int index = 0;
    bool reached = false;
    float timer = 0;

    AudioSource _audio;
    bool playing = false;
    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
        if (timer > 0)
        {
            if (playing)
            {
                _audio.Pause();
                playing = false;
            }

            timer -= Time.deltaTime;

            if (timer <= 0) index = (index + 1) % checkpoints.Length;
        }
        else
        {
            if (!playing)
            {
                _audio.time = 0;
                _audio.Play();
                playing = true;
            }

            Vector3 dir = (checkpoints[index].position - this.transform.position);
            float value = speed * Time.deltaTime;

            if (dir.magnitude <= value)
            {
                this.transform.position = checkpoints[index].position;
                timer = waitingTime;
            }
            else this.transform.position += dir.normalized * value;
            
        }
	}
}
