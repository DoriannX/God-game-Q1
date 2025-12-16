using UnityEngine;

public class TIpsoverlay : MonoBehaviour
{
    [SerializeField] private GameObject overlayRoot;

    private GameObject Root => overlayRoot != null ? overlayRoot : gameObject;

    // Sans paramètre : bascule l'état (appelable depuis un bouton On Click)
    public void SetVisible() => SetVisible(!Root.activeSelf);

    // Avec paramètre : force l'état visible/invisible (appelable depuis code ou depuis On Click(Boolean))
    public void SetVisible(bool visible) => Root.SetActive(visible);

    public bool IsVisible => Root.activeSelf;
}