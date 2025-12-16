using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TilemapButton : MonoBehaviour
{
    public PainterMode mode;
    public int index;
    public Button button { get; private set; }

    public void Init()
    {
        button = GetComponent<Button>();
    }
}