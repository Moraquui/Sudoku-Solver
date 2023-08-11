using System;

interface ISudokuSolver
{
    bool Solve(int[,] board);
}

class BacktrackingSolver : ISudokuSolver
{
    public bool Solve(int[,] board)
    {
        return SolveHelper(board, 0, 0);
    }

    private bool SolveHelper(int[,] board, int row, int col)
    {
        int size = board.GetLength(0);

        // Check if we have reached the end of the board
        if (row == size)
        {
            return true;
        }

        // Check if we have reached the end of the row, move to the next row
        if (col == size)
        {
            return SolveHelper(board, row + 1, 0);
        }

        // Check if the cell is already filled, move to the next cell
        if (board[row, col] != 0)
        {
            return SolveHelper(board, row, col + 1);
        }

        // Try each possible value in the cell and backtrack if necessary
        for (int num = 1; num <= size; num++)
        {
            if (IsSafe(board, row, col, num))
            {
                // Place the number in the cell
                board[row, col] = num;

                // Recursively solve the remaining part of the board
                if (SolveHelper(board, row, col + 1))
                {
                    return true;
                }

                // Backtrack: undo the current cell and try another number
                board[row, col] = 0;
            }
        }

        return false; // No valid number found for this cell, backtrack
    }

    private bool IsSafe(int[,] board, int row, int col, int num)
    {
        // Check if the number already exists in the current row
        for (int i = 0; i < board.GetLength(1); i++)
        {
            if (board[row, i] == num)
            {
                return false;
            }
        }

        // Check if the number already exists in the current column
        for (int i = 0; i < board.GetLength(0); i++)
        {
            if (board[i, col] == num)
            {
                return false;
            }
        }

        // Check if the number already exists in the current 3x3 box
        int sqrtSize = (int)Math.Sqrt(board.GetLength(0));
        int boxRow = row - row % sqrtSize;
        int boxCol = col - col % sqrtSize;

        for (int i = boxRow; i < boxRow + sqrtSize; i++)
        {
            for (int j = boxCol; j < boxCol + sqrtSize; j++)
            {
                if (board[i, j] == num)
                {
                    return false;
                }
            }
        }

        return true; // The number can be safely placed in the current cell
    }
}

