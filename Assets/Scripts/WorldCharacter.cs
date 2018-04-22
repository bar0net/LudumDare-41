using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacter : MonoBehaviour {
    const float _MOVE_THRESHOLD_ = 0.01f;

    public float speed = 3.0f;

    public Transform left;
    public Transform right;

    public Transform cam;
    public int camOffset = -3;

    Animator _anim;
    bool inBoundary = false;
	// Use this for initialization
	void Start () {
        _anim = GetComponent<Animator>();
        cam.position = new Vector3(this.transform.position.x + camOffset, cam.position.y, cam.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        Move(Input.GetAxis("Horizontal"));
	}

    private void LateUpdate()
    {
        // Ensure character is within boundaries
        if (this.transform.position.x < left.position.x)
        {
            this.transform.position = new Vector3(left.position.x, this.transform.position.y, this.transform.position.z);
            inBoundary = true;
        }
        else if (this.transform.position.x > right.position.x)
        {
            this.transform.position = new Vector3(right.position.x, this.transform.position.y, this.transform.position.z);
            inBoundary = true;
        }
        else inBoundary = false;
    }

    private void Move(float move)
    {
        if (Mathf.Abs(move) < _MOVE_THRESHOLD_)
        {
            _anim.speed = 0;
            return;
        }

        if (inBoundary) _anim.speed = 0;
        else _anim.speed = Mathf.Abs(move);
        this.transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        this.transform.position += Vector3.right * speed * Time.deltaTime * move;
        cam.position = new Vector3(this.transform.position.x + camOffset, cam.position.y, cam.position.z);
    }

    public void SetPosition(Vector3 pos)
    {
        this.transform.position = pos;
        cam.position = new Vector3(pos.x + camOffset, cam.position.y, cam.position.z);
    }
}
