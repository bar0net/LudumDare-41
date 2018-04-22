using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {

    public const KeyCode _EXIT_KEY_ = KeyCode.E;
    public const KeyCode _INTERACT_ = KeyCode.W;

    [System.Serializable]
    public class Cars
    {
        public string name;
        public Sprite image;
        public Sprite addon;
        public int health;
        public int armour;
        public int accel;
        public int steering;
        public int brakes;
        public int size;
    }

    public enum interactions { None = 0, Track = 1, Box = 2, Custom = 3 };
    public enum carTypes { Car = 0, Moto = 1, Truck = 2, F1 = 3};

    [Header("Tracks")]
    public List<string> trackNames;

    [Header("Vehicles")]
    public List<string> carColors;
    public Cars[] cars;

    [Header("UI")]
    public GameObject vehiclePanel;
    public Text vehicleText;
    public Text continueText;

    [Space(5)]
    public GameObject currentCarPanel;
    public Text currentCarName;
    public Image currentCarImage;
    public Image currentCarAddon;
    public Transform currentCarStats;

    [Space(5)]
    public GameObject interactionBubble;
    public Text textMoney;

    interactions inter = interactions.None;
    GameObject openPanel = null;
    // Use this for initialization
    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    void Start ()
    {
        continueText.text = "Press [" + _EXIT_KEY_.ToString() + "] to continue";
        textMoney.text = "$ " + PlayerPrefs.GetInt("money", 0).ToString();

        // Generate new car if necessary
        if (PlayerPrefs.GetInt("new_car", -1) == -1)
        {
            carTypes car = GetNewCar();

            Time.timeScale = 0;
            vehiclePanel.SetActive(true);
            vehicleText.text = car.ToString();
            openPanel = vehiclePanel;

            PlayerPrefs.SetInt("new_car", (int)car);
            int idx = Random.Range(0, carColors.Count);
            PlayerPrefs.SetString("color", carColors[idx]);
        }
        else UpdateCurrentCarPanel();

        interactionBubble.SetActive(false);
        FindObjectOfType<WorldCharacter>().SetPosition(new Vector3(PlayerPrefs.GetFloat("world_x", -4), -3, 0) );
        
    }
	
	// Update is called once per frame
	void Update () {
		if (openPanel != null && Input.GetKeyDown(_EXIT_KEY_))
        {
            Time.timeScale = 1;
            openPanel.SetActive(false);
            openPanel = null;
            UpdateCurrentCarPanel();
        }

        if (inter != interactions.None && Input.GetKeyDown(_INTERACT_) && openPanel == null) Interact();
	}

    public void EnterInteraction(interactions interaction)
    {
        inter = interaction;
        interactionBubble.SetActive(true);
    }

    public void ExitInteraction(interactions interaction)
    {
        if (inter == interaction) inter = interactions.None;
        interactionBubble.SetActive(false);
    }

    public void Interact()
    {        switch (inter)
        {
            case interactions.None:
                break;

            case interactions.Track:
                Cars car = cars[PlayerPrefs.GetInt("new_car", 0)];

                // Reset spawning position
                PlayerPrefs.DeleteKey("world_x");

                // Delete new_car. Next time we get back to this screen
                // we need to generate a new pick
                PlayerPrefs.SetInt("car", PlayerPrefs.GetInt("new_car", 0));
                PlayerPrefs.SetInt("new_car", -1);

                // Set all parameters for racing
                PlayerPrefs.SetInt("health", car.health + PlayerPrefs.GetInt("box_health", 0));
                PlayerPrefs.SetInt("armour", car.armour + PlayerPrefs.GetInt("box_armour", 0));
                PlayerPrefs.SetInt("accel", car.accel + PlayerPrefs.GetInt("box_accel", 0));
                PlayerPrefs.SetInt("brake", car.brakes + PlayerPrefs.GetInt("box_brake", 0));
                PlayerPrefs.SetInt("steer", car.steering + PlayerPrefs.GetInt("box_steer", 0));

                int money = Mathf.FloorToInt(PlayerPrefs.GetInt("box_money") * 0.2f * PlayerPrefs.GetInt("money", 0));
                PlayerPrefs.SetInt("money", money);

                int idx = Random.Range(0, trackNames.Count);
                GetComponent<SceneLoader>().Load(trackNames[idx]);
                break;

            case interactions.Box:
                PlayerPrefs.SetFloat("world_x", GameObject.FindWithTag("Player").transform.position.x);
                GetComponent<SceneLoader>().Load("TechTree");
                break;

            case interactions.Custom:
                Debug.Log("Custom");
                PlayerPrefs.SetFloat("world_x", GameObject.FindWithTag("Player").transform.position.x);
                break;
        }
    }

    carTypes GetNewCar()
    {
        string unlocked = PlayerPrefs.GetString("cars", "1000");
        Debug.Log("Cars: " + unlocked);
        List<int> available = new List<int>();

        for (int i = 0; i < unlocked.Length; i++)
        {
            if (unlocked[i] == '1') available.Add(i);
        }

        return (carTypes)(available[Random.Range(0, available.Count)]);
    }

    void UpdateCurrentCarPanel()
    {
        Cars car = cars[PlayerPrefs.GetInt("new_car",0)];
        string carColor = PlayerPrefs.GetString("color", "#ff0000");

        Color c;
        if (!ColorUtility.TryParseHtmlString(carColor, out c)) c = Color.white;

        currentCarName.text = car.name;
        currentCarImage.sprite = car.image;
        currentCarImage.color = c;
        if (car.addon != null)
        {
            currentCarAddon.color = Color.white;
            currentCarAddon.sprite = car.addon;
        }
        else currentCarAddon.color = new Color(1, 1, 1, 0);

        UpdateCurrentCarStat(
            currentCarStats.GetChild(0).GetChild(1),
            car.health + PlayerPrefs.GetInt("box_health",0),
            Color.red);

        UpdateCurrentCarStat(
            currentCarStats.GetChild(1).GetChild(1),
            car.armour + PlayerPrefs.GetInt("box_armour", 0),
            Color.red);

        UpdateCurrentCarStat(
            currentCarStats.GetChild(2).GetChild(1),
            car.accel + PlayerPrefs.GetInt("box_accel", 0),
            Color.red);

        UpdateCurrentCarStat(
            currentCarStats.GetChild(3).GetChild(1),
            car.steering + PlayerPrefs.GetInt("box_steer", 0),
            Color.red);

        UpdateCurrentCarStat(
            currentCarStats.GetChild(4).GetChild(1),
            car.brakes + PlayerPrefs.GetInt("box_brake", 0),
            Color.red);

        UpdateCurrentCarStat(
            currentCarStats.GetChild(5).GetChild(1),
            car.size,
            Color.red);

        currentCarPanel.SetActive(true);
    }

    void UpdateCurrentCarStat(Transform bars, int value, Color color)
    {
        for (int i = 0; i < bars.childCount; i++)
        {
            if (i < value) bars.GetChild(i).GetComponent<Image>().color = color;
            else bars.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }
}
