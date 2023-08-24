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
    private class Node
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Block { get; set; }
        public int Value { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Up { get; set; }
        public Node Down { get; set; }
        public int Size { get; set; }
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

    private bool SolveSudoku()
    {
        if (head.Right == head)
        {
            return true; // Solution found
        }

        // Choose the column with the fewest nodes
        Node col = ChooseColumn();

        // Cover the chosen column
        Cover(col);

        // Iterate over the rows of the chosen column
        for (Node row = col.Down; row != col; row = row.Down)
        {
            // Add the row to the solution
            AddToSolution(row);

            // Cover the columns of the chosen row
            for (Node right = row.Right; right != row; right = right.Right)
            {
                Cover(right.ColHeader);
            }

            // Recursively call SolveSudoku
            if (SolveSudoku())
            {
                return true; // Solution found
            }

            // Remove the row from the solution
            RemoveFromSolution(row);

            // Uncover the columns of the chosen row
            for (Node left = row.Left; left != row; left = left.Left)
            {
                Uncover(left.ColHeader);
            }
        }

        // Uncover the chosen column
        Uncover(col);

        return false; // No solution found
    }

    private Node ChooseColumn()
    {
        Node col = null;
        int minSize = int.MaxValue;

        // Iterate over the column headers
        for (Node right = head.Right; right != head; right = right.Right)
        {
            if (right.Size < minSize)
            {
                col = right;
                minSize = right.Size;
            }
        }

        return col;
    }

    private void Cover(Node col)
    {
        // Unlink col from the header row
        col.Left.Right = col.Right;
        col.Right.Left = col.Left;

        // Iterate over the rows of col
        for (Node row = col.Down; row != col; row = row.Down)
        {
            // Iterate over the cells of row
            for (Node right = row.Right; right != row; right = right.Right)
            {
                // Unlink the cell from its column
                right.Up.Down = right.Down;
                right.Down.Up = right.Up;

                // Decrement the size of the column
                right.ColHeader.Size--;
            }
        }
    }

    private void Uncover(Node col)
    {
        // Iterate over the rows of col in reverse order
        for (Node row = col.Up; row != col; row = row.Up)
        {
            // Iterate over the cells of row in reverse order
            for (Node left = row.Left; left != row; left = left.Left)
            {
                // Increment the size of the column
                left.ColHeader.Size++;

                // Relink the cell to its column
                left.Up.Down = left;
                left.Down.Up = left;
            }
        }

        // Relink col to the header row
        col.Left.Right = col;
        col.Right.Left = col;
    }

    private void AddToSolution(Node row)
    {
        // Iterate over the cells of the row
        for (Node right = row.Right; right != row; right = right.Right)
        {
            // Store the value of the cell as a solution to the Sudoku puzzle
            current = current.Right = new Node
            {
                Row = right.Row,
                Col = right.Col,
                Block = right.Block,
                Value = right.Value
            };
        }
    }

    private void RemoveFromSolution(Node row)
    {
        // Remove the cells of the row from the solution
        for (Node left = row.Left; left != row; left = left.Left)
        {
            current = left;
        }

        current.Right = head; // Set the current node as the head node
    }

    private int GetIndex(int row, int col, int block, int value)
    {
        return (row * BoardSize * BoardSize) + (col * NumValues) + (value - 1);
    }

    private int GetColHeaderIndex(int row, int col, int block, int value)
    {
        return row * BoardSize + col + block * BoardSize + value - 1;
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
    private int[,] board;

    public SudokuGame(int[,] initialDesk)
    {
        solverFactory = new SudokuSolverFactory();
        board = initialDesk;
    }

    public void Play()
    {
        string solverType = GetUserInput("Enter the solver type ('backtracking'): ");
        ISudokuSolver solver = solverFactory.CreateSolver(solverType);

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

    private void PrintBoard(int[,] board)
    {
        Console.WriteLine("Sudoku Board:");
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
        int[,] initialDesk = new int[,]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        SudokuGame game = new SudokuGame(initialDesk);
        game.Play();
    }
}

