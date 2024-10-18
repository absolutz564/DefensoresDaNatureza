using System.Collections;
using UnityEngine;

public class LixeiraController : MonoBehaviour
{
    public IEnumerator ShakeLixeira()
    {
        Debug.Log("Shake");
        Vector3 posicaoInicial = transform.position;
        float duracao = 0.3f; // Dura��o do shake
        float intensidade = 0.08f; // Intensidade do shake

        for (float t = 0; t < duracao; t += Time.deltaTime)
        {
            // Move a lixeira em uma pequena quantidade aleat�ria a cada frame
            float offsetX = Random.Range(-intensidade, intensidade);
            float offsetY = Random.Range(-intensidade, intensidade);
            transform.position = new Vector3(posicaoInicial.x + offsetX, posicaoInicial.y + offsetY, posicaoInicial.z);

            yield return null; // Espera at� o pr�ximo frame
        }

        // Retorna a lixeira � posi��o original
        transform.position = posicaoInicial;
    }
}
