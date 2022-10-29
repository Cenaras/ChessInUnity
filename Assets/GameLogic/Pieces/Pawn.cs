using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Pawn : Piece {

    private GameConstants.GameColor color;
    private BoardPosition position;

    // Auto generate getter and setter
    public bool HasMoved { get; set; }


    public Pawn(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
        HasMoved = false;
    }

    
    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Pawn;
    }

    public GameConstants.GameColor PieceColor() {
        return color;
    }
    public BoardPosition GetPosition() {
        return position;
    }

    public void SetPosition(BoardPosition position) {
        this.position = position;
    }

    // TODO: Ensure only create boardposition if position is valid, i.e. not out of the board.
    public List<BoardPosition> CandidateSquares() {
        List<BoardPosition> candidatePositions = new List<BoardPosition>();

        bool isWhitePiece = color == GameConstants.GameColor.White;

        /*BoardPosition infrontTest = null;  
        if (isWhitePiece) {
            BoardPosition bp = new BoardPosition(position.file + 1, position.rank);
            if (bp.IsValidPosition())
                infrontTest = bp;
        } else {
            BoardPosition bp = new BoardPosition(position.file - 1, position.rank);
            if (bp.IsValidPosition())
                infrontTest = bp;
        }

        if (infrontTest != null)
            candidatePositions.Add(infrontTest);*/

        BoardPosition infront = isWhitePiece
            ? new BoardPosition(position.file + 1, position.rank)
            : new BoardPosition(position.file - 1, position.rank);
        candidatePositions.Add(infront);

        if (!HasMoved) {
            BoardPosition twoInfront = isWhitePiece
                ? new BoardPosition(position.file + 2, position.rank)
                : new BoardPosition(position.file - 2, position.rank);
            candidatePositions.Add(twoInfront);
        }

        return candidatePositions;

    }

    public List<BoardPosition> CandidateCaptureSquares() {
        List<BoardPosition> candidatePositions = new List<BoardPosition>();

        bool isWhite = color == GameConstants.GameColor.White;

        BoardPosition captureLeft = isWhite
            ? new BoardPosition(position.file + 1, position.rank - 1)
            : new BoardPosition(position.file - 1, position.rank + 1);

        BoardPosition captureRight = isWhite
            ? new BoardPosition(position.file + 1, position.rank + 1)
            : new BoardPosition(position.file - 1, position.rank - 1);

        Piece.AddPositionIfValid(candidatePositions, captureLeft);
        Piece.AddPositionIfValid(candidatePositions, captureRight);

        return candidatePositions;
    }
}
