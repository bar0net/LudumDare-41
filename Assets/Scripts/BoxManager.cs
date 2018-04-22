using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BoxManager : MonoBehaviour {

    public Color lockedColor = Color.white;
    public Color unlockedColor = Color.yellow;
    public Color unaffordableColor = Color.red;
    public GameObject[] buttons;
    public int initialCost = 4;

    [Header("UI")]
    public Text textMoney;
    public Text textCost;

    string techs;
    public Vector2[] links;
    Dictionary<int, List<int>> linkDict = new Dictionary<int, List<int>>();
    int money = 0;

    private void Start()
    {
        techs = PlayerPrefs.GetString("techs", "");
        money = PlayerPrefs.GetInt("money", 0);

        // Set a default activation if there is none recorded
        if (techs.Length == 0) ResetTechs();

        // ToExpand: Manage different lengths between num buttorns and recorded activations
        if (techs.Length < buttons.Length)
        {
            for (int i = techs.Length; i < buttons.Length; i++) techs += '0';
        }
        else if (techs.Length > buttons.Length) ResetTechs();

        // Load Links into dictionary for convenience
        for (int i = 0; i < links.Length; i++)
        {
            int key = (int)links[i].x;
            if (!linkDict.ContainsKey(key)) linkDict.Add(key, new List<int>());

            linkDict[key].Add((int)links[i].y);
        }

        // Update UI and functionalities with previously unlocked techs
        UpdateTechUI();

        // Update the money and cost panels
        UpdateCostUI();
    }

    void SetDefaults()
    {
            techs = "1";
            for (int i = 1; i < buttons.Length; i++) techs += "0";
            PlayerPrefs.SetString("techs", techs);
    }

    void UpdateUnlockedTech(int id)
    {
        buttons[id].GetComponent<Image>().color = unlockedColor;
        buttons[id].GetComponent<Button>().enabled = false;

        if (!linkDict.ContainsKey(id)) return;

        for (int j = 0; j < linkDict[id].Count; j++)
        {
            int idx = linkDict[id][j];
            buttons[idx].GetComponent<Button>().interactable = true;

            if (money < initialCost)
            {
                buttons[idx].GetComponent<Image>().color = unaffordableColor;
                buttons[idx].GetComponent<Button>().enabled = false;
            }
            else buttons[idx].GetComponent<Image>().color = lockedColor;
        }
    }

    public void UnlockTech(int id)
    {
        // Update techs string and save data
        techs = StringReplace(techs, id, '1');
        PlayerPrefs.SetString("techs", techs);

        money -= initialCost; 
        initialCost++;
        PlayerPrefs.SetInt("money", money);
        UpdateCostUI();
        UpdateTechUI();
    }

    public void ResetTechs()
    {
        SetDefaults();
        PlayerPrefs.SetString("cars", "1000");

        PlayerPrefs.SetInt("box_health", 0);
        PlayerPrefs.SetInt("box_armour", 0);
        PlayerPrefs.SetInt("box_accel", 0);
        PlayerPrefs.SetInt("box_steer", 0);
        PlayerPrefs.SetInt("box_brake", 0);
        PlayerPrefs.SetInt("box_money", 0);
    }

    public void UnlockCar(int car)
    {
        string unlocked = PlayerPrefs.GetString("cars", "1000");
        unlocked = StringReplace(unlocked, (int)car, '1');
        PlayerPrefs.SetString("cars", unlocked);
    }

    string StringReplace(string s, int index, char value)
    {
        string output = "";

        for (int i = 0; i < s.Length; i++)
        {
            if (i == index) output += value;
            else output += s[i];
        }
        return output;
    }

    public void IncreaseStat(string stat)
    {
        if (money < initialCost) return;

        int value = PlayerPrefs.GetInt(stat, 0);
        PlayerPrefs.SetInt(stat, value + 1);
    }

    void UpdateCostUI ()
    {
        textMoney.text = "$ " + money.ToString();
        textCost.text = "Cost of new tech: $" + initialCost.ToString();
    }

    void UpdateTechUI()
    {
        for (int i = 0; i < techs.Length; i++)
        {
            if (techs[i] == '1')
            {
                UpdateUnlockedTech(i);
                initialCost++;
            }
            else
            {
                if (money < initialCost && !buttons[i].GetComponent<Button>().enabled)
                    buttons[i].GetComponent<Image>().color = unaffordableColor;
                else
                    buttons[i].GetComponent<Image>().color = lockedColor;
            }
        }
    }
}
