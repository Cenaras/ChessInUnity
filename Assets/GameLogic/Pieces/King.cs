using System.Collections.Generic;
using Unity.VisualScripting;

public class King : Piece {

    private GameConstants.GameColor color;
    private BoardPosition position;
    public bool HasMoved { get; set; }


    private static List<BoardPosition> directionOffsets = new List<BoardPosition> {
        new BoardPosition(-1, -1),
        new BoardPosition(1, -1),
        new BoardPosition(-1, 1),
        new BoardPosition(1, 1),
        new BoardPosition(0, -1),
        new BoardPosition(0, 1),
        new BoardPosition(1, 0),
        new BoardPosition(-1, 0),
    };

    public King(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
        HasMoved = false;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.King;
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

    public List<BoardPosition> CandidateSquares() {
        List<BoardPosition> candidatePositions = new List<BoardPosition>();
        foreach (BoardPosition direction in directionOffsets) {
            BoardPosition candidatePosition = BoardPosition.Add(position, BoardPosition.ScalarMult(direction, 1));
            Piece.AddPositionIfValid(candidatePositions, candidatePosition);
        }
        return candidatePositions;
    }

    public BoardPosition QueenSideCastlePosition() {
        if (color == GameConstants.GameColor.White)
            return new BoardPosition(2, 0);
        else
            return new BoardPosition(2, 7);
    }

    public BoardPosition KingSideCastlePosition() {
        if (color == GameConstants.GameColor.White)
            return new BoardPosition(6, 0);
        else
            return new BoardPosition(6, 7);
    }

}
