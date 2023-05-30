using System.Linq;

namespace RadioFuckTetris;

public class GameState
{
    private Block _currentBlock;

    public Block CurrentBlock
    {
        get => _currentBlock;
        private set
        {
            _currentBlock = value;
            _currentBlock.Reset();

            for (var i = 0; i < 2; i++)
            {
                _currentBlock.Move(1, 0);

                if (!BlockFits())
                {
                    _currentBlock.Move(-1, 0);
                }
            }
        }
    }

    public GameGrid GameGrid { get; }
    public BlockQueue BlockQueue { get; }
    public bool GameOver { get; private set; }
    public int Score { get; private set; }
    public Block HeldBlock { get; private set; }
    private bool CanHold { get; set; }

    public GameState()
    {
        GameGrid = new GameGrid(22, 10);
        BlockQueue = new BlockQueue();
        CurrentBlock = BlockQueue.GetAndUpdate();
        CanHold = true;
    }

    private bool BlockFits() => CurrentBlock.TilePositions().All(p => GameGrid.IsEmpty(p.Row, p.Column));

    public void HoldBlock()
    {
        if (!CanHold)
        {
            return;
        }

        if (HeldBlock == null)
        {
            HeldBlock = CurrentBlock;
            CurrentBlock = BlockQueue.GetAndUpdate();
        }
        else
        { 
            var tmp = CurrentBlock;
            CurrentBlock = HeldBlock; 
            HeldBlock = tmp;
        }

        CanHold = false;
    }

    public void RotateBlockCW()
    {
        CurrentBlock.RotateCW();

        if (!BlockFits())
        {
            CurrentBlock.RotateCCW();
        }
    }

    public void RotateBlockCCW()
    {
        CurrentBlock.RotateCCW();

        if (!BlockFits())
        {
            CurrentBlock.RotateCW();
        }
    }

    public void MoveBlockLeft()
    {
        CurrentBlock.Move(0, -1);

        if (!BlockFits())
        {
            CurrentBlock.Move(0, 1);
        }
    }

    public void MoveBlockRight()
    {
        CurrentBlock.Move(0, 1);

        if (!BlockFits())
        {
            CurrentBlock.Move(0, -1);
        }
    }

    private bool IsGameOver()
    {
        return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
    }

    private void PlaceBlock()
    {
        foreach (var p in CurrentBlock.TilePositions())
        {
            GameGrid[p.Row, p.Column] = CurrentBlock.Id;
        }

        Score += GameGrid.ClearFullRows();

        if (IsGameOver())
        {
            GameOver = true;
        }
        else
        {
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }
    }

    public void MoveBlockDown()
    {
        CurrentBlock.Move(1, 0);

        if (BlockFits()) 
            return;
        CurrentBlock.Move(-1, 0);
        PlaceBlock();
    }

    private int TileDropDistance(Position p)
    {
        var drop = 0;

        while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
        {
            drop++;
        }

        return drop;
    }

    public int BlockDropDistance() => 
        CurrentBlock.TilePositions().Aggregate(GameGrid.Rows, (current, p) => 
        System.Math.Min(current, TileDropDistance(p)));

    public void DropBlock()
    {
        CurrentBlock.Move(BlockDropDistance(), 0);
        PlaceBlock();
    }
}