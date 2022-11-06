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
    void Start() {
        boardUI = FindObjectOfType<BoardUI>();
        NewGame();
    }

    // Update is called once per frame
    void Update() {
        playerToMove.Update();
    }

    /* This gets called by the player whenever he makes a legal move. It plays the move on the board and updates the UI position. */
    void OnMoveMade(Move move) {
        board.MakeMove(move);
        boardUI.UpdatePosition(board);
        playerToMove = board.ColorToMove == Piece.White ? whitePlayer : blackPlayer;
    }

    /* Creates a new game, creating new players a new board and subscribing to the OnMoveMade event */
    private void NewGame() {
        board = BoardUtils.ParseFenString(BoardUtils.FenStartingPosition);
        BoardUtils.PrintBoard(board);
        whitePlayer = new Player(board);
        blackPlayer = new Player(board);
        playerToMove = whitePlayer;

        // Subscribe to the OnMoveMade from the player.
        Player.onMoveMade += OnMoveMade;

        // Draw the initial position
        boardUI.UpdatePosition(board);
    }
}
