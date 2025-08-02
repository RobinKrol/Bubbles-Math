using UnityEngine;
using TMPro;

public class MainNumberManager : MonoBehaviour
{
    public TextMeshProUGUI mainNumberText;
    [HideInInspector] public int mainNumber;
    
    void Start ()
    {
       
    }
    
    public void SetMainNumber(int value)
    {
        mainNumber = value;
        UpdateMainNumberText();
    }
    
    public void UpdateMainNumberText()
    {
        if (mainNumberText != null)
            mainNumberText.text = mainNumber.ToString();
    }
}
