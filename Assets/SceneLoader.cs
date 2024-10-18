using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneIndex(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }

    public void SetCharacter(bool isFemale)
    {
        if (!isFemale)
        {
            PlayerPrefs.SetString("Sex", "Male");
        } else
        {
            PlayerPrefs.SetString("Sex", "Female");
        }
    }
}
