using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    // Velocidade de rota��o para suavizar a transi��o
    public float rotationSpeed = 5f;

    // Tag para identificar os objetos de lixo
    public string trashTag = "Trash";

    // Refer�ncias para TextMeshPro
    public TextMeshProUGUI timerText;  // Para exibir o tempo
    public TextMeshProUGUI collectedItemsText; // Para exibir os itens coletados
    public TextMeshProUGUI scoreText; // Para exibir a pontua��o

    // Contadores
    private int collectedItems = 0; // Contador de itens coletados
    private int maxItems = 10; // N�mero total de itens a serem coletados
    private int score = 0; // Pontua��o do jogador
    private float elapsedTime = 0f; // Tempo total decorrido
    private float timeLimit = 60f; // Tempo limite em segundos

    private bool isHandlingTrash = false;
    private bool gameEnded = false; // Controla se o jogo j� acabou

    // Refer�ncias para os popups de vit�ria e gameover
    public GameObject victoryPopup;
    public GameObject gameoverPopup;

    // Refer�ncias para os textos de vit�ria e gameover
    public TextMeshProUGUI victoryCollectedItemsText;
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI gameoverCollectedItemsText;
    public TextMeshProUGUI gameoverScoreText;
    public Image TimerImage;
    public Sprite TimerMale;
    public Sprite TimerFemale;

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
        // Refer�ncias ao NavMeshAgent e Animator
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Desativa a rota��o autom�tica
        agent.updateRotation = false;

        // Atualiza os textos iniciais
        UpdateCollectedItemsText();
        UpdateScoreText();
    }

    IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(2);
        EndGame(true); // Mostra o popup de vit�ria

    }

    void Update()
    {
        if (gameEnded) return; // Se o jogo j� acabou, n�o faz nada

        // Atualiza o tempo decorrido
        elapsedTime += Time.deltaTime;
        UpdateTimerText(); // Atualiza o contador de tempo no TextMeshPro

        // Verifica se o tempo acabou
        if (elapsedTime >= timeLimit)
        {
            EndGame(false); // Mostra o popup de gameover
            return;
        }

        // Verifica se o jogador coletou todos os itens
        if (collectedItems >= maxItems)
        {
            StartCoroutine(WaitToEnd());
            return;
        }

        // Verifica se chegou ao destino
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            // Se o personagem chegar ao destino, volta para a anima��o de idle
            animator.SetBool("isWalking", false);
        }
        else
        {
            // Controla manualmente a rota��o do personagem enquanto ele se move
            RotateTowards(agent.steeringTarget);
        }

        // Checa o clique do mouse, mas s� se o personagem n�o estiver ocupado
        if (Input.GetMouseButtonDown(0) && !isHandlingTrash)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verifica se o clique foi em um objeto com a tag "Trash"
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(trashTag))
            {
                // Move o personagem at� o objeto clicado
                agent.SetDestination(hit.point);

                // Inicia a anima��o de andar
                animator.SetBool("isWalking", true);

                // Chama a coroutine para lidar com o lixo ao chegar
                StartCoroutine(HandleTrash(hit.transform.gameObject));
            }
        }
    }

    // Coroutine para lidar com o desaparecimento do lixo
    private IEnumerator HandleTrash(GameObject trash)
    {
        // Indica que o personagem est� ocupado
        isHandlingTrash = true;

        // Espera at� que o personagem chegue ao destino
        while (agent.remainingDistance > agent.stoppingDistance || agent.pathPending)
        {
            yield return null;
        }

        // Muda de volta para a anima��o de idle
        animator.SetBool("isWalking", false);

        // Libera o personagem para lidar com o pr�ximo lixo
        isHandlingTrash = false;

        // Incrementa os itens coletados e a pontua��o
        collectedItems++;
        score += 150;

        // Atualiza o texto dos itens coletados e pontua��o
        UpdateCollectedItemsText();
        UpdateScoreText();

        // Chama a coroutine para destruir o lixo com uma pequena anima��o
        yield return StartCoroutine(WaitToDestroyTrash(trash));
    }

    // Fun��o para rotacionar o personagem suavemente em dire��o ao destino
    private void RotateTowards(Vector3 target)
    {
        // Calcula a dire��o para o alvo
        Vector3 direction = (target - transform.position).normalized;

        // Se existir movimento na dire��o
        if (direction != Vector3.zero)
        {
            // Calcula a rota��o alvo
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Suavemente interpola a rota��o atual para a rota��o alvo
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Coroutine para destruir o lixo
    IEnumerator WaitToDestroyTrash(GameObject trash)
    {
        Animator anim = trash.GetComponent<Animator>();
        anim.SetTrigger("Collected");
        yield return new WaitForSeconds(1.5f);

        Destroy(trash);
    }

    // Fun��o para atualizar o texto do contador de tempo
    private void UpdateTimerText()
    {
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        timerText.text = string.Format("{0:00}", seconds);
    }

    // Fun��o para atualizar o texto dos itens coletados
    private void UpdateCollectedItemsText()
    {
        collectedItemsText.text = collectedItems + "/" + maxItems;
        victoryCollectedItemsText.text = collectedItems + "/" + maxItems;
        gameoverCollectedItemsText.text = collectedItems + "/" + maxItems;
    }

    // Fun��o para atualizar o texto da pontua��o
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
        victoryScoreText.text = score.ToString();
        gameoverScoreText.text = score.ToString();
    }

    // Fun��o para finalizar o jogo
    private void EndGame(bool victory)
    {
        gameEnded = true; // Marca que o jogo acabou

        if (victory)
        {
            victoryPopup.SetActive(true); // Mostra o popup de vit�ria
        }
        else
        {
            gameoverPopup.SetActive(true); // Mostra o popup de gameover
        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene("Level1");
    }
    public void GoToHome()
    {
        SceneManager.LoadScene(0);
    }
}
