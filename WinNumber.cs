using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinNumber : MonoBehaviour
{
    [SerializeField] TMP_Text winText;
    float[] amountlist = { 100, 2000, 4000, 200, 300, 10000 };
    float activeAmount;

    public void SetRandomWinAmount()
    {
        int index = Random.Range(0, amountlist.Length);
        activeAmount = amountlist[index];
        winText.text = "+" + activeAmount.ToString();
    }

    public void SetWinAmount(float amount)
    {
        activeAmount = amount;
        winText.text = "+" + activeAmount.ToString();
    }

    public float GetWinAmount()
    {
        return activeAmount;
    }
}
