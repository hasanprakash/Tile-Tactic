using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

[CreateAssetMenu(fileName = "New Level", menuName = "Create Level")]
public class Level : ScriptableObject
{
    [SerializeField] public int gridWidth;
    [SerializeField] public int gridHeight;
    [SerializeField] public Tile tile;
    [SerializeField] public List<GameObject> attachments;
    [SerializeField] public List<GameObject> directors;
    [SerializeField] public Array2DString arrangement;
    [HideInInspector] public List<GameObject> instantiatedAttachments;

    public Array2DString GetArrangement()
    {
        return arrangement;
    }
}
