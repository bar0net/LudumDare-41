using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [Header("Game Params.")]
    public float timer = 20.0f;
    public GameObject initialTimeGate;

    [Header("UI Elements")]
    public Text timerText;
    public Image[] healthBars;
    public Color healthColor = Color.red;
    public Color armourColor = Color.cyan;
    public Color brokenArmourColor = Color.gray;

	// Use this for initialization
	void Start () {
        if (initialTimeGate == null) Debug.LogError("Initial time gate not defined.");
        initialTimeGate.SetActive(true);
	}
	
	void LateUpdate () {
        // Update game timer
        timer -= Time.deltaTime;
        timerText.text = Mathf.Max(0, timer).ToString("F2");

        if (timer <= 0) GameOver();
	}

    // Trigger Game Over State
    public void GameOver()
    {
        Debug.Log("Die");
        Time.timeScale = 0;
        this.enabled = false;
    }

    // Extend the remaining time
    public void AddTime(float time)
    {
        timer += time;
    }

    // Update the UI elements related to health
    public void UpdateHealthUI(int health, int armour, int brokenArmour = 0)
    {
        int initial = 0;

        // Update visible markers
        initial = UpdateHealthSegment(initial, health, healthColor, true);
        initial = UpdateHealthSegment(initial, armour, armourColor, true);
        initial = UpdateHealthSegment(initial, brokenArmour, brokenArmourColor, true);

        // Hide unnecessary markers
        for (int i = initial; i < healthBars.Length; ++i) healthBars[i].enabled = false;
    }

    // Update a single health segment type in the UI
    // return the initial value for the next loop (for convenience)
    int UpdateHealthSegment(int initial, int end, Color color, bool enable = true)
    {
        for (int i = initial; i < initial + end; i++)
        {
            if (i >= healthBars.Length) break;
            healthBars[i].enabled = enable;
            healthBars[i].color = color;
        }

        return initial + end;
    }
}
