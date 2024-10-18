using UnityEngine;
using UnityEngine.AI; // Certifique-se de que o NavMeshAgent esteja dispon�vel

public class CharacterControllerScript : MonoBehaviour
{
    public Transform trash; // Refer�ncia ao objeto lixo
    public Transform trashBin; // Refer�ncia � lixeira
    public float pickupDistance = 2f; // Dist�ncia para pegar o lixo
    public float throwDistance = 2f; // Dist�ncia para jogar o lixo

    private NavMeshAgent agent; // Agente de navega��o
    private bool hasTrash = false; // Se o personagem est� segurando lixo

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!hasTrash)
        {
            MoveToTrash();
        }
        else
        {
            MoveToTrashBin();
        }
    }

    void MoveToTrash()
    {
        // Move para o lixo
        agent.SetDestination(trash.position);

        // Verifica se est� perto do lixo para pegar
        if (Vector3.Distance(transform.position, trash.position) < pickupDistance)
        {
            PickUpTrash();
        }
    }

    void PickUpTrash()
    {
        hasTrash = true;
        trash.gameObject.SetActive(false); // Desativa o objeto lixo ou fa�a o que precisar com ele
        Debug.Log("Lixo pegado!");
    }

    void MoveToTrashBin()
    {
        // Move para a lixeira
        agent.SetDestination(trashBin.position);

        // Verifica se est� perto da lixeira para descartar
        if (Vector3.Distance(transform.position, trashBin.position) < throwDistance)
        {
            ThrowTrash();
        }
    }

    void ThrowTrash()
    {
        hasTrash = false;
        Debug.Log("Lixo descartado!");
        // Aqui voc� pode implementar a l�gica para destruir o objeto lixo ou para outra a��o
    }
}
