using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Piece {
    public const int None = 0;  
    public const int Pawn = 1;  
    public const int Knight = 2;  
    public const int Bishop = 3;  
    public const int Rook = 4;  
    public const int Queen = 5;  
    public const int King = 6;

    public const int White = 8;
    public const int Black = 16;

    // Encoding a piece using 5 bits.
    // 01/10 for White/Black, 3 bits for the type.

    // Therefore a piece is simply (Color) OR (Type)

    // Last 3 bit determine the type
    public const int typeMask = 0b00111;

    // White is 01, Black is 10
    public const int whiteMask = 0b01000;
    public const int blackMask = 0b10000;

    // Color is either white or black - AND'ing with this will give either 8 or 16 for pieces
    public const int colorMask = whiteMask | blackMask;


    /// <summary>
    /// Returns true if the piece has the specified color. 
    /// </summary>
    /// <param name="piece"> the encoding of the piece </param>
    /// <param name="color"> the encoding of the color </param>
    /// <returns> true if 'piece' is of color 'color' </returns>
    public static bool IsColor(int piece, int color) {
        return (piece & colorMask) == color;
    }


    /// <summary>
    /// Returns the bit-representation of the type of the piece.
    /// </summary>
    /// <param name="piece"> the encoding of the piece </param>
    /// <returns> bit-representation of the type of the piece </returns>
    public static int Type(int piece) {
        return piece & typeMask;
    }

    public static int GetColor(int piece) {
        return piece & colorMask;
    }
}
