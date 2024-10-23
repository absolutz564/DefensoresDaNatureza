using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{

    public GameObject MaleCharacter;
    public GameObject FemaleCharacter;
    public GameObject MaleCharacter2;
    public GameObject FemaleCharacter2;

    private void Awake()
    {
        if (PlayerPrefs.GetString("Sex") == "Male")
        {
            MaleCharacter.SetActive(true);
            if (MaleCharacter2)
            {
                MaleCharacter2.SetActive(true);
            }
        }
        else
        {
            FemaleCharacter.SetActive(true);
            if (FemaleCharacter2)
            {
                FemaleCharacter2.SetActive(true);
            }
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
