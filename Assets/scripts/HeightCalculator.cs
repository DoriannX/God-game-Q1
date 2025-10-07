using UnityEngine;
using UnityEngine.Tilemaps;

public class HeightCalculator : MonoBehaviour
{
    [SerializeField] private TileBase tileToCheck;
    [SerializeField] private HeightManager heightManager;
    #if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(HeightCalculator))]
        public class HeightCalculatorEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
    
                if (GUILayout.Button("Calculer la hauteur"))
                {
                    ((HeightCalculator)target).CalculateHeight();
                }
            }
        }
    #endif
    
    public void CalculateHeight()
    {
        print(heightManager.GetHeightIndex(tileToCheck));
    }
}
