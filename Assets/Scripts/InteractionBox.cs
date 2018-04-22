using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBox : MonoBehaviour {

    public WorldManager.interactions interaction;

    WorldManager _wm;
	// Use this for initialization
	void Start () {
        _wm = FindObjectOfType<WorldManager>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _wm.EnterInteraction(interaction);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _wm.ExitInteraction(interaction);
    }
}
