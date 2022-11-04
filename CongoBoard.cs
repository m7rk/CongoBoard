using System;
using System.Collections.Generic;

public class CongoBoard
{
    // really this is all a board is LOL
    // inspired by sunfish, capital letters are "to move"
    // uses array instead of string though, makes it a little easier to reason about
    public char[,] board;

    public static CongoBoard initial()
    {
        CongoBoard cb = new CongoBoard();
        char[,] board = new char[7, 7];
        board[0, 0] = 'g'; board[1, 0] = 'm'; board[2, 0] = 'e'; board[3, 0] = 'l'; board[4, 0] = 'e'; board[5, 0] = 'c'; board[6, 0] = 'z';
        board[0, 1] = 'p'; board[1, 1] = 'p'; board[2, 1] = 'p'; board[3, 1] = 'p'; board[4, 1] = 'p'; board[5, 1] = 'p'; board[6, 1] = 'p';

        board[0, 5] = 'P'; board[1, 5] = 'P'; board[2, 5] = 'P'; board[3, 5] = 'P'; board[4, 5] = 'P'; board[5, 5] = 'P'; board[6, 5] = 'P';
        board[0, 6] = 'G'; board[1, 6] = 'M'; board[2, 6] = 'E'; board[3, 6] = 'L'; board[4, 6] = 'E'; board[5, 6] = 'C'; board[6, 6] = 'Z';

        cb.board = board;
        return cb;
    }


    // no obvious way to make this faster. could conv char[,] to string but that is a last resort.
    public CongoBoard duplicate()
    {
        CongoBoard cb = new CongoBoard();
        cb.board = board.Clone() as char[,];
        return cb;
    }

    // these pieces are on the team that's to move
    public bool active(char c)
    {
        return c == 'P' || c == 'G' || c == 'M' || c == 'E' || c == 'L' || c == 'C' || c == 'Z' || c == 'S';
    }

    public bool inRiver(int y)
    {
        return y == 3;
    }

    // print the board state to a string
    public string printBoard()
    {
        string printed = "";
        for (int y = 0; y != 7; ++y)
        {
            for (int x = 0; x != 7; ++x)
            {
                if (board[x, y] != '\0')
                {
                    printed += board[x, y].ToString();
                }
                else
                {
                    printed += " ";
                }
            }
            printed += "\n";
        }
        return printed;
    }

    // is given square part of the board
    public static bool inBounds(int x, int y)
    {
        return x >= 0 && x < 7 && y >= 0 && y < 7;
    }

    // precached moves for faster move generation
    static private int[][] pawnMoves =
    {
        new int[] {-1, -1},
        new int[] { 0, -1},
        new int[] { 1, -1}
    };

    static private int[][] superPawnMoves =
    {
        new int[] {-1, -1},
        new int[] { 0, -1},
        new int[] { 1, -1},
        new int[] { 1, 0},
        new int[] {-1, 0}
    };

    static private int[][] superPawnRetreats =
{
        new int[] {-1, 1},
        new int[] { 0, 1},
        new int[] { 1, 1},
    };

    static private int[][] zebraMoves =
    {
        new int[] {-2, -1},
        new int[] {-2, 1},
        new int[] {-1, -2},
        new int[] {-1, 2},
        new int[] {1, -2},
        new int[] {1, 2},
        new int[] {2, -1},
        new int[] {2, 1},
    };
    static private int[][] adjacent =
    {
        new int[] {-1, -1},
        new int[] {0, -1},
        new int[] {1, -1},
        new int[] {-1, 0},
        new int[] {1, 0},
        new int[] {-1, 1},
        new int[] {0, 1},
        new int[] {1, 1},
    };
    static int[][] jumpMoves =
    {
        new int[] {-2, 0},
        new int[] {0, 2},
        new int[] {0, -2},
        new int[] {2, 0},

        new int[] {-2, 2},
        new int[] {-2, -2},
        new int[] {2, -2},
        new int[] {2, 2},
    };
    static int[][] elephantMoves =
    {
        new int[] {1,0},
        new int[] {2,0},
        new int[] {-1,0},
        new int[] {-2,0},
        new int[] {0,1},
        new int[] {0,2},
        new int[] {0,-1},
        new int[] {0,-2}
    };

