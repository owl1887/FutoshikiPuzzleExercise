public enum ConstraintType
{
    GreaterThan,
    LessThan
}

public class Constraint
{
    public Cell Cell1 { get;  set; }
    public Cell Cell2 { get;  set; }
    public ConstraintType Type { get;  set; }

    public Constraint(Cell cell1, Cell cell2, ConstraintType type)
    {
        Cell1 = cell1;
        Cell2 = cell2;
        Type = type;
    }

    public bool IsSatisfied()
    {
        if (!Cell1.Value.HasValue || !Cell2.Value.HasValue)
            return true; // Can't evaluate yet

        return Type switch
        {
            ConstraintType.GreaterThan => Cell1.Value > Cell2.Value,
            ConstraintType.LessThan => Cell1.Value < Cell2.Value,
            _ => true,
        };
    }
}
