using UnityEngine;

public class ResetPositionRotation : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Função para armazenar a posição e rotação originais do objeto
    public void StoreOriginalTransform()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        Debug.Log("Posição e rotação originais armazenadas.");
    }

    // Função para retornar o objeto à posição e rotação originais
    public void ResetToOriginalTransform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        Debug.Log("Objeto retornou à posição e rotação originais.");
    }

    // Exemplo de como usar as funções nos eventos do Unity
    void Start()
    {
        // Armazenar a posição e rotação originais quando o jogo começa
        StoreOriginalTransform();
    }

    // Exemplo de como chamar a função para resetar
    void Update()
    {
        // Pressione a tecla "R" para resetar o objeto
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToOriginalTransform();
        }
    }
}
