using TMPro;
using UnityEngine;

public class bunnyUI : MonoBehaviour
{
    public TextMeshProUGUI bunnyAmtText;

    private void Awake()
    {
        //GameController.instance.setBunnyUI(this);
        GameController gC = FindAnyObjectByType<GameController>();  
        gC.setBunnyUI(this);
    }

    public void addBuns(int bun)
    {
        bunnyAmtText.text = bun.ToString();
    }
}
