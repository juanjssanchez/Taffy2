using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    wait,
    move
}
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    //private void SetUp()
    //{
    //    for (int i=0; i < width; i++){
    //        for (int j = 0; j < height; j++){
    //            // Instantiates the board by filling with tiles
    //            Vector2 tempPosition = new Vector2(i, j + offSet);
    //            GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
    //            backgroundTile.transform.parent = this.transform;
    //            backgroundTile.name = "( " + i + "," + j + " )";

    //            // Fills with random dots w/ no matches at the start
    //            int dotToUse = Random.Range(0, dots.Length);
    //            List<int> tempDotList = new List<int>();
    //            for (int t = 0; t < dots.Length; t++) {
    //                tempDotList.Add(t);
    //            }

    //            while (MatchesAt(i, j, dots[dotToUse])){
    //                tempDotList.Remove(dotToUse);
    //                if (tempDotList.Count == 0) {break;}
    //                int x = Random.Range(0, tempDotList.Count);
    //                dotToUse = tempDotList[x];
    //            }

    //            GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
    //            dot.GetComponent<Dot>().row = j;
    //            dot.GetComponent<Dot>().column = i;
    //            dot.transform.parent = this.transform;
    //            dot.name = "( " + i + "," + j + " )";

    //            allDots[i, j] = dot;
    //        }
    //    }
    //}

    private void SetUp() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity, transform);
                backgroundTile.name = "(" + i + "," + j + ")";
                allTiles[i, j] = backgroundTile.GetComponent<BackgroundTile>();

                int dotToUse = GetRandomDotIndex();

                while (MatchesAt(i, j, dots[dotToUse])) {
                    dotToUse = GetRandomDotIndex();
                }

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity, transform);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.name = "(" + i + "," + j + ")";
                allDots[i, j] = dot;
            }
        }
    }

    private int GetRandomDotIndex() {
        return Random.Range(0, dots.Length);
    }


    private bool MatchesAt(int column, int row, GameObject piece) {
        //if the pieces to my left (already generated) are both of the same type as me then ...
        if (column > 1 && allDots[column - 1, row].GetComponent<Dot>().tag == piece.GetComponent<Dot>().tag
            && allDots[column - 2, row].GetComponent<Dot>().tag == piece.GetComponent<Dot>().tag) {
            return true;
        }
        //if the pieces below me (already generated) are both of the same type as me then ...
        if (row > 1 && allDots[column, row - 1].GetComponent<Dot>().tag == piece.GetComponent<Dot>().tag
            && allDots[column, row - 2].GetComponent<Dot>().tag == piece.GetComponent<Dot>().tag) {
            return true;
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row) {
        Dot dot = allDots[column, row].GetComponent<Dot>();
        if (dot.isMatched) {
            //How many elements are in the matched pieces list from findmatches?
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7) {
                findMatches.CheckBombs();
            }
            findMatches.currentMatches.Remove(allDots[column, row]);
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches() {
        for (int i = 0; i < width; i++) {
            for (int j = 0;j < height; j++) {
                if (allDots[i, j] != null) {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo() {
        int nullCount = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] == null) {
                    nullCount++;
                }else if(nullCount > 0) {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] == null) {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = GetRandomDotIndex();
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity, transform);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched) {
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo() {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard()) {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }


}
