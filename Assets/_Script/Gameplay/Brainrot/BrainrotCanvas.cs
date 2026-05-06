using TMPro;
using UnityEngine;

public class BrainrotCanvas : MonoBehaviour
{
    public TMP_Text TimeTMP;
    public TMP_Text ModifierTMP;
    public TMP_Text NameTMP;
    public TMP_Text RarityTMP;
    public TMP_Text ValueTMP;

    public void DisableCanvas()
    {
        gameObject.SetActive(false);
    }

    public void EnableCanvas()
    {
        gameObject.SetActive(true);
    }

}
