using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player {
    private Camera camera;
    private InputState currentState;

    // Register an event triggered when the player makes a move. The GameManager subscribes to this, to update the turns after a move is made.
    public static Action<Move> onMoveMade;


    private BoardUI boardUI;
    private Board board;

    private MoveGen moveGen;

    // Store this value in state only once when a square is clicked
    private BoardPosition selectedPos; 

    public Player(Board board) {
        camera = Camera.main;
        currentState = InputState.None;

        boardUI = UnityEngine.Object.FindObjectOfType<BoardUI>();
        this.board = board;
        moveGen = board.moveGen;

    }


    enum InputState {
        None,
        PieceDragged,
        PieceSelected
    }

    public void Update() {
        HandleMouseInput();
    }


    void HandleMouseInput() {
        Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        BoardPosition position = BoardUI.PositionFromVector(mousePos);
       
        if (currentState == InputState.None) {
            HandlePieceSelection(position);
        } else if (currentState == InputState.PieceDragged) {
            HandleDragging(mousePos);
        } 
        /*else if (currentState == InputState.PieceSelected) {
            HandlePiecePlacement();
        }*/

        if (Input.GetMouseButtonDown(1)) {
            CancelPieceSelection();
        }

    }


    private void HandlePieceSelection(BoardPosition position) {
        if (Input.GetMouseButtonDown(0)) {
            // Ensure the position is valid within the board, and the piece has same color as the player to move.
            if (position != null) { 
                int square = BoardUtils.SquareFrom(position);
                if (Piece.IsColor(board.PieceAt(square), board.ColorToMove)) {

                    // Highlight the selected square - display all legal moves - set input state to dragging piece
                    boardUI.HighLightLegalMoves();
                    boardUI.HighlightSelectedSquare(position);
                    currentState = InputState.PieceDragged;
                    selectedPos = position;
                }
            }
        }
    }

    private void HandleDragging(Vector2 mousePos) {
        boardUI.DragPiece(selectedPos, mousePos); 

        if (Input.GetMouseButtonUp(0)) {
            HandlePiecePlacement(mousePos);
        }

    }

    /* Handles piece placement from selectedSquare to the square clicked on from mousePos */
    private void HandlePiecePlacement(Vector2 mousePos) {
        BoardPosition targetPosition = BoardUI.PositionFromVector(mousePos);

        // If target is same as selected, we must handle click piece...

        // Clicking outsite board leads to cancel piece selection
        if (targetPosition == null)
            CancelPieceSelection();


        int targetSquare = BoardUtils.SquareFrom(targetPosition);
        // If clicking at own piece again, handle piece selection again for new piece... TODO

        TryMakeMove(selectedPos, targetPosition);
    }


    void TryMakeMove(BoardPosition selectedPos, BoardPosition targetPos) {

        // TODO: Handle validity of move - we only send valid moves to the board, i.e. the board expects moves to be valid.
        // Handle other cases such as promotion of pawn

        int startSquare = BoardUtils.SquareFrom(selectedPos);
        int targetSquare = BoardUtils.SquareFrom(targetPos);

        List<Move> legalMoves = moveGen.LegalMoves(board);

        bool legalMove = false;
        Move chosenMove = new();

        foreach (Move move in legalMoves) {
            if (move.StartSquare == startSquare && move.TargetSquare == targetSquare) {
                // Trying move is valid. Mark as valid and stop looking
                legalMove = true;
                chosenMove = move;
            }
        }

        // If move was legal, notify the GameManager to play the move and update state, else cancel piece selection
        if (legalMove) {
            currentState = InputState.None;
            boardUI.DeselectSquare(selectedPos);

            // If someone subscribes to our onMoveMade event, trigger the code for it here - e.g. the GameMangager gets notified here.
            onMoveMade?.Invoke(chosenMove);


        } else {
            CancelPieceSelection();
        }


        /*
            The strategy for move stuff is as follows:
            - We generate the valid moves for the position
            - We loop through them all
            - For each move, if startSquare and targetSquare is the same, select this move and call board.MakeMove on this move.
            - If we do like this, we can just search through the moves, spot them on their squares and avoid dealing with special move types like checking for castle and
                en passant an so forth here. We can just do that in the move function.
         */        
    }

    void CancelPieceSelection() {
        // TODO: Deselect squares.
        // Reset piece position for dragging.

        currentState = InputState.None;
        boardUI.DeselectSquare(selectedPos);
        boardUI.ResetPiecePosition(selectedPos);
    }    

}
