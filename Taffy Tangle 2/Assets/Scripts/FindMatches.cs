using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>() ;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches() {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo() {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++) {
            for (int j = 0; j < board.height; j++) {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null) {
                    if(i > 0 && i < board.width - 1) {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null) {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) {

                                if (currentDot.GetComponent<Dot>().isRowBomb
                                    || leftDot.GetComponent<Dot>().isRowBomb
                                    || rightDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                // gets the correct column of the bomb
                                if (currentDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i));
                                }
                                if (leftDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                }
                                if (rightDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                }

                                if (!currentMatches.Contains(leftDot)) {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(rightDot)) {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1) {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null) {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag) {

                                if (currentDot.GetComponent<Dot>().isColumnBomb
                                    || upDot.GetComponent<Dot>().isColumnBomb
                                    || downDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                // gets the row  column of the bomb
                                if (currentDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j));
                                }
                                if (upDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j + 1));
                                }
                                if (downDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j - 1));
                                }

                                if (!currentMatches.Contains(upDot)) {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(downDot)) {
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color) {
        for (int i = 0; i< board.width; i++) {
            for (int j = 0; j < board.height; j++) {
                // Check if piece exists
                if (board.allDots[i,j] != null) {
                    // check tag on the dot
                    if (board.allDots[i,j].tag == color) {
                        // set dot to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++) {
            if(board.allDots[column, i] != null) {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++) {
            if (board.allDots[i, row] != null) {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBombs() {
        // Did player move smthn?
        if (board.currentDot != null) {
            // is the piece they moved matched?
            if (board.currentDot.isMatched) {
                // make unmatched
                board.currentDot.isMatched = false;
                //decide what kind of bomb

                /*
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50) {
                    //make a row bomb
                    board.currentDot.MakeRowBomb();
                } else if (typeOfBomb >= 50) {
                    //make a column bomb
                    board.currentDot.MakeColumnBomb();
                }
                */

                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                    board.currentDot.MakeRowBomb();
                } else {
                    board.currentDot.MakeColumnBomb();
                }
            }

            //is other piece matched
            else if (board.currentDot.otherDot != null) {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                //Is the other dot matched?
                if (otherDot.isMatched) {
                    //make it unmatched
                    otherDot.isMatched = false;

                    /*
                    //Decide what kind of bomb to make
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50) {
                        //make a row bomb
                        otherDot.MakeRowBomb();
                    } else if (typeOfBomb >= 50) {
                        //make a column bomb
                        otherDot.MakeColumnBomb();
                    }
                    */

                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                        || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                        otherDot.MakeRowBomb();
                    } else {
                        otherDot.MakeColumnBomb();
                    }
                }


            }
        }
    }

}