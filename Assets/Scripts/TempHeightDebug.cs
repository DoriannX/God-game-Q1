using UnityEngine;

public class TempHeightDebug : MonoBehaviour
{
    private Vector3 worldMousePosition;
    private float currentHeight;
    private bool hasValidPosition;
        
    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
            
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            worldMousePosition = hitInfo.point;
            Vector3Int cell = TilemapManager.instance.WorldToHexAxial(hitInfo.point);
            Vector2Int columnKey = new Vector2Int(cell.x, cell.y);
            currentHeight = TilemapManager.instance.GetColumnTopCoordinate(columnKey);
            hasValidPosition = true;
        }
        else
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                worldMousePosition = ray.GetPoint(distance);
                Vector3Int cell = TilemapManager.instance.WorldToHexAxial(worldMousePosition);
                Vector2Int columnKey = new Vector2Int(cell.x, cell.z);
                currentHeight = TilemapManager.instance.GetColumnTopCoordinate(columnKey);
                hasValidPosition = true;
            }
            else
            {
                hasValidPosition = false;
            }
        }
    }
        
    private void OnGUI()
    {
        if (!hasValidPosition) return;
            
        // Display height text near the mouse cursor
        Vector3 screenPos = Input.mousePosition;
        screenPos.y = Screen.height - screenPos.y; // Flip Y coordinate for GUI
            
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
            
        // Add black outline for better visibility
        GUIStyle outlineStyle = new GUIStyle(style);
        outlineStyle.normal.textColor = Color.black;
            
        string text = $"Height: {currentHeight:F2}";
        Vector2 textSize = style.CalcSize(new GUIContent(text));
            
        // Position text above the cursor
        Rect textRect = new Rect(screenPos.x - textSize.x / 2, screenPos.y - 30, textSize.x, textSize.y);
            
        // Draw outline
        GUI.Label(new Rect(textRect.x - 1, textRect.y - 1, textRect.width, textRect.height), text, outlineStyle);
        GUI.Label(new Rect(textRect.x + 1, textRect.y - 1, textRect.width, textRect.height), text, outlineStyle);
        GUI.Label(new Rect(textRect.x - 1, textRect.y + 1, textRect.width, textRect.height), text, outlineStyle);
        GUI.Label(new Rect(textRect.x + 1, textRect.y + 1, textRect.width, textRect.height), text, outlineStyle);
            
        // Draw main text
        GUI.Label(textRect, text, style);
    }
        
    private void OnDrawGizmos()
    {
        if (!hasValidPosition) return;
            
        // Draw cross at world mouse position
        Gizmos.color = Color.yellow;
        float crossSize = 0.5f;
            
        // Horizontal line
        Gizmos.DrawLine(worldMousePosition - Vector3.right * crossSize, 
            worldMousePosition + Vector3.right * crossSize);
            
        // Vertical line
        Gizmos.DrawLine(worldMousePosition - Vector3.up * crossSize, 
            worldMousePosition + Vector3.up * crossSize);
            
        // Forward/Back line
        Gizmos.DrawLine(worldMousePosition - Vector3.forward * crossSize, 
            worldMousePosition + Vector3.forward * crossSize);
    }
}