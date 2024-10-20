using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip maleClip;
    public AudioClip femaleClip;

    private void Awake()
    {
        // Verifica se o sexo salvo � masculino ou feminino
        if (PlayerPrefs.GetString("Sex") == "Male")
        {
            // Se for masculino, define o AudioClip masculino
            audioSource.clip = maleClip;
        }
        else
        {
            // Se for feminino, define o AudioClip feminino
            audioSource.clip = femaleClip;
        }

        // Opcional: Toca o som quando o �udio � atribu�do
        audioSource.Play();
    }
}
