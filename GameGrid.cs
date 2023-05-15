namespace RadioFuckTetris;

public class GameGrid
{
    private readonly int[,] _grid;
    public int Rows { get; }
    public int Columns { get; }

    public int this[int r, int c]
    {
        get => _grid[r, c];
        set => _grid[r, c] = value;
    }

    public GameGrid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _grid = new int[rows, columns];
    }

    private bool IsInside(int r, int c)
    {
        return r >= 0 && r < Rows && c >= 0 && c < Columns;
    }

    public bool IsEmpty(int r, int c)
    {
        return IsInside(r, c) && _grid[r, c] == 0;
    }

    private bool IsRowFull(int r)
    {
        for (var c = 0; c < Columns; c++)
        {
            if (_grid[r, c] == 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsRowEmpty(int r)
    {
        for (var c = 0; c < Columns; c++)
        {
            if (_grid[r, c] != 0)
            {
                return false;
            }
        }

        return true;
    }

    private void ClearRow(int r)
    {
        for (var c = 0; c < Columns; c++)
        {
            _grid[r, c] = 0;
        }
    }

    private void MoveRowDown(int r, int numRows)
    {
        for (var c = 0; c < Columns; c++)
        {
            _grid[r + numRows, c] = _grid[r, c];
            _grid[r, c] = 0;
        }
    }

    public int ClearFullRows()
    {
        var cleared = 0;

        for (var r = Rows-1; r >= 0; r--)
        {
            if (IsRowFull(r))
            {
                ClearRow(r);
                cleared++;
            }
            else if (cleared > 0)
            {
                MoveRowDown(r, cleared);
            }
        }

        return cleared;
    }
}