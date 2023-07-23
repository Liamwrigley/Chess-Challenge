using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    private int Inf = int.MaxValue;
    //private List<string> _moves;
    //public MyBot() 
    //{
    //    _moves = new List<string>();
    //}

    public Dictionary<PieceType, int> piecePriority = new Dictionary<PieceType, int>()
    {
        { PieceType.None,   0 },
        { PieceType.Pawn,   2000 },
        { PieceType.Bishop, 10000 },
        { PieceType.Rook,   10000 },
        { PieceType.Knight, 10000 },
        { PieceType.Queen,  90000 },
        { PieceType.King,   100000 },
    };

    public Move Think(Board board, Timer timer)
    {
        ConsolePST();
        Move[] moves = board.GetLegalMoves();

        // get initial board value
        int initialBoardValue = CurrentBoardValue(board);

        Move higestValueMove = moves[0];// Move.NullMove;
        foreach (var move in moves)
        {
            var skipped = NextTurn(board, move);
            
            var damage = piecePriority[move.CapturePieceType];
            var cost = board.SquareIsAttackedByOpponent(move.TargetSquare) ? piecePriority[move.MovePieceType] : 0;
            int boardValue = CurrentBoardValue(board) + (damage - cost);

            //if (_moves.Contains(GetMoveSet(move)))
            //    boardValue = -Inf;

            PrevTurn(board, move, skipped);

            // foreach move, calculate the new board value
            // take the highest value board
            if (boardValue > initialBoardValue)
            {
                initialBoardValue = boardValue;
                higestValueMove = move;
            }
        }
        //_moves.Add(GetMoveSet(higestValueMove));
        return higestValueMove;
    }

    //public string GetMoveSet(Move move) => move.ToString() + move.MovePieceType.ToString();

    public bool NextTurn(Board board, Move move)
    {
        board.MakeMove(move);
        return board.TrySkipTurn();
    }

    public void PrevTurn(Board board, Move move, bool skipped)
    {
        board.UndoMove(move);
        if (skipped)
            board.UndoSkipTurn();
    }

    public int CurrentBoardValue(Board board)
    {
        if (board.IsInCheckmate())
            return Inf;

        var value = 0;
        foreach (PieceType pieceType in pst.Keys)
        {
            var v = board.GetPieceList(pieceType, board.IsWhiteToMove).Select(p => pst[pieceType][p.Square.Index]);
            value += board.GetPieceList(pieceType, board.IsWhiteToMove).Sum(p => pst[pieceType][p.Square.Index]);
        }
        return value;
    }
    public void ConsolePST()
    {
        foreach (var entry in pst)
        {
            PieceType pieceType = entry.Key;
            int[] values = entry.Value;

            Console.WriteLine($"Piece: {pieceType}");
            Console.WriteLine("Piece-square table:");
            Console.Write("   a   b   c   d   e   f   g   h\n");
            for (int rank = 8; rank >= 1; rank--)
            {
                Console.Write(rank);
                for (int file = 1; file <= 8; file++)
                {
                    int index = (8 - rank) * 8 + (file - 1);
                    Console.Write($" {values[index],3}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    public Dictionary<PieceType, int[]> pst = new Dictionary<PieceType, int[]>()
    {
        { PieceType.Pawn, new int[]
            //{
            //    0, 0, 0, 0, 0, 0, 0, 0, 78, 83, 86, 73, 102, 82, 85, 90, 7, 29, 21, 44, 40, 31, 44, 7, -17, 16, -2, 15, 14, 0, 15, -13, -26, 3, 10, 9, 6, 1, 0, -23, -22, 9, 5, -11, -10, -2, 3, -19, -31, 8, -7, -37, -36, -14, 3, -31, 0, 0, 0, 0, 0, 0, 0, 0
            //}
                    {
            0,   0,  0,  0,  0,  0,  0,  0,
            78,  83, 86, 73, 102, 82, 85, 90,
            7,   29, 21, 44, 40, 31, 44,  7,
            -17, 16, -2, 15, 14, 0,  15, -13,
            -26, 3,  10, 9,  6,  1,  0,  -23,
            -22, 9,  5,  -11, -10, -2, 3,  -19,
            -31, 8,  -7, -37, -36, -14, 3,  -31,
            0,   0,  0,  0,  0,  0,  0,  0
        }
        },
        { PieceType.Knight, new int[]
            {
                -66, -53, -75, -75, -10, -55, -58, -70, -3, -6, 100, -36, 4, 62, -4, -14, 10, 67, 1, 74, 73, 27, 62, -2, 24, 24, 45, 37, 33, 41, 25, 17, -1, 5, 31, 21, 22, 35, 2, 0, -18, 10, 13, 22, 18, 15, 11, -14, -23, -15, 2, 0, 2, 0, -23, -20, -74, -23, -26, -24, -19, -35, -22, -69
            }
        },
        { PieceType.Bishop, new int[]
            {
                -59, -78, -82, -76, -23, -107, -37, -50, -11, 20, 35, -42, -39, 31, 2, -22, -9, 39, -32, 41, 52, -10, 28, -14, 25, 17, 20, 34, 26, 25, 15, 10, 13, 10, 17, 23, 17, 16, 0, 7, 14, 25, 24, 15, 8, 25, 20, 15, 19, 20, 11, 6, 7, 6, 20, 16, -7, 2, -15, -12, -14, -15, -10, -10
            }
        },
        { PieceType.Rook, new int[]
            {
                -500, 29, 33, 4, 37, 33, 56, -500, 55, 29, 56, 67, 55, 62, 34, 60, 19, 35, 28, 33, 45, 27, 25, 15, 0, 5, 16, 13, 18, -4, -9, -6, -28, -35, -16, -21, -13, -29, -46, -30, -42, -28, -42, -25, -25, -35, -26, -46, -53, -38, -31, -26, -29, -43, -44, -53, -500, -24, -18, 5, -2, -18, -31, -500
            }
        },
        { PieceType.Queen, new int[]
            {
                6,1, -8, -104, 69, 24, 88, 26, 14, 32, 60, -10, 20, 76, 57, 24, -2, 43, 32, 60, 72, 63, 43, 2, 1, -16, 22, 17, 25, 20, -13, -6, -14, -15, -2, -5, -1, -10, -20, -22, -30, -6, -13, -11, -16, -11, -16, -27, -36, -18, 0, -19, -15, -15, -21, -38, -39, -30, -31, -13, -31, -36, -34, -42
            }
        },
        { PieceType.King, new int[]
            {
                4, 54, 47, -99, -99, 60, 83, -62, -32, 10, 55, 56, 56, 55, 10, 3, -62, 12, -57, 44, -67, 28, 37, -31, -55, 50, 11, -4, -19, 13, 0, -49, -55, -43, -52, -28, -51, -47, -8, -50, -47, -42, -43, -79, -64, -32, -29, -32, -4, 3, -14, -50, -57, -18, 13, 4, 17, 30, -3, -14, 6, -1, 40, 18
            }
        },
    };


    /*
     * Piece list is a list of PieceType
     * 
     * after I makeMove, it is now the opponent's turn
     * 
     * 
     */
}