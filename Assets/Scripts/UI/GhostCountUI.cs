using TMPro;
using UnityEngine;

public class GhostCountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text ghostCountText;

    private void Start()
    {
        UpdateUI(0);
        GhostManager.instance.onGhostsChanged += UpdateUI;
    }

    private void UpdateUI(int count)
    {
        ghostCountText.text = count.ToString();
    }
}
