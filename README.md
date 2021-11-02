# Minesweeper

Minesweeper implementation in C# as a class library.


- SweeperForm: Basic .Net Framework window which can run the game, uses the images from the original Microsoft Minesweeper game.
- Solver: Attempts to solve any given arrangement of cells by noting which cells must be mines and which cells must then be safe. Won't work if the current arrangement requires a guess to continue.
- GameHistory: Isn't necessary and was being used to test other things.
