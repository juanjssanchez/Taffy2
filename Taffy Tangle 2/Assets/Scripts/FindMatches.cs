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
                    CheckHorizontalMatches(currentDot, i, j);
                    CheckVerticalMatches(currentDot, i, j);
                }

            }
        }
    }


    private void CheckHorizontalMatches(GameObject currentDot, int x, int y) {
        if (x > 0 && x < board.width - 1) {
            GameObject leftDot = board.allDots[x - 1, y];
            GameObject rightDot = board.allDots[x + 1, y];

            if (leftDot != null && rightDot != null) {
                if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) {
                    AddToCurrentMatches(currentDot);
                    AddToCurrentMatches(leftDot);
                    AddToCurrentMatches(rightDot);
                    SetDotMatched(currentDot);
                    SetDotMatched(leftDot);
                    SetDotMatched(rightDot);

                    CheckRowBombs(currentDot, leftDot, rightDot, y);
                    CheckColumnBombs(currentDot, leftDot, rightDot, x);
                }
            }
        }
    }

    private void CheckVerticalMatches(GameObject currentDot, int x, int y) {
        if (y > 0 && y < board.height - 1) {
            GameObject upDot = board.allDots[x, y + 1];
            GameObject downDot = board.allDots[x, y - 1];

            if (upDot != null && downDot != null) {
                if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag) {
                    AddToCurrentMatches(currentDot);
                    AddToCurrentMatches(upDot);
                    AddToCurrentMatches(downDot);
                    SetDotMatched(currentDot);
                    SetDotMatched(upDot);
                    SetDotMatched(downDot);

                    CheckColumnBombs(currentDot, upDot, downDot, x);
                    CheckRowBombs(currentDot, upDot, downDot, y);
                }
            }
        }
    }

    private void CheckRowBombs(GameObject currentDot, GameObject dot1, GameObject dot2, int row) {
        if (currentDot.GetComponent<Dot>().isRowBomb
            || dot1.GetComponent<Dot>().isRowBomb
            || dot2.GetComponent<Dot>().isRowBomb) {
            List<GameObject> rowPieces = GetRowPieces(row);
            currentMatches.AddRange(rowPieces);
        }
    }

    private void CheckColumnBombs(GameObject currentDot, GameObject dot1, GameObject dot2, int column) {
        if (currentDot.GetComponent<Dot>().isColumnBomb
            || dot1.GetComponent<Dot>().isColumnBomb
            || dot2.GetComponent<Dot>().isColumnBomb) {
            List<GameObject> columnPieces = GetColumnPieces(column);
            currentMatches.AddRange(columnPieces);
        }
    }

    private void AddToCurrentMatches(GameObject dot) {
        if (!currentMatches.Contains(dot)) {
            currentMatches.Add(dot);
        }
    }

    void SetDotMatched(GameObject dot) {
        dot.GetComponent<Dot>().isMatched = true;
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
            GameObject dot = board.allDots[column, i];
            if (dot != null) {
                dots.Add(dot);
                SetDotMatched(dot);
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++) {
            GameObject dot = board.allDots[i, row];
            if (dot != null) {
                dots.Add(dot);
                SetDotMatched(dot);
            }
        }

        return dots;
    }

    public void CheckBombs() {
        if (board.currentDot != null) {
            if (board.currentDot.isMatched) {
                board.currentDot.isMatched = false;
                DecideBombType(board.currentDot);
            } else if (board.currentDot.otherDot != null) {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched) {
                    otherDot.isMatched = false;
                    DecideBombType(otherDot);
                }
            }
        }
    }

    void DecideBombType(Dot dot) {
        if ((dot.swipeAngle > -45 && dot.swipeAngle <= 45)
            || (dot.swipeAngle < -135 || dot.swipeAngle >= 135)) {
            dot.MakeRowBomb();
        } else {
            dot.MakeColumnBomb();
        }
    }

}