    static int[][] legalLionLocations =
    {
        new int[] {2,4},
        new int[] {2,5},
        new int[] {2,6},
        new int[] {3,4},
        new int[] {3,5},
        new int[] {3,6},
        new int[] {4,4},
        new int[] {4,5},
        new int[] {4,6},
    };

    private CongoBoard movePiece(int oldX, int oldY, int newX, int newY, int[] drowningPiece)
    {
        CongoBoard newBoard = duplicate();
        newBoard.board[newX, newY] = newBoard.board[oldX,oldY];
        newBoard.board[oldX, oldY] = '\0';

        // drown the piece if it was not moved
        if(drowningPiece[0] == oldX && drowningPiece[1] == oldY)
        {
            // piece is saved if it moved out of water.
            if(newY == 3)
            {
                // piece drowned lol
                newBoard.board[newX, newY] = '\0';
            }
        }
        // -1 => no piece drowning
        else if (drowningPiece[0] != -1)
        {
            // piece drowned lol
            newBoard.board[drowningPiece[0], drowningPiece[1]] = '\0';
        }

        return newBoard;
    }

    public List<CongoBoard> generatePMoves(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();
        foreach (var p in pawnMoves)
        {
            int newX = x + p[0];
            int newY = y + p[1];
            if (inBounds(newX, newY) && !active(board[newX, newY]))
            {
                var nbState = movePiece(x, y, newX, newY, drowningPiece);
                if(newY == 0)
                {
                    nbState.board[newX, newY] = 'S';
                }
                boards.Add(nbState);
            }
        }

        if(y < 3)
        {
            //  Across the river it may retreat one or two squares straight backwards, but it may neither capture nor jump a piece in doing so.
            if (board[x, y + 1] == '\0')
            {
                boards.Add(movePiece(x, y, x, y + 1, drowningPiece));
                if (board[x, y + 2] == '\0')
                {
                    boards.Add(movePiece(x, y, x, y + 2, drowningPiece));
                }
            }
        }
        return boards;
    }

    // A superpawn has the additional power to move & capture one square sideways.
    // It may now retreat one or two squares straight or diagonally backwards,
    // and this ability no longer depends on its position with regard to the river.
    public List<CongoBoard> generateSMoves(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();
        foreach (var p in superPawnMoves)
        {
            int newX = x + p[0];
            int newY = y + p[1];
            if (inBounds(newX, newY) && !active(board[newX, newY]))
            {
                boards.Add(movePiece(x, y, newX, newY, drowningPiece));
            }
        }

        foreach (var p in superPawnRetreats)
        {
            if (inBounds(x + p[0], y + p[1]) && board[x + p[0], y + p[1]] == '\0')
            {
                boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
                if (inBounds(x + p[0]*2, y + p[1]*2) && board[x + p[0]*2, y + p[1]*2] == '\0')
                {
                    boards.Add(movePiece(x, y, x + p[0] * 2, y + p[1] * 2, drowningPiece));
                }
            }
        }
        return boards;
    }

    public List<CongoBoard> generateGMoves(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();

        foreach (var p in adjacent)
        {
            if (inBounds(x + p[0], y + p[1]) && board[x + p[0], y + p[1]] == '\0')
            {
                boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
            }
        }

        foreach (var p in jumpMoves)
        {
            if (inBounds(x + p[0], y + p[1]) && !active(board[x + p[0], y + p[1]]))
            {
                boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
            }
        }
        return boards;
    }

    public List<CongoBoard> generateMMoves(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();

        foreach (var p in adjacent)
        {
            if (inBounds(x + p[0], y + p[1]) && board[x + p[0], y + p[1]] == '\0')
            {
                boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
            }
        }

        boards.AddRange(generateMonkeyJumps(x,y, drowningPiece));

        return boards;
    }

