using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public void Load(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ResetAndLoad(string scene)
    {
        PlayerPrefs.DeleteAll();
        Load(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
