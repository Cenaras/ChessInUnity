using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Board
{

    private int[] Squares;

    public int ColorToMove;

    public Board(int[] Squares) {
        this.Squares = Squares;
        ColorToMove = Piece.White;
    }

    /* The board should have the following 
        - MakeMove which takes a move as input and performs it
        - Unmake move which takes a move as input an undoes it
        - 64 squares for our pieces
        - A way to generate the board
     */



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
