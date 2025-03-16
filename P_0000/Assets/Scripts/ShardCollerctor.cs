using UnityEngine;
using TMPro;

public class ShardCollector : MonoBehaviour
{
    public static int shardCount = 0; // Contador global de fragmentos
    public TextMeshProUGUI shardText; // Referencia al TextMeshPro en la UI

    private void Start()
    {
        UpdateUI(); // Asegurar que la UI inicie correctamente
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shardCount++; // Sumar 1 al contador
            UpdateUI(); // Actualizar la UI
            Destroy(gameObject); // Destruir el fragmento recogido
        }
    }

    private void UpdateUI()
    {
        if (shardText != null)
        {
            shardText.text = shardCount.ToString(); // Mostrar solo el número
        }
    }
}
