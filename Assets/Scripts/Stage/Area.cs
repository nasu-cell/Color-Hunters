using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Area : MonoBehaviour
{
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private float captureRate;
    [SerializeField] private float lostRate;
    [SerializeField] private PlayerStatus[] players;

    private int ownerId = -1;
    private Color ownerColor;
    private bool isAreaActive;
    public int OwnerId => ownerId;
    public Color OwnerColor => ownerColor;
    public bool IsAreaActive => isAreaActive;
    
    void Start()
    {
        
    }

    void Update()
    {
       if(ownerId == -1)
        {
            if (CountPaintRate(0) >= captureRate)
            {
                ownerId = 0;
                isAreaActive = true;
                CaptureArea(0, players[0].PlayerColor);
            }
            else if (CountPaintRate(1) >= captureRate)
            {
                ownerId = 1;
                isAreaActive = true;
                CaptureArea(1, players[1].PlayerColor);
            }
        }
       else if(ownerId == 0)
        {
            if(CountPaintRate(0) <= lostRate)
            {
                ownerId = -1;
                isAreaActive = false;
            }
        }
        else
        {
            if(CountPaintRate(1) <= lostRate)
            {
                ownerId = -1;
                isAreaActive = false;
            }
        }
       
    }

    private float CountPaintRate(int id)
    {
        if (tiles.Count == 0) return 0f;

        int tileCount = 0;
        foreach(Tile tile in tiles)
        {
            if(tile.OwnerId == id)
            {
                tileCount++;
            }
        }
        return (float) tileCount / tiles.Count;
    }

    private void CaptureArea(int id, Color color)
    {
        foreach(Tile tile in tiles)
        {
            tile.Paint(id, color);
        }
    }
}
