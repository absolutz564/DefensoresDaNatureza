using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Referência à câmera
    private Transform cameraTransform;

    void Start()
    {
        // Pega a câmera principal
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Faz o objeto olhar para a câmera
        transform.LookAt(cameraTransform);
    }
}
