using System;
using System.Collections;
using System.Collections.Generic;
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

    private int[] Squares;

    public int ColorToMove;

    public MoveGen moveGen { get; }


    public Board(int[] Squares) {
        this.Squares = Squares;
        ColorToMove = Piece.White;
        moveGen = new MoveGen();

    }

    /* The board should have the following 
        - MakeMove which takes a move as input and performs it
        - Unmake move which takes a move as input an undoes it
        - 64 squares for our pieces
        - A way to generate the board
     */


    // Most likely needs the state info struct passed as well.
    public void MakeMove(Move move) {
        int movePiece = PieceAt(move.StartSquare);
        Squares[move.StartSquare] = Piece.None;
        Squares[move.TargetSquare] = movePiece;
        
    
    }
    //public void UnmakeMove(Move move) { }

    public int PieceAt(int square) {
        return Squares[square];
    }
}