/// <summary>
/// DancingLinksSolver DOESN'T WORK, use BacktrackingSolver
/// </summary>
class DancingLinksSolver : ISudokuSolver
{
    // Internal representation of the Sudoku board as a 2D matrix of cells
    private class Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Block { get; set; }
        public int Value { get; set; }
    }

    // Constraints for the Sudoku board
    private const int BoardSize = 9;
    private const int BlockSize = 3;
    private const int NumValues = 9;

    // Pointers to the head and current nodes in the dancing links matrix
    private Node head;
    private Node current;

    public bool Solve(int[,] board)
    {
        // Create the dancing links matrix
        CreateDancingLinksMatrix(board);

        // Solve the Sudoku puzzle using dancing links
        return SolveSudoku();
    }

    // Create the dancing links matrix based on the given Sudoku board
    private void CreateDancingLinksMatrix(int[,] board)
    {
        head = new Node();

        // Create the column header nodes
        Node[] colHeaders = new Node[BoardSize * BoardSize * 4];
        for (int i = 0; i < colHeaders.Length; i++)
        {
            colHeaders[i] = new Node();
            colHeaders[i].Size = 0;
            colHeaders[i].Up = colHeaders[i];
            colHeaders[i].Down = colHeaders[i];
        }

        // Create the cell nodes and link them to the column headers
        Node[,] cellNodes = new Node[BoardSize, BoardSize * BoardSize];
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (board[row, col] == 0)
                {
                    for (int value = 1; value <= NumValues; value++)
                    {
                        int block = (row / BlockSize) * BlockSize + (col / BlockSize);
                        int index = GetIndex(row, col, block, value);

                        // Create the cell/node
                        cellNodes[row, col] = new Node();
                        cellNodes[row, col].Row = row;
                        cellNodes[row, col].Col = col;
                        cellNodes[row, col].Block = block;
                        cellNodes[row, col].Value = value;

                        // Link the cell/node to the corresponding column headers
                        cellNodes[row, col].Left = colHeaders[GetColHeaderIndex(0, col, block, value)];
                        cellNodes[row, col].Right = colHeaders[GetColHeaderIndex(0, col, block, value)].Right;
                        cellNodes[row, col].Left.Right = cellNodes[row, col];
                        cellNodes[row, col].Right.Left = cellNodes[row, col];
                        cellNodes[row, col].Up = colHeaders[GetColHeaderIndex(row, 0, block, value)].Up;
                        cellNodes[row, col].Down = colHeaders[GetColHeaderIndex(row, 0, block, value)];
                        cellNodes[row, col].Up.Down = cellNodes[row, col];
                        cellNodes[row, col].Down.Up = cellNodes[row, col];

                        // Increment the size of the corresponding column header
                        colHeaders[GetColHeaderIndex(0, col, block, value)].Size++;
                        colHeaders[GetColHeaderIndex(row, 0, block, value)].Size++;
                    }
                }
                else
                {
                    int value = board[row, col];
                    int block = (row / BlockSize) * BlockSize + (col / BlockSize);
                    int index = GetIndex(row, col, block, value);

                    // Create the cell/node
                    cellNodes[row, col] = new Node();
                    cellNodes[row, col].Row = row;
                    cellNodes[row, col].Col = col;
                    cellNodes[row, col].Block = block;
                    cellNodes[row, col].Value = value;

                    // Link the cell/node to the corresponding column headers
                    cellNodes[row, col].Left = colHeaders[GetColHeaderIndex(0, col, block, value)];
                    cellNodes[row, col].Right = colHeaders[GetColHeaderIndex(0, col, block, value)].Right;
                    cellNodes[row, col].Left.Right = cellNodes[row, col];
                    cellNodes[row, col].Right.Left = cellNodes[row, col];
                    cellNodes[row, col].Up = colHeaders[GetColHeaderIndex(row, 0, block, value)].Up;
                    cellNodes[row, col].Down = colHeaders[GetColHeaderIndex(row, 0, block, value)];
                    cellNodes[row, col].Up.Down = cellNodes[row, col];
                    cellNodes[row, col].Down.Up = cellNodes[row, col];

                    // Increment the size of the corresponding column header
                    colHeaders[GetColHeaderIndex(0, col, block, value)].Size++;
                    colHeaders[GetColHeaderIndex(row, 0, block, value)].Size++;

                    // Cover the corresponding column headers to exclude other cells in the same row, column, and block
                    Cover(colHeaders[GetColHeaderIndex(0, col, block, value)]);
                    Cover(colHeaders[GetColHeaderIndex(row, 0, block, value)]);
                }
            }
        }

        // Link the column headers to form a circular doubly linked list
        for (int i = 0; i < colHeaders.Length; i++)
        {
            colHeaders[i].Left = colHeaders[(i - 1 + colHeaders.Length) % colHeaders.Length];
            colHeaders[i].Right = colHeaders[(i + 1) % colHeaders.Length];
        }

        // Set the head node's right pointer to the first column header
        head.Right = colHeaders[0];
        colHeaders[0].Left = head;

        current = head;
    }

    // Solve the Sudoku puzzle using the dancing links algorithm
    private bool SolveSudoku()
    {
        if (head.Right == head)
        {
            return true; // All constraints have been satisfied
        }

        Node column = ChooseColumn();
        Cover(column);

        for (Node rowNode = column.Down; rowNode != column; rowNode = rowNode.Down)
        {
            current = rowNode;

            for (Node node = rowNode.Right; node != rowNode; node = node.Right)
            {
                Cover(node.ColHeader);
            }

            if (SolveSudoku())
            {
                return true;
            }

            for (Node node = rowNode.Left; node != rowNode; node = node.Left)
            {
                Uncover(node.ColHeader);
            }

            current = rowNode; // restore the current node
        }

        Uncover(column);

        return false;
    }

    // Choose the next column to cover based on the heuristic
    private Node ChooseColumn()
    {
        Node column = null;
        int minSize = int.MaxValue;

        for (Node node = head.Right; node != head; node = node.Right)
        {
            if (node.Size < minSize)
            {
                minSize = node.Size;
                column = node;
            }
        }

        return column;
    }

    // Cover a column and exclude its rows and columns from further consideration
    private void Cover(Node column)
    {
        column.Right.Left = column.Left;
        column.Left.Right = column.Right;

        for (Node rowNode = column.Down; rowNode != column; rowNode = rowNode.Down)
        {
            for (Node node = rowNode.Right; node != rowNode; node = node.Right)
            {
                node.Up.Down = node.Down;
                node.Down.Up = node.Up;
                node.ColHeader.Size--;
            }
        }
    }

    // Uncover a column and restore its rows and columns
    private void Uncover(Node column)
    {
        for (Node rowNode = column.Up; rowNode != column; rowNode = rowNode.Up)
        {
            for (Node node = rowNode.Left; node != rowNode; node = node.Left)
            {
                node.Up.Down = node;
                node.Down.Up = node;
                node.ColHeader.Size++;
            }
        }

        column.Right.Left = column;
        column.Left.Right = column;
    }

    // Get the index of a column header node in the dancing links matrix
    private int GetColHeaderIndex(int row, int col, int block, int value)
    {
        return row * BoardSize + col + BoardSize * BoardSize * block + BoardSize * BoardSize * BlockSize + value - 1;
    }

    // Get the index of a cell node in the dancing links matrix
    private int GetIndex(int row, int col, int block, int value)
    {
        return row * BoardSize + col * NumValues + block * NumValues * BlockSize + value - 1;
    }

    // Internal class representing a node in the dancing links matrix
    private class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Up { get; set; }
        public Node Down { get; set; }
        public Node ColHeader { get { return this; } } // Column header node itself
        public int Size { get; set; } // Number of nodes in the column
        public int Row { get; set; }
        public int Col { get; set; }
        public int Block { get; set; }
        public int Value { get; set; }
    }
}


