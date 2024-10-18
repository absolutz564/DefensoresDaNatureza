using UnityEngine;
using TMPro; // Importa a biblioteca TextMeshPro
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para usar listas e dicionários

public class GameManager : MonoBehaviour
{
    public int pontuacao = 0;
    public TextMeshProUGUI textoPontuacao; // Campo de texto usando TextMeshProUGUI
    public TextMeshProUGUI victoryTextoPontuacao; // Campo de texto usando TextMeshProUGUI
    public TextMeshProUGUI gameoverTextoPontuacao; // Campo de texto usando TextMeshProUGUI
    public TextMeshProUGUI textoMensagem; // Campo de texto para mensagens de coleta

    public GameObject lixeiraPapelPrefab;
    public GameObject lixeiraVidroPrefab;
    public GameObject lixeiraOrganicaPrefab;
    public GameObject lixeiraMetalPrefab;
    public GameObject popupVitoria; // Referência para o popup de vitória


    private List<GameObject> lixeirasEscolhidas = new List<GameObject>(); // Armazena as lixeiras sorteadas
    public Image TimerImage;
    public Sprite TimerMale;
    public Sprite TimerFemale;
    private Vector3[] posicoesLixeiras = new Vector3[]
    {
        new Vector3(-1.85f, 0.62f, 9.18f),
        new Vector3(2.62f, 0.62f, 9.18f)
    };

    private Dictionary<string, GameObject> lixeiras = new Dictionary<string, GameObject>();
    private int indiceColetaAtual = 0; // Índice da coleta atual
    private int lixosRestantes = 0; // Contador de lixos restantes para coleta

    private string[] tiposLixo = { "Papel", "Vidro" }; // Tipos de lixo a serem coletados
    public int countCollected = 0;

    public LixoController.TipoLixo tipoAtual;
    public Image lixeiraImage1; // Primeiro componente Image
    public Image lixeiraImage2; // Segundo componente Image
    public TextMeshProUGUI lixeiraContador1; // Contador da primeira lixeira
    public TextMeshProUGUI lixeiraContador2; // Contador da segunda lixeira
    public Image gameoverLixeiraImage1; // Primeiro componente Image
    public Image gameoverLixeiraImage2; // Segundo componente Image
    public Image victoryLixeiraImage1; // Primeiro componente Image
    public Image victoryLixeiraImage2; // Segundo componente Image
    public TextMeshProUGUI gameoverLixeiraContador1; // Contador da primeira lixeira
    public TextMeshProUGUI gameoverLixeiraContador2; // Contador da segunda lixeira
    private int[] contadores = new int[2]; // Contadores para as lixeiras
    public Sprite spriteLixeiraPapel; // Sprite da lixeira de papel
    public Sprite spriteLixeiraVidro; // Sprite da lixeira de vidro
    public Sprite spriteLixeiraMetal; // Sprite da lixeira de metal
    public Sprite spriteLixeiraOrganico; // Sprite da lixeira orgânica
    private Dictionary<string, Sprite> lixeirasSprites;
    public TextMeshProUGUI timerText; // Arraste seu objeto TextMeshPro aqui no Inspector
    private float timeElapsed = 0f; // Tempo decorrido
    public GameObject gameoverPopup;
    public GameObject FirstAreaEffect;
    public GameObject SecondAreaEffect;
    void Start()
    {
        if (PlayerPrefs.GetString("Sex") == "Male")
        {
            TimerImage.sprite = TimerMale;
        }
        else
        {
            TimerImage.sprite = TimerFemale;
        }
        timeElapsed = 0f; // Inicia o tempo em 0
        // Inicializa o dicionário de lixeiras
        lixeiras.Add("LixeiraPapel", lixeiraPapelPrefab);
        lixeiras.Add("LixeiraVidro", lixeiraVidroPrefab);
        lixeiras.Add("LixeiraOrganica", lixeiraOrganicaPrefab);
        lixeiras.Add("LixeiraMetal", lixeiraMetalPrefab);

        lixeirasSprites = new Dictionary<string, Sprite>
        {
            { "LixeiraPapel", spriteLixeiraPapel },
            { "LixeiraVidro", spriteLixeiraVidro },
            { "LixeiraMetal", spriteLixeiraMetal },
            { "LixeiraOrganica", spriteLixeiraOrganico }
        };
        // Sortear e instanciar as lixeiras
        SortearLixeiras();
        if (SecondAreaEffect != null)
        {
            SecondAreaEffect.SetActive(false);
        }
        // Atualizar os lixos na cena com base nas lixeiras sorteadas
        AtualizarLixosNaCena();

        // Inicia a coleta com a primeira mensagem
        AtualizarMensagemColeta();
    }

    public bool LixeiraEscolhidaParaTipo(LixoController.TipoLixo tipoLixo)
    {
        switch (tipoLixo)
        {
            case LixoController.TipoLixo.Papel:
                return LixeiraEscolhida("LixeiraPapel");
            case LixoController.TipoLixo.Vidro:
                return LixeiraEscolhida("LixeiraVidro");
            case LixoController.TipoLixo.Organico:
                return LixeiraEscolhida("LixeiraOrganica");
            case LixoController.TipoLixo.Metal:
                return LixeiraEscolhida("LixeiraMetal");
            default:
                return false;
        }
    }

