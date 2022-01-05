using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentZ { set; get; }
    public int score;
    public bool isWhite;
    public int value;
    public void setposition(int x, int z)
    {
        CurrentX = x;
        CurrentZ = z;
    }
    public virtual void move(Vector3 pos)
    {
        
    }
    public virtual bool[,] possibleMove()
    {

        return new bool[8,8];
    }
    public virtual List<int[]> minmaxPossibleMoves()
    {
        Chessman c, c2;
        bool[,] r = new bool[8, 8];
        List<int[]> possibleMoves = new List<int[]>();
        int[] move = new int[4];
        r = possibleMove();


        for (int x2 = 0; x2 < 8; x2++)
        {
            for (int z2 = 0; z2 < 8; z2++)
            {
                if (r[x2, z2] == true)  //  && Chessmans[x, z].isWhite != isWhiteTurn
                {
                    move[0] = CurrentX; move[1] = CurrentZ;
                    move[2] = x2; move[3] = z2;
                    possibleMoves.Add(move);
                }
            }
        }

        return possibleMoves;
    }
}
