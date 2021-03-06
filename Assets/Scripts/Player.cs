﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character {
    const float _DRAG_THRESHOLD_ = 0.05f;

    const float _MIN_SPEED_RANGE_ = 7.5f;
    const float _MAX_SPEED_RANGE_ = 15.0f;

    [Header("Perks Params")]
    public float boostDelay = 4.0f;

    [Header("Control")]
    public bool enableControl = true;
    public float acceleration = 2.0f;
    public float brakes = 10.0f;
    public float steering = 30.0f;

    public float dynamicLinearDrag = 0.1f;
    public float staticlinearDrag = 0.8f;

    public float lateralDrag = 0.8f;

    [Header("Camera")]
    public Transform cam;

    [Header("Audio")]
    public AudioSource motorAudio;
    public AudioSource impactAudio;

    Rigidbody2D _rb;
    GameManager _gm;

    bool audioPlaying = false;

    bool canBoost = true;
    float boostTimer = 0;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        canBoost = (PlayerPrefs.GetInt("boost", 0) == 1);

        _rb = this.GetComponent<Rigidbody2D>();
        _gm = FindObjectOfType<GameManager>();

        _gm.UpdateHealthUI(health, currArmour, armour - currArmour);
	}

    // Update is called once per frame
    private void LateUpdate()
    {
        if (canBoost)
        {
            if (boostTimer > 0) boostTimer -= Time.deltaTime;
            else if (boostTimer < 0) boostTimer = 0;

            _gm.UpdateBoostUI((boostDelay - boostTimer) / boostDelay);
        }
    }

    void FixedUpdate () {
        if (!enableControl || health <= 0) return;

        Vector2 move = new Vector2(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Apply Steering
        this.transform.Rotate(0, 0, move.x * steering * Time.fixedDeltaTime);

        // Get Velocity in base defined as (0,1) is the car's forward direction
        Vector3 based_speed = this.transform.InverseTransformVector(_rb.velocity);

        // Boost
        if (Input.GetKeyDown(KeyCode.Space) && canBoost && boostTimer <= 0)
        {
            _rb.AddForce(this.transform.up * 400);
            boostTimer = boostDelay;
        }

        // Update drag coeficient
        float speedRatio = (based_speed.y - _MIN_SPEED_RANGE_) / (_MAX_SPEED_RANGE_ - _MIN_SPEED_RANGE_);
        if (Mathf.Abs(move.y) > _DRAG_THRESHOLD_) _rb.drag = Mathf.Lerp(dynamicLinearDrag, 0.95f, speedRatio);
        else _rb.drag = staticlinearDrag;


        // Accelerate if vertical input goes with the movement
        // otherwise brake
        if (Mathf.Sign(move.y) == Mathf.Sign(based_speed.y)) _rb.AddForce( move.y * this.transform.up * acceleration);
        else _rb.AddForce(move.y * this.transform.up * brakes);

        // audio management
        if (Mathf.Sign(move.y) == Mathf.Sign(based_speed.y) && Mathf.Abs(based_speed.y) > 0.2 && !audioPlaying)
        {
            audioPlaying = true;
            motorAudio.Play();
        }
        else if (Mathf.Abs(based_speed.y) <= 0.8f && audioPlaying)
        {
            audioPlaying = false;
            motorAudio.Pause();
        }
        
        if (audioPlaying)
        {
            float ratio = Mathf.Abs(based_speed.y) / 20;
            motorAudio.volume = Mathf.Lerp(0.1f, 0.3f, ratio);
            motorAudio.pitch = Mathf.Lerp(-0.1f, 1.5f, ratio);
        }

        // Apply lateral drag (kill lateral velocity)
            based_speed.x *= (1 - lateralDrag);
        _rb.velocity = this.transform.TransformVector(based_speed);
	}

    // Hit effects managed by parent class, but this must trigger
    // an update of the UI
    public override void Hit(int damage, bool healthOnly = false, float x = 0, float y = 0)
    {
        base.Hit(damage, healthOnly, x, y);

        impactAudio.pitch = Random.Range(0.90f, 1.10f);
        impactAudio.Play();

        _gm.UpdateHealthUI(health, currArmour, armour - currArmour);

        Vector2 shake = new Vector2(x, y);
        if (x == 0 && y == 0) shake = Random.insideUnitCircle;
        else shake = shake.normalized;

        cam.localPosition += new Vector3(shake.x,shake.y,0) * Random.Range(0.1f, 0.3f);
    }

    // The Player doesn't have any health left. Trigger Game Over.
    protected override void Die()
    {
        _gm.UpdateHealthUI(health, currArmour, armour - currArmour);
        _gm.GameOver();
    }

    public void SetCarColor(string color)
    {
        Color c;
        SpriteRenderer _sr = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        ColorUtility.TryParseHtmlString(color, out c);

        if (c != null) _sr.color = c;
        else _sr.color = new Color(0, 0, 0, 1);
    }
}
