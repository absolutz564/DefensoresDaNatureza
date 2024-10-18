using UnityEngine;
using TMPro;

public class LixeiraOrganica : MonoBehaviour
{
    public TextMeshProUGUI scoreText;  // Referência ao componente TextMeshPro para exibir o score
    public string tag;
    public VRGameManager vrGameManager;

    // Este método é chamado quando outro objeto entra no trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger tem a tag "lixoOrganico"
        if (other.CompareTag(tag))
        {
            // Destrói o objeto de lixo orgânico
            Destroy(other.gameObject);

            // Adiciona 100 pontos ao score
            vrGameManager.score += 100;

            // Atualiza o texto no TextMeshPro com o novo score
            scoreText.text = "Score: " + vrGameManager.score;
        }
    }
}
