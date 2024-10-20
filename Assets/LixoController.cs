using System.Collections;
using UnityEngine;

public class LixoController : MonoBehaviour
{
    public enum TipoLixo { Reciclavel, Naoreciclavel, Papel, Vidro, Organico, Metal } // Novos tipos de lixo
    public TipoLixo tipoLixo; // Define o tipo de lixo
    public Vector3 posicaoInicial; // Guarda a posição original do lixo
    private GameManager gameManager; // Referência ao GameManager para controlar a pontuação
    private bool estaSendoArrastado = false; // Para verificar se o lixo está sendo arrastado
    private bool colidiuComLixeira = false; // Para verificar se o lixo entrou em contato com uma lixeira
    private string tagLixeiraAtual; // Tag da lixeira com a qual o lixo colidiu
    private Camera cameraPrincipal; // Referência para a câmera
    private Plane planoArrasto; // Plano de referência para o arrasto do objeto

    void Start()
    {
        // Salva a posição inicial do lixo
        posicaoInicial = transform.position;
        gameManager = FindObjectOfType<GameManager>();
        cameraPrincipal = Camera.main; // Obtém a câmera principal

        // Define o plano de arrasto com base na posição inicial do objeto no eixo Z
        planoArrasto = new Plane(Vector3.up, transform.position);
    }

    void OnMouseDown()
    {
        // Quando clicamos no lixo, começamos a arrastar
        estaSendoArrastado = true;
        colidiuComLixeira = false; // Resetamos a verificação da lixeira
    }

    void OnMouseUp()
    {
        // Para de arrastar o objeto ao soltar o botão do mouse
        estaSendoArrastado = false;

        // Verifica se o objeto ainda está ativo (deve ser coletável)
        if (!gameObject.activeSelf) return;

        // Se o lixo foi jogado em uma lixeira
        if (colidiuComLixeira)
        {
            // Verifica se a lixeira está ativa no GameManager e corresponde ao tipo de lixo correto
            if (gameManager.LixeiraEscolhidaParaTipo(tipoLixo) && tipoLixo == gameManager.tipoAtual &&
                ((tagLixeiraAtual == "LixeiraReciclavel" && tipoLixo == TipoLixo.Reciclavel) ||
                 (tagLixeiraAtual == "LixeirNaoReciclavel" && tipoLixo == TipoLixo.Naoreciclavel) ||
                 (tagLixeiraAtual == "LixeiraOrganica" && tipoLixo == TipoLixo.Organico) ||
                 (tagLixeiraAtual == "LixeiraMetal" && tipoLixo == TipoLixo.Metal)))
            {
                // Atualiza a pontuação e desativa o lixo
                gameManager.AtualizarPontuacao(100);
                gameObject.SetActive(false); // Desativa o lixo após jogá-lo na lixeira correta

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
                // Lixo foi jogado na lixeira errada ou em uma lixeira não ativa, perde pontos
                gameManager.AtualizarPontuacao(-100);
                transform.position = posicaoInicial; // Volta o lixo à posição inicial
            }
        }
        else
        {
            // Se o lixo não foi jogado em nenhuma lixeira, volta à posição original
            transform.position = posicaoInicial;
        }
    }

    void Update()
    {
        if (estaSendoArrastado)
        {
            // Lança um raio da câmera para a posição do mouse
            Ray raio = cameraPrincipal.ScreenPointToRay(Input.mousePosition);
            float distancia;

            // Interseção do raio com o plano de arrasto para obter a posição do mouse no plano
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
            colidiuComLixeira = true; // Marca que houve colisão com uma lixeira
            tagLixeiraAtual = other.tag; // Salva a tag da lixeira para validação posterior
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Quando o objeto sai da lixeira, remove a marcação de colisão
        if (other.CompareTag("LixeiraReciclavel") || other.CompareTag("LixeiraNaoReciclavel") ||
            other.CompareTag("LixeiraOrganica") || other.CompareTag("LixeiraMetal"))
        {
            colidiuComLixeira = false;
        }
    }
}
