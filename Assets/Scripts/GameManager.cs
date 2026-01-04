using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //use this script to make references to this useful in many other scripts
    [HideInInspector] public Vector2 mousePosition;
    public void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}