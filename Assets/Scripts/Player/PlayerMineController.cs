using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class PlayerMineController : MonoBehaviour
{
    [SerializeField] private int maxMineCount;
    private Queue<PaintMine> mines = new Queue<PaintMine>();
    public void AddMine(PaintMine mine)
    {
        mines.Enqueue(mine);
        if(mines.Count > maxMineCount)
        {
            PaintMine destroyMine = mines.Dequeue();
            Destroy(destroyMine.gameObject);
        }
    }
    public void RemoveMine(PaintMine mine)
    {
        Queue<PaintMine> newMines = new Queue<PaintMine>();

        foreach (PaintMine currentMine in mines)
        {
            if (currentMine != mine)
            {
                newMines.Enqueue(currentMine);
            }
        }

        mines = newMines;
    }
}
