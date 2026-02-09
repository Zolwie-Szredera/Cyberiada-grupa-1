using UnityEngine;

public class WeaponDirectionIndicator : MonoBehaviour
{
    [Header("Indicator Settings")]
    [Tooltip("Length of the direction arrow")]
    public float arrowLength = 1.5f;
    
    [Tooltip("Color of the arrow")]
    public Color arrowColor = Color.red;
    
    [Tooltip("Show in game (not just editor)")]
    public bool showInGame = true;
    
    [Header("Arrow Offset")]
    [Tooltip("Offset from weapon position")]
    public Vector2 offsetFromWeapon = Vector2.zero;

    void OnDrawGizmos()
    {
        if (!showInGame && Application.isPlaying) return;
        
        DrawDirectionArrow();
    }
    
    void OnDrawGizmosSelected()
    {
        DrawDirectionArrow();
    }
    
    void DrawDirectionArrow()
    {
        // Get weapon's rotation
        float rotation = transform.parent != null ? transform.parent.eulerAngles.z : transform.eulerAngles.z;
        float rotationRad = rotation * Mathf.Deg2Rad;
        
        // Calculate direction based on rotation
        Vector3 direction = new Vector3(Mathf.Cos(rotationRad), Mathf.Sin(rotationRad), 0);
        
        // Start position (weapon position + offset)
        Vector3 startPos = transform.position + new Vector3(offsetFromWeapon.x, offsetFromWeapon.y, 0);
        Vector3 endPos = startPos + direction * arrowLength;
        
        // Draw main arrow line
        Gizmos.color = arrowColor;
        Gizmos.DrawLine(startPos, endPos);
        
        // Draw arrowhead
        float arrowHeadSize = 0.3f;
        float arrowHeadAngle = 25f * Mathf.Deg2Rad;
        
        // Left wing of arrowhead
        Vector3 leftWing = new Vector3(
            Mathf.Cos(rotationRad + Mathf.PI - arrowHeadAngle),
            Mathf.Sin(rotationRad + Mathf.PI - arrowHeadAngle),
            0
        ) * arrowHeadSize;
        
        // Right wing of arrowhead  
        Vector3 rightWing = new Vector3(
            Mathf.Cos(rotationRad + Mathf.PI + arrowHeadAngle),
            Mathf.Sin(rotationRad + Mathf.PI + arrowHeadAngle),
            0
        ) * arrowHeadSize;
        
        Gizmos.DrawLine(endPos, endPos + leftWing);
        Gizmos.DrawLine(endPos, endPos + rightWing);
        
        // Draw rotation text
        #if UNITY_EDITOR
        UnityEditor.Handles.color = arrowColor;
        UnityEditor.Handles.Label(endPos + Vector3.up * 0.3f, $"Rotation: {rotation:F0}°");
        #endif
    }
}

