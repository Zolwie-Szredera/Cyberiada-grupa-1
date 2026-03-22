using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DisableTutorial : MonoBehaviour
{
    public GameObject tutorialPanel;
    private BoxCollider2D boxCollider;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tutorialPanel.SetActive(false);
        }
    }
    public void OnDrawGizmos()
    {
        //draw the box collider in the editor for better visualization
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
