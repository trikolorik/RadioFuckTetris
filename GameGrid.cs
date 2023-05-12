namespace RadioFuckTetris;

public class GameGrid
{
    public int Rows;
    public int Columns;
    
    private readonly int[,] _grid;

    public int this[int rows, int columns]
    {
        get => _grid[rows, columns];
        set => _grid[rows, columns] = value;
    }

    public GameGrid(int r, int c)
    {
        Rows = r;
        Columns = c;
        _grid = new int[r, c];
    }

    public bool IsInside(int r, int c) => r >= 0 && r < Rows && c >= 0 && c < Columns;

    public bool IsEmpty(int r, int c) => IsInside(r, c) && _grid[r, c] == 0;

    public bool IsRowFull(int r)
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

        for (var r = Rows - 1; r >= 0; r--)
        {
            if (IsRowFull(r))
            {
                ClearRow(r);
                cleared++;
            }
            else 
            {
                MoveRowDown(r, cleared);
            }
        }
        
        return cleared;
    }
}