    // Função para sortear duas lixeiras e colocá-las nas posições específicas
    void SortearLixeiras()
    {
        List<string> nomesLixeiras = new List<string>(lixeiras.Keys); // Lista de nomes de lixeiras

        // Sorteia duas lixeiras diferentes
        for (int i = 0; i < 2; i++)
        {
            int index = Random.Range(0, nomesLixeiras.Count);
            string nomeLixeira = nomesLixeiras[index];

            // Define a rotação no eixo Y como 90 graus
            Quaternion rotacao = Quaternion.Euler(0, 90, 0);

            // Instancia a lixeira na posição correspondente com a rotação ajustada
            GameObject lixeira = Instantiate(lixeiras[nomeLixeira], posicoesLixeiras[i], rotacao);
            lixeira.tag = nomeLixeira; // Define a tag da lixeira
            Transform areaCircles = null;

            foreach (Transform child in lixeira.transform)
            {
                if (child.name.StartsWith("Area_circles_"))
                {
                    areaCircles = child;
                    break; // Para no primeiro que encontrar
                }
            }

            // Verifica se encontramos um objeto com o nome correto
            if (areaCircles != null)
            {
                // Atribui ao FirstAreaEffect ou SecondAreaEffect baseado no índice
                if (i == 0)
                {
                    FirstAreaEffect = areaCircles.gameObject; // Atribui ao FirstAreaEffect
                }
                else if (i == 1)
                {
                    SecondAreaEffect = areaCircles.gameObject; // Atribui ao SecondAreaEffect
                }
            }


            // Adiciona à lista de lixeiras escolhidas
            lixeirasEscolhidas.Add(lixeira);

            if (i == 0)
            {
                lixeiraImage1.sprite = lixeirasSprites[nomeLixeira]; // Atribui o sprite da primeira lixeira
                contadores[0] = 0; // Inicializa contador
                lixeiraContador1.text = $"{contadores[0]}/5"; // Mostra 0/5

                gameoverLixeiraImage1.sprite = lixeirasSprites[nomeLixeira]; // Atribui o sprite da primeira lixeira
                victoryLixeiraImage1.sprite = lixeirasSprites[nomeLixeira]; // Atribui o sprite da primeira lixeira
                gameoverLixeiraContador1.text = $"{contadores[0]}/5"; // Mostra 0/5
            }
            else
            {
                lixeiraImage2.sprite = lixeirasSprites[nomeLixeira]; // Atribui o sprite da segunda lixeira
                contadores[1] = 0; // Inicializa contador
                lixeiraContador2.text = $"{contadores[1]}/5"; // Mostra 0/5

                gameoverLixeiraImage2.sprite = lixeirasSprites[nomeLixeira]; // Atribui o sprite da segunda lixeira
                victoryLixeiraImage2.sprite = lixeirasSprites[nomeLixeira]; // Atribui o sprite da segunda lixeira
                gameoverLixeiraContador2.text = $"{contadores[1]}/5"; // Mostra 0/5
            }

            // Remove a lixeira sorteada da lista para não sortear a mesma
            nomesLixeiras.RemoveAt(index);
        }
    }

    // Função para exibir ou ocultar lixos com base nas lixeiras sorteadas


    // Função auxiliar para verificar se uma lixeira específica foi sorteada
    bool LixeiraEscolhida(string tagLixeira)
    {
        return lixeirasEscolhidas.Exists(lixeira => lixeira.tag == tagLixeira);
    }

    // Função para atualizar a pontuação e exibir na UI
    public void AtualizarPontuacao(int valor)
    {
        pontuacao += valor;
        if (valor > 0)
        {
            if (countCollected < 5)
            {
                AtualizarContador(0);
            }
            else
            {
                AtualizarContador(1);
            }
            countCollected++;
        }
        AtualizarTextoPontuacao();

        // Sempre verifica se a mensagem precisa ser atualizada
        AtualizarMensagemColeta();
    }

    public void AtualizarContador(int index)
    {
        if (index < 0 || index >= contadores.Length) return;

        contadores[index]++;

        // Atualiza o contador da lixeira correspondente
        if (index == 0)
        {
            lixeiraContador1.text = $"{contadores[index]}/5";
            gameoverLixeiraContador1.text = $"{contadores[index]}/5";
        }
        else if (index == 1)
        {
            lixeiraContador2.text = $"{contadores[index]}/5";
            gameoverLixeiraContador2.text = $"{contadores[index]}/5";
        }
    }

