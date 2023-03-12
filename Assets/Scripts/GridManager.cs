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
    private List<GameObject> otherTiles;
    private Dictionary<string, GameObject> attachmentsDict;
    private Dictionary<string, GameObject> directorsDict;
    private Dictionary<string, Tile> otherTilesDict;
    Array2DString arrangement;
    private int width;
    private int height;
    private AttachmentTracker attTracker;
    private Stack<Tuple<Attachment, Vector3>> attachmentsInstantiatedInsideGrid;

    // Less Imp
    private Tile myTile;
    private GameObject myGameObject;
    private string cellValue;

    [HideInInspector]
    public Dictionary<Vector3, Transform> d; // coordinate position, tile transform
    void Awake()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
        level = levelInfo.level;
        width = level.gridWidth; height = level.gridHeight;
        attachmentsDict= new Dictionary<string, GameObject>();
        directorsDict= new Dictionary<string, GameObject>();
        otherTilesDict= new Dictionary<string, Tile>();
        tile = level.tile;
        attachments = level.attachments;
        directors = level.directors;
        otherTiles = level.otherTiles;
        attachmentsInstantiatedInsideGrid = new Stack<Tuple<Attachment, Vector3>>();
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
        foreach(GameObject otherTileObj in otherTiles)
        {
            myTile = otherTileObj.GetComponent<Tile>();
            otherTilesDict.Add(myTile.GetTileId(), myTile);
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
                cellValue = arrangement.GetCell(j, i);
                if(cellValue.Contains('*'))
                {
                    attTracker = new AttachmentTracker();

                    string[] mul = cellValue.Split('*');
                    int total = Convert.ToInt32(mul[1]);
                    int wid = level.attachmentTracker.GetLength(0), hei = level.attachmentTracker.GetLength(1);
                    GameObject attObj = null;
                    Attachment att = attachmentsDict[mul[0]].GetComponent<Attachment>();
                    if (yValue != hei - 2)
                    {
                        TileInstantiate(tile, xValue, yValue);
                    }
                    for (int k = 0; k < total; k++)
                    {
                        attObj = AttachmentInstantiate(attachmentsDict[mul[0]], xValue, yValue);
                        attTracker.attachments.Add(attObj.GetComponent<Attachment>());
                    }
                    level.attachmentCountInfo[attObj.name] = total;

                    attTracker.attachmentId = att.GetAttachmentId();
                    attTracker.attachmentsCount = total;
                    attTracker.currentPosition = new Vector3(xValue, yValue);

                    if (yValue != hei - 2)
                    {
                        attachmentsInstantiatedInsideGrid.Push(Tuple.Create(attObj.GetComponent<Attachment>(), new Vector3(xValue, yValue)));
                    }

                    if (xValue >= 0 && yValue >= 0 && xValue < wid && yValue < hei)
                    {
                        level.attachmentTracker[xValue, yValue] = attTracker;
                    }
                    else
                        Debug.Log("Attachment was not tracked, attachment id = " + att.GetAttachmentId());

                    continue;
                }
                if (cellValue == null) return;

                attTracker = new AttachmentTracker();
                attTracker.attachmentsCount = 0;
                level.attachmentTracker[xValue, yValue] = attTracker;

                if (cellValue == "1")
                {
                    TileInstantiate(tile, xValue, yValue);
                }
                else if(cellValue == "End")
                {
                    TileInstantiate(level.end.GetComponent<Tile>(), xValue, yValue);

                    attTracker.occupied = true;
                    level.attachmentTracker[xValue, yValue] = attTracker;
                }
                else if(cellValue == "Hero")
                {
                    TileInstantiate(tile, xValue, yValue);

                    GameObject instantiatedHero = TileInstantiate(level.hero.GetComponent<Tile>() , xValue, yValue);
                    level.instantiatedHero = instantiatedHero;

                    attTracker.occupied = true;
                    level.attachmentTracker[xValue, yValue] = attTracker;
                }
                else if(cellValue == "Blocker")
                {
                    TileInstantiate(level.blocker.GetComponent<Tile>(), xValue, yValue);

                    attTracker.occupied = true;
                    level.attachmentTracker[xValue, yValue] = attTracker;
                }
                else if(cellValue == "2R" || cellValue == "3R" || cellValue == "2L" || cellValue == "3L")
                {
                    AttachmentInstantiate(attachmentsDict[cellValue], xValue, yValue);
                }
                else if(cellValue == "LD" || cellValue == "RD" || cellValue == "UD" || cellValue == "DD")
                {
                    DirectorInstantiate(directorsDict[cellValue], xValue, yValue);

                    attTracker = new AttachmentTracker();
                    attTracker.occupied = true;
                    level.attachmentTracker[xValue, yValue] = attTracker;
                }
                else if(cellValue == "Freezer")
                {
                    myGameObject = TileInstantiate(otherTilesDict[cellValue], xValue, yValue);

                    attTracker = new AttachmentTracker();
                    if (IsCoordinateInsideTileGrid(new Vector3(xValue, yValue)))
                    {
                        attTracker.occupied = true;
                        myGameObject.GetComponent<Draggable>().isDraggable = false;
                    }
                    else
                    {
                        attTracker.occupied = false;
                        myGameObject.GetComponent<Draggable>().isDraggable = true;
                    }
                    level.attachmentTracker[xValue, yValue] = attTracker;
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

    GameObject AttachmentInstantiate(GameObject attachement, int i, int j)
    {
        float positionX = (width % 2 == 1) ? i - (width / 2) : (i - (width / 2) + 0.5f);
        float positionY = (height % 2 == 1) ? j - (height / 2) : (j - (height / 2) + 0.5f);
        var attachmentObject = Instantiate(attachement, new Vector3(positionX, positionY), Quaternion.identity, parent);
        attachmentObject.name = $"Attachment {i} {j}";
        attachmentObject.GetComponent<Attachment>().Init();
        attachments.Add((GameObject)attachmentObject);

        d[new Vector3(i, j)] = attachmentObject.transform;

        return attachmentObject;
    }
    GameObject TileInstantiate(Tile tile, int i, int j)
    {
        float positionX, positionY;
        ConvertTileCoordinateToWorldPosition(new Vector3(i, j), out positionX, out positionY);
        
        var tileObject = Instantiate(tile, new Vector3(positionX, positionY), Quaternion.identity, parent);
        tileObject.name = $"Tile {i} {j}";

        var isOffset = ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0));
        tileObject.Init(isOffset);

        d[new Vector3(i, j)] = tileObject.transform;

        return tileObject.gameObject;
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
    public Vector3 ConvertWorldPositionToTileCoordinate(Vector3 pos)
    {
        float x, y;
        ConvertWorldPositionToTileCoordinate(pos, out x, out y);
        return new Vector3(x, y);
    }
    public void ConvertTileCoordinateToWorldPosition(Vector3 pos, out float x, out float y)
    {
        x = (width % 2 == 1) ? pos.x - (width / 2) : (pos.x - (width / 2) + 0.5f);
        y = (height % 2 == 1) ? pos.y - (height / 2) : (pos.y - (height / 2) + 0.5f);
    }
    public Vector3 ConvertTileCoordinateToWorldPosition(Vector3 coordinate)
    {
        float x, y;
        x = (width % 2 == 1) ? coordinate.x - (width / 2) : (coordinate.x - (width / 2) + 0.5f);
        y = (height % 2 == 1) ? coordinate.y - (height / 2) : (coordinate.y - (height / 2) + 0.5f);
        return new Vector3(x, y);
    }

    void PostGridCreate() // executes after grid creates
    {
        level.instantiatedAttachments = attachments;
        foreach(GameObject go in attachments)
        {
            go.GetComponent<Attachment>().RefreshAttachmentCount();
        }
        Tuple<Attachment, Vector3> tuple;
        while (attachmentsInstantiatedInsideGrid.Count > 0)
        {
            tuple = attachmentsInstantiatedInsideGrid.Peek();
            tuple.Item1.isDraggable = false;
            tuple.Item1.SetOccupiedStatus(tuple.Item2, true, this);
            attachmentsInstantiatedInsideGrid.Pop();
        }
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

    public bool IsPositionInsideTileGrid(Vector3 pos)
    {
        float x, y;
        ConvertWorldPositionToTileCoordinate(pos, out x, out y);
        return x >= 0 && y >= 1 && x < width && y < height - 2;
    }
    public bool IsCoordinateInsideTileGrid(Vector3 coordinate)
    {
        float x = coordinate.x, y = coordinate.y;
        return x >= 0 && y >= 1 && x < width && y < height - 2;
    }

    public bool IsPositionInsideAttachmentGrid(Vector3 pos)
    {
        float x, y;
        ConvertWorldPositionToTileCoordinate(pos, out x, out y);
        return x >= 0 && x < width && y == height - 2;
    }
    public bool IsCoordinateInsideAttachmentGrid(Vector3 pos)
    {
        float x = pos.x, y = pos.y;
        return x >= 0 && x < width && y == height - 2;
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
