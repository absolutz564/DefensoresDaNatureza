using UnityEngine;

public class AddAnimationEvent : MonoBehaviour
{
    /// Referência ao VFX que já está na cena (atribua no Inspector)
    public ParticleSystem vfx;

    // Função que será chamada pelo evento de animação
    public void PlayVFX()
    {
        if (vfx != null)
        {
            vfx.Play(); // Ativa o VFX
            Debug.Log("VFX ativado pela animação!");
        }
        else
        {
            Debug.LogWarning("ParticleSystem VFX não atribuído.");
        }
    }
}
