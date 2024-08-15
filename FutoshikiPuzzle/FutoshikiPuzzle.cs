public class FutoshikiPuzzlee
{
    public int GridSize { get; private set; }
    public Cell[,] Grid { get; private set; }
    public List<Constraint> Constraints { get; private set; }
    private Random random = new Random();

    public FutoshikiPuzzlee(int gridSize)
    {
        GridSize = gridSize;
        Grid = new Cell[gridSize, gridSize];
        Constraints = new List<Constraint>();
        InitializeGrid();
        ApplyRandomConstraints();
    }

    private void InitializeGrid()
    {
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                Grid[row, col] = new Cell(row, col, GridSize);
            }
        }
    }

    private void ApplyRandomConstraints()
    {
        // Add random constraints
        for (int i = 0; i < GridSize; i++)
        {
            int row = random.Next(GridSize);
            int col = random.Next(GridSize - 1);
            if (random.Next(2) == 0)
            {
                Constraints.Add(new Constraint(Grid[row, col], Grid[row, col + 1], ConstraintType.LessThan));
            }
            else
            {
                Constraints.Add(new Constraint(Grid[row, col], Grid[row, col + 1], ConstraintType.GreaterThan));
            }
        }
    }

    public bool IsValid()
    {
        foreach (var constraint in Constraints)
        {
            if (!constraint.IsSatisfied())
                return false;
        }
        return true;
    }

    public bool IsSolved()
    {
        foreach (var cell in Grid)
        {
            if (!cell.Value.HasValue)
                return false;
        }
        return IsValid();
    }

    public bool Solve()
    {
        return BacktrackSolve(0, 0);
    }

    private bool BacktrackSolve(int row, int col)
    {
        if (row == GridSize)
            return true;
        if (col == GridSize)
            return BacktrackSolve(row + 1, 0);

        var cell = Grid[row, col];
        if (cell.Value.HasValue)
            return BacktrackSolve(row, col + 1);

        foreach (int candidate in cell.Candidates)
        {
            cell.Value = candidate;
            if (IsValid() && BacktrackSolve(row, col + 1))
                return true;
            cell.Value = null;
        }
        return false;
    }

    public void ProvideHint()
    {
        // Provide a hint by solving the puzzle partially
        Solve();
        // You can choose to reveal a single cell here
    }

    public void UndoLastMove()
    {
        if (moveHistory.Any())
        {
            var lastMove = moveHistory.Pop();
            Grid[lastMove.row, lastMove.col].Value = lastMove.previousValue;
        }
    }


    private Stack<(int row, int col, int? previousValue)> moveHistory = new Stack<(int, int, int?)>();

    public void MakeMove(int row, int col, int value)
    {
        var cell = Grid[row, col];
        moveHistory.Push((row, col, cell.Value));
        cell.Value = value;
    }

    

}
