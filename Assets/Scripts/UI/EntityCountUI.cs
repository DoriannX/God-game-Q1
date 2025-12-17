using TMPro;
using UnityEngine;

public class EntityCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI entityCountText;
    [SerializeField] private EntityType entity;

    private void Start() {
        UpdateUI(entity, 0);
        EntityManager.instance.onEntityChanged += UpdateUI;
    }

    private void UpdateUI(EntityType entityType, int count) {
        if (entityType == entity) {
            entityCountText.text = count.ToString();
        }
    }
}
