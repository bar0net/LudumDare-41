using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [Header("Game Params.")]
    public float timer = 20.0f;
    public GameObject[] timeGates;
    public GameObject[] playerModels;

    [Header("UI Elements")]
    public GameObject gameoverPanel;
    public Text gameoverTitle;
    public Text gameOverGates;
    public Text gameOverMoney;
    public Text gameOverTime;

    [Space(10)]
    public Text moneyText;
    public Text timerText;

    [Space(10)]
    public Image[] healthBars;
    public Color healthColor = Color.red;
    public Color armourColor = Color.cyan;
    public Color brokenArmourColor = Color.gray;

    [Header("Audio")]
    public AudioClip deathAudio;

    int money = 0;
    int extra_money = 0;
    int gate_count = 0;
    bool isGameOver = false;

	// Use this for initialization
	void Start () {
        gameoverPanel.SetActive(false);

        money = PlayerPrefs.GetInt("money", 0);
        moneyText.text = "$ " + money.ToString();
        extra_money = PlayerPrefs.GetInt("extra_money", 0);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = players.Length; i > 0; i--) Destroy(players[i - 1]);
        {

        }

        // Define a time gate as spawning point
        int gate = Random.Range(0, timeGates.Length);

        // Load car at spawning point
        int car_id = PlayerPrefs.GetInt("car", 0);

        if (car_id >= playerModels.Length || car_id < 0)
        {
            Debug.LogWarning("car_id out of range [" + car_id.ToString() + "]");
            car_id = 0;
        }
        if (gate >= timeGates.Length || gate < 0)
        {
            Debug.LogWarning("gate out of range [" + gate.ToString() + "]");
            gate = 0;
        }

        GameObject go = (GameObject)Instantiate(
            playerModels[car_id],
            timeGates[gate].transform.position,
            timeGates[gate].transform.rotation,
            null);

        // Set Player stats
        Player p = go.GetComponent<Player>();
        p.health = PlayerPrefs.GetInt("health",2);
        p.armour = PlayerPrefs.GetInt("armour", 1);
        p.acceleration = Mathf.Lerp(2, 4, PlayerPrefs.GetInt("accel", 2) / 5.0f);
        p.brakes = Mathf.Lerp(7, 12, PlayerPrefs.GetInt("brake", 2) / 5.0f);
        p.steering = Mathf.Lerp(30, 60, PlayerPrefs.GetInt("steer", 2) / 5.0f);
        p.SetCarColor(PlayerPrefs.GetString("color", "#ff0000"));

        // activate next gate
        gate = (gate + 1) % timeGates.Length;
        timeGates[gate].SetActive(true);
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(WorldManager._EXIT_KEY_))
        {
            Time.timeScale = 1.0f;
            GetComponent<SceneLoader>().Load("Overworld");
        }
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
        AudioSource _as = GetComponent<AudioSource>();
        _as.Stop();
        _as.PlayOneShot(deathAudio);
        _as.loop = false;

        gameoverTitle.text = "Totaled!";
        gameOverMoney.text = "Money earned: $" + money.ToString();
        gameOverGates.text = "Gates crossed: " + gate_count.ToString();
        gameOverTime.text = "Time left: " + Mathf.Max(0, timer).ToString("F2") + "s";
        gameoverPanel.SetActive(true);

        isGameOver = true;

        Time.timeScale = 0;
    }

    // Extend the remaining time
    public void AddTime(float time)
    {
        timer += time;
        gate_count++;
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

    public void AddMoney(int value)
    {
        money += value + extra_money;
        moneyText.text = "$ " + money.ToString();
        PlayerPrefs.SetInt("money", money);
    }
}
