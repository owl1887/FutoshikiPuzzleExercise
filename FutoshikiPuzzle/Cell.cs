public class Cell
{
    public int? Value { get; set; }
    public List<int> Candidates { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public Cell(int row, int column, int gridSize)
    {
        Row = row;
        Column = column;
        Candidates = Enumerable.Range(1, gridSize).ToList();
    }
}
