using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkManager : MonoBehaviour {
    public Button[] perks;
    public Color selectedColor = Color.yellow;
    public Color unselectedColor = Color.white;

    private void Start()
    {
        PaintCards(PlayerPrefs.GetInt("perk_id", 0));
    }
    
    public void SelectPerk(int idx)
    {
        PlayerPrefs.SetInt("perk_id", idx);
        PaintCards(idx);
    }

    void PaintCards (int idx)
    {
        for (int i = 0; i < perks.Length; i++)
        {
            if (i == idx) perks[i].GetComponent<Image>().color = selectedColor;
            else perks[i].GetComponent<Image>().color = unselectedColor;

            if (!Interactacle(i)) perks[i].GetComponent<Button>().interactable = false;
        }
    }

    public void ResetAllPerks()
    {
        PlayerPrefs.SetInt("extra_gold", 0);
        PlayerPrefs.SetInt("extra_time", 0);
        PlayerPrefs.SetInt("boost", 0);
        PlayerPrefs.SetInt("obstacles", 0);
    }

    public void MoreGold()
    {
        ResetAllPerks();
        PlayerPrefs.SetInt("extra_gold", 1);
    }

    public void MoreTime()
    {
        ResetAllPerks();
        PlayerPrefs.SetInt("extra_time", 1);
    }

    public void Boost()
    {
        ResetAllPerks();
        PlayerPrefs.SetInt("boost", 1);
    }

    public void LessObstacles()
    {
        ResetAllPerks();
        PlayerPrefs.SetInt("obstacles", 1);
    }

    // Todo: Un-Hardcode This!
    bool Interactacle(int id)
    {
        if (id == 0) return true;
        else if (id == 1 && PlayerPrefs.GetInt("perk_gold", 0) != 0) return true;
        else if (id == 2 && PlayerPrefs.GetInt("perk_time", 0) != 0) return true;
        else if (id == 3 && PlayerPrefs.GetInt("perk_boost", 0) != 0) return true;
        else if (id == 4 && PlayerPrefs.GetInt("perk_obstacles", 0) != 0) return true;

        return false;
    }
}
