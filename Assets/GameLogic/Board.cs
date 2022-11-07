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
        Debug.Log(currentState);

    }

    /* 
       The board should have the following 
        - MakeMove which takes a move as input and performs it
        - Unmake move which takes a move as input an undoes it
        - 64 squares for our pieces
        - A way to generate the board
     */


    // Most likely needs the state info struct passed as well.
    public void MakeMove(Move move) {

        GameState previous = currentState.Clone();
        history.Push(previous);

        int movePiece = PieceAt(move.StartSquare);
        Squares[move.StartSquare] = Piece.None;
        Squares[move.TargetSquare] = movePiece;
        ColorToMove = Piece.OppositeColor(ColorToMove);
    
    }
    //public void UnmakeMove(Move move) { }

    public int PieceAt(int square) {
        return Squares[square];
    }
}
