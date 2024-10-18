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

    // Velocidade de rotação para suavizar a transição
    public float rotationSpeed = 5f;

    // Tag para identificar os objetos de lixo
    public string trashTag = "Trash";

    // Referências para TextMeshPro
    public TextMeshProUGUI timerText;  // Para exibir o tempo
    public TextMeshProUGUI collectedItemsText; // Para exibir os itens coletados
    public TextMeshProUGUI scoreText; // Para exibir a pontuação

    // Contadores
    private int collectedItems = 0; // Contador de itens coletados
    private int maxItems = 10; // Número total de itens a serem coletados
    private int score = 0; // Pontuação do jogador
    private float elapsedTime = 0f; // Tempo total decorrido
    private float timeLimit = 60f; // Tempo limite em segundos

    private bool isHandlingTrash = false;
    private bool gameEnded = false; // Controla se o jogo já acabou

    // Referências para os popups de vitória e gameover
    public GameObject victoryPopup;
    public GameObject gameoverPopup;

    // Referências para os textos de vitória e gameover
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
        // Referências ao NavMeshAgent e Animator
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Desativa a rotação automática
        agent.updateRotation = false;

        // Atualiza os textos iniciais
        UpdateCollectedItemsText();
        UpdateScoreText();
    }

    IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(2);
        EndGame(true); // Mostra o popup de vitória

    }

    void Update()
    {
        if (gameEnded) return; // Se o jogo já acabou, não faz nada

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
            // Se o personagem chegar ao destino, volta para a animação de idle
            animator.SetBool("isWalking", false);
        }
        else
        {
            // Controla manualmente a rotação do personagem enquanto ele se move
            RotateTowards(agent.steeringTarget);
        }

        // Checa o clique do mouse, mas só se o personagem não estiver ocupado
        if (Input.GetMouseButtonDown(0) && !isHandlingTrash)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verifica se o clique foi em um objeto com a tag "Trash"
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(trashTag))
            {
                // Move o personagem até o objeto clicado
                agent.SetDestination(hit.point);

                // Inicia a animação de andar
                animator.SetBool("isWalking", true);

                // Chama a coroutine para lidar com o lixo ao chegar
                StartCoroutine(HandleTrash(hit.transform.gameObject));
            }
        }
    }

    // Coroutine para lidar com o desaparecimento do lixo
    private IEnumerator HandleTrash(GameObject trash)
    {
        // Indica que o personagem está ocupado
        isHandlingTrash = true;

        // Espera até que o personagem chegue ao destino
        while (agent.remainingDistance > agent.stoppingDistance || agent.pathPending)
        {
            yield return null;
        }

        // Muda de volta para a animação de idle
        animator.SetBool("isWalking", false);

        // Libera o personagem para lidar com o próximo lixo
        isHandlingTrash = false;

        // Incrementa os itens coletados e a pontuação
        collectedItems++;
        score += 150;

        // Atualiza o texto dos itens coletados e pontuação
        UpdateCollectedItemsText();
        UpdateScoreText();

        // Chama a coroutine para destruir o lixo com uma pequena animação
        yield return StartCoroutine(WaitToDestroyTrash(trash));
    }

    // Função para rotacionar o personagem suavemente em direção ao destino
    private void RotateTowards(Vector3 target)
    {
        // Calcula a direção para o alvo
        Vector3 direction = (target - transform.position).normalized;

        // Se existir movimento na direção
        if (direction != Vector3.zero)
        {
            // Calcula a rotação alvo
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Suavemente interpola a rotação atual para a rotação alvo
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

    // Função para atualizar o texto do contador de tempo
    private void UpdateTimerText()
    {
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        timerText.text = string.Format("{0:00}", seconds);
    }

    // Função para atualizar o texto dos itens coletados
    private void UpdateCollectedItemsText()
    {
        collectedItemsText.text = collectedItems + "/" + maxItems;
        victoryCollectedItemsText.text = collectedItems + "/" + maxItems;
        gameoverCollectedItemsText.text = collectedItems + "/" + maxItems;
    }

    // Função para atualizar o texto da pontuação
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
        victoryScoreText.text = score.ToString();
        gameoverScoreText.text = score.ToString();
    }

    // Função para finalizar o jogo
    private void EndGame(bool victory)
    {
        gameEnded = true; // Marca que o jogo acabou

        if (victory)
        {
            victoryPopup.SetActive(true); // Mostra o popup de vitória
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
