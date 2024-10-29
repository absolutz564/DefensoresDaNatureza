using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip maleClip;
    public AudioClip femaleClip;
    public GameObject MaleMessage;
    public GameObject FemaleMessage;
    private void Awake()
    {
        // Verifica se o sexo salvo é masculino ou feminino
        if (PlayerPrefs.GetString("Sex") == "Male")
        {
            // Se for masculino, define o AudioClip masculino
            audioSource.clip = maleClip;
            if (MaleMessage != null ) { 
                MaleMessage.SetActive(true);
            }
        }
        else
        {
            // Se for feminino, define o AudioClip feminino
            audioSource.clip = femaleClip;
            if (FemaleMessage != null)
            {
                FemaleMessage.SetActive(true);
            }
        }

        // Opcional: Toca o som quando o áudio é atribuído
        audioSource.Play();
    }
}
