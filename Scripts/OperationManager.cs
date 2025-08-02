using UnityEngine;
using TMPro;
public class OperationManager : MonoBehaviour
{
    public TextMeshProUGUI operationText;
    [HideInInspector] public string currentOperation;

    private static readonly string[] operations = { "+", "-", "*", "/" };
    
    void Start()
    {
        GenerateOperation();
    }

    public void GenerateOperation()
    {
        int index = Random.Range(0, operations.Length);
        currentOperation = operations[index];
        UpdateOperationText();
    }

    public void UpdateOperationText()
    {
        if (operationText != null)
            operationText.text = currentOperation;
    }

}
