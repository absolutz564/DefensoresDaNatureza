using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{

    public GameObject MaleCharacter;
    public GameObject FemaleCharacter;

    private void Awake()
    {
        if (PlayerPrefs.GetString("Sex") == "Male")
        {
            MaleCharacter.SetActive(true);
        }
        else
        {
            FemaleCharacter.SetActive(true);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
