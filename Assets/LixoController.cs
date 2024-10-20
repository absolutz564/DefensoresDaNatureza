using System.Collections;
using UnityEngine;

public class LixoController : MonoBehaviour
{
    public enum TipoLixo { Reciclavel, Naoreciclavel, Papel, Vidro, Organico, Metal } // Novos tipos de lixo
    public TipoLixo tipoLixo; // Define o tipo de lixo
    public Vector3 posicaoInicial; // Guarda a posi��o original do lixo
    private GameManager gameManager; // Refer�ncia ao GameManager para controlar a pontua��o
    private bool estaSendoArrastado = false; // Para verificar se o lixo est� sendo arrastado
    private bool colidiuComLixeira = false; // Para verificar se o lixo entrou em contato com uma lixeira
    private string tagLixeiraAtual; // Tag da lixeira com a qual o lixo colidiu
    private Camera cameraPrincipal; // Refer�ncia para a c�mera
    private Plane planoArrasto; // Plano de refer�ncia para o arrasto do objeto

    void Start()
    {
        // Salva a posi��o inicial do lixo
        posicaoInicial = transform.position;
        gameManager = FindObjectOfType<GameManager>();
        cameraPrincipal = Camera.main; // Obt�m a c�mera principal

        // Define o plano de arrasto com base na posi��o inicial do objeto no eixo Z
        planoArrasto = new Plane(Vector3.up, transform.position);
    }

    void OnMouseDown()
    {
        // Quando clicamos no lixo, come�amos a arrastar
        estaSendoArrastado = true;
        colidiuComLixeira = false; // Resetamos a verifica��o da lixeira
    }

    void OnMouseUp()
    {
        // Para de arrastar o objeto ao soltar o bot�o do mouse
        estaSendoArrastado = false;

        // Verifica se o objeto ainda est� ativo (deve ser colet�vel)
        if (!gameObject.activeSelf) return;

        // Se o lixo foi jogado em uma lixeira
        if (colidiuComLixeira)
        {
            // Verifica se a lixeira est� ativa no GameManager e corresponde ao tipo de lixo correto
            if (gameManager.LixeiraEscolhidaParaTipo(tipoLixo) && tipoLixo == gameManager.tipoAtual &&
                ((tagLixeiraAtual == "LixeiraReciclavel" && tipoLixo == TipoLixo.Reciclavel) ||
                 (tagLixeiraAtual == "LixeirNaoReciclavel" && tipoLixo == TipoLixo.Naoreciclavel) ||
                 (tagLixeiraAtual == "LixeiraOrganica" && tipoLixo == TipoLixo.Organico) ||
                 (tagLixeiraAtual == "LixeiraMetal" && tipoLixo == TipoLixo.Metal)))
            {
                // Atualiza a pontua��o e desativa o lixo
                gameManager.AtualizarPontuacao(100);
                gameObject.SetActive(false); // Desativa o lixo ap�s jog�-lo na lixeira correta

                // Inicia o shake da lixeira
                GameObject lixeira = GameObject.FindWithTag(tagLixeiraAtual);
                if (lixeira != null)
                {
                    LixeiraController lixeiraController = lixeira.GetComponent<LixeiraController>();
                    if (lixeiraController != null)
                    {
                        lixeiraController.StartCoroutine(lixeiraController.ShakeLixeira());
                    }
                }
            }
            else
            {
                // Lixo foi jogado na lixeira errada ou em uma lixeira n�o ativa, perde pontos
                gameManager.AtualizarPontuacao(-100);
                transform.position = posicaoInicial; // Volta o lixo � posi��o inicial
            }
        }
        else
        {
            // Se o lixo n�o foi jogado em nenhuma lixeira, volta � posi��o original
            transform.position = posicaoInicial;
        }
    }

    void Update()
    {
        if (estaSendoArrastado)
        {
            // Lan�a um raio da c�mera para a posi��o do mouse
            Ray raio = cameraPrincipal.ScreenPointToRay(Input.mousePosition);
            float distancia;

            // Interse��o do raio com o plano de arrasto para obter a posi��o do mouse no plano
            if (planoArrasto.Raycast(raio, out distancia))
            {
                // Move o objeto para o ponto onde o raio intercepta o plano
                transform.position = raio.GetPoint(distancia);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Apenas marcamos que o objeto entrou em contato com uma lixeira
        if (other.CompareTag("LixeiraReciclavel") || other.CompareTag("LixeiraNaoReciclavel") ||
            other.CompareTag("LixeiraOrganica") || other.CompareTag("LixeiraMetal"))
        {
            colidiuComLixeira = true; // Marca que houve colis�o com uma lixeira
            tagLixeiraAtual = other.tag; // Salva a tag da lixeira para valida��o posterior
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Quando o objeto sai da lixeira, remove a marca��o de colis�o
        if (other.CompareTag("LixeiraReciclavel") || other.CompareTag("LixeiraNaoReciclavel") ||
            other.CompareTag("LixeiraOrganica") || other.CompareTag("LixeiraMetal"))
        {
            colidiuComLixeira = false;
        }
    }
}
