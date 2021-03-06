using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
namespace AmysMasterMind
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        // constants Varibles
        private const int NUM_OF_TURNS = 10;

        private const int PEG_SIZE = 25;

        // globals varibles
        private int number_of_pegs = 4;
        private int checkColBtn = 5;
        private int currentRow;
        private Color emptyColour = Color.Sienna;
        private Color selected_colour;

        //TRIAL AND ERROR
        // a 2D int array to save user data
       // private List<int[]> saveData;
        private int[] firstRowData = new int[4];
        private GameData gameData = new GameData();
        private int lastRowBlkPegs = 0;

        //Creating all the random colours in an array
        Color[] myColours = new Color[]
                            {Color.White, Color.Green,  Color.Red, Color.Yellow,  Color.Blue, Color.Purple,  Color.Pink, Color.Black};

        int counterOfAnswers;

        // an integer array of 4 numbers
        Color[] _userGuess = new Color[4];

        // filling in the GenerateCode
        Color[] _theCode = new Color[4]; 

        public MainPage()
        {
            InitializeComponent();
            CreatingTheBoard();
        }

        //Creating the new game part
        private void CreatingTheBoard()
        {
            int row = 1;
            for (row = 0; row < 10; row++)
            {
                MakeingOneRow(row);
            }
           
            CheckAnswerButton(NUM_OF_TURNS - 1);
            DisplayAlert("--New Game--", "Press New Game to Start!", "OK");
        }


        private void CheckAnswerButton(int row)
        {
            Button b;
            b = new Button(); 
            b.Text = "Check"; 
            b.SetValue(Grid.RowProperty, row);
            b.SetValue(Grid.ColumnProperty, checkColBtn);
            b.Margin = 3;
            b.HorizontalOptions = LayoutOptions.Center;
            b.VerticalOptions = LayoutOptions.Center;
          
            // When it is clicked
            b.Clicked += CheckGuessButton_Clicked;
            GrdEntered.Children.Add(b);
            currentRow = row;
        }

        private void MakeingOneRow(int row)
        {
            BoxView b;
            Grid g;
            TapGestureRecognizer tap;
            int col = 0;

            // instantiate
            tap = new TapGestureRecognizer();
            tap.Tapped += UserTurnSpace_Tapped;

            // Createing the feeback GRid
            #region Create Feedback Grid
            g = new Grid();
            g.Margin = 2;
            g.SetValue(Grid.RowProperty, 9-row);
            g.SetValue(Grid.ColumnProperty, col);
            g.HorizontalOptions = LayoutOptions.Center;
            g.VerticalOptions = LayoutOptions.Center;

            // ROWS 2
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());

            //COLUMNS 2
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            GrdEntered.Children.Add(g);
            #endregion

            //adding 4 boxviews to the feedback grid
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    #region Create One BoxView
                    b = new BoxView(); // instantiate
                    b.Color = Color.Sienna;
                    b.Margin = 2;
                    b.SetValue(Grid.RowProperty, r);
                    b.SetValue(Grid.ColumnProperty, c);
                    b.HorizontalOptions = LayoutOptions.Center;
                    b.VerticalOptions = LayoutOptions.Center;
                    b.HeightRequest = 8;
                    b.WidthRequest = 8;
                    b.CornerRadius = 4;
                    g.Children.Add(b);




                    #endregion

                }
            }

            // boxviews
            for (col = 1; col < 5; col++)
            {

                b = new BoxView(); // instantiate
                b.Margin = 2;
                b.SetValue(Grid.RowProperty, 9-row);
                b.SetValue(Grid.ColumnProperty, col);
                b.HorizontalOptions = LayoutOptions.Center;
                b.VerticalOptions = LayoutOptions.Center;
                b.HeightRequest = PEG_SIZE;
                b.WidthRequest = PEG_SIZE;
                b.CornerRadius = PEG_SIZE / 2;
                //b.Color = emptyColour; // default grid colour

                // row 0, 1,....9
                // 9, 8 ,7
                // 0
                if (row < gameData.rows)
                {
                    switch (col){
                        case 1:
                            b.Color = myColours[gameData.posData[row, col - 1]];
                            break;
                        case 2:
                            b.Color = myColours[gameData.posData[row, col - 1]];
                            break;
                        case 3:
                            b.Color = myColours[gameData.posData[row, col - 1]];
                            break;
                        case 4:
                            b.Color = myColours[gameData.posData[row, col - 1]];
                            break;
                    }
                } else
                {
                    b.Color = emptyColour;
                }

                // add tap event
                b.GestureRecognizers.Add(tap);

                // add to the the main grid
                GrdEntered.Children.Add(b);

            }

            #region Create the Colour Choice Grid
            MakingColourChosenGrid();
            #endregion


        }

        private void CheckGuessButton_Clicked(object sender, EventArgs e)
        {
            //chnaging the value of the row button up each time (take away 1)
            Button b;
            int row;

            //if the user didnt pick 4 colours
            if (!CheckIfRowIsFull()) 
            {
          
                DisplayAlert("HEY! ",
                             "Choose four colours!!!",
                             "Ok");
                return;
            }

            //if u picked all 4 colours
            if (currentRow == 0)
            {
                Debug.WriteLine("this is the line num 10");
                display.IsVisible = true;/////THIS IS THE ANSWERS 

                //display alert.
                DisplayAlert("You Lost!", "Hard Luck", " continue ");

                return; 
            }

            b = (Button)sender;
            row = (int)b.GetValue(Grid.RowProperty);

            // all four colors has been picked up
            // let's check what are these colours - (with number)
            int[] colorIndexs = new int[4];
            for(int i = 0; i < _userGuess.Length; i++)
            {
                int colorCounter = 0;
                foreach (Color eachColor in myColours)
                {
                    if (_userGuess[i] == eachColor)
                        colorIndexs[i] = colorCounter;
                    colorCounter++;                             
                }
            }
            // Debug.WriteLine(colorIndexs[3]);
            // [0, 7, 4, 5] gonna save this to a 2D array (an int array nested in a list)
            //saveData.Add(colorIndexs);
            firstRowData = colorIndexs;
            // save eachRow data (colorIndexs) in gameData
            gameData.rows++;
            for (int i = 0; i < 4; i++)
            {
                gameData.posData[gameData.rows - 1, i] = colorIndexs[i];
            }

            //checking if the users guess is correct / incorrect
            CheckingUsersGuess();

            // check if it's the last row
            /*
            Debug.WriteLine(gameData.rows);
            if (gameData.rows == 10 && lastRowBlkPegs < 4)
            {
                Debug.WriteLine("abcde");
                display.IsVisible = true;/////THIS IS THE ANSWERS 
            }
            */

            b.SetValue(Grid.RowProperty, row - 1);

            //the value of the current row is reset
            currentRow = row - 1;
        }

        //This is being all broke and bad and awful :((((-------------------------
        private void CheckingUsersGuess()
        {
            int whitePegs = 0;
            int blackPegs = 0;

            //check if the user got any black or white pegs
            for (int x = 0; x < 4; x++)
            {
                bool WhiteMatch = false;
                for (int y = 0; y < 4; y++)
                {
                    if (_theCode[x] == _userGuess[y])
                    {
                        if (WhiteMatch == false)
                        {
                            whitePegs++;
                            WhiteMatch = true;
                        }

                        if (x == y)
                        {
                            blackPegs++;
                        }
                    }
                }
            }

            Grid GridFeedback = null;

            //array is full so displaying the feedback in the current row
            foreach (var item in GrdEntered.Children)
            {

                if (item.GetType() == typeof(Grid))
                {
                    if (currentRow == (int)((Grid)item).GetValue(Grid.RowProperty))
                    {
                        //Getting the feedback
                        GridFeedback = (Grid)item;
                        break; 
                    }
                }
            } 


            // fill in the feedback
            //r = Row c=Collums coun= Counter
            int r = 0; 
            int c = 0;
            int counter = 0;
            BoxView b;

          
            while (r < 4)
            {
                //Box for the feedback --have done in makingOneRow
                b = new BoxView();
                b.BackgroundColor = emptyColour;
                if (r < blackPegs)
                {
                    b.BackgroundColor = Color.Black;
                }
                else if (r < whitePegs)
                {
                    b.BackgroundColor = Color.White;
                }
                b.Margin = 2;
                b.SetValue(Grid.RowProperty, c);
                b.SetValue(Grid.ColumnProperty, counter);
                b.HorizontalOptions = LayoutOptions.Center;
                b.VerticalOptions = LayoutOptions.Center;
                b.HeightRequest = 8;
                b.WidthRequest = 8;
                b.CornerRadius = 4;
                GridFeedback.Children.Add(b);

                r++;
                if (r == number_of_pegs / 2)
                {
                    c = 1;
                    counter = 0;
                }
                else
                {
                    counter++;
                }
            } 

            // if the score was a win //ALl BLACKPEGS---------------------------------------------------
            if (blackPegs == 4)
            {
                DisplayAlert("You Won!", "Well Done", " continue ");
            }
            lastRowBlkPegs = blackPegs;
        }

        private bool CheckIfRowIsFull()
        {
            bool answer = true;

            //Checking all boxview on that row
            foreach (var item in GrdEntered.Children)
            {
                //If it is equal to the boxview
                if (item.GetType() == typeof(BoxView))
                {
                    int r = (int)item.GetValue(Grid.RowProperty);
                    if (r == currentRow)
                    {
                        if (((BoxView)item).Color == emptyColour)
                        {
                            answer = false;
                            break;
                        }
                    }
                }
            }
            return answer;
        }


         private void UserTurnSpace_Tapped(object sender, EventArgs e)
        {
            BoxView b = (BoxView)sender;
            // place the currentColour on the sender
            // check that the user tapped on a boxview
            // on the same row as the currentRow
            if (((int)b.GetValue(Grid.RowProperty) != currentRow) ||
                (selected_colour == null))
            {
                return;
            }

            b.Color = selected_colour;
            //  addining it to a guess arrray. -1 to go to next column
            _userGuess[(int)b.GetValue(Grid.ColumnProperty) - 1] = b.Color;
        }


        private void MakingColourChosenGrid()
        {
            int r, c = 0, iColour = 0;
            BoxView b; // this holds each colour
            TapGestureRecognizer tap;

            tap = new TapGestureRecognizer();
            tap.Tapped += ColourChoice_Tapped;

            // The same as feebackGrid ------------------------
            for (r = 0; r < 2; r++)
            {
                for (c = 0; c < 4; c++)
                {
                    #region Create One BoxView
                    b = new BoxView(); 
                    b.Margin = 2;
                    b.SetValue(Grid.RowProperty, r);
                    b.SetValue(Grid.ColumnProperty, c);
                    b.HorizontalOptions = LayoutOptions.Center;
                    b.VerticalOptions = LayoutOptions.Center;
                    b.HeightRequest = PEG_SIZE;
                    b.WidthRequest = PEG_SIZE;
                    b.CornerRadius = PEG_SIZE / 2;
                    b.GestureRecognizers.Add(tap);
                    // b.Color = emptyColour; // some form of brown for now
                    b.Color = myColours[iColour++];
                    GrdColourChoice.Children.Add(b);
                    #endregion

                } 
            } 

        }


        private void ColourChoice_Tapped(object sender, EventArgs e)
        {
           //Saves the colour to use it for the peg //Saves in varible
            selected_colour = ((BoxView)sender).Color;

        }


        private void BtnNewGame_Clicked(object sender, EventArgs e)
        {
            // New code being generate
            CreateNewCode();

            //clearing the board and reset the currentrow value
            foreach (var item in GrdEntered.Children)
            {
                if (item.GetType() == typeof(BoxView))
                {
                    ((BoxView)item).Color = emptyColour;
                }

                if (item.GetType() == typeof(Grid))
                {
                    // feedback grid has 4 children
                    foreach (var My in ((Grid)item).Children)
                    {
                        if (My.GetType() == typeof(BoxView))
                        {
                            ((BoxView)My).Color = emptyColour;
                        }
                    }
                }

                if (item.GetType() == typeof(Button))
                {
                    // set to the first guess
                    ((Button)item).SetValue(Grid.RowProperty, NUM_OF_TURNS - 1);

                }
            } //End of for each
            currentRow = NUM_OF_TURNS - 1;

            display.IsVisible = false;/////THIS IS THE ANSWERS 

            //THIS IS ALL TRIAL AND ERROR FOR SAVING GAME-----------------------------------------------------------------------------
            // save prev Data (if it exist)
            //File and path you want to create and write to
            //string fileName = @"C:\Users\amywa\Downloads\saveData\saveData.txt";
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.txt");
            //C:\Users\amywa\OneDrive - GMIT\App Development\AmysMasterMind
            // C:\Users\amywa\Downloads\saveData
            //Check if the file exists
            if (firstRowData != null)
            {
                // Create the file and use streamWriter to write text to it.
                //If the file existence is not check, this will overwrite said file.
                //Use the using block so the file can close and vairable disposed correctly
                using (StreamWriter writer = File.CreateText(fileName))
                {
                    writer.WriteLine("[" + firstRowData[0] + ", " + firstRowData[1] + ", " + firstRowData[2] + ", "  + firstRowData[3] + "]");
                   
                }
            }
            Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }

        private void CreateNewCode()
        {
            //Creaiting an array of 4 differnt ints with none the same
            int[] iCode = new int[] { -1, -1, -1, -1 };
            int current, numColours;
            Boolean HasDuplicates = false;
          
            // Creating a new Random number
            Random random = new Random(DateTime.Now.Millisecond);

            numColours = 0;
            while (numColours < 4)
            {
                HasDuplicates = false;
                current = random.Next(0, 8);
                for (int index = 0; index < 4; index++)
                {
                    if (iCode[index] == current)
                    {
                        HasDuplicates = true;
                    } 
                }
                if (HasDuplicates == false)
                {
                    iCode[numColours++] = current;
                } 
            }

            //Have a new set  - all differnt
            MyColour0.Color = myColours[iCode[0]];
            MyColour1.Color = myColours[iCode[1]];
            MyColour2.Color = myColours[iCode[2]];
            MyColour3.Color = myColours[iCode[3]];

            // store in the global array for checking the guesses
            _theCode[0] = myColours[iCode[0]];
            _theCode[1] = myColours[iCode[1]];
            _theCode[2] = myColours[iCode[2]];
            _theCode[3] = myColours[iCode[3]];

            Debug.WriteLine("0 position "+ _theCode[0]);
        }

        private void BtnSaveGame_Clicked(object sender, EventArgs e)
        {
            //string workingDirectory = Environment.CurrentDirectory; // access denied
            //string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.Parent.Parent.FullName;
            //Debug.WriteLine(projectDirectory);
            string persistentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(persistentPath, "Save.json");

            string text = JsonConvert.SerializeObject(gameData);
            File.WriteAllText(filePath, text);
        }

        private void BtnLoadGame_Clicked(object sender, EventArgs e)
        {
            string persistentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(persistentPath, "Save.json");

            if (File.Exists(filePath) == false)
            {
                DisplayAlert("No save game found!", "Save a game to load it later", "Close");
                return;
            }

            string text = File.ReadAllText(filePath);
            gameData = JsonConvert.DeserializeObject<GameData>(text);

            CreatingTheBoard();
        }
    }
}