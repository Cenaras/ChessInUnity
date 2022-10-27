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

public enum GameColor {
    WHITE,
    BLACK
}


public class Player : PlayerStrategy {

    private static Vector2 offset = new Vector2(0.5f, 0.5f);

    private String username;
    private Board board;
    private BoardUI boardUI;

    private MovingActionPhase movingPhase;
    private BoardPosition selectedSquare;
    private GameColor color;



    Camera cam;

    public Player(String username, GameColor color, Board board) {
        cam = Camera.main;
        this.username = username;
        this.board = board;
        this.color = color;
        movingPhase = MovingActionPhase.NO_ACTION;
        boardUI = GameObject.FindObjectOfType<BoardUI>();

    }


    public String Username() {
        return username;
    }


    /** Returns true if a move was performed */
    public bool TryGenerateMove() {
        return HandleMouseInput();

    }

    /* TODO: FIX THIS HORRENDOUS CODE. Return stuff instead of altering state, and give arguments to functions instead of fetching from state. */

    private bool HandleMouseInput() {


        if (Input.GetMouseButtonDown(1)) {
            CancelPieceSelection();
        }

        // Prolly use some events instead of bool and returns. 

        // Detect the current input state and act accordingly
        if (movingPhase == MovingActionPhase.NO_ACTION) {
            HandlePieceSelection();
            return false;
        } else if (movingPhase == MovingActionPhase.PIECE_DRAGGED) {
            return HandleDraggedPlacement();
        }



        return false;

    }

    void HandlePieceSelection() {
        if (Input.GetMouseButtonDown(0)) {
            BoardPosition clickedPosition = BoardPosFromMouse();
            Piece piece = board.PieceAt(clickedPosition);

            selectedSquare = clickedPosition;
            movingPhase = MovingActionPhase.PIECE_DRAGGED;
        }
    }

    bool HandleDraggedPlacement() {
        BoardPosition fromSquare = selectedSquare;
        boardUI.DragPieceAnim(fromSquare, cam.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButtonUp(0)) {
            BoardPosition targetSquare = BoardPosFromMouse();
            return HandlePiecePlacement(fromSquare, targetSquare);
        }
        return false;
    }

    bool HandlePiecePlacement(BoardPosition fromPos, BoardPosition toPos) {
        if (movingPhase == MovingActionPhase.PIECE_DRAGGED) {
            movingPhase = MovingActionPhase.NO_ACTION;
            bool moveMade = board.MakeMove(color, fromPos, toPos);
            if (!moveMade) {
                CancelPieceSelection();
            }

            return moveMade;
        }
        return false;
    }

    void CancelPieceSelection() {
        movingPhase = MovingActionPhase.NO_ACTION;
        // Maybe this gets called on all clicks also? There's a lot of clean up to do in this project if you ever feel like it.
        //Debug.Log("Resetting square " + selectedSquare);
        boardUI.ResetPiecePosition(selectedSquare);

    }

    BoardPosition BoardPosFromMouse() {
        Vector2 mousePosRaw = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = mousePosRaw + offset;
        // Get the position on the board. y is first, since first index describes rank which effectively is the "y" coordinate on the board
        return new BoardPosition((int)Math.Floor(mousePos.y), (int)Math.Floor(mousePos.x));
    }


}

