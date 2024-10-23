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

    public float rotationSpeed = 5f;
    public string trashTag = "Trash";

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI collectedItemsText;
    public TextMeshProUGUI collectedItems2Text;
    public TextMeshProUGUI scoreText;

    private int collectedItems = 0;
    private int collectedItemstrash1 = 0;
    private int collectedItemstrash2 = 0;
    private int maxItems = 10;
    private int score = 0;
    private float elapsedTime = 0f;
    private float timeLimit = 60f;

    private bool isHandlingTrash = false;
    private bool gameEnded = false;

    public GameObject victoryPopup;
    public GameObject gameoverPopup;

    public TextMeshProUGUI victoryCollectedItemsText;
    public TextMeshProUGUI victoryCollectedItems2Text;
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI gameoverCollectedItemsText;
    public TextMeshProUGUI gameoverCollectedItems2Text;
    public TextMeshProUGUI gameoverScoreText;
    public Image TimerImage;
    public Sprite TimerMale;
    public Sprite TimerFemale;
    public bool isLevel2 = false;

    // Referências para as lixeiras
    public Transform trashBin1; // Lixeira para os primeiros 5 itens
    public Transform trashBin2; // Lixeira para os próximos itens
    public LixeiraController trashBin1Controller;
    public LixeiraController trashBin2Controller;
    public GameObject effectTrash1;
    public GameObject effectTrash2;
    public GameObject messageTrash1;
    public GameObject messageTrash2;
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

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;

        UpdateCollectedItemsText();
        UpdateScoreText();
    }

    IEnumerator WaitToEnd()
    {
        gameEnded = true;
        yield return new WaitForSeconds(1);
        EndGame(true); // Mostra o popup de vitória
    }

    void Update()
    {
        if (gameEnded) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerText();

        if (elapsedTime >= timeLimit)
        {
            EndGame(false);
            return;
        }

        if (collectedItems >= maxItems)
        {
            StartCoroutine(WaitToEnd());
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            RotateTowards(agent.steeringTarget);
        }

        if (Input.GetMouseButtonDown(0) && !isHandlingTrash)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(trashTag))
            {
                if (isLevel2 &&
                    ((hit.transform.ToString().Contains("Paper") && collectedItems < 5) ||
                    (hit.transform.ToString().Contains("Organic") && collectedItems >= 5)))
                {
                    Debug.Log(hit.transform.ToString());
                    agent.SetDestination(hit.point);
                    animator.SetBool("isWalking", true);
                    StartCoroutine(HandleTrash(hit.transform.gameObject));
                }
                else if (!isLevel2)
                {
                    // Se não for o level 2, mantenha o comportamento normal
                    Debug.Log(hit.transform.ToString());
                    agent.SetDestination(hit.point);
                    animator.SetBool("isWalking", true);
                    StartCoroutine(HandleTrash(hit.transform.gameObject));
                }
            }
        }
    }

    private IEnumerator HandleTrash(GameObject trash)
    {
        isHandlingTrash = true;

        while (agent.remainingDistance > agent.stoppingDistance || agent.pathPending)
        {
            yield return null;
        }

        animator.SetBool("isWalking", false);

        if (isLevel2)
        {
            // Escolhe a lixeira apropriada baseado no número de itens coletados
            Transform targetBin = collectedItems < 5 ? trashBin1 : trashBin2;
            animator.SetBool("isWalking", false);
  
            // Muda para animação de idle
            yield return new WaitForSeconds(0.5f);
            // Mover até a lixeira
            agent.SetDestination(targetBin.position);
            animator.SetBool("isWalking", true);

            yield return StartCoroutine(WaitToDestroyTrash(trash));

            // Espera o personagem chegar na lixeira
            while (agent.remainingDistance > agent.stoppingDistance || agent.pathPending)
            {
                yield return null;
            }
            if (collectedItems < 5)
            {
                Debug.Log("Tentando shake");
                StartCoroutine(trashBin1Controller.ShakeLixeira());
            } 
            else
            {
                StartCoroutine(trashBin2Controller.ShakeLixeira());
            }
            isHandlingTrash = false;

            // Muda para animação de idle
            animator.SetBool("isWalking", false);
        }
        else
        {
            // Libera o personagem para lidar com o próximo lixo
            isHandlingTrash = false;
            // Chama a coroutine para destruir o lixo com uma pequena animação
            yield return StartCoroutine(WaitToDestroyTrash(trash));
        }

        // Atualiza os contadores de itens e pontuação
        if (isLevel2)
        {
            if (collectedItems < 5)
            {
                collectedItems++;
                collectedItemstrash1++;
            }
            else
            {
                collectedItems++;
                collectedItemstrash2++;
            }
            if (collectedItems == 5)
            {
                effectTrash1.SetActive(false);
                messageTrash1.SetActive(false);
                effectTrash2.SetActive(true);
                messageTrash2.SetActive(true);
            }
        }
        else
        {
            collectedItems++;
        }
        score += 150;

        UpdateCollectedItemsText();
        UpdateScoreText();


    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    IEnumerator WaitToDestroyTrash(GameObject trash)
    {
        Animator anim = trash.GetComponent<Animator>();
        anim.SetTrigger("Collected");
        yield return new WaitForSeconds(1.5f);

        Destroy(trash);
    }

    private void UpdateTimerText()
    {
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        timerText.text = string.Format("{0:00}", seconds);
    }

    private void UpdateCollectedItemsText()
    {
        if (isLevel2)
        {
            collectedItemsText.text = collectedItemstrash1 + "/5";
            victoryCollectedItemsText.text = collectedItemstrash1 + "/5";
            gameoverCollectedItemsText.text = collectedItemstrash1 + "/5";
            if (collectedItems2Text)
            {
                collectedItems2Text.text = collectedItemstrash2 + "/5";
                victoryCollectedItems2Text.text = collectedItemstrash2 + "/5";
                gameoverCollectedItems2Text.text = collectedItemstrash2 + "/5";
            }
        }
        else
        {
            collectedItemsText.text = collectedItems + "/10";
            victoryCollectedItemsText.text = collectedItems + "/10";
            gameoverCollectedItemsText.text = collectedItems + "/10";
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
        victoryScoreText.text = score.ToString();
        gameoverScoreText.text = score.ToString();
    }

    private void EndGame(bool victory)
    {
        gameEnded = true;

        if (victory)
        {
            victoryPopup.SetActive(true);
        }
        else
        {
            gameoverPopup.SetActive(true);
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
