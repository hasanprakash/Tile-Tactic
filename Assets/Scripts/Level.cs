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
    [SerializeField] public GameObject hero;
    [SerializeField] public GameObject end;
    [SerializeField] public List<GameObject> attachments;
    [SerializeField] public List<GameObject> directors;
    [SerializeField] public Array2DString arrangement;
    [HideInInspector] public List<GameObject> instantiatedAttachments;
    [HideInInspector] public Dictionary<string, int> attachmentCountInfo = new Dictionary<string, int>();
    [HideInInspector] public AttachmentTracker[,] attachmentTracker = new AttachmentTracker[4, 9];

    public Array2DString GetArrangement()
    {
        return arrangement;
    }

    public int GetAttachmentCount(string goName)
    {
        if(attachmentCountInfo.ContainsKey(goName))
        {
            return attachmentCountInfo[goName];
        }
        return -1;
    }
}
