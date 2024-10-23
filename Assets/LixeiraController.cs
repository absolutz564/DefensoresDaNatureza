using System.Collections;
using UnityEngine;

public class LixeiraController : MonoBehaviour
{
    public AudioSource lixoAudioSource;
    public bool isVR = false;  // Flag para verificar se está em VR

    public IEnumerator ShakeLixeira()
    {
        lixoAudioSource.Play();
        Debug.Log("Shake");

        Vector3 posicaoInicial = transform.position;

        // Definindo a duração e intensidade com base no estado do VR
        float duracao = isVR ? 0.2f : 0.3f;  // Duração menor no VR
        float intensidade = isVR ? 0.006f : 0.08f;  // Intensidade menor no VR

        for (float t = 0; t < duracao; t += Time.deltaTime)
        {
            // Move a lixeira em uma pequena quantidade aleatória a cada frame
            float offsetX = Random.Range(-intensidade, intensidade);
            float offsetY = Random.Range(-intensidade, intensidade);
            transform.position = new Vector3(posicaoInicial.x + offsetX, posicaoInicial.y + offsetY, posicaoInicial.z);

            yield return null; // Espera até o próximo frame
        }

        // Retorna a lixeira à posição original
        transform.position = posicaoInicial;
    }
}
