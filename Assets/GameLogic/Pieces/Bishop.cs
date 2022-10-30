using System.Collections.Generic;

public class Bishop : Piece {

    private GameConstants.GameColor color;
    private BoardPosition position;

    private static List<BoardPosition> directionOffset = new List<BoardPosition> {
        new BoardPosition(-1, -1),
        new BoardPosition(1, -1),
        new BoardPosition(-1, 1),
        new BoardPosition(1, 1),
    };


    public Bishop(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Bishop;
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

        // Loop over all possible distances
        for (int distance = 1; distance < 8; distance++) {
            foreach (BoardPosition direction in directionOffset) {
                // Take every direction offset (i.e. diagonal) and get every length of it and add it to the current position.
                BoardPosition candidatePos = BoardPosition.Add(position, BoardPosition.ScalarMult(direction, distance));
                Piece.AddPositionIfValid(candidatePositions, candidatePos);
            }
        }
        return candidatePositions;
    }
}
