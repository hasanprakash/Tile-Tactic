using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [SerializeField] private string tileId;
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    
    public void Init(bool isOffset)
    {
        renderer.color = (isOffset) ? offsetColor : baseColor;
    }

    public string GetTileId()
    {
        return tileId;
    }
}
