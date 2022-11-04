
// Magic for records to work
namespace System.Runtime.CompilerServices {
    internal static class IsExternalInit { }
}
// Maybe rework this and remove it instead of having to write new BoardPosition every time...
public record BoardPosition(int file, int rank) {

    public bool IsValidPosition() {
        return (rank >= 0 && rank <= 7 && file >= 0 && file <= 7);
    }

    public override string ToString() {
        return $"[{file}, {rank}]";
    }

    public static BoardPosition Add(BoardPosition left, BoardPosition right) {
        return new BoardPosition(left.file + right.file, left.rank + right.rank);
    }

    public static BoardPosition ScalarAdd(BoardPosition pos, int scalar) {
        return new BoardPosition(pos.file + scalar, pos.rank + scalar);

    }

    public static BoardPosition ScalarMult(BoardPosition pos, int scalar) {
        return new BoardPosition(pos.file * scalar, pos.rank * scalar);
    }

    public static bool Equals(BoardPosition one, BoardPosition two) {
        return (one.file == two.file) && (one.rank == two.rank);
    }
}
