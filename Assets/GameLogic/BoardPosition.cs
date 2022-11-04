using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPosition {


    public int File { get; }
    public int Rank { get; }

    public BoardPosition(int file, int rank) {
        this.File = file;
        this.Rank = rank;
    }

    public override string ToString() {
        return $"[{FilePretty[File]}{Rank+1}]";
    }

    private static Dictionary<int, char> FilePretty = new Dictionary<int, char>() {
        {0, 'A'},
        {1, 'B' },
        {2, 'C'},
        {3, 'D' },
        { 4, 'E'},
        { 5, 'F'},
        { 6, 'G'},
        { 7, 'H'},
    };


    public bool IsWhiteSquare() {
        return (File + Rank) % 2 != 0;
    }


}
