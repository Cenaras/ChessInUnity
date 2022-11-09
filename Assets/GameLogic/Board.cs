using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Board
{

    // TODO: Maybe a history struct or game state struct to keep track of en passant square, castling rights etc...
    // Need some way to record the state when making a move and then undoing it. As an example: If computer generated move enables en passant undoing it needs to reset
    // the ep-square to what it was before the move was made.

    // The state struct could just keep a previous state struct info maybe? This way, we can undo several moves, since each state keeps the previous state to restrore.
    // Only question is: How bad is this for memory? We could keep a local field for the board with the state info and then use that as the previous?

    // For move generation, store the possible offset directions in an array and iterate through it for each piece.

    // Two obvious choices: Either the game history state stores the previous state, or the board has a stack of histories and pushes new and pops when undoing moves.

    // The current state describes the state after a move has been made. The history describes all previous game states in a stack like manner
    // Each time a move is made, the state is updated and the previous state is pushed into the history. When undoing moves, the stack is updated back to the previous.
    public GameState currentState;
    public Stack<GameState> history = new();

    private int[] Squares;

    public int ColorToMove;

    public MoveGen MoveGen { get; }


    public Board(int[] Squares) {
        this.Squares = Squares;
        ColorToMove = Piece.White;
        MoveGen = new MoveGen();
        currentState = GameState.Initialize();
    }

    /* 
       The board should have the following 
        - MakeMove which takes a move as input and performs it
        - Unmake move which takes a move as input an undoes it
        - 64 squares for our pieces
        - A way to generate the board
     */


    // Most likely needs the state info struct passed as well.
    /* Making a move on the board. The MoveGen has already ensured this move is valid and thus we do not need any assertions on the game state. */
    public void MakeMove(Move move) {
        // Clone and push the game state before this move to the stack to allow for retrieval if move is undone.
        GameState previous = currentState.Clone();
        history.Push(previous);

        int movePiece = PieceAt(move.StartSquare);
        int friendlyColor = Piece.GetColor(movePiece);

        /* If a piece was on the target square, mark it as being captured. Else mark no piece as captured */
        int capturedPiece = PieceAt(move.TargetSquare);
        if (capturedPiece != Piece.None) {
            currentState.CapturedPiece = capturedPiece;
        } else {
            currentState.CapturedPiece = Piece.None;
        }

        /* Handle Rook movement from castling */
        if (move.MoveType == Move.Type.KingCastle) {
            MoveRookForCastle(Move.Type.KingCastle, friendlyColor, move.TargetSquare - 1, false);
        } else if (move.MoveType == Move.Type.QueenCastle) {
            MoveRookForCastle(Move.Type.QueenCastle, friendlyColor, move.TargetSquare + 1, false);
        }


        /* Moving from or to king/rook square disallows castling in the game state - this includes the castle move. */
        if (move.StartSquare == BoardUtils.A1 || move.TargetSquare == BoardUtils.A1) currentState.WhiteCastleQueenSide = false;
        if (move.StartSquare == BoardUtils.H1 || move.TargetSquare == BoardUtils.H1) currentState.WhiteCastleKingSide = false;
        if (move.StartSquare == BoardUtils.A8 || move.TargetSquare == BoardUtils.A8) currentState.BlackCastleQueenSide = false;
        if (move.StartSquare == BoardUtils.H8 || move.TargetSquare == BoardUtils.H8) currentState.BlackCastleKingSide = false;


        /* If move is double pawn, mark the epsquare, else set to -1 */
        if (move.MoveType == Move.Type.PawnDouble) {
            // Ep square is just behind the target square
            currentState.EnPassantSquare = friendlyColor == Piece.White ? move.TargetSquare - 8 : move.TargetSquare + 8; 
        } else {
            currentState.EnPassantSquare = -1;
        }


        /* Handle capturing en-passant */
        if (move.MoveType == Move.Type.EnPassant) {
            // Capture the pawn
            int capturedPawnSquare = friendlyColor == Piece.White ? move.TargetSquare - 8 : move.TargetSquare + 8;
            Squares[capturedPawnSquare] = Piece.None;
        }

        /* Update position of king if kingmove */
        if (Piece.Type(movePiece) == Piece.King) {
            if (friendlyColor == Piece.White) currentState.WhiteKingSquare = move.TargetSquare;
            else currentState.BlackKingSquare = move.TargetSquare;
        }


        Squares[move.StartSquare] = Piece.None;
        Squares[move.TargetSquare] = movePiece;
        ColorToMove = Piece.OppositeColor(ColorToMove);
    
    }

    /* Moves the rooks in a castle move to the target square. */
    private void MoveRookForCastle(Move.Type castleType, int friendlyColor, int rookTargetSquare, bool reverseMove) {
        bool kingSide = castleType == Move.Type.KingCastle;
        int rookStartSquare = kingSide ? BoardUtils.RookKingStartSquare(friendlyColor) : BoardUtils.RookQueenStartSquare(friendlyColor);
        int rookPiece = PieceAt(rookStartSquare);

        // If reversing move in Unmake move, do opposite castling.
        if (reverseMove) {
            Squares[rookStartSquare] = rookPiece;
            Squares[rookTargetSquare] = Piece.None;
        } else {
            Squares[rookStartSquare] = Piece.None;
            Squares[rookTargetSquare] = rookPiece;
        }
        
    }

    /* Unmakes a move, restoring the board state and current game state to that of the previous position. Since we're storing the previous state in the stack, this
     * method does not have to worry about anything else that popping the stack to previous state. */
    public void UnmakeMove(Move move) {
        // TODO: Maybe we can merge this together with MakeMove to have a simple method instead of having two. Right now we have code duplication tendencies.

        // The move is the one made on the board so we flip the from and to squares
        int startSquare = move.TargetSquare;
        int targetSquare = move.StartSquare;
        int movePiece = PieceAt(startSquare);
        int friendlyColor = Piece.GetColor(movePiece);

        // Before restoring the state, we retrieve the captured piece so we can place it
        int restoredPiece = currentState.CapturedPiece;

        // Restore the current state to the state from the previous move
        currentState = history.Pop();

        /* Undoing moving of rook in castle */
        if (move.MoveType == Move.Type.KingCastle) {
            MoveRookForCastle(Move.Type.KingCastle, friendlyColor, move.TargetSquare - 1, true);
        } else if (move.MoveType == Move.Type.QueenCastle) {
            MoveRookForCastle(Move.Type.QueenCastle, friendlyColor, move.TargetSquare + 1, true);
        }

        /* Undo capture */
        if (restoredPiece != Piece.None) {
            // EP is handled differently from normal captures.
            if (move.MoveType == Move.Type.EnPassant) {
                int epPawnSquare = friendlyColor == Piece.White ? startSquare - 8 : startSquare + 8;
                Squares[epPawnSquare] = restoredPiece;
            } else {
                Squares[startSquare] = restoredPiece;
            }
        }
        
        // Moving piece back to start position
        Squares[startSquare] = Piece.None;
        Squares[targetSquare] = movePiece;
        ColorToMove = Piece.OppositeColor(ColorToMove);
    }

    public int PieceAt(int square) {
        return Squares[square];
    }

    public int KingSquareForColor(int color) {
        return color == Piece.White ? currentState.WhiteKingSquare : currentState.BlackKingSquare;
    }
}
