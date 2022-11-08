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

        /* Handle Rook movement from castling */

        if (move.MoveType == Move.Type.KingCastle) {
            MoveRookForCastle(Move.Type.KingCastle, friendlyColor, move.TargetSquare - 1);
        } else if (move.MoveType == Move.Type.QueenCastle) {
            MoveRookForCastle(Move.Type.QueenCastle, friendlyColor, move.TargetSquare + 1);
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

        Squares[move.StartSquare] = Piece.None;
        Squares[move.TargetSquare] = movePiece;
        ColorToMove = Piece.OppositeColor(ColorToMove);
    
    }

    /* Moves the rooks in a castle move to the target square. */
    private void MoveRookForCastle(Move.Type castleType, int friendlyColor, int rookTargetSquare) {
        int rookStartSquare = castleType == Move.Type.KingCastle ? BoardUtils.RookKingStartSquare(friendlyColor) : BoardUtils.RookQueenStartSquare(friendlyColor);
        int rookPiece = PieceAt(rookStartSquare);
        Squares[rookStartSquare] = Piece.None;
        Squares[rookTargetSquare] = rookPiece;
    }

    /* Unmakes a move, restoring the board state and current game state to that of the previous position. */
    //public void UnmakeMove(Move move) { }

    public int PieceAt(int square) {
        return Squares[square];
    }
}
