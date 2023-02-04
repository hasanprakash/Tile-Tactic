using Array2DEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Transform parent;
    private LevelInfo levelInfo;
    Level level;
    private Tile tile;
    private List<GameObject> attachments;
    private List<GameObject> directors;
    private Dictionary<string, GameObject> attachmentsDict;
    private Dictionary<string, GameObject> directorsDict;
    Array2DString arrangement;
    private int width;
    private int height;

    [HideInInspector]
    public Dictionary<Vector3, Transform> d; // coordinate position, tile transform
    void Start()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
        level = levelInfo.level;
        width = level.gridWidth; height = level.gridHeight;
        attachmentsDict= new Dictionary<string, GameObject>();
        directorsDict= new Dictionary<string, GameObject>();
        tile = level.tile;
        attachments = level.attachments;
        directors = level.directors;
        d = new Dictionary<Vector3, Transform>();
        arrangement = level.GetArrangement();
        foreach(GameObject attObj in attachments)
        {
            Attachment attachment = attObj.GetComponent<Attachment>();
            attachmentsDict.Add(attachment.GetAttachmentId(), attObj);
        }
        foreach(GameObject dirObj in directors)
        {
            directorsDict.Add(dirObj.GetComponent<Director>().GetDirectorId(), dirObj);
        }
        attachments = new List<GameObject>();
        /*for(int i=arrangement.GridSize.y-1;i>=0;i--)
        {
            for(int j=0;j<arrangement.GridSize.x;j++)
            {
                // j => width, i => height
                //Debug.Log(arrangement.GetCell(j, i));
            }
        }*/
        renderGrid();
    }

    void renderGrid()
    {
        int xValue = 0, yValue = 0;
        for(int i=height-1;i>=0;i--)
        {
            for(int j=0;j<width;j++)
            {
                xValue = j; yValue = height - i - 1;
                string value = arrangement.GetCell(j, i);
                if(value.Contains('*'))
                {
                    string[] mul = value.Split('*');
                    Debug.Log(mul[0] + " " + mul[1]);
                    for (int k = 0; k < Convert.ToInt32(mul[1]); k++)
                        AttachmentInstantiate(attachmentsDict[mul[0]], xValue, yValue);
                    continue;
                }
                if (value == null) return;
                if(value == "1")
                {
                    TileInstantiate(tile, xValue, yValue);
                }
                else if(value == "2R" || value == "3R")
                {
                    AttachmentInstantiate(attachmentsDict[value], xValue, yValue);
                }
                else if(value == "Hero")
                {
                    AttachmentInstantiate(attachmentsDict[value], xValue, yValue);
                }
                else if(value == "End")
                {
                    AttachmentInstantiate(attachmentsDict[value], xValue, yValue);
                }
                else if(value == "LD" || value == "RD" || value == "UD" || value == "DD")
                {
                    DirectorInstantiate(directorsDict[value], xValue, yValue);
                }
            }
        }
        /*for(int i=0;i<width; i++)
        {
            for(int j=0;j<height; j++)
            {
                if (j == 7) continue;
                if(j == 8)
                {
                    if (i < attachments.Count)
                        AttachmentInstantiate(attachments[i], i, j);
                    continue;
                }
                TileInstantiate(tile, i, j);
            }
        }*/
        PostGridCreate();
    }

    void AttachmentInstantiate(GameObject attachement, int i, int j)
    {
        float positionX = (width % 2 == 1) ? i - (width / 2) : (i - (width / 2) + 0.5f);
        float positionY = (height % 2 == 1) ? j - (height / 2) : (j - (height / 2) + 0.5f);
        var attachmentObject = Instantiate(attachement, new Vector3(positionX, positionY), Quaternion.identity, parent);
        attachmentObject.name = $"Attachment {i} {j}";
        attachmentObject.GetComponent<Attachment>().Init();
        attachments.Add((GameObject)attachmentObject);

        d[new Vector3(i, j)] = attachmentObject.transform;
    }
    void TileInstantiate(Tile tile, int i, int j)
    {
        float positionX, positionY;
        ConvertTileCoordinateToWorldPosition(new Vector3(i, j), out positionX, out positionY);
        
        var tileObject = Instantiate(tile, new Vector3(positionX, positionY), Quaternion.identity, parent);
        tileObject.name = $"Tile {i} {j}";

        var isOffset = ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0));
        tileObject.Init(isOffset);

        d[new Vector3(i, j)] = tileObject.transform;
    }
    void DirectorInstantiate(GameObject director, int i, int j)
    {
        float positionX, positionY;
        ConvertTileCoordinateToWorldPosition(new Vector3(i, j), out positionX, out positionY);
        var directorObj = Instantiate(director, new Vector3(positionX, positionY), Quaternion.identity, parent);
        directorObj.name = $"Director {i} {j}";

        d[new Vector3(i, j)] = directorObj.transform;
    }

    public void ConvertInputPositionToTileCoordinate(Vector3 pos, out float x, out float y)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(pos);
        x = (width % 2 == 0) ? (worldPosition.x + width / 2) - 0.5f : (worldPosition.x + width / 2);
        y = (height % 2 == 0) ? (worldPosition.y + height / 2) - 0.5f : (worldPosition.y + height / 2);
        x = Mathf.Round(x);
        y = Mathf.Round(y);
    }
    public void ConvertWorldPositionToTileCoordinate(Vector3 pos, out float x, out float y)
    {
        x = (width % 2 == 0) ? (pos.x + width / 2) - 0.5f : (pos.x + width / 2);
        y = (height % 2 == 0) ? (pos.y + height / 2) - 0.5f : (pos.y + height / 2);
        x = Mathf.Round(x);
        y = Mathf.Round(y);
    }
    public void ConvertTileCoordinateToWorldPosition(Vector3 pos, out float x, out float y)
    {
        x = (width % 2 == 1) ? pos.x - (width / 2) : (pos.x - (width / 2) + 0.5f);
        y = (height % 2 == 1) ? pos.y - (height / 2) : (pos.y - (height / 2) + 0.5f);
    }

    void PostGridCreate() // executes after grid creates
    {
        level.instantiatedAttachments = attachments;
    }

    public Vector3 ReorderedPosition(Vector3 pos)
    {
        float x, y;
        ConvertWorldPositionToTileCoordinate(pos, out x, out y);
        ConvertTileCoordinateToWorldPosition(new Vector3(x, y), out x, out y);
        return new Vector3(x, y);
    }
    public Vector3 GetTileCoordinate(Vector3 pos)
    {
        float x, y;
        ConvertWorldPositionToTileCoordinate(pos, out x, out y);
        return new Vector3(x, y);
    }

    public int GetGridWidth()
    {
        return width;
    }
    public int GetGridHeight()
    {
        return height;
    }
}
