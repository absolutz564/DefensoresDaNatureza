using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sprite muteSprite;          // Sprite de mute
    public Sprite unmuteSprite;        // Sprite de unmute
    private bool isMuted = false;      // Estado de mute
    public AudioSource[] allAudioSources;
    private Button muteButton;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Buscar todos os AudioSources na cena, incluindo objetos desativados
        allAudioSources = FindAllAudioSources();

        // Procurar pelo botão "ButtonMute" na cena
        muteButton = GameObject.Find("ButtonMute")?.GetComponent<Button>();

        // Verifica se o botão foi encontrado
        if (muteButton != null)
        {
            // Atualizar o sprite inicial do botão e o estado de mute
            UpdateAudioSources();
            UpdateButtonSprite();

            // Adicionar evento ao botão
            muteButton.onClick.RemoveAllListeners(); // Remove todos os listeners existentes
            muteButton.onClick.AddListener(ToggleMute);
        }
        else
        {
            Debug.LogWarning("ButtonMute não foi encontrado na cena.");
        }
    }

    // Alterna o estado de mute
    private void ToggleMute()
    {
        Debug.Log("Toggle mute");
        isMuted = !isMuted; // Inverte o estado de mute
        UpdateAudioSources(); // Atualiza todos os audios da cena
        UpdateButtonSprite(); // Atualiza o sprite do botão
    }

    // Atualiza todos os AudioSources na cena para o estado atual (mute/unmute)
    private void UpdateAudioSources()
    {
        // Criar uma nova lista temporária para armazenar AudioSources válidos
        List<AudioSource> validAudioSources = new List<AudioSource>();

        // Filtrar apenas os AudioSources que ainda são válidos
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null) // Verifica se o AudioSource não é nulo
            {
                validAudioSources.Add(audioSource);
            }
        }

        // Atualiza os AudioSources válidos para o estado atual (mute/unmute)
        foreach (AudioSource audioSource in validAudioSources)
        {
            audioSource.mute = isMuted;
        }
    }


    // Atualiza o sprite do botão com base no estado atual (mute/unmute)
    private void UpdateButtonSprite()
    {
        if (muteButton != null)
        {
            if (isMuted)
            {
                muteButton.GetComponent<Image>().sprite = muteSprite;
            }
            else
            {
                muteButton.GetComponent<Image>().sprite = unmuteSprite;
            }
        }
    }

    // Este método será chamado sempre que uma nova cena for carregada
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Este método será chamado sempre que uma nova cena for carregada
    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Quando a cena é carregada, procure o novo botão "ButtonMute"
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Atualiza a referência do botão para o "ButtonMute" da nova cena
        muteButton = GameObject.Find("ButtonMute")?.GetComponent<Button>();

        // Verifica se o botão foi encontrado na nova cena
        if (muteButton != null)
        {
            // Atualiza o sprite e associa o listener de evento
            UpdateButtonSprite();

            // Remove listeners antigos para evitar duplicidade
            muteButton.onClick.RemoveAllListeners();
            muteButton.onClick.AddListener(ToggleMute);
        }
        else
        {
            Debug.LogWarning("ButtonMute não foi encontrado na cena.");
        }

        // Atualizar os AudioSources na nova cena, incluindo objetos desativados
        allAudioSources = FindAllAudioSources();
        UpdateAudioSources();
    }

    // Função para encontrar todos os AudioSources, incluindo em objetos desativados
    private AudioSource[] FindAllAudioSources()
    {
        return Resources.FindObjectsOfTypeAll<AudioSource>();
    }
}
