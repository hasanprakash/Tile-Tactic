using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    
    public void Init(bool isOffset)
    {
        renderer.color = (isOffset) ? offsetColor : baseColor;
    }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }
    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
