using TMPro;
using UnityEngine;

public class VictoryPanel : MonoBehaviour
{
    public TextMeshProUGUI popupText;

    void Start()
    {
        gameObject.SetActive(false); // hidden at start
    }

    public void ShowMessage(string msg)
    {
        popupText.text = msg;
        gameObject.SetActive(true);
    }
}
