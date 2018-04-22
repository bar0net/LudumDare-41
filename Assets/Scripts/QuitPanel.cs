using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPanel : MonoBehaviour {
    public GameObject quitPanel;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && !quitPanel.activeSelf) quitPanel.SetActive(true);
        else if (Input.GetKeyDown(KeyCode.Escape) && quitPanel.activeSelf) Application.Quit();
        else if (Input.GetKeyDown(KeyCode.E) && quitPanel.activeSelf) quitPanel.SetActive(false);
    }
}