class SudokuSolverFactory
{
    public ISudokuSolver CreateSolver(string solverType)
    {
        switch (solverType)
        {
            case "backtracking":
                return new BacktrackingSolver();
            case "dancing_links":
                return new DancingLinksSolver();
            default:
                throw new ArgumentException("Invalid solver type");
        }
    }
}
 
class SudokuGame
{
    private SudokuSolverFactory solverFactory;

    public SudokuGame()
    {
        solverFactory = new SudokuSolverFactory();
    }

    public void Play()
    {
        
        string solverType = GetUserInput("Enter the solver type ('backtracking'): ");

        ISudokuSolver solver = solverFactory.CreateSolver(solverType);
        int[,] board = InitializeBoard();
        Console.WriteLine("Start Board");
        PrintBoard(board);
        if (solver.Solve(board))
        {
            Console.WriteLine("Sudoku solved successfully!");
            PrintBoard(board);
        }
        else
        {
            Console.WriteLine("Sudoku has no solution.");
        }
    }

    private string GetUserInput(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    private int[,] InitializeBoard()
    {
        int[,] array = new int[9, 9];
        Random rnd = new Random();
        // Board initialization code
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                
                array[i, j] = rnd.Next() % 30 < 2 ? rnd.Next(1, 10) : 0;
            }
        }
        return array; // Placeholder
    }

    private void PrintBoard(int[,] board)
    {
        // Board printing code
        Console.WriteLine("Sudoku Board:"); // Placeholder
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
                Console.Write($"{board[i, j]}, ");
            Console.WriteLine();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        SudokuGame game = new SudokuGame();
        game.Play();
    }
}

