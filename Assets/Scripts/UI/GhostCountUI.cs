using TMPro;
using UnityEngine;

public class GhostCountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text ghostCountText;

    private void Start()
    {
        UpdateUI(EntityType.Ghost, 0);
        EntityManager.instance.onEntityChanged += UpdateUI;
    }

    private void UpdateUI(EntityType entityType, int count)
    {
        ghostCountText.text = count.ToString();
    }
}
