using System;

namespace RadioFuckTetris;

public class BlockQueue
{
    private readonly Block[] _blocks = new Block[]
    {
        new IBlock(),
        new JBlock(),
        new LBlock(),
        new OBlock(),
        new SBlock(),
        new TBlock(),
        new ZBlock()
    };

    private readonly Random _random = new Random();

    public Block NextBlock { get; set; }

    public BlockQueue() => NextBlock = RandomBlock();

    private Block RandomBlock() => _blocks[_random.Next(_blocks.Length)];

    public Block GetAndUpdate()
    {
        var block = NextBlock;

        do
        {
            NextBlock = RandomBlock();
        } 
        while (block.Id == NextBlock.Id);

        return block;

    }

}