    private void AtualizarMensagemColeta()
    {
        Debug.Log("Coletou item " + countCollected);
        // Verifique se o tipo atual ainda tem itens restantes
        tipoAtual = ObterTipoLixoAtual();

        // Se não existem lixos do tipo atual, atualize o índice
        if (countCollected == 5)
        {
            FirstAreaEffect.SetActive(false);
            SecondAreaEffect.SetActive(true);
            // Avança para o próximo tipo de lixo se não houver lixos do tipo atual
            indiceColetaAtual = 1;

            // Verifica se ainda há tipos de lixo para coletar
            if (indiceColetaAtual < lixeirasEscolhidas.Count)
            {
                tipoAtual = ObterTipoLixoAtual(); // Atualiza o tipo atual
                textoMensagem.text = "Colete todos os lixos do tipo " + tipoAtual.ToString();
                AtualizarLixosNaCena(); // Atualiza os lixos na cena para o novo tipo
            }
        }
        if (countCollected == 10)
        {
            Victory(); // Se não houver mais tipos, chama a vitória
        }
        else
        {
            // Se ainda houver lixos do tipo atual, exibe a mensagem atual
            textoMensagem.text = "Colete todos os lixos do tipo " + tipoAtual.ToString();
        }
    }


    private bool ExistemLixosDoTipoAtual(string tipoAtual)
    {
        LixoController[] lixosNaCena = FindObjectsOfType<LixoController>();

        foreach (LixoController lixo in lixosNaCena)
        {
            Debug.Log($"Verificando lixo: {lixo.tipoLixo}, ativo: {lixo.gameObject.activeInHierarchy}");
            if (lixo.gameObject.activeInHierarchy && lixo.tipoLixo.ToString() == tipoAtual)
            {
                return true; // Existe pelo menos um lixo do tipo atual
            }
        }

        return false; // Se nenhum lixo do tipo atual foi encontrado, retorna false
    }

    private LixoController.TipoLixo ObterTipoLixoAtual()
    {
        if (lixeirasEscolhidas[indiceColetaAtual].CompareTag("LixeiraPapel"))
        {
            return LixoController.TipoLixo.Papel;
        }
        else if (lixeirasEscolhidas[indiceColetaAtual].CompareTag("LixeiraVidro"))
        {
            return LixoController.TipoLixo.Vidro;
        }
        else if (lixeirasEscolhidas[indiceColetaAtual].CompareTag("LixeiraOrganica"))
        {
            return LixoController.TipoLixo.Organico;
        }
        else if (lixeirasEscolhidas[indiceColetaAtual].CompareTag("LixeiraMetal"))
        {
            return LixoController.TipoLixo.Metal;
        }
        else
        {
            return LixoController.TipoLixo.Papel; // Valor padrão, pode ser ajustado conforme necessário
        }
    }


    // Função para exibir ou ocultar lixos com base nas lixeiras sorteadas
    private void AtualizarLixosNaCena()
    {
        // Encontra todos os objetos de lixo na cena
        LixoController[] lixos = FindObjectsOfType<LixoController>();

        // Reseta lixos restantes para o tipo atual
        lixosRestantes = 0;

        foreach (var lixo in lixos)
        {
            // Verifica se o tipo de lixo corresponde à lixeira atual que está sendo coletada
            if ((lixo.tipoLixo == LixoController.TipoLixo.Papel && LixeiraEscolhida("LixeiraPapel")) ||
                (lixo.tipoLixo == LixoController.TipoLixo.Vidro && LixeiraEscolhida("LixeiraVidro")) ||
                (lixo.tipoLixo == LixoController.TipoLixo.Organico && LixeiraEscolhida("LixeiraOrganica")) ||
                (lixo.tipoLixo == LixoController.TipoLixo.Metal && LixeiraEscolhida("LixeiraMetal")))
            {
                // Ativa o lixo se for compatível com as lixeiras escolhidas
                lixo.gameObject.SetActive(true);
                lixosRestantes++; // Conta o lixo que está ativo
            }
            else
            {
                // Desativa o lixo se não for compatível
                lixo.gameObject.SetActive(false);
            }
        }

        // Verifica se todos os lixos do tipo atual foram coletados
        if (lixosRestantes == 0)
        {
            AtualizarMensagemColeta(); // Atualiza a mensagem
        }
    }
    void Update()
    {
        timeElapsed += Time.deltaTime; // Incrementa o tempo decorrido
        UpdateTimerUI();

        // Verifica se o tempo chegou a 60 segundos
        if (timeElapsed >= 60f)
        {
            GameOver();
        }
    }
    void UpdateTimerUI()
    {
        int seconds = Mathf.FloorToInt(timeElapsed % 60F);
        timerText.text = string.Format("{0:00}", seconds);
    }

    void GameOver()
    {
        gameoverPopup.SetActive(true);
    }

    // Exibe o popup de vitória
    private void Victory()
    {
        textoMensagem.text = "Parabéns! Você coletou todos os lixos!";
        StartCoroutine(WaitToVictory());
    }
    IEnumerator WaitToVictory()
    {
        yield return new WaitForSeconds(1);
        popupVitoria.SetActive(true); // Ativa o popup de vitória
    }
    // Atualiza o texto da pontuação
    private void AtualizarTextoPontuacao()
    {
        if (textoPontuacao != null)
        {
            textoPontuacao.text = pontuacao.ToString();
            victoryTextoPontuacao.text = pontuacao.ToString();
            gameoverTextoPontuacao.text = pontuacao.ToString();
        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene("Level2");
    }
    public void GoToHome()
    {
        SceneManager.LoadScene(0);
    }
}
