using TMPro;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    
    private void Start()
    {
        HideOverlay();
    }
    
    public void SetOverlayTo(OverlayInfo info)
    {
        gameObject.SetActive(true);
        SetTitle(info.title);
        SetDescription(info.description);
        transform.position = info.transform.position;
    }

    public void HideOverlay()
    {
        gameObject.SetActive(false);
    }

    private void SetTitle(string title)
    {
        titleText.text = title;
    }

    private void SetDescription(string description)
    {
        descriptionText.text = description;
    }
}