﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AmysMasterMind
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        // == constants ==
        private const int NUM_OF_TURNS = 10;

        private const int PEG_SIZE = 25;

        // == globals == 
        private int _checkBtnCol = 5;
        private int _number_pegs = 4;
        private int _currentRow;
        private Color _emptyColour = Color.RosyBrown;
        private Color _currentColour;

        // array of colours for generating the code
        // setting up the grid to choose from
        // an integer array of 4 numbers
        // int[] iArray = new int[] {1,2,3,4};

        Color[] myColours = new Color[]
                            {Color.Black, Color.Yellow,
                            Color.Red, Color.Green,
                            Color.White, Color.Brown,
                            Color.Blue, Color.Orange};

        // myColours[1] = Color.Yellow;
        // boxview.Color = myColours[1] same as Color.Yellow

        Color[] _userGuess = new Color[4];
        Color[] _theCode = new Color[4];    // fill in GenerateCode

        public MainPage()
        {
            InitializeComponent();
            // add row 2
            GenerateTheBoard();
        }

        private void GenerateTheBoard()
        {
            int row = 1;
            for (row = 0; row < 10; row++)
            {
                CreateOneRow(row);
            }
            // add the check answer button
            // add this in row = NUM_OF_TURNS - 1
            AddCheckAnswerButton(NUM_OF_TURNS - 1);
            DisplayAlert("New Game", "Press New Game to Start!", "OK");
        }


        private void AddCheckAnswerButton(int row)
        {
            Button b;
            b = new Button();   // instantiate
            b.Text = "Check";
            b.SetValue(Grid.RowProperty, row);
            b.SetValue(Grid.ColumnProperty, _checkBtnCol);
            b.Margin = 2;
            b.HorizontalOptions = LayoutOptions.Center;
            b.VerticalOptions = LayoutOptions.Center;
            // clicked event
            b.Clicked += CheckGuessButton_Clicked;
            // add to the board somewhere
            GrdGame.Children.Add(b);
            // set a value for the current row
            _currentRow = row;
        }

        private void CheckGuessButton_Clicked(object sender, EventArgs e)
        {
            // change the current row of the button 
            // to one less
            Button b;
            int row;

            if (!IsCurrentRowFull())   // returns false
            {
                // give user a message - fill in the values
                DisplayAlert("Hey!!!",
                             "Choose four colours!!!",
                             "Ok");
                return;
            }

            if (_currentRow == 0)
            {
                return; // no more guesses
            }

            b = (Button)sender;
            row = (int)b.GetValue(Grid.RowProperty);

            // check the guess here....
            // 3 if statements
            // it would be nice to compare two arrays
            // need to add to the array when the user selects
            // a colour

            CheckTheGuess();

            b.SetValue(Grid.RowProperty, row - 1);

            // reset the value of the current row
            // help in UserTurnSpace_Tapped
            _currentRow = row - 1;
        }

        private void CheckTheGuess()
        {
            // need to compare the userguess against thecode and then give feedback based on that.
            // compare 2 arrays
            // if a[x] == b[x] => black peg
            // if a[x] == b[y] => white peg
            // else nothing (empty space)
            // need an array for feedback
            //could use a colour array - difficult to sort
            // use an array of ints - 2=black, 1=white
            int[] fb = new int[4] { 0, 0, 0, 0 };
            int i, j, f, score;
            Grid grdFB = null; // find this one

            f = 0;
            i = 0;
            score = 0;
            while (i < _number_pegs)
            {
                if (_theCode[i] == _userGuess[i])
                {
                    fb[f++] = 2; // black peg
                    score += 2;
                }
                else
                {
                    // check for white peg
                    for (j = 0; j < _number_pegs; j++)
                    {
                        if (_theCode[i] == _userGuess[i])
                        {
                            fb[f++] = 1; // white peg
                            j = _number_pegs + 1;
                        }
                    }
                } // end if(_userGuess[i])

                i++;
            } // end while (i < _number_pegs)

            Array.Sort(fb);
            Array.Reverse(fb);



            /*DisplayAlert("Feedback",
                        fb[0] + ", " +
                        fb[1] + ", " +
                        fb[2] + ", " +
                        fb[3] + "OK");*/

            // array is full, display feedback
            // need to find the feedback grid on the _currentRow
            foreach (var item in GrdGame.Children)
            {

                if (item.GetType() == typeof(Grid))
                {
                    // is it on _currentRow
                    // Grid.RowProperty == _currentRow
                    if (_currentRow == (int)((Grid)item).GetValue(Grid.RowProperty))
                    {
                        // found it, save it, add feedback
                        grdFB = (Grid)item;
                        break; // exit loop
                    }
                }
            } // end foreach

            // fill in the feedback
            // use i for rows, j for cols, f for the counter
            f = 0; // start at fb[f = 0]
            i = 0; j = 0; // start on ROW 0 COL 0
            BoxView b;

            while (f < 4)
            {
                // create a boxview for feedback - done already
                // in GenerateBoard
                #region Create One BoxView
                b = new BoxView(); // instantiate
                b.BackgroundColor = _emptyColour;
                if (fb[f] == 2)
                {
                    b.BackgroundColor = Color.Black;
                }
                else if (fb[f] == 1)
                {
                    b.BackgroundColor = Color.White;
                }
                b.Margin = 2;
                b.SetValue(Grid.RowProperty, i);
                b.SetValue(Grid.ColumnProperty, j);
                b.HorizontalOptions = LayoutOptions.Center;
                b.VerticalOptions = LayoutOptions.Center;
                b.HeightRequest = 8;
                b.WidthRequest = 8;
                b.CornerRadius = 4;
                grdFB.Children.Add(b);

                #endregion

                // increment counters
                /*
                 *f= 0, i = 0, j =  0
                 *f= 1, i = 0, j =  1  - j++
                 *f= 2, i = 1, j =  0  - set explicitly
                 *f= 3, i = 1, j =  1  - j++
                 */

                f++;
                if (f == _number_pegs / 2)
                {
                    i = 1;
                    j = 0;
                }
                else
                {
                    j++;
                }
            } // while (f < 4)

            // if score == win
            if (score == (_number_pegs * 2))
            {
                DisplayAlert("Winner", "Congratulations", "");
            }

        }

        private bool IsCurrentRowFull()
        {
            bool answer = true;
            // check all the boxviews on that row
            // _currentRow
            foreach (var item in GrdGame.Children)
            {   // if it has a type equal to boxview
                if (item.GetType() == typeof(BoxView))
                {
                    int r = (int)item.GetValue(Grid.RowProperty);
                    if (r == _currentRow)
                    {
                        if (((BoxView)item).Color == _emptyColour)
                        {
                            answer = false;
                            break;
                        }
                    }
                }
            }
            return answer;
        }


        private void CreateOneRow(int row)
        {
            BoxView b;
            Grid g;
            ImageButton ib;
            TapGestureRecognizer tap; // added to all boxviews
            int col = 0;

            tap = new TapGestureRecognizer(); // instantiate
            tap.Tapped += UserTurnSpace_Tapped;

            // Create the feedback grid 
            #region Create Feedback Grid
            g = new Grid(); // instantiate feedback grid
            g.Margin = 2;
            g.SetValue(Grid.RowProperty, row);
            g.SetValue(Grid.ColumnProperty, col);
            g.HorizontalOptions = LayoutOptions.Center;
            g.VerticalOptions = LayoutOptions.Center;
            // add 2 rows
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            //add 2 columns
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            GrdGame.Children.Add(g);

            #endregion

            //add 4 boxviews to the feedback grid
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    #region Create One BoxView
                    b = new BoxView(); // instantiate
                    b.Color = Color.RosyBrown;
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

                } // end for (c = 0
            } // end for (r = 0


            #region Create User Turns
            // add 4 image buttons for the user guess
            // change to boxviews  rename 
            for (col = 1; col < 5; col++)
            {
                #region Add one Image Button
                b = new BoxView(); // instantiate
                b.Margin = 2;
                b.SetValue(Grid.RowProperty, row);
                b.SetValue(Grid.ColumnProperty, col);
                b.HorizontalOptions = LayoutOptions.Center;
                b.VerticalOptions = LayoutOptions.Center;
                b.HeightRequest = PEG_SIZE;
                b.WidthRequest = PEG_SIZE;
                b.CornerRadius = PEG_SIZE / 2;
                b.Color = _emptyColour;
                // add tap event
                b.GestureRecognizers.Add(tap);

                // i += 1 same as i = i + 1
                // b.Clicked += EmptySpace_Clicked;
                // b.Source = EMPTY_SPACE;
                #endregion

                // add to the game grid
                GrdGame.Children.Add(b);

            } // end for (col = 1

            #endregion

            #region Create the Colour Choice Grid
            CreateColourChoiceGrid();
            #endregion

            // add everything to the GrdGame children

        }

        private void CreateColourChoiceGrid()
        {
            // fill a 2x4 grid with colours
            int r, c = 0, iColour = 0;
            BoxView b; // to hold each colour
            TapGestureRecognizer tap; // different tap event

            tap = new TapGestureRecognizer();
            tap.Tapped += ColourChoice_Tapped;

            // copied from feedback grid
            for (r = 0; r < 2; r++)
            {
                for (c = 0; c < 4; c++)
                {
                    #region Create One BoxView
                    b = new BoxView(); // instantiate
                                       //  b.BackgroundColor = Color.Red;
                    b.Margin = 2;
                    b.SetValue(Grid.RowProperty, r);
                    b.SetValue(Grid.ColumnProperty, c);
                    b.HorizontalOptions = LayoutOptions.Center;
                    b.VerticalOptions = LayoutOptions.Center;
                    b.HeightRequest = PEG_SIZE;
                    b.WidthRequest = PEG_SIZE;
                    b.CornerRadius = PEG_SIZE / 2;
                    b.GestureRecognizers.Add(tap);
                    // b.Color = _emptyColour; // temporary
                    b.Color = myColours[iColour++];
                    GrdColourChoice.Children.Add(b);
                    #endregion

                } // end for (c = 0
            } // end for (r = 0

        }

        private void ColourChoice_Tapped(object sender, EventArgs e)
        {
            //  ((BoxView)sender).Color = Color.Red;
            //save the colour of the sender for use on the turn peg
            //create a global variable to save in
            _currentColour = ((BoxView)sender).Color;

        }

        private void UserTurnSpace_Tapped(object sender, EventArgs e)
        {
            BoxView b = (BoxView)sender;
            // place the currentColour on the sender
            // check that the user tapped on a boxview
            // on the same row as the _currentRow
            if (((int)b.GetValue(Grid.RowProperty) != _currentRow) ||
                (_currentColour == null))
            {
                return;
            }

            b.Color = _currentColour;
            // add this to a guess array
            // subscript = b.Column - 1
            _userGuess[(int)b.GetValue(Grid.ColumnProperty) - 1] =
                                                            b.Color;
        }

        private void EmptySpace_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnNewGame_Clicked(object sender, EventArgs e)
        {
            // generate a new code 
            GenerateNewCode();

            // clear the board - foreach, if child == boxview, color
            // reset the currentrow value
            foreach (var item in GrdGame.Children)
            {
                if (item.GetType() == typeof(BoxView))
                {
                    ((BoxView)item).Color = _emptyColour;
                }

                if (item.GetType() == typeof(Grid))
                {
                    // feedback grid has 4 children
                    foreach (var bv in ((Grid)item).Children)
                    {
                        if (bv.GetType() == typeof(BoxView))
                        {
                            ((BoxView)bv).Color = _emptyColour;
                        }
                    }
                }

                if (item.GetType() == typeof(Button))
                {
                    // set to the first guess
                    ((Button)item).SetValue(Grid.RowProperty, NUM_OF_TURNS - 1);

                }
            } // end foreach
            _currentRow = NUM_OF_TURNS - 1;

            SLCode.IsVisible = false;
        }

        private void GenerateNewCode()
        {
            // generate an array of 4 different integers
            // no duplicates
            int[] iCode = new int[] { -1, -1, -1, -1 };
            int current, numColours;
            Boolean HasDuplicates = false;
            // random number generator
            Random random = new Random(DateTime.Now.Millisecond);

            //    current = random.Next(0, 8);
            //   iCode[0] = current;

            // will loop

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
                    } // if
                } // for
                if (HasDuplicates == false)
                {
                    iCode[numColours++] = current;
                } // if
            } // while

            // now have a unique set
            BVColour0.Color = myColours[iCode[0]];
            BVColour1.Color = myColours[iCode[1]];
            BVColour2.Color = myColours[iCode[2]];
            BVColour3.Color = myColours[iCode[3]];

            // store in the global array for checking the guesses
            _theCode[0] = myColours[iCode[0]];
            _theCode[1] = myColours[iCode[1]];
            _theCode[2] = myColours[iCode[2]];
            _theCode[3] = myColours[iCode[3]];
        }
    }
}

