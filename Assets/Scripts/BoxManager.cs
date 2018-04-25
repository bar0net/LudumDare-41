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

        PlayerPrefs.SetInt("perk_gold", 0);
        PlayerPrefs.SetInt("perk_time", 0);
        PlayerPrefs.SetInt("perk_boost", 0);
        PlayerPrefs.SetInt("perk_obstacles", 0);

        initialCost = 4; // TODO: Un-hardcode this
        UpdateCostUI();
        UpdateTechUI();
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
        // if (money < initialCost) return;

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
        RecursiveFill(0, true);
    }

    void RecursiveFill(int id, bool prevEnabled)
    {
        if (techs[id] == '1')
        {
            PaintOwnedTech(id);
            FillNext(id, true);
            return;
        }

        if (prevEnabled) PaintAllowedTech(id);
        else PaintDisabledTech(id);
        FillNext(id, false);
    }

    void FillNext(int id, bool prevEnabled)
    {
            if (linkDict.ContainsKey(id))
            {
                for (int i = 0; i < linkDict[id].Count; i++) RecursiveFill(linkDict[id][i], prevEnabled);
            }
    }

    void PaintDisabledTech(int id)
    {
        buttons[id].GetComponent<Image>().color = lockedColor;
        buttons[id].GetComponent<Button>().enabled = true;
        buttons[id].GetComponent<Button>().interactable = false;
    }

    void PaintOwnedTech(int id)
    {
        buttons[id].GetComponent<Image>().color = unlockedColor;
        buttons[id].GetComponent<Button>().enabled = false;
    }

    void PaintAllowedTech(int id)
    {
        if (money < initialCost)
        {
            buttons[id].GetComponent<Image>().color = unaffordableColor;
            buttons[id].GetComponent<Button>().enabled = false;
        }
        else
        {
            buttons[id].GetComponent<Image>().color = lockedColor;
            buttons[id].GetComponent<Button>().enabled = true;
            buttons[id].GetComponent<Button>().interactable = true;
        }
    }

    public void MoreMoney()
    {
        money += 10;
        UpdateCostUI();
        RecursiveFill(0, true);
    }
}
