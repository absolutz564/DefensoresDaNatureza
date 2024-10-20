using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VRGameManager : MonoBehaviour
{
    public int score = 0; // Inicializa o score com 0
    public int victoryScore = 0;

    public TextMeshProUGUI organicText;
    public TextMeshProUGUI glassText;
    public TextMeshProUGUI paperText;
    public TextMeshProUGUI plasticText;
    public int organicCount;
    public int glassCount;
    public int paperCount;
    public int plasticCount;

    public GameObject Victory;
    public GameObject GameOver;

    public TextMeshProUGUI timerText; // Texto para exibir o tempo
    private float timer = 0f; // Vari�vel que armazenar� o tempo decorrido
    public float gameDuration = 120f; // Tempo limite do jogo (2 minutos)

    public int totalTrashCount; // Contador total de lixo coletado
    public TextMeshProUGUI victoryScoreText;
    void Start()
    {
        timer = gameDuration; // Inicializa o timer com o tempo total de jogo
        Victory.SetActive(false); // Certifica-se de que Victory n�o est� ativo no in�cio
        GameOver.SetActive(false); // Certifica-se de que GameOver n�o est� ativo no in�cio
        UpdateTimerText(); // Atualiza o texto do timer no in�cio
    }

    void Update()
    {
        // Atualiza o timer a cada frame
        if (Victory.activeSelf == false && GameOver.activeSelf == false)
        {
            timer -= Time.deltaTime; // Reduz o tempo com base no tempo real
            UpdateTimerText(); // Atualiza o texto do timer

            if (timer <= 0f)
            {
                GameOver.SetActive(true); // Exibe o popup de GameOver
            }

            // Verifica se o jogador coletou 16 lixos
            totalTrashCount = organicCount + glassCount + paperCount + plasticCount;
            if (totalTrashCount >= 16)
            {
                Victory.SetActive(true); // Exibe o popup de Victory
            }
        }
    }

    // Fun��o para atualizar o contador de lixo
    public void UpdateCountByTag(string tag)
    {
        if (tag == "LixoOrganico")
        {
            organicCount++;
            organicText.text = organicCount.ToString() + "/4";
        }
        else if (tag == "LixoVidro")
        {
            glassCount++;
            glassText.text = glassCount.ToString() + "/4";
        }
        else if (tag == "LixoPapel")
        {
            paperCount++;
            paperText.text = paperCount.ToString() + "/4";
        }
        else if (tag == "LixoPlastico")
        {
            plasticCount++;
            plasticText.text = plasticCount.ToString() + "/4";
        }
    }

    // Fun��o para atualizar o texto do timer no formato MM:SS
    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60); // Converte segundos para minutos
        int seconds = Mathf.FloorToInt(timer % 60); // Obt�m os segundos restantes

        // Atualiza o texto do Timer no formato MM:SS
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
