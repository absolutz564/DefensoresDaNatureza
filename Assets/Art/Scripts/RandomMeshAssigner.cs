using UnityEngine;
using System.Collections.Generic;

public class RandomMeshAssigner : MonoBehaviour
{
    // Lista de meshes específicas para este tipo de lixo reciclável
    public Mesh[] meshes;

    // O MeshFilter do objeto onde a mesh será aplicada
    public MeshFilter targetMeshFilter;

    // Lista temporária para rastrear as meshes disponíveis
    private List<Mesh> availableMeshes = new List<Mesh>();

    void Start()
    {
        // Inicializa a lista temporária com as meshes específicas
        availableMeshes.AddRange(meshes);

        // Verifica se o MeshFilter e a lista de meshes estão configurados
        if (availableMeshes.Count > 0 && targetMeshFilter != null)
        {
            // Seleciona uma mesh aleatória da lista temporária
            int randomIndex = Random.Range(0, availableMeshes.Count);
            Mesh randomMesh = availableMeshes[randomIndex];

            // Atribui a mesh ao MeshFilter
            targetMeshFilter.mesh = randomMesh;

            // Remove a mesh selecionada da lista temporária para evitar repetições
            availableMeshes.RemoveAt(randomIndex);

            Debug.Log("Mesh aleatória atribuída com sucesso.");
        }
        else
        {
            Debug.LogWarning("O MeshFilter não foi atribuído ou não há meshes disponíveis.");
        }
    }
}
