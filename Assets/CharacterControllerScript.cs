using UnityEngine;
using UnityEngine.AI; // Certifique-se de que o NavMeshAgent esteja disponível

public class CharacterControllerScript : MonoBehaviour
{
    public Transform trash; // Referência ao objeto lixo
    public Transform trashBin; // Referência à lixeira
    public float pickupDistance = 2f; // Distância para pegar o lixo
    public float throwDistance = 2f; // Distância para jogar o lixo

    private NavMeshAgent agent; // Agente de navegação
    private bool hasTrash = false; // Se o personagem está segurando lixo

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

        // Verifica se está perto do lixo para pegar
        if (Vector3.Distance(transform.position, trash.position) < pickupDistance)
        {
            PickUpTrash();
        }
    }

    void PickUpTrash()
    {
        hasTrash = true;
        trash.gameObject.SetActive(false); // Desativa o objeto lixo ou faça o que precisar com ele
        Debug.Log("Lixo pegado!");
    }

    void MoveToTrashBin()
    {
        // Move para a lixeira
        agent.SetDestination(trashBin.position);

        // Verifica se está perto da lixeira para descartar
        if (Vector3.Distance(transform.position, trashBin.position) < throwDistance)
        {
            ThrowTrash();
        }
    }

    void ThrowTrash()
    {
        hasTrash = false;
        Debug.Log("Lixo descartado!");
        // Aqui você pode implementar a lógica para destruir o objeto lixo ou para outra ação
    }
}
