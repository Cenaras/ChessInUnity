using System;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;


enum MovingActionPhase {
    NO_ACTION,
    PIECE_CLICKED,
    PIECE_DRAGGED,
}

public class Player : PlayerStrategy {

    private static Vector2 offset = new Vector2(0.5f, 0.5f);

    private String username;
    private Board board;
    private MovingActionPhase movingPhase;
    private BoardPosition selectedSquare;

    Camera cam;


    public Player(String username, Board board) {
        cam = Camera.main;
        this.username = username;
        this.board = board;
        movingPhase = MovingActionPhase.NO_ACTION;
    }


    public String Username() {
        return username;
    }


    public void TryGenerateMove() {
        HandleMouseInput();

    }

    /* TODO: FIX THIS HORRENDOUS CODE. Return stuff instead of altering state, and give arguments to functions instead of fetching from state. */

    private void HandleMouseInput() {

        // Detect the current input state and act accordingly

        if (movingPhase == MovingActionPhase.NO_ACTION) {
            HandlePieceSelection();

        } else if (movingPhase == MovingActionPhase.PIECE_DRAGGED) {
            HandleDraggedPlacement();
        }

    }

    void HandlePieceSelection() {
        if (Input.GetMouseButtonDown(0)) {
            BoardPosition clickedPosition = BoardPosFromMouse();
            Piece piece = board.PieceAt(clickedPosition);

            selectedSquare = clickedPosition;
            movingPhase = MovingActionPhase.PIECE_DRAGGED;
        }
    }

    void HandleDraggedPlacement() {
        if (Input.GetMouseButtonUp(0)) {
            BoardPosition fromSquare = selectedSquare;
            BoardPosition targetSquare = BoardPosFromMouse();
            HandlePiecePlacement(fromSquare, targetSquare);
        }
    }

    void HandlePiecePlacement(BoardPosition fromPos, BoardPosition toPos) {
        if (movingPhase == MovingActionPhase.PIECE_DRAGGED) {
            board.MakeMove(fromPos, toPos);
            movingPhase = MovingActionPhase.NO_ACTION;
        }
    }


    BoardPosition BoardPosFromMouse() {
        Vector2 mousePosRaw = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = mousePosRaw + offset;
        // Get the position on the board. y is first, since first index describes rank which effectively is the "y" coordinate on the board
        return new BoardPosition((int)Math.Floor(mousePos.y), (int)Math.Floor(mousePos.x)); 
    }


}