    // given a boardstate with monkey M to move, generate all possible resulting boardstates for that monkey
    private List<CongoBoard> generateMonkeyJumps(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();
        foreach (var p in jumpMoves)
        {
            // place in bounds, landing place is clear, jump target exists, jump target is inactive
            if (inBounds(x + p[0], y + p[1]) && board[x + p[0], y + p[1]] == '\0' && board[x + (p[0]/2), y + (p[1]/2)] != '\0' && !active(board[x + (p[0] / 2), y + (p[1] / 2)]))
            {
                CongoBoard newBoard = movePiece(x, y, x + p[0], y + p[1], drowningPiece);
                newBoard.board[x + (p[0]/2), y + (p[1]/2)] = '\0';
                boards.Add(newBoard);
                // jump was successful - search for double jump
                boards.AddRange(newBoard.generateMonkeyJumps(x + (p[0]), y + (p[1]), drowningPiece));
            }
        }
        return boards;
    }

    public List<CongoBoard> generateEMoves(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();
        foreach (var p in elephantMoves)
        {
            if (inBounds(x + p[0], y + p[1]) && !active(board[x + p[0], y + p[1]]))
            {
                boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
            }
        }
        return boards;
    }

    public List<CongoBoard> generateLMoves(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();
        foreach (var p in adjacent)
        {
            bool legal = false;
            foreach(var lll in legalLionLocations)
            {
                if (lll[0] == (x + p[0]) && lll[1] == (y + p[1]))
                {
                    legal = true;
                    break;
                }
            }

            if (legal)
            {
                if (inBounds(x + p[0], y + p[1]) && !active(board[x + p[0], y + p[1]]))
                {
                    boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
                }
            }
        }

        // move like a pawn until we hit the opposing lion (special jump move)
        foreach (var p in adjacent)
        {
            int[] pointer = new int[2] { x + p[0], y + p[1] };
            while(inBounds(pointer[0],pointer[1]))
            {
                // if this is enemy lion break
                if (board[pointer[0], pointer[1]] == 'l')
                {
                    boards.Add(movePiece(x, y, pointer[0], pointer[1], drowningPiece));
                    break;
                }
                // if something is here break
                if (board[pointer[0], pointer[1]] != '\0')
                {
                    break;
                }
                pointer = new int[2] { pointer[0] + p[0], pointer[1] + p[1] };
            }
        }
        return boards;
    }

    public List<CongoBoard> generateCMoves(int x, int y, int[] drowningPiece)
    {
        // croc basic move
        List<CongoBoard> boards = new List<CongoBoard>();
        foreach (var p in adjacent)
        {
            if (inBounds(x + p[0], y + p[1]) && !active(board[x + p[0], y + p[1]]))
            {
                boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
            }
        }

        if (inRiver(y))
        {
            // only do if adj unblocked (this has already been added to move list)
            if (inBounds(x + 1, y) && board[x + 1, y] == '\0')
            {
                // adjanct space clear; iterate
                int itr = 2;
                while (inBounds(x + itr, y))
                {
                    // nothing
                    if (board[x + itr, y] == '\0')
                    {
                        boards.Add(movePiece(x, y, x + itr, y, drowningPiece));
                    }
                    // enemy
                    else if (!active(board[x + itr, y]))
                    {
                        boards.Add(movePiece(x, y, x + itr, y, drowningPiece));
                        break;
                    } 
                    // friendly
                    else
                    {
                        break;
                    }    
                    itr += 1;
                }
            }

            // only do if adj unblocked (this has already been added to move list)
            if (inBounds(x - 1, y) && board[x - 1, y] == '\0')
            {
                int itr = -2;
                while (inBounds(x + itr, y))
                {
                    // nothing
                    if (board[x + itr, y] == '\0')
                    {
                        boards.Add(movePiece(x, y, x + itr, y, drowningPiece));
                    }
                    // enemy
                    else if (!active(board[x + itr, y]))
                    {
                        boards.Add(movePiece(x, y, x + itr, y, drowningPiece));
                        break;
                    }
                    // friendly
                    else
                    {
                        break;
                    }
                    itr -= 1;
                }
            }
        }
        else
        {
            // croc "rush" move
            int itr = 0;
            itr += Math.Sign(3 - y);

            // only do if adj unblocked (this has already been added to move list) and we aren't adj to the river
            if (inBounds(x, y + itr) && board[x, y + itr] == '\0' && y != 2 && y != 4)
            {
                // clear path to river, check two spaces out
                itr += Math.Sign(3 - y);
                while (true)
                {
                    // break if we hit a friendly
                    if (active(board[x, y + itr]))
                    {
                        break;
                    }

                    // this is a legal move
                    boards.Add(movePiece(x, y, x, y + itr, drowningPiece));

                    // if we are in the river, or hit an enemy, break
                    if (inRiver(y + itr) || board[x, y + itr] != '\0')
                    {
                        break;
                    }
                    itr += Math.Sign(3 - y);
                }
            }
        }

        return boards;
    }

