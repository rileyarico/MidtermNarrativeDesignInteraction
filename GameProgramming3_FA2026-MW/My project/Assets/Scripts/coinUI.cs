using TMPro;
using UnityEngine;

public class coinUI : MonoBehaviour
{
    public TextMeshProUGUI coinAmtText;

    private void Awake()
    {
        GameController.instance.setUI(this);
    }

    public void addCoins(int coin)
    {
        coinAmtText.text = coin.ToString();
    }
}
