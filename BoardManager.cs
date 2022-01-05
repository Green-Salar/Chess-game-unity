using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Random = UnityEngine.Random;
public class BM : MonoBehaviour
{

    public static BM Instance { set; get; }
    private bool[,] allowedMoves { set; get; }
    private List<int[]> minmaxPossibleMoves = new List<int[]>();
    private List<int[]> minmaxOWW = new List<int[]>();
    private int currentEval, j = 0;
    public bool isWhiteTurn = true;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = .5f;
    //public int maxEval = -9000;
    //public int minEval = 9000;
    private int selectionX = -1;
    private int selectionY = -1;
    public int dept = 3;
    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;
    private int lt;

    private Quaternion orientation = Quaternion.Euler(0, 180, 0);
    public Chessman[,] Chessmans { set; get; }
    public Chessman[,] minmaxChessmans { set; get; }
    public Chessman selectedChessman;
    private int[] bestMove;
    private Vector3 targetPosition;
    private bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        spawnAllChessman();


    }
    // Update is called once per frame
    void Update()
    {
        DrawChessBoard();
        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (isWhiteTurn)
            {
                if (selectionX >= 0 && selectionY >= 0 && isMoving == false)
                {
                    if (selectedChessman == null)
                    {
                        //selet chessman
                        selectChessman(selectionX, selectionY);
                    }
                    else
                    {
                        moveChessman(selectionX, selectionY);
                        //selectedChessman = null;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        minmaxChessmans[i, j] = null;
                        if (Chessmans[i, j] != null)  //  && Chessmans[x, z].isWhite != isWhiteTurn
                        {

                            minmaxChessmans[i, j] = Chessmans[i, j];
                        }
                    }
                }
                int[] b = new int[5];
                int l = 0;
                int max = -3000;
                minmaxOWW.Clear();
                lt = 0;
                b = minimax(minmaxChessmans, dept, -9000, +9000, true, false);
                for (int m = 0; m < minmaxOWW.Count; m++)
                {
                    if (minmaxOWW[m][0] > max)
                        Debug.Log(minmaxOWW[m][0]);
                }
                minimaxMoveChessman(minmaxOWW[l][1], minmaxOWW[l][2], minmaxOWW[l][3], minmaxOWW[l][4]);

            }
        }

        // else
        // {
        //MinMax();
        // moveChessman(selectionX, selectionY);
        // }
    }

    // for moving the selected chessamn to the new position
    public void moveChessman(int x, int z)
    {
        if (allowedMoves[x, z])
        {
            Chessman c = Chessmans[x, z];

            if (c != null && c.isWhite != isWhiteTurn)
            {
                c.gameObject.tag = "TargetEnemy";
                //capture
                if (c.GetType() == typeof(King))
                {
                    //endgame
                    endGame();
                    return;
                }
                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject, 2f);
            }

            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentZ] = null;

            targetPosition = gettileCenter(x, z);

            selectedChessman.setposition(x, z);
            selectedChessman.transform.position = targetPosition;
            Chessmans[x, z] = selectedChessman;

            if (isWhiteTurn == false)
            {

                Chessman[,] temp = new Chessman[8, 8];

                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        temp[i, j] = Chessmans[i, j];
                    }

            }

            isWhiteTurn = !isWhiteTurn;


            //foundAMove = true;
            //isMoving = true;

        }

        BoardHighlights.Instance.hideHighlights();

        if (isMoving == false)
            selectedChessman = null;


    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            // Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                // Debug.DrawLine(start, start + heightLine);
            }

        }

        //Draw the Selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    public void minimaxMoveChessman(int x1, int z1, int x2, int z2)
    {
        Chessman a = Chessmans[x1, z1];
        Chessman c = Chessmans[x2, z2];

        if (c != null && c.isWhite != isWhiteTurn)
        {
            c.gameObject.tag = "TargetEnemy";
            //capture
            if (c.GetType() == typeof(King))
            {
                //endgame
                endGame();
                return;
            }
            activeChessman.Remove(c.gameObject);
            Destroy(c.gameObject, 2f);
        }

        Chessmans[x1, z1] = null;

        targetPosition = gettileCenter(x2, z2);

        a.setposition(x2, z2);
        a.transform.position = targetPosition;
        Chessmans[x2, z2] = a;

        if (isWhiteTurn == false)
        {

            Chessman[,] temp = new Chessman[8, 8];


            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    temp[i, j] = Chessmans[i, j];
                }


        }

        isWhiteTurn = !isWhiteTurn;


        //foundAMove = true;
        //isMoving = true;

        BoardHighlights.Instance.hideHighlights();


        selectedChessman = null;



    }

    public int[] minimax(Chessman[,] tempChessmans, int depth, int maxmax, int minmin, bool maximizing_player, bool isWhiteturn)
    {
        int[] aa = new int[5];
        //minimaxChessmanss
        // Chessman[,] tempChessmans = new Chessman[8, 8];
        List<int[]> moves = new List<int[]>();
        List<int[]> maximoves = new List<int[]>();
        List<int[]> minimoves = new List<int[]>();
        int[] bb = new int[5];
        if (depth == 0) //board.gameover..
        {
            aa[0] = evaluate(tempChessmans, isWhiteturn);
            for (int k = 1; k < 5; k++) aa[k] = 0;
            return aa;
        }
        // moves ...
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (tempChessmans[i, j] != null)  //  && Chessmans[x, z].isWhite != isWhiteTurn
                {
                    if (tempChessmans[i, j].isWhite == true)
                    {
                        tempChessmans[i, j].minmaxPossibleMoves().ForEach(n => { minimoves.Add(n); Debug.Log("["); });
                        
                        Debug.Log("mini" + i + "  " + j+minimoves.Count);
                    }
                    if (tempChessmans[i, j].isWhite == false)
                    {
                        tempChessmans[i, j].minmaxPossibleMoves().ForEach(n => { maximoves.Add(n); Debug.Log("p"); });
                        Debug.Log(i + "  " + j +"==" + maximoves.Count);
                    }
                    //tempChessmans[i, j].minmaxPossibleMoves().ForEach(n => moves.Add(n));
                }
            }
        }

        int moveLentgh = moves.Count;
        int randomm = Random.Range(0, moveLentgh);
        int[] bestMove = new int[4];
        bestMove = maximoves[randomm];

        if (maximizing_player)
        {
            int maxEval;
            maxEval = -9000;
            for (int i = 0; i < maximoves.Count; i++)
            {
                Debug.Log("max geted" + maxEval);
                int x1, x2, z2, z1;
                x1 = maximoves[i][0]; z1 = maximoves[i][1]; x2 = maximoves[i][2]; z2 = maximoves[i][3];
                Chessman a = tempChessmans[x1, z1];
                Chessman c = tempChessmans[x2, z2];
                tempChessmans[x1, z1] = null;
                tempChessmans[x2, z2] = null;
                tempChessmans[x2, z2] = a;
                currentEval = minimax(tempChessmans, depth - 1, maxmax, minmin, false, isWhiteturn)[0];
                tempChessmans[x1, z1] = null;
                tempChessmans[x2, z2] = null;

                tempChessmans[x1, z1] = a;
                tempChessmans[x2, z2] = c;
                if (currentEval > maxEval)
                {
                    maxEval = currentEval;
                    bestMove = maximoves[i];
                    //Debug.Log(!isWhiteturn + "m?" + maximizing_player +"mx" + moves[i][0] + "" + moves[i][1] + "" + moves[i][2] + "" + moves[i][3] + "sx" + currentEval+ "max" + maxEval);
                    aa[0] = maxEval;
                    for (int k = 1; k < 5; k++) aa[k] = maximoves[i][k - 1];
                    minmaxOWW.Add(aa);
                    Debug.Log("max geted" + maxEval);
                }


            }
            return aa;

        }
        else
        {
            int minEval;
            minEval = 9000;

            for (int i = 0; i < minimoves.Count; i++)
            {
                Debug.Log("min geted" + minEval);
                int x1, x2, z1, z2;
                x1 = minimoves[i][0]; z1 = minimoves[i][1]; x2 = minimoves[i][2]; z2 = minimoves[i][3];

                Chessman a = tempChessmans[x1, z1];
                Chessman c = tempChessmans[x2, z2];

                tempChessmans[x1, z1] = null;
                tempChessmans[x2, z2] = null;
                tempChessmans[x2, z2] = a;
                currentEval = 0;
                currentEval = minimax(tempChessmans, depth - 1, maxmax, minmin, true, isWhiteturn)[0];
                tempChessmans[x1, z1] = null;
                tempChessmans[x2, z2] = null;
                tempChessmans[x1, z1] = a;
                tempChessmans[x2, z2] = c;

                if (currentEval < minEval)
                {
                    minEval = currentEval;
                    aa[0] = minEval;
                    for (int k = 1; k < 5; k++) aa[k] = minimoves[i][k - 1];
                    Debug.Log("min geted" + minEval);
                }

                minmin = Mathf.Min(minmin, currentEval);
                if (minmin <= maxmax) break;
            }
            return aa;


        }
        //

    }

    private void makeSampleMove(Chessman[,] mChessmans, int x1, int z1, int x2, int z2)
    {

        Chessman a = mChessmans[x1, z1];
        Chessman c = mChessmans[x2, z2];

        mChessmans[x1, z1] = null;
        mChessmans[x2, z2] = a;

    }

    private int evaluate(Chessman[,] mChessmans, bool whiteisMaximizing)
    {
        int sum = 0;
        int scoree = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (mChessmans[i, j] != null)  //  && Chessmans[x, z].isWhite != isWhiteTurn
                {
                    scoree = mChessmans[i, j].score;
                    if (mChessmans[i, j].isWhite)
                    {

                        sum = sum - scoree;
                    }
                    else
                    {

                        sum = sum + scoree;
                    }

                }
            }
        }


        return sum;
    }




    private bool selectChessman(int x, int z)
    {
        if (Chessmans[x, z] == null) return false;
        //Correct turn
        if (Chessmans[x, z].isWhite != isWhiteTurn) return false;
        bool hasAtLeastOneMove = false;
        allowedMoves = Chessmans[x, z].possibleMove();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtLeastOneMove = true;
                }
            }
        }

        if (!hasAtLeastOneMove)
            return false;
        selectedChessman = Chessmans[x, z];

        BoardHighlights.Instance.highlightAllowedMoves(allowedMoves);

        return true;
    }
    private void SpawnBlackChessman(int index, int x, int z)
    {
        GameObject go = Instantiate(chessmanPrefabs[index], gettileCenter(x, z), orientation) as GameObject;
        go.transform.SetParent(transform);
        Chessmans[x, z] = go.GetComponent<Chessman>();
        Chessmans[x, z].setposition(x, z);
        activeChessman.Add(go);
    }
    private void SpawnWhiteChessman(int index, int x, int z)
    {
        GameObject go = Instantiate(chessmanPrefabs[index], gettileCenter(x, z), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Chessmans[x, z] = go.GetComponent<Chessman>();
        Chessmans[x, z].setposition(x, z);
        activeChessman.Add(go);
    }
    //Helper function to set the exact position of each chessaman
    private Vector3 gettileCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * z) + TILE_OFFSET;

        return origin;
    }
    //Draw all the chessamn at the bginnign of the game;
    private void spawnAllChessman()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        minmaxChessmans = new Chessman[8, 8];
        //White team
        //king
        //       SpawnWhiteChessman(0, 3, 0);
        //queen
        //       SpawnWhiteChessman(1, 4, 0);
        //rooks
        SpawnWhiteChessman(2, 0, 0);
        //       SpawnWhiteChessman(2, 7, 0);
        //bishop
        //       SpawnWhiteChessman(3, 2, 0);
        //      SpawnWhiteChessman(3, 5, 0);

        //      SpawnWhiteChessman(4, 1, 0);
        //       SpawnWhiteChessman(4, 6, 0);
        //pawns
        for (int i = 0; i < 8; i++)
        {
            //               SpawnWhiteChessman(5, i, 1);
        }

        //Black team
        //king
        //       SpawnBlackChessman(6, 4, 7);
        //queen
        //       SpawnBlackChessman(7, 3, 7);
        //rooks
        //       SpawnBlackChessman(8, 0, 7);
        //      SpawnBlackChessman(8, 7, 7);
        //bishop
              SpawnBlackChessman(9, 2, 7);
        //        SpawnBlackChessman(9, 5, 7);

        //       SpawnBlackChessman(10, 1, 7);
        //       SpawnBlackChessman(10, 6, 7);

        for (int i = 0; i < 3; i++)
        {
         //   SpawnBlackChessman(11, i, 6);
        }

    }
    private void endGame()
    {
        if (isWhiteTurn)
        {
            //white team wins
        }
        else
        {
            //black team wins
        }

        foreach (GameObject go in activeChessman)
        {
            Destroy(go);
        }

        isWhiteTurn = true;
        BoardHighlights.Instance.hideHighlights();
        spawnAllChessman();
    }
    public void extGame()
    {

    }
    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("chess plane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;

        }
        else
        {
            selectionY = -1;
            selectionX = -1;
        }

    }

}