    public List<CongoBoard> generateZMoves(int x, int y, int[] drowningPiece)
    {
        List<CongoBoard> boards = new List<CongoBoard>();
        foreach (var p in zebraMoves)
        {
            if (inBounds(x + p[0], y + p[1]) && !active(board[x + p[0], y + p[1]]))
            {
                boards.Add(movePiece(x, y, x + p[0], y + p[1], drowningPiece));
            }
        }
        return boards;
    }


    // 30% runtime (could be made faster)
    public CongoBoard flipBoard()
    {
        CongoBoard newBoard = new CongoBoard();
        newBoard.board = new char[7, 7];
        for (int y = 0; y != 7; ++y)
        {
            for (int x = 0; x != 7; ++x)
            {
                var c = board[6 - x, 6 - y];
                if (c == '\0')
                {
                    continue;
                }
                // lower to upper
                if (c < 91)
                {
                    newBoard.board[x, y] = (char)(c + ' ');
                }
                else
                {
                    newBoard.board[x, y] = (char)(c - ' ');
                }
            }
        }
        return newBoard;
    }

    public int[] identifyDrowningPiece()
    {
        for (int x = 0; x != 7; ++x)
        {
            char piece = board[x, 3];
            if(active(piece) && piece != 'C')
            {
                return new int[] { x, 3 };
            }
        }
        return new int[] { -1, -1};
    }

    public List<CongoBoard> movesForPiece(int x, int y, int[] drowningPiece)
    {
        switch (board[x, y])
        {
            case '\0': return null;
            // Lion moves will win or lose games. 
            case 'L': return generateLMoves(x, y, drowningPiece);
            // Elephants and crocodiles seem to be more powerful pieces (guessing);
            case 'E': return generateEMoves(x, y, drowningPiece);
            case 'C': return generateCMoves(x, y, drowningPiece);
            // Monkeys often have very strong moves, but situational
            case 'M': return generateMMoves(x, y, drowningPiece);
            // Giraffes and Zebras somewhat boring
            case 'G': return generateGMoves(x, y, drowningPiece);
            case 'Z': return generateZMoves(x, y, drowningPiece);
            // Most pawn moves are quiet
            case 'S': return generateSMoves(x, y, drowningPiece);
            case 'P': return generatePMoves(x, y, drowningPiece);
        }
        return null;
    }

    public List<CongoBoard> generateMoves()
    {
        List<CongoBoard> generated = new List<CongoBoard>();
        var drowningPiece = identifyDrowningPiece();
        for (int y = 0; y != 7; ++y)
        {
            for (int x = 0; x != 7; ++x)
            {
                var moves = movesForPiece(x, y, drowningPiece);
                if (moves != null)
                {
                    generated.AddRange(moves);
                }
            }
        }
        return generated;
    }
}
