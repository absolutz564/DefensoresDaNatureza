using UnityEngine;

public class ResetPositionRotation : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Fun��o para armazenar a posi��o e rota��o originais do objeto
    public void StoreOriginalTransform()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        Debug.Log("Posi��o e rota��o originais armazenadas.");
    }

    // Fun��o para retornar o objeto � posi��o e rota��o originais
    public void ResetToOriginalTransform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        Debug.Log("Objeto retornou � posi��o e rota��o originais.");
    }

    // Exemplo de como usar as fun��es nos eventos do Unity
    void Start()
    {
        // Armazenar a posi��o e rota��o originais quando o jogo come�a
        StoreOriginalTransform();
    }

    // Exemplo de como chamar a fun��o para resetar
    void Update()
    {
        // Pressione a tecla "R" para resetar o objeto
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToOriginalTransform();
        }
    }
}
