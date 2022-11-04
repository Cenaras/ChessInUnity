using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private BoardUI boardUI;
    private Board board;

    public Player playerToMove;
    public Player whitePlayer;
    public Player blackPlayer;

    // Start is called before the first frame update
    void Awake() {
        boardUI = FindObjectOfType<BoardUI>();
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        playerToMove.Update();
        // Placeholder
        boardUI.UpdatePosition(board); // TODO: Set up an event when triggered calls this. Then the dragging should work fine?
    }


    private void NewGame() {
        board = BoardUtils.ParseFenString(BoardUtils.FenStartingPosition);
        BoardUtils.PrintBoard(board);
        whitePlayer = new Player(board);
        blackPlayer = new Player(board);
        playerToMove = whitePlayer;

    }

}
