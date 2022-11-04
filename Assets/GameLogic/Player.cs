using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
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


    public void Update() {
        // Somewhere, make sure we can only do stuff it its our turn...
        HandleMouseInputNew();
    }

    private void HandleMouseInputNew() {

        // Prolly use some events instead of bool and returns. 

        BoardPosition mousePos = BoardPosFromMouse();


        // Detect the current input state and act accordingly
        if (currentState == InputState.None) {
            HandlePieceSelection(mousePos);
        } else if (currentState == InputState.PieceDragged) {
            HandleDragMovementNew(mousePos);
        }

        if (Input.GetMouseButtonDown(1)) {
            CancelPieceSelection();
        }
    }


    /* TODO: FIX THIS HORRENDOUS CODE. Return stuff instead of altering state, and give arguments to functions instead of fetching from state. */

    void HandlePieceSelection(BoardPosition clickedPosition) {
        if (Input.GetMouseButtonDown(0)) {

            // Compute valid moves for selected piece and mark as dragging piece
            Piece selectedPiece = board.PieceAt(clickedPosition);
            // Ensure pieces only move on their own turn.
            // && selectedPiece.PieceColor() == board.colorToMove
            if (selectedPiece != null && selectedPiece.PieceColor() == board.colorToMove) {
                List<Move> validMoves = board.moveGen.GenerateValidMoves(selectedPiece, board);
                boardUI.HighlightValidSquares(selectedPiece, validMoves);
                currentState = InputState.PieceDragged;
            }
            // Mark selected square as where we clicked
            selectedSquare = clickedPosition;

        }
    }

    void HandleDragMovementNew(BoardPosition targetSquare) {
        if (Input.GetMouseButtonUp(0)) {
            HandlePiecePlacementNew(selectedSquare, targetSquare);
        }
    }

    /* Lovely code innit? */
    private void HandlePiecePlacementNew(BoardPosition fromSquare, BoardPosition targetSquare) {
        Piece heldPiece = board.PieceAt(fromSquare);
        Move tryingMove;

        if (Move.IsCastleQueenSideMove(fromSquare, targetSquare)) {
            tryingMove = new Move(fromSquare, targetSquare, Move.MoveType.QueenCastle);
        } else if (Move.IsCastleKingSideMove(fromSquare, targetSquare)) {
            //Debug.Log("King castle trying move");
            tryingMove = new Move(fromSquare, targetSquare, Move.MoveType.KingCastle);
        } else if (Move.IsPawnDoubleMove(heldPiece, fromSquare, targetSquare)) {
            tryingMove = new Move(fromSquare, targetSquare, Move.MoveType.PawnDoubleMove);
        } else if (Move.IsEnPassantCapture(heldPiece, board.enPassantPawn, fromSquare, targetSquare)) {
            Debug.Log("Trying move is en passant capture");
            tryingMove = new Move(fromSquare, targetSquare, Move.MoveType.EnPassantCapture);
        }
        
        else {
            tryingMove = new Move(fromSquare, targetSquare);
        }

        TryMakeMove(heldPiece, tryingMove);
    }


    private void TryMakeMove(Piece piece, Move move) {
        List<Move> validMoves = board.moveGen.GenerateValidMoves(piece, board);

        // If a move is valid, make it - else cancel piece selection.
        if (!validMoves.Contains(move)) {
            //Debug.Log("Move invalid");
            CancelPieceSelection();
        } else {
            board.MakeMove(piece, move);
            boardUI.ResetHighlightedSquare();

        }
        currentState = InputState.None;
    }


    void CancelPieceSelection() {
        currentState = InputState.None;
        // Maybe this gets called on all clicks also? There's a lot of clean up to do in this project if you ever feel like it.
        //Debug.Log("Resetting square " + selectedSquare);
        boardUI.ResetPiecePosition(selectedSquare);
        boardUI.ResetHighlightedSquare();
    }

    BoardPosition BoardPosFromMouse() {
        Vector2 mousePosRaw = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = mousePosRaw + offset;
        // Get the position on the board. 
        return new BoardPosition((int)Math.Floor(mousePos.x), (int)Math.Floor(mousePos.y));
    }


}

