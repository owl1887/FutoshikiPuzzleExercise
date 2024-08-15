

using System.Diagnostics;

namespace FutoshikiPuzzle
{
    public partial class MainPage : ContentPage
    {
        private FutoshikiPuzzlee _puzzle;

        public MainPage()
        {
            InitializeComponent();
        }

        // Click event za dugme Zapocni igru
        private void OnStartGameClicked(object sender, EventArgs e)
        {
            string selectedGridSize = GridSizePicker.SelectedItem?.ToString();

            if (selectedGridSize != null)
            {
                int gridSize = int.Parse(selectedGridSize.Split('x')[0]);
                _puzzle = new FutoshikiPuzzlee(gridSize);
                GenerateGridUI(gridSize);
            }
            else
            {
                DisplayAlert("Greska", "Molimo vas da odaberete velicinu mreze.", "OK");
            }
        }

        private void GenerateGridUI(int gridSize)
        {
            // Ciscenje bilo kakvih postojecih UI elemenata u mrezi 
            PuzzleGrid.Children.Clear();

            // Ciscenje redova i kolona 
            PuzzleGrid.RowDefinitions.Clear();
            PuzzleGrid.ColumnDefinitions.Clear();

            // Definisanje redova i kolona za celije i za strelice
            for (int i = 0; i < gridSize * 2 - 1; i++)
            {
                if (i % 2 == 0)
                {
                    // Normalne redovi/kolone za celije
                    PuzzleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
                    PuzzleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
                }
                else
                {
                    // Smanjeni redovi/kolone za strelice (cetvrtina velicine)
                    PuzzleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.25, GridUnitType.Star) });
                    PuzzleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.25, GridUnitType.Star) });
                }
            }

            // Generisanje mreze celija
            Cell[,] cells = new Cell[gridSize, gridSize];
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    cells[row, col] = new Cell(row, col, gridSize);

                    var entry = new Entry
                    {
                        Text = "", //Zapocinjanje sa praznim celijama 
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.5, // POvecan font za labele u koje unosimo brojeve
                        BackgroundColor = Colors.Gray,
                        TextColor = Colors.Black,
                        IsReadOnly = true, // onemoguceno koristenje tastature za unos u celije jer smo odabrali da unosimo brjeve klikom na celiju
                    };

                    var frame = new Frame
                    {
                        Content = entry,
                        BorderColor = Colors.Black,
                        BackgroundColor = Colors.White,
                        CornerRadius = 30,
                        Padding = new Thickness(0),
                        Margin = new Thickness(5), // margina za prosto izmedju celija
                        HasShadow = false
                    };

                    // Podesavanje fonta na osnovu velicine celije (entry labele) tako da se broj uvijek vidi i u 4x4 i u 9x9 mrezi
                    entry.SizeChanged += (s, e) =>
                    {
                        entry.FontSize = entry.Width / 2; // Podesavanje fonta na osnovu sirine celije (entry labele)
                    };

                    // Add a TapGestureRecognizer to increment the number
                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += (s, e) =>
                    {
                        if (int.TryParse(entry.Text, out int value))
                        {
                            value++;
                            if (value > gridSize)
                            {
                                value = 1; //Resetovanje celije na broj 1 kada broj predje broj velicine mreze, tj. ako imamo 4x4 mrezu i ako klikamo na celiju nonstop,poslije broja 4 ce se resetovati na 1
                            }
                        }
                        else
                        {
                            value = 1; // Zapocni sa 1 ako je celija prazna
                        }
                        entry.Text = value.ToString();
                    };
                    entry.GestureRecognizers.Add(tapGesture);

                    // Setovanje reda i kolone za Frame
                    Grid.SetRow(frame, row * 2); //Koristimo 2x spacing da bi mogli smjstiti strelice izmedju celija
                    Grid.SetColumn(frame, col * 2);

                    // Dodavanje Frejma u mrezu puzle
                    PuzzleGrid.Children.Add(frame);
                }
            }

            // Dodavanje strelica (Constraint)-ova izmedju celija [Comtraint smo dodali kao klasu sa pravilima koja vaze za strelice]
            Random random = new Random();
            int maxConstraints = Math.Min((gridSize * gridSize) / 2, (gridSize - 1) * gridSize + (gridSize - 1) * gridSize); // Max number of constraints
            int addedConstraints = 0;

            // Horizontalne strelice
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize - 1; col++)
                {
                    if (addedConstraints < maxConstraints && random.Next(2) == 0)
                    {
                        addedConstraints++;
                        var constraintType = random.Next(2) == 0 ? ConstraintType.GreaterThan : ConstraintType.LessThan;
                        var constraint = new Constraint(cells[row, col], cells[row, col + 1], constraintType);

                        // Vizuelni prikaz strelica odnosno Constraint-ova
                        var arrowLabel = new Label
                        {
                            Text = constraintType == ConstraintType.GreaterThan ? "►" : "◄",
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontSize = 5, // Reduced font size for arrows
                            FontAttributes = FontAttributes.Bold, // Make arrows bold
                            BackgroundColor = Colors.Transparent,
                            TextColor = Colors.Black,
                        };

                        // Podesavanje redova i kolona za strelice koje ce biti izmedju redova i kolona
                        Grid.SetRow(arrowLabel, row * 2); // Same row as the cell
                        Grid.SetColumn(arrowLabel, col * 2 + 1); // Between cells

                        // Dodavanje strelica u mrezu igre
                        PuzzleGrid.Children.Add(arrowLabel);
                    }
                }
            }

            // Verticalne strelice
            for (int row = 0; row < gridSize - 1; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    if (addedConstraints < maxConstraints && random.Next(2) == 0)
                    {
                        addedConstraints++;
                        var constraintType = random.Next(2) == 0 ? ConstraintType.GreaterThan : ConstraintType.LessThan;
                        var constraint = new Constraint(cells[row, col], cells[row + 1, col], constraintType);

                        // Vizuelna reprezentacije za strelice
                        var arrowLabel = new Label
                        {
                            Text = constraintType == ConstraintType.GreaterThan ? "▲" : "▼",
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontSize = 5, // Reduced font size for arrows
                            FontAttributes = FontAttributes.Bold, // Make arrows bold
                            BackgroundColor = Colors.Transparent,
                            TextColor = Colors.Black,
                        };

                        // Podesavanje redova i kolona za smjestanje strelica izmedju njih
                        Grid.SetRow(arrowLabel, row * 2 + 1); // Between rows
                        Grid.SetColumn(arrowLabel, col * 2); // Same column as the cell

                        // Dodavanje strelica u mrezu 
                        PuzzleGrid.Children.Add(arrowLabel);
                    }
                }
            }
        }








        private int GetNextValueForCell(Cell cell)
        {
            //  Logika za dodavanje naredne vrijednosti u celiju kada korisnik klikne na nju tj. dodirne
            //  npr. prolazak kroz moguce vrijednosti
            return cell.Value.HasValue ? (cell.Value.Value % _puzzle.GridSize) + 1 : 1;
        }

        // Handle event za undo dugme odnosno u nasem slucaju RESET dugme, posto nije bilo potrebe da se vraca korak unazad ako korisnik moze gresku(pogresan broj u celiji) ispraviti jednim klikom 
        private void OnUndoClicked(object sender, EventArgs e)
        {
            _puzzle?.UndoLastMove();
            GenerateGridUI(_puzzle.GridSize);
        }

        // Handle event za  dugme pokazi savjet
        private void OnShowHintsClicked(object sender, EventArgs e)
       {
           _puzzle?.ProvideHint();
           GenerateGridUI(_puzzle.GridSize);
       }
       
       // Handle event za dugme Pokazi rjesenje
       private void OnShowSolutionClicked(object sender, EventArgs e)
       {
           _puzzle?.Solve();
           GenerateGridUI(_puzzle.GridSize);
       }
        //handle event za dugme za provjeru rjesenja
        private void CheckAnswerButton_Clicked(object sender, EventArgs e)
        {
            bool isCorrect = true;
            int gridSize = PuzzleGrid.RowDefinitions.Count / 2 + 1;

            // Provjera da li ima duplih brojeva u redovima
            for (int row = 0; row < gridSize; row++)
            {
                HashSet<int> rowNumbers = new HashSet<int>();
                for (int col = 0; col < gridSize; col++)
                {
                    // pronalazak odgovarajueg Frejma za specifican red i kolonu
                    var frame = PuzzleGrid.Children
                        .Cast<View>()
                        .FirstOrDefault(c => Grid.GetRow(c) == row * 2 && Grid.GetColumn(c) == col * 2) as Frame;
                    var entry = frame?.Content as Entry;

                    if (entry != null && int.TryParse(entry.Text, out int number))
                    {
                        if (rowNumbers.Contains(number))
                        {
                            isCorrect = false;
                            break;
                        }
                        rowNumbers.Add(number);
                    }
                    else
                    {
                        isCorrect = false;
                        break;
                    }
                }
                if (!isCorrect) break;
            }

            // Provjera kolona za duple brojeve
            if (isCorrect)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    HashSet<int> colNumbers = new HashSet<int>();
                    for (int row = 0; row < gridSize; row++)
                    {
                        // pronalazak odgovarajueg Frejma za specifican red i kolonu
                        var frame = PuzzleGrid.Children
                            .Cast<View>()
                            .FirstOrDefault(c => Grid.GetRow(c) == row * 2 && Grid.GetColumn(c) == col * 2) as Frame;
                        var entry = frame?.Content as Entry;

                        if (entry != null && int.TryParse(entry.Text, out int number))
                        {
                            if (colNumbers.Contains(number))
                            {
                                isCorrect = false;
                                break;
                            }
                            colNumbers.Add(number);
                        }
                        else
                        {
                            isCorrect = false;
                            break;
                        }
                    }
                    if (!isCorrect) break;
                }
            }

            // Obavjestavanje korisnika sa odgovarajucom porukom
            if (isCorrect)
            {
                DisplayAlert("Svaka cast", "Cestitamo na rjesavanju puzle!", "OK");
            }
            else
            {
                DisplayAlert("Provjerite vase korake", "Imate duple brojeve, molimo projerite brojeve koje ste unosili.", "OK");
            }
        }


        private bool SolvePuzzle(Cell[,] cells, Constraint[] constraints, int row = 0, int col = 0)
        {
            int gridSize = cells.GetLength(0);

            if (row == gridSize)
            {
                
                return true;
            }

            int nextRow = (col == gridSize - 1) ? row + 1 : row;
            int nextCol = (col == gridSize - 1) ? 0 : col + 1;

            if (cells[row, col].Value.HasValue)
            {
                // Preskoci vec popunjene celije
                return SolvePuzzle(cells, constraints, nextRow, nextCol);
            }

            foreach (int value in cells[row, col].Candidates)
            {
                if (IsValid(cells, row, col, value) &&
                    constraints.All(constraint => constraint.IsSatisfied()))
                {
                    cells[row, col].Value = value;

                    if (SolvePuzzle(cells, constraints, nextRow, nextCol))
                    {
                        return true;
                    }

                    cells[row, col].Value = null; // Backtrack algoritam
                }
            }

            return false; // Nije pronadjeno rjesenje
        }


        private (int row, int col) FindEmptyCell(Cell[,] cells)
        {
            for (int row = 0; row < cells.GetLength(0); row++)
            {
                for (int col = 0; col < cells.GetLength(1); col++)
                {
                    if (!cells[row, col].Value.HasValue)
                    {
                        return (row, col);
                    }
                }
            }
            return (-1, -1); // nema praznih celija
        }

        private bool IsValid(Cell[,] cells, int row, int col, int value)
        {
            // Privremeno postavi vrijednost da se testira ispravnost
            var tempValue = cells[row, col].Value;
            cells[row, col].Value = value;

            bool isValid = true;

            // Provjeri red
            for (int c = 0; c < cells.GetLength(1); c++)
            {
                if (c != col && cells[row, c].Value == value)
                {
                    isValid = false;
                    break;
                }
            }

            // Provjeri kolonu
            if (isValid)
            {
                for (int r = 0; r < cells.GetLength(0); r++)
                {
                    if (r != row && cells[r, col].Value == value)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            // Vrati originalnu vrijednost
            cells[row, col].Value = tempValue;

            return isValid;
        }

        private bool ValidateConstraints(Cell[,] cells, Constraint[] constraints)
        {
            foreach (var constraint in constraints)
            {
                if (!constraint.IsSatisfied())
                {
                    return false;
                }
            }
            return true;
        }
        //original kod bez paralelizacije 
          private void ShowSolutionButton_Clicked(object sender, EventArgs e)
          {
              int gridSize = PuzzleGrid.RowDefinitions.Count / 2 + 1;
              Cell[,] cells = new Cell[gridSize, gridSize];
              Constraint[] constraints = GetConstraints(); // metoda da prikupimo sva pravila odnosno Constraint-ove
        
              // inicijalizuj celije
              for (int row = 0; row < gridSize; row++)
              {
                  for (int col = 0; col < gridSize; col++)
                  {
                      cells[row, col] = new Cell(row, col, gridSize);
                  }
              }
        
              // Rjesavanje puzle
              if (SolvePuzzle(cells, constraints))
              {
                  //Popuni mrezu sa rjesenjem
                  foreach (var cell in cells)
                  {
                      var frame = PuzzleGrid.Children
                          .Cast<View>()
                          .FirstOrDefault(c => Grid.GetRow(c) == cell.Row * 2 && Grid.GetColumn(c) == cell.Column * 2) as Frame;
                      var entry = frame?.Content as Entry;
        
                      if (entry != null)
                      {
                          entry.Text = cell.Value.ToString();
                      }
                  }
                  DisplayAlert("Rjesenje", "Puzla je rijesena!", "OK");
              }
              else
              {
                  DisplayAlert("Greska", "Nemoguce rijesiti puzlu.", "OK");
              }
          }







        //komentar

        private void ShowHintButton_Clicked(object sender, EventArgs e)
        {
            int gridSize = PuzzleGrid.RowDefinitions.Count / 2 + 1;
            Cell[,] cells = new Cell[gridSize, gridSize];
            Constraint[] constraints = GetConstraints(); // Method to get all constraints

            // Initialize cells from the current state of the grid
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    var frame = PuzzleGrid.Children
                        .Cast<View>()
                        .FirstOrDefault(c => Grid.GetRow(c) == row * 2 && Grid.GetColumn(c) == col * 2) as Frame;
                    var entry = frame?.Content as Entry;

                    int value;
                    if (entry != null && int.TryParse(entry.Text, out value))
                    {
                        cells[row, col] = new Cell(row, col, gridSize) { Value = value };
                    }
                    else
                    {
                        cells[row, col] = new Cell(row, col, gridSize);
                    }
                }
            }

            // Find a cell that can be determined based on the current constraints and state
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    if (!cells[row, col].Value.HasValue)
                    {
                        // Check possible values for the cell based on constraints
                        var possibleValues = cells[row, col].Candidates
                            .Where(val => IsValid(cells, row, col, val) &&
                                          constraints.All(constraint => constraint.IsSatisfied()))
                            .ToList();

                        if (possibleValues.Count == 1)
                        {
                            // If there's only one valid candidate, it's a strong hint
                            var hintValue = possibleValues.First();

                            var hintFrame = PuzzleGrid.Children
                                .Cast<View>()
                                .FirstOrDefault(c => Grid.GetRow(c) == row * 2 && Grid.GetColumn(c) == col * 2) as Frame;
                            var hintEntry = hintFrame?.Content as Entry;

                            if (hintEntry != null)
                            {
                                hintEntry.Text = hintValue.ToString();
                            }

                            DisplayAlert("Savjet", $"Ispravan broj za Red  {row + 1}, Kolonu {col + 1} je {hintValue}.", "OK");
                            return;
                        }
                    }
                }
            }

            // If no hints could be found
            DisplayAlert("Savjet", "Nisu pronadjeni savjeti na osnovu trenutnog stanja u igri.", "OK");
        }

        private Constraint[] GetConstraints()
        {
            var constraints = new List<Constraint>();

            // Prikupi contraintove iz mreze
            

            return constraints.ToArray();
        }




    }

}
