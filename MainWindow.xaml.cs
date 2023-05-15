﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

 namespace RadioFuckTetris; 

 /// <summary>
 /// Interaction logic for MainWindow.xaml
 /// </summary>
 public partial class MainWindow : Window
 {
     private readonly ImageSource[] tileImages = new ImageSource[]
     {
         new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
     };

     private readonly ImageSource[] blockImages = new ImageSource[]
     {
         new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
         new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
     };

     private readonly Image[,] _imageControls;
     private const int MaxDelay = 1000;
     private const int MinDelay = 75;
     private const int DelayDecrease = 25;

     private GameState _gameState = new GameState();

     public MainWindow()
     {
         InitializeComponent();
         _imageControls = SetupGameCanvas(_gameState.GameGrid);
     }

     private Image[,] SetupGameCanvas(GameGrid grid)
     {
         var imageControls = new Image[grid.Rows, grid.Columns];
         const int cellSize = 25;

         for (var r = 0; r < grid.Rows; r++)
         {
             for (var c = 0; c < grid.Columns; c++)
             {
                 var imageControl = new Image
                 {
                     Width = cellSize,
                     Height = cellSize
                 };

                 Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                 Canvas.SetLeft(imageControl, c * cellSize);
                 GameCanvas.Children.Add(imageControl);
                 imageControls[r, c] = imageControl;
             }
         }

         return imageControls;
     }

     private void DrawGrid(GameGrid grid)
     {
         for (var r = 0; r < grid.Rows; r++)
         {
             for (var c = 0; c < grid.Columns; c++)
             {
                 var id = grid[r, c];
                 _imageControls[r, c].Opacity = 1;
                 _imageControls[r, c].Source = tileImages[id];
             }
         }
     }

     private void DrawBlock(Block block)
     {
         foreach (var p in block.TilePositions())
         {
             _imageControls[p.Row, p.Column].Opacity = 1;
             _imageControls[p.Row, p.Column].Source = tileImages[block.Id];
         }
     }

     private void DrawNextBlock(BlockQueue blockQueue)
     {
         var next = blockQueue.NextBlock;
         NextImage.Source = blockImages[next.Id];
     }

     private void DrawHeldBlock(Block heldBlock)
     {
         if (heldBlock == null)
         {
             HoldImage.Source = blockImages[0];
         }
         else
         {
             HoldImage.Source = blockImages[heldBlock.Id];
         }
     }

     private void DrawGhostBlock(Block block)
     {
         var dropDistance = _gameState.BlockDropDistance();

         foreach (var p in block.TilePositions())
         {
             _imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
             _imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
         }
     }

     private void Draw(GameState gameState)
     {
         DrawGrid(gameState.GameGrid);
         DrawGhostBlock(gameState.CurrentBlock);
         DrawBlock(gameState.CurrentBlock);
         DrawNextBlock(gameState.BlockQueue);
         DrawHeldBlock(gameState.HeldBlock);
         ScoreText.Text = $"Score: {gameState.Score}";
     }

     private async Task GameLoop()
     {
         Draw(_gameState);

         while (!_gameState.GameOver)
         {
             var delay = Math.Max(MinDelay, MaxDelay - (_gameState.Score * DelayDecrease));
             await Task.Delay(delay);
             _gameState.MoveBlockDown();
             Draw(_gameState);
         }

         GameOverMenu.Visibility = Visibility.Visible;
         FinalScoreText.Text = $"Score: {_gameState.Score}";
     }

     private void Window_KeyDown(object sender, KeyEventArgs e)
     {
         if (_gameState.GameOver)
         {
             return;
         }

         switch (e.Key)
         {
             case Key.Left:
                 _gameState.MoveBlockLeft();
                 break;
             case Key.Right:
                 _gameState.MoveBlockRight();
                 break;
             case Key.Down:
                 _gameState.MoveBlockDown();
                 break;
             case Key.D:
                 _gameState.RotateBlockCW();
                 break;
             case Key.A:
                 _gameState.RotateBlockCCW();
                 break;
             case Key.C:
                 _gameState.HoldBlock();
                 break;
             case Key.H:
                 _gameState.HoldBlock();
                 break;
             case Key.Space:
                 _gameState.DropBlock();
                 break;
             default:
                 return;
         }

         Draw(_gameState);
     }

     private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
     {
         await GameLoop();
     }

     private async void PlayAgain_Click(object sender, RoutedEventArgs e)
     {
         _gameState = new GameState();
         GameOverMenu.Visibility = Visibility.Hidden;
         await GameLoop();
     }
 }