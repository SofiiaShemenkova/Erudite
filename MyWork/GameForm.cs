using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace MyWork
{
    public partial class GameForm : Form
    {
        private const int BOARD_SIZE = 15;
        private const int TILE_SIZE = 40;
        private const int CENTER = BOARD_SIZE / 2;

        private Button[,] boardButtons;
        private Button[] playerTiles;
        private Label currentPlayerLabel;
        private Button btnSubmit;
        private Button btnSkip;
        private Button btnExchange;
        private Button btnEndGame;

        private List<Player> players;
        private int currentPlayerIndex;
        private List<Tile> tileBag;
        private Dictionary<string, bool> dictionary;
        private bool isFirstMove;
        private List<Point> currentMoveTiles;
        private Dictionary<Point, Tile> boardTiles;
        private Tile selectedTile;

        private Random random;
        private static readonly Dictionary<char, int> letterScores = new Dictionary<char, int>()
        {
            {'А', 1}, {'Б', 3}, {'В', 2}, {'Г', 3}, {'Д', 2},
            {'Е', 1}, {'Ё', 3}, {'Ж', 5}, {'З', 5}, {'И', 1},
            {'Й', 2}, {'К', 2}, {'Л', 2}, {'М', 2}, {'Н', 1},
            {'О', 1}, {'П', 2}, {'Р', 2}, {'С', 2}, {'Т', 1},
            {'У', 2}, {'Ф', 10}, {'Х', 5}, {'Ц', 5}, {'Ч', 5},
            {'Ш', 10}, {'Щ', 10}, {'Ъ', 10}, {'Ы', 5}, {'Ь', 5},
            {'Э', 8}, {'Ю', 8}, {'Я', 3}
        };

        private static readonly Dictionary<char, int> letterDistrib = new Dictionary<char, int>()
        {
            {'А', 10}, {'Б', 3}, {'В', 5}, {'Г', 3}, {'Д', 5},
            {'Е', 9}, {'Ё', 1}, {'Ж', 2}, {'З', 2}, {'И', 8},
            {'Й', 4}, {'К', 6}, {'Л', 4}, {'М', 5}, {'Н', 8},
            {'О', 10}, {'П', 6}, {'Р', 6}, {'С', 6}, {'Т', 5},
            {'У', 3}, {'Ф', 1}, {'Х', 2}, {'Ц', 1}, {'Ч', 2},
            {'Ш', 1}, {'Щ', 1}, {'Ъ', 1}, {'Ы', 2}, {'Ь', 2},
            {'Э', 1}, {'Ю', 1}, {'Я', 3}
        };

        public GameForm(int playerCount)
        {
            InitializeComponent();
            InitializeGame(playerCount);
        }

        private void InitializeGame(int playerCount)
        {
            CreateBoard();
            CreatePlayerPanel();

            random = new Random();
            players = new List<Player>();
            tileBag = new List<Tile>();
            currentMoveTiles = new List<Point>();
            boardTiles = new Dictionary<Point, Tile>();
            isFirstMove = true;
            selectedTile = null;

            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Player($"Игрок {i + 1}"));
            }

            InitializeTileBag();

            foreach (Player player in players)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (tileBag.Count > 0)
                    {
                        player.Tiles.Add(DrawTile());
                    }
                }
            }

            currentPlayerIndex = 0;

            LoadDictionary();

            UpdateDisplay();
        }

        private void CreateBoard()
        {
            int startX = 20;
            int startY = 20;

            boardButtons = new Button[BOARD_SIZE, BOARD_SIZE];

            Dictionary<Point, Color> bonusCells = GetBonusCells();

            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(TILE_SIZE, TILE_SIZE);
                    btn.Location = new Point(startX + col * TILE_SIZE, startY + row * TILE_SIZE);
                    btn.Font = new Font("Arial", 12, FontStyle.Bold);
                    btn.BackColor = Color.Beige;
                    btn.ForeColor = Color.Black;
                    btn.Tag = new Point(row, col); // В Tag храним позицию ячейки

                    Point pos = new Point(row, col);
                    if (bonusCells.ContainsKey(pos))
                    {
                        btn.BackColor = bonusCells[pos];
                        if (btn.BackColor == Color.Red || btn.BackColor == Color.Yellow)
                            btn.ForeColor = Color.White;
                    }

                    if (row == CENTER && col == CENTER)
                    {
                        btn.BackColor = Color.DarkRed;
                        btn.ForeColor = Color.White;
                        btn.Text = "★";
                    }

                    btn.Click += BoardButton_Click;
                    boardButtons[row, col] = btn;
                    this.Controls.Add(btn);
                }
            }
        }

        private Dictionary<Point, Color> GetBonusCells()
        {
            Dictionary<Point, Color> bonuses = new Dictionary<Point, Color>();

            // Красные клетки (утроение буквы)
            int[] tripleWord = { 0, 7, 14 };
            foreach (int pos in tripleWord)
            {
                bonuses[new Point(pos, pos)] = Color.Red;
                bonuses[new Point(pos, BOARD_SIZE - 1 - pos)] = Color.Red;
            }

            // Желтые клетки (утроение слова)
            int[] tripleLetter = { 1, 5, 9, 13 };
            foreach (int row in tripleLetter)
            {
                foreach (int col in tripleLetter)
                {
                    bonuses[new Point(row, col)] = Color.Yellow;
                }
            }

            // Синие клетки (удвоение буквы)
            int[] doubleLetter = { 0, 3, 7, 11, 14 };
            for (int i = 0; i < BOARD_SIZE; i += 7)
            {
                for (int j = 3; j < BOARD_SIZE; j += 4)
                {
                    if (!bonuses.ContainsKey(new Point(i, j)))
                        bonuses[new Point(i, j)] = Color.Blue;
                }
            }

            // Зеленые клетки (удвоение слова)
            for (int i = 1; i < BOARD_SIZE - 1; i++)
            {
                for (int j = 1; j < BOARD_SIZE - 1; j++)
                {
                    if (i == j || i == BOARD_SIZE - 1 - j)
                    {
                        if (!bonuses.ContainsKey(new Point(i, j)))
                            bonuses[new Point(i, j)] = Color.Green;
                    }
                }
            }

            return bonuses;
        }

        private void CreatePlayerPanel()
        {
            currentPlayerLabel = new Label();
            currentPlayerLabel.Location = new Point(650, 20);
            currentPlayerLabel.Size = new Size(300, 70);
            currentPlayerLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            this.Controls.Add(currentPlayerLabel);

            btnSubmit = new Button();
            btnSubmit.Text = "Сделать ход";
            btnSubmit.Location = new Point(650, 110);
            btnSubmit.Size = new Size(150, 40);
            btnSubmit.Click += BtnSubmit_Click;
            this.Controls.Add(btnSubmit);

            btnSkip = new Button();
            btnSkip.Text = "Пропустить ход";
            btnSkip.Location = new Point(650, 160);
            btnSkip.Size = new Size(150, 40);
            btnSkip.Click += BtnSkip_Click;
            this.Controls.Add(btnSkip);

            btnExchange = new Button();
            btnExchange.Text = "Поменять буквы";
            btnExchange.Location = new Point(650, 210);
            btnExchange.Size = new Size(150, 40);
            btnExchange.Click += BtnExchange_Click;
            this.Controls.Add(btnExchange);

            btnEndGame = new Button();
            btnEndGame.Text = "Завершить игру";
            btnEndGame.Location = new Point(650, 260);
            btnEndGame.Size = new Size(150, 40);
            btnEndGame.Click += BtnEndGame_Click;
            this.Controls.Add(btnEndGame);

            playerTiles = new Button[7];
            for (int i = 0; i < 7; i++)
            {
                playerTiles[i] = new Button();
                playerTiles[i].Size = new Size(40, 40);
                playerTiles[i].Location = new Point(650 + (i * 45), 320);
                playerTiles[i].Font = new Font("Arial", 14, FontStyle.Bold);
                playerTiles[i].BackColor = Color.Wheat;
                playerTiles[i].Click += PlayerTile_Click;
                this.Controls.Add(playerTiles[i]);
            }
        }

        private void InitializeTileBag()
        {
            foreach (var kvp in letterDistrib)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    tileBag.Add(new Tile(kvp.Key, letterScores[kvp.Key]));
                }
            }

            ShuffleTiles();
        }

        private void ShuffleTiles()
        {
            int n = tileBag.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Tile value = tileBag[k];
                tileBag[k] = tileBag[n];
                tileBag[n] = value;
            }
        }

        private Tile DrawTile()
        {
            if (tileBag.Count == 0) return null;

            Tile tile = tileBag[0];
            tileBag.RemoveAt(0);
            return tile;
        }

        private void LoadDictionary()
        {
            dictionary = new Dictionary<string, bool>();

            try
            {
                string dictionaryPath = Path.Combine(Application.StartupPath, "erudite_game.txt");

                if (!File.Exists(dictionaryPath))
                {
                    dictionaryPath = "erudite_game.txt";

                    if (!File.Exists(dictionaryPath))
                    {
                        CreateBackup();
                        MessageBox.Show("Файл словаря не найден. Используется стандартный словарь.",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string[] lines = File.ReadAllLines(dictionaryPath, Encoding.UTF8);

                foreach (string line in lines)
                {
                    string word = line.Trim().ToUpper();

                    if (!string.IsNullOrEmpty(word) && !word.StartsWith("#") && !word.StartsWith("//"))
                    {
                        dictionary[word] = true;
                    }
                }

                if (dictionary.Count > 0)
                {
                    Console.WriteLine($"Загружено {dictionary.Count} слов из словаря.");
                }
                else
                {
                    CreateBackup();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки словаря: {ex.Message}\nИспользуется стандартный словарь.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateBackup();
            }
        }

        private void CreateBackup()
        {
            // Резервный словарь
            string[] sampleWords = {
                "КОТ", "СОБАКА", "ДОМ", "СТОЛ", "СТУЛ",
                "ОКНО", "ДВЕРЬ", "КНИГА", "РУЧКА", "БУМАГА",
                "ВОДА", "ОГОНЬ", "ЗЕМЛЯ", "ВОЗДУХ", "СОЛНЦЕ",
                "ЛУНА", "ЗВЕЗДА", "ЛЕС", "РЕКА", "ГОРА",
                "ГОРОД", "УЛИЦА", "МАШИНА", "ЧЕЛОВЕК", "РАБОТА",
                "ШКОЛА", "УЧИТЕЛЬ", "УЧЕНИК", "ТЕТРАДЬ", "КАРАНДАШ"
            };

            foreach (string word in sampleWords)
            {
                dictionary[word] = true;
            }
        }

        private void UpdateDisplay()
        {
            Player currentPlayer = players[currentPlayerIndex];
            for (int i = 0; i < 7; i++)
            {
                if (i < currentPlayer.Tiles.Count && currentPlayer.Tiles[i] != null)
                {
                    playerTiles[i].Text = currentPlayer.Tiles[i].Letter.ToString();
                    playerTiles[i].Tag = currentPlayer.Tiles[i];
                    playerTiles[i].Visible = true;

                    if (currentPlayer.Tiles[i] == selectedTile)
                    {
                        playerTiles[i].BackColor = Color.LightGreen;
                        playerTiles[i].FlatStyle = FlatStyle.Flat;
                        playerTiles[i].FlatAppearance.BorderColor = Color.Green;
                        playerTiles[i].FlatAppearance.BorderSize = 3;
                    }
                    else
                    {
                        playerTiles[i].BackColor = Color.Wheat;
                        playerTiles[i].FlatStyle = FlatStyle.Standard;
                    }
                }
                else
                {
                    playerTiles[i].Visible = false;
                }
            }

            currentPlayerLabel.Text = $"Ход: {currentPlayer.Name}\nОчки: {currentPlayer.Score}\nФишек в мешке: {tileBag.Count}";

            UpdateScoreDisp();
        }

        private void UpdateScoreDisp()
        {
            var labelsToRemove = this.Controls.OfType<Label>()
                .Where(l => l != currentPlayerLabel && l.Location.X == 650 && l.Location.Y >= 350)
                .ToList();

            foreach (var label in labelsToRemove)
            {
                this.Controls.Remove(label);
                label.Dispose();
            }

            int yPos = 390;
            foreach (Player player in players)
            {
                Label scoreLabel = new Label();
                scoreLabel.Text = $"{player.Name}: {player.Score} очков";
                scoreLabel.Location = new Point(650, yPos);
                scoreLabel.Size = new Size(200, 25);
                scoreLabel.Font = new Font("Arial", 10);
                this.Controls.Add(scoreLabel);
                yPos += 30;
            }
        }

        private void BoardButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Point position = (Point)btn.Tag;

            if (btn.Text != "" && btn.Text != "★")
            {
                if (currentMoveTiles.Contains(position) && boardTiles.ContainsKey(position))
                {
                    Tile tileToReturn = boardTiles[position];
                    players[currentPlayerIndex].Tiles.Add(tileToReturn);

                    boardTiles.Remove(position);

                    RestoreButton(btn, position);

                    currentMoveTiles.Remove(position);
                    UpdateDisplay();
                }
                return;
            }

            if (selectedTile != null)
            {
                btn.Text = selectedTile.Letter.ToString();
                btn.BackColor = Color.LightGray;
                btn.ForeColor = Color.Black;

                boardTiles[position] = selectedTile;

                currentMoveTiles.Add(position);

                players[currentPlayerIndex].Tiles.Remove(selectedTile);

                selectedTile = null;
                UpdateDisplay();
            }
        }

        private void RestoreButton(Button btn, Point position)
        {
            Dictionary<Point, Color> bonusCells = GetBonusCells();

            if (position.X == CENTER && position.Y == CENTER)
            {
                btn.BackColor = Color.DarkRed;
                btn.ForeColor = Color.White;
                btn.Text = "★";
            }
            else if (bonusCells.ContainsKey(position))
            {
                btn.BackColor = bonusCells[position];
                if (btn.BackColor == Color.Red || btn.BackColor == Color.Yellow)
                {
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.ForeColor = Color.Black;
                }
                btn.Text = "";
            }
            else
            {
                btn.BackColor = Color.Beige;
                btn.ForeColor = Color.Black;
                btn.Text = "";
            }
        }

        private void PlayerTile_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Tag is Tile tile)
            {
                if (selectedTile == tile)
                {
                    selectedTile = null;
                }
                else
                {
                    selectedTile = tile;
                }

                UpdateDisplay();
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (currentMoveTiles.Count == 0)
            {
                MessageBox.Show("Вы не выложили ни одной фишки!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateMove())
            {
                MessageBox.Show("Такого слова нет! Попробуйте другое.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CalcScore();
            EndTurn();
        }

        private bool ValidateMove()
        {
            if (isFirstMove)
            {
                bool passesThroughCenter = false;
                foreach (Point pos in currentMoveTiles)
                {
                    if (pos.X == CENTER && pos.Y == CENTER)
                    {
                        passesThroughCenter = true;
                        break;
                    }
                }

                if (!passesThroughCenter)
                {
                    MessageBox.Show("Первое слово должно проходить через центральную клетку!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                isFirstMove = false;
            }
            else
            {
                bool touchesExisting = false;
                foreach (Point pos in currentMoveTiles)
                {
                    if (HasNeighbor(pos))
                    {
                        touchesExisting = true;
                        break;
                    }
                }

                if (!touchesExisting)
                {
                    MessageBox.Show("Новое слово должно соприкасаться с существующими!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return ValidateAllWords();
        }

        private bool HasNeighbor(Point pos)
        {
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newX = pos.X + dx[i];
                int newY = pos.Y + dy[i];

                if (newX >= 0 && newX < BOARD_SIZE && newY >= 0 && newY < BOARD_SIZE)
                {
                    Button neighborBtn = boardButtons[newX, newY];
                    if (neighborBtn.Text != "" && neighborBtn.Text != "★")
                        return true;
                }
            }

            return false;
        }

        private bool ValidateAllWords()
        {
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                string word = "";
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    Button btn = boardButtons[row, col];
                    if (btn.Text != "" && btn.Text != "★")
                    {
                        word += btn.Text;
                    }
                    else if (word.Length > 1)
                    {
                        if (!IsValidWord(word))
                            return false;
                        word = "";
                    }
                    else
                    {
                        word = "";
                    }
                }
                if (word.Length > 1 && !IsValidWord(word))
                    return false;
            }

            for (int col = 0; col < BOARD_SIZE; col++)
            {
                string word = "";
                for (int row = 0; row < BOARD_SIZE; row++)
                {
                    Button btn = boardButtons[row, col];
                    if (btn.Text != "" && btn.Text != "★")
                    {
                        word += btn.Text;
                    }
                    else if (word.Length > 1)
                    {
                        if (!IsValidWord(word))
                            return false;
                        word = "";
                    }
                    else
                    {
                        word = "";
                    }
                }
                if (word.Length > 1 && !IsValidWord(word))
                    return false;
            }

            return true;
        }

        private bool IsValidWord(string word)
        {
            if (dictionary.ContainsKey(word.ToUpper()))
                return true;

            if (word.Contains('Ё'))
            {
                string wordWithE = word.Replace('Ё', 'Е');
                if (dictionary.ContainsKey(wordWithE))
                    return true;
            }

            if (word.Contains('Е'))
            {
                string wordWithYo = word.Replace('Е', 'Ё');
                if (dictionary.ContainsKey(wordWithYo))
                    return true;
            }

            return false;
        }

        private void CalcScore()
        {
            int totalScore = 0;

            List<string> newWords = GetNewWords();

            foreach (string word in newWords)
            {
                int wordScore = CalcWordScore(word);
                totalScore += wordScore;
            }

            if (currentMoveTiles.Count == 7)
            {
                totalScore += 50;
            }

            players[currentPlayerIndex].Score += totalScore;

            MessageBox.Show($"Начислено очков: {totalScore}\nСлова: {string.Join(", ", newWords)}",
                "Результат хода", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int CalcWordScore(string word)
        {
            int score = 0;
            int wordMultiplier = 1;

            foreach (char letter in word)
            {
                if (letterScores.ContainsKey(letter))
                {
                    score += letterScores[letter];
                }
            }

            return score * wordMultiplier;
        }

        private List<string> GetNewWords()
        {
            List<string> words = new List<string>();

            foreach (Point pos in currentMoveTiles)
            {
                string horizontalWord = GetWordAtPosition(pos, true);
                if (horizontalWord.Length > 1 && !words.Contains(horizontalWord))
                {
                    words.Add(horizontalWord);
                }

                string verticalWord = GetWordAtPosition(pos, false);
                if (verticalWord.Length > 1 && !words.Contains(verticalWord))
                {
                    words.Add(verticalWord);
                }
            }

            return words;
        }

        private string GetWordAtPosition(Point pos, bool horizontal)
        {
            int row = pos.X;
            int col = pos.Y;
            string word = "";

            if (horizontal)
            {
                int startCol = col;
                while (startCol > 0 && boardButtons[row, startCol - 1].Text != "")
                {
                    startCol--;
                }

                for (int c = startCol; c < BOARD_SIZE; c++)
                {
                    if (boardButtons[row, c].Text != "")
                    {
                        word += boardButtons[row, c].Text;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                int startRow = row;
                while (startRow > 0 && boardButtons[startRow - 1, col].Text != "")
                {
                    startRow--;
                }

                for (int r = startRow; r < BOARD_SIZE; r++)
                {
                    if (boardButtons[r, col].Text != "")
                    {
                        word += boardButtons[r, col].Text;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return word;
        }

        private void EndTurn()
        {
            Player currentPlayer = players[currentPlayerIndex];
            while (currentPlayer.Tiles.Count < 7 && tileBag.Count > 0)
            {
                Tile newTile = DrawTile();
                if (newTile != null)
                {
                    currentPlayer.Tiles.Add(newTile);
                }
            }

            currentMoveTiles.Clear();
            selectedTile = null;

            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

            UpdateDisplay();

            CheckGameEnd();
        }

        private void CheckGameEnd()
        {
            bool gameEnd = false;

            foreach (Player player in players)
            {
                if (player.Tiles.Count == 0 && tileBag.Count == 0)
                {
                    gameEnd = true;
                    break;
                }
            }

            if (gameEnd)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            Player winner = players.OrderByDescending(p => p.Score).First();
            string result = $"Игра окончена!\nПобедитель: {winner.Name} с {winner.Score} очками!\n\n";

            foreach (Player player in players.OrderByDescending(p => p.Score))
            {
                result += $"{player.Name}: {player.Score} очков\n";
            }

            MessageBox.Show(result, "Игра завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Пропустить ход?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                UpdateDisplay();
            }
        }

        private void BtnExchange_Click(object sender, EventArgs e)
        {
            Player currentPlayer = players[currentPlayerIndex];

            if (currentPlayer.Tiles.Count == 0)
            {
                MessageBox.Show("У вас нет фишек для обмена!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tileBag.Count < 7) 
            {
                MessageBox.Show("В мешке недостаточно фишек для обмена!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var exchangeForm = new ExchangeTilesForm(currentPlayer.Tiles))
            {
                if (exchangeForm.ShowDialog() == DialogResult.OK)
                {
                    List<Tile> tilesToExchange = exchangeForm.SelectedTiles;

                    if (tilesToExchange.Count > 0)
                    {
                        string exchangeMessage = $"Вы действительно хотите обменять {tilesToExchange.Count} фишек?\n";
                        foreach (Tile tile in tilesToExchange)
                        {
                            exchangeMessage += $"{tile.Letter} ";
                        }

                        if (MessageBox.Show(exchangeMessage, "Подтверждение обмена",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            foreach (Tile tile in tilesToExchange)
                            {
                                currentPlayer.Tiles.Remove(tile);
                                tileBag.Add(tile); 
                            }

                            ShuffleTiles();

                            int tilesToDraw = Math.Min(7 - currentPlayer.Tiles.Count, tileBag.Count);
                            for (int i = 0; i < tilesToDraw; i++)
                            {
                                currentPlayer.Tiles.Add(DrawTile());
                            }

                            MessageBox.Show($"Обменено {tilesToExchange.Count} фишек. Получено {tilesToDraw} новых фишек.",
                                "Обмен завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                            UpdateDisplay();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Вы не выбрали ни одной фишки для обмена.", "Обмен отменен",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void BtnEndGame_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Завершить игру досрочно?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                EndGame();
            }
        }
    }
}