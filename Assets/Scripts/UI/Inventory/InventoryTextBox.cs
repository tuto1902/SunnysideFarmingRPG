using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryTextBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textTop1 = null;
    [SerializeField] TextMeshProUGUI textTop2 = null;
    [SerializeField] TextMeshProUGUI textTop3 = null;
    [SerializeField] TextMeshProUGUI textBottom1 = null;
    [SerializeField] TextMeshProUGUI textBottom2 = null;
    [SerializeField] TextMeshProUGUI textBottom3 = null;

    public void SetTextBoxes(string top1, string top2, string top3, string bottom1, string bottom2, string bottom3)
    {
        textTop1.text = top1;
        textTop2.text = top2;
        textTop3.text = top3;
        textBottom1.text = bottom1;
        textBottom2.text = bottom2;
        textBottom3.text = bottom3;
    }
}
