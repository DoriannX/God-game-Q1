using DG.Tweening;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SaveUI : MonoBehaviour
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(SaveUI))]
    public class SaveUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
    
            SaveUI saveUI = (SaveUI)target;
            if (GUILayout.Button("Display"))
            {
                saveUI.Display();
            }
        }
    }
    #endif
    private TMP_Text saveText;

    private void Awake()
    {
        saveText = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    public void Display()
    {
        gameObject.SetActive(true);
        Color color = saveText.color;
        color.a = 1f;
        saveText.color = color;
        color.a = 0f;
        saveText.DOColor( color, 2f).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
