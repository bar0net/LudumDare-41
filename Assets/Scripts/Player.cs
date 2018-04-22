using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    const float _DRAG_THRESHOLD_ = 0.05f;

    [Header("Control")]
    public bool enableControl = true;
    public float acceleration = 2.0f;
    public float breaking = 10.0f;
    public float steering = 30.0f;

    public float dynamicLinearDrag = 0.1f;
    public float staticlinearDrag = 0.8f;

    public float lateralDrag = 0.8f;

    [Header("Camera")]
    public Transform cam;

    Rigidbody2D _rb;
    GameManager _gm;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        _rb = this.GetComponent<Rigidbody2D>();
        _gm = FindObjectOfType<GameManager>();

        _gm.UpdateHealthUI(health, currArmour, armour - currArmour);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!enableControl) return;

        Vector2 move = new Vector2(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Update drag coeficient
        if (Mathf.Abs(move.y) > _DRAG_THRESHOLD_) _rb.drag = dynamicLinearDrag;
        else _rb.drag = staticlinearDrag;

        // Apply Steering
        this.transform.Rotate(0, 0, move.x * steering * Time.fixedDeltaTime);

        // Get Velocity in base defined as (0,1) is the car's forward direction
        Vector3 based_speed = this.transform.InverseTransformVector(_rb.velocity);

        // Accelerate if vertical input goes with the movement
        // otherwise break
        if (Mathf.Sign(move.y) == Mathf.Sign(based_speed.y))
            _rb.AddForce( move.y * this.transform.up * acceleration);
        else
            _rb.AddForce(move.y * this.transform.up * breaking);

        // Apply lateral drag (kill lateral velocity)
        based_speed.x *= (1 - lateralDrag);
        _rb.velocity = this.transform.TransformVector(based_speed);
	}

    // Hit effects managed by parent class, but this must trigger
    // an update of the UI
    public override void Hit(int damage, bool healthOnly = false, float x = 0, float y = 0)
    {
        base.Hit(damage, healthOnly, x, y);

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
}
