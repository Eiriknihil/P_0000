using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f; // Duración del trail
    public float meshRefreshRate = 0.1f; // Frecuencia de actualización del mesh
    public float meshDestroyDelay = 3f; // Tiempo antes de destruir el mesh
    public Transform positionToSpawn; // Posición donde se genera el trail

    [Header("Shader Related")]
    public Material material; // Material para el trail
    public string shaderVarRef; // Referencia a la variable del shader
    public float shaderVarRate = 0.1f; // Tasa de cambio de la variable del shader
    public float shaderVarRefreshRate = 0.05f; // Frecuencia de actualización del shader

    private bool isTrailActive; // Indica si el trail está activo
    private SkinnedMeshRenderer[] skinnedMeshRenderers; // Renderers para el trail

    public void ActivateTrail()
    {
        if (!isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrailCoroutine(activeTime));
        }
    }

    IEnumerator ActivateTrailCoroutine(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            // Generar el trail para cada SkinnedMeshRenderer
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject trailObject = new GameObject();
                trailObject.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer meshRenderer = trailObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = trailObject.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                meshFilter.mesh = mesh;
                meshRenderer.material = material;

                // Animar el material del trail
                StartCoroutine(AnimateMaterialFloat(meshRenderer.material, 0, shaderVarRate, shaderVarRefreshRate));

                // Destruir el objeto del trail después de un tiempo
                Destroy(trailObject, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false; // Finalizar el trail
    }

    IEnumerator AnimateMaterialFloat(Material material, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = material.GetFloat(shaderVarRef);

        // Animar la variable del shader
        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            material.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}

/*using System.Collections;
using Unity.IntegerTime;
using Unity.Mathematics;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material material;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    
    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)&& !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; i < skinnedMeshRenderers.Length; i++) 
            {
                GameObject gameObject = new GameObject();
                gameObject.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);


                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);


                meshFilter.mesh = mesh;
                meshRenderer.material = material;

                StartCoroutine(AnimateMaterialFloat(meshRenderer.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy (gameObject, meshDestroyDelay);
            }    

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat (Material material, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = material.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            material.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds (refreshRate);
        }
    }
}
*/