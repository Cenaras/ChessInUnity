using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;


enum InputState {
    None,
    PieceSelected,
    PieceDragged,
}


public class Player : PlayerStrategy {

    private static Vector2 offset = new Vector2(0.5f, 0.5f);

    private string username;
    private Board board;
    private BoardUI boardUI;

    private InputState currentState;
    private BoardPosition selectedSquare;
    private GameConstants.GameColor color;

    /* TODO: Only compute valid moves once pr turn */

    Camera cam;

    public Player(string username, GameConstants.GameColor color, Board board) {
        cam = Camera.main;
        this.username = username;
        this.board = board;
        this.color = color;
        currentState = InputState.None;
        boardUI = GameObject.FindObjectOfType<BoardUI>();

    }


    public string Username() {
        return username;
    }


    /** Returns true if a move was performed */
    public bool TryGenerateMove() {
        return HandleMouseInput();

    }

    /* TODO: FIX THIS HORRENDOUS CODE. Return stuff instead of altering state, and give arguments to functions instead of fetching from state. */

    private bool HandleMouseInput() {

        // Prolly use some events instead of bool and returns. 

        BoardPosition mousePos = BoardPosFromMouse();
       

        // Detect the current input state and act accordingly
        if (currentState == InputState.None) {
            HandlePieceSelection(mousePos);
            return false;
        } else if (currentState == InputState.PieceDragged) {
            return HandleDragMovement(mousePos);
        }

        if (Input.GetMouseButtonDown(1)) {
            CancelPieceSelection();
        }
        return false;

    }

    void HandlePieceSelection(BoardPosition clickedPosition) {
        if (Input.GetMouseButtonDown(0)) {

            // Compute valid moves for selected piece and mark as dragging piece
            Piece selectedPiece = board.PieceAt(clickedPosition);
            if (selectedPiece != null) {
                List<Move> validMoves = board.moveGen.GenerateValidMoves(selectedPiece, board);
                boardUI.HighlightValidSquares(selectedPiece, validMoves);
                currentState = InputState.PieceDragged;
            }
            // Mark selected square as where we clicked
            selectedSquare = clickedPosition;

        }
    }

    bool HandleDragMovement(BoardPosition targetSquare) {

        if (Input.GetMouseButtonUp(0)) {
            return HandlePiecePlacement(selectedSquare, targetSquare);
        }
        return false;
    }

    private bool HandlePiecePlacement(BoardPosition fromSquare, BoardPosition targetSquare) {
        Piece heldPiece = board.PieceAt(fromSquare);
        List<Move> validMoves = board.moveGen.GenerateValidMoves(heldPiece, board);
        Move tryingMove;
        
        if (Move.IsCastleQueenSideMove(fromSquare, targetSquare)) {
            tryingMove = new Move(fromSquare, targetSquare, Move.MoveType.QueenCastle);
        } else if (Move.IsCastleKingSideMove(fromSquare, targetSquare)) {
            tryingMove = new Move(fromSquare, targetSquare, Move.MoveType.KingCastle);
        } else {
            tryingMove = new Move(fromSquare, targetSquare);
        }

        bool moveMade = board.TryMakeMove(color, tryingMove, validMoves);
        if (!moveMade) {
            CancelPieceSelection();
        }

        boardUI.ResetHighlightedSquare();
        currentState = InputState.None;
        return moveMade;


    }

    void CancelPieceSelection() {
        currentState = InputState.None;
        // Maybe this gets called on all clicks also? There's a lot of clean up to do in this project if you ever feel like it.
        //Debug.Log("Resetting square " + selectedSquare);
        boardUI.ResetPiecePosition(selectedSquare);
    }

    BoardPosition BoardPosFromMouse() {
        Vector2 mousePosRaw = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = mousePosRaw + offset;
        // Get the position on the board. 
        return new BoardPosition((int)Math.Floor(mousePos.x), (int)Math.Floor(mousePos.y));
    }


}

