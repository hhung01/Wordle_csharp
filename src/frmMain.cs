using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Forms;
using Wordle.src;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Wordle
{
    // TODO: Allow physical/external keyboard inputs
    // TODO: Allow player to select a box directly to enter a letter
    // TODO: Allow more dynamic play: different word lengths, more/less guessing rows

    public partial class frmMain : Form
    {
        #region . Variables, Constants, and other stuff .

        #region . Constants .

        private const int _cMAX_GUESSES = 6;
        private const int _cWORD_LENGTH = 5;

        private const string _cAPP_TITLE = "Wordle Fun!";
        private const string _cNEW_GAME = "Click 'New Game' to start.";
        private const string _cINVALID_WORD = "Invalid word!!!!";
        private const string _cCORRECT_WORD = "Correct Guess!!!!  WINNER!";
        private const string _cINCORRECT_WORD = "Incorrect guess.  Try again.";
        private const string _cGAME_OVER = "Out of guesses!!!! The word was {0}.";
        private const string _cINVALID_LENGTH = "Invalid word length.";
        private const string _cCONFIRM_NEW_GAME = "Are you sure you want to start a new game?";
        private const string _cDIALOG_BOX_CAPTION = "New Game";

        private BorderStyle _cLowlightBorderStyle = BorderStyle.Fixed3D;
        private BorderStyle _cHighlightBorderStyle = BorderStyle.FixedSingle;

        #endregion

        #region . Enums .

        private enum eKeys : int
        {
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
        }

        // This enum must match in size with _LabelColors array
        private enum eLabelType : int
        {   // Forecolor for label
            Normal = 0,
            Red,
            Green
        }

        // This enum must match in size with _KeyColors array
        private enum eColors : int
        {   // Backcolor for keys and letters
            Normal = 0,     // Unused letter
            Close,          // Correct letter, wrong position
            Correct,        // Correct letter, correct position
            Invalid,        // Invalid letter
            Disabled        // Disabled letter
        }

        #endregion

        #region . Variables .

        private int _CurGuessRow = 0;
        private bool _GameOn = false;

        private Color[] _KeyColors = { Color.White, Color.Yellow, Color.LimeGreen, Color.LightGray, Color.Gray };
        private Color[] _LabelColors = { Color.Black, Color.Red, Color.Green };

        private Label[,] _Labels = new Label[_cMAX_GUESSES, _cWORD_LENGTH];
        private Button[] _Keys = new Button[26];
        private clsGuess _CurGuess = new clsGuess(_cWORD_LENGTH);
        private HttpClient _httpClient = new HttpClient();
        private clsWordAPI _WordAPI = new clsWordAPI(_cWORD_LENGTH);

        #endregion

        #endregion

        #region . Constructors .

        public frmMain()
        {
            InitializeComponent();
        }

        #endregion

        #region . Private Functions .

        #region . Private Event Functions .

        private void frmMain_Load(object sender, EventArgs e)
        {
            InitVariables();
            ResetBoard();
            SetLabel(_cNEW_GAME);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            ValidateGuess();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            DeleteLastChar();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearRow();
        }

        #region . Alphabet Keys .

        private void btnA_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.A);
        }

        private void btnB_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.B);
        }

        private void btnC_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.C);
        }

        private void btnD_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.D);
        }

        private void btnE_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.E);
        }

        private void btnF_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.F);
        }

        private void btnG_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.G);
        }

        private void btnH_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.H);
        }

        private void btnI_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.I);
        }

        private void btnJ_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.J);
        }

        private void btnK_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.K);
        }

        private void btnL_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.L);
        }

        private void btnM_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.M);
        }

        private void btnN_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.N);
        }

        private void btnO_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.O);
        }

        private void btnP_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.P);
        }

        private void btnQ_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.Q);
        }

        private void btnR_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.R);
        }

        private void btnS_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.S);
        }

        private void btnT_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.T);
        }

        private void btnU_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.U);
        }

        private void btnV_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.V);
        }

        private void btnW_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.W);
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.X);
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.Y);
        }

        private void btnZ_Click(object sender, EventArgs e)
        {
            ProcessKey(eKeys.Z);
        }

        #endregion

        #endregion

        #region . Private Class Functions .

        private void InitVariables()
        {
            // Initialize the Guess variable
            _CurGuess.ClearGuess();

            // Initialize the Label array
            _Labels = new Label[_cMAX_GUESSES, _cWORD_LENGTH];
            _Labels[0, 0] = lblGuess11;
            _Labels[0, 1] = lblGuess12;
            _Labels[0, 2] = lblGuess13;
            _Labels[0, 3] = lblGuess14;
            _Labels[0, 4] = lblGuess15;
            _Labels[1, 0] = lblGuess21;
            _Labels[1, 1] = lblGuess22;
            _Labels[1, 2] = lblGuess23;
            _Labels[1, 3] = lblGuess24;
            _Labels[1, 4] = lblGuess25;
            _Labels[2, 0] = lblGuess31;
            _Labels[2, 1] = lblGuess32;
            _Labels[2, 2] = lblGuess33;
            _Labels[2, 3] = lblGuess34;
            _Labels[2, 4] = lblGuess35;
            _Labels[3, 0] = lblGuess41;
            _Labels[3, 1] = lblGuess42;
            _Labels[3, 2] = lblGuess43;
            _Labels[3, 3] = lblGuess44;
            _Labels[3, 4] = lblGuess45;
            _Labels[4, 0] = lblGuess51;
            _Labels[4, 1] = lblGuess52;
            _Labels[4, 2] = lblGuess53;
            _Labels[4, 3] = lblGuess54;
            _Labels[4, 4] = lblGuess55;
            _Labels[5, 0] = lblGuess61;
            _Labels[5, 1] = lblGuess62;
            _Labels[5, 2] = lblGuess63;
            _Labels[5, 3] = lblGuess64;
            _Labels[5, 4] = lblGuess65;

            // Initialize the Keys array
            _Keys[(int)eKeys.A] = btnA;
            _Keys[(int)eKeys.B] = btnB;
            _Keys[(int)eKeys.C] = btnC;
            _Keys[(int)eKeys.D] = btnD;
            _Keys[(int)eKeys.E] = btnE;
            _Keys[(int)eKeys.F] = btnF;
            _Keys[(int)eKeys.G] = btnG;
            _Keys[(int)eKeys.H] = btnH;
            _Keys[(int)eKeys.I] = btnI;
            _Keys[(int)eKeys.J] = btnJ;
            _Keys[(int)eKeys.K] = btnK;
            _Keys[(int)eKeys.L] = btnL;
            _Keys[(int)eKeys.M] = btnM;
            _Keys[(int)eKeys.N] = btnN;
            _Keys[(int)eKeys.O] = btnO;
            _Keys[(int)eKeys.P] = btnP;
            _Keys[(int)eKeys.Q] = btnQ;
            _Keys[(int)eKeys.R] = btnR;
            _Keys[(int)eKeys.S] = btnS;
            _Keys[(int)eKeys.T] = btnT;
            _Keys[(int)eKeys.U] = btnU;
            _Keys[(int)eKeys.V] = btnV;
            _Keys[(int)eKeys.W] = btnW;
            _Keys[(int)eKeys.X] = btnX;
            _Keys[(int)eKeys.Y] = btnY;
            _Keys[(int)eKeys.Z] = btnZ;
        }

        private void NewGame()
        {
            if (IsGameOn())
            {
                DialogResult result = MessageBox.Show(_cCONFIRM_NEW_GAME, _cDIALOG_BOX_CAPTION,
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    StartNewGame();
                }
            }
            else
            {
                StartNewGame();
            }
        }

        private void StartNewGame()
        {
            _GameOn = true;
            ResetBoard();
            GetNewWordAsync();
        }

        private async void GetNewWordAsync()
        {
            if (!(await _WordAPI.GetNewWordAsync()))
            {
                DisplayWordAPIError();
            }
            Debug.WriteLine(_WordAPI.Word);    // TODO: Remove this for the final
        }

        private void DisplayWordAPIError()
        {
            if (!String.IsNullOrEmpty(_WordAPI.Error))
            {
                SetLabel(_WordAPI.Error, eLabelType.Red);
            }
        }

        private void ResetBoard()
        {
            ClearBoard();
            SetLabel("");
            EnableAllKeys();
        }

        private void ClearBoard()
        {
            // Clear all guess labels
            for (int i = 0; i < _cMAX_GUESSES; i++)
            {
                for (int j = 0; j < _cWORD_LENGTH; j++)
                {
                    _Labels[i, j].Text = "";
                }
            }

            // Set current guess row and enable it
            _CurGuessRow = 0;
            SetBoard();

            // Clear Guess word
            _CurGuess.ClearGuess();
        }

        private void SetBoard()
        {
            SetEnableRow();
            for (int i = _CurGuessRow + 1; i < _cMAX_GUESSES; i++)
            {
                SetEnableRow(i, false);
            }
        }

        private void SetEnableRow()
        {
            SetEnableRow(_CurGuessRow);
            HighlightLetter(0);
        }

        private void DisableRow()
        {
            SetEnableRow(_CurGuessRow, false);
        }

        private void SetEnableRow(int row, bool flag = true)
        {
            for (int i = 0; i < _cWORD_LENGTH; i++)
            {
                if (flag)
                {
                    _Labels[row, i].BackColor = _KeyColors[(int)eColors.Normal];
                    _Labels[row, i].ForeColor = Color.Black;
                }
                else
                {
                    _Labels[row, i].BackColor = _KeyColors[(int)eColors.Disabled];
                }
                _Labels[row, i].BorderStyle = _cLowlightBorderStyle;
            }
        }

        private void EnableAllKeys()
        {
            for (int i = 0; i < 26; i++)
            {
                _Keys[i].Enabled = true;
                _Keys[i].BackColor = _KeyColors[(int)eColors.Normal];
            }
        }

        private void ClearRow()
        {
            if (IsGameOn())
            {
                _CurGuess.ClearGuess();
                for (int i = 0; i < _cWORD_LENGTH; i++)
                {
                    _Labels[_CurGuessRow, i].Text = " ";
                    _Labels[_CurGuessRow, i].BorderStyle = _cLowlightBorderStyle;
                }
                HighlightLetter(0);
            }
        }

        private async void ValidateGuess()
        {
            if (IsGameOn())
            {
                if (_CurGuess.IsComplete())
                {
                    if (await IsGuessValidWord())
                    {
                        ProcessNextRow();
                    }
                }
                else
                {
                    SetLabel(_cINVALID_LENGTH, eLabelType.Red);
                }
            }
        }

        private async Task<bool> IsGuessValidWord()
        {
            if (await _WordAPI.IsValidWordAsync(_CurGuess.Word, _cINVALID_WORD))
            {
                return true;
            }
            else
            {
                DisplayWordAPIError();
                return false;
            }
        }

        private void ProcessNextRow()
        {
            DisableRow();
            CheckLetters();
            if (_CurGuess.Word.CompareTo(_WordAPI.Word) == 0)
            {
                SetLabel(_cCORRECT_WORD, eLabelType.Green);
                _GameOn = false;
            }
            else
            {
                SetLabel(_cINCORRECT_WORD);
                InvalidGuess();
            }
        }

        private void InvalidGuess()
        {
            _CurGuessRow += 1;
            if (_CurGuessRow >= _cMAX_GUESSES)
            {
                SetLabel(String.Format(_cGAME_OVER, _WordAPI.Word), eLabelType.Red);
                _GameOn = false;
            }
            else
            {
                SetEnableRow();
                _CurGuess.ClearGuess();
            }
        }

        private void CheckLetters()
        {
            for (int i = 0; i < _cWORD_LENGTH; i++)
            {
                char chrGuess = _CurGuess.GetChar(i);
                int keyIndex = (int)chrGuess - 65;
                if (_WordAPI.Word[i] == chrGuess)
                {
                    SetKeyColor(i, keyIndex, eColors.Correct);
                }
                else if (_WordAPI.Word.IndexOf(_CurGuess.GetChar(i)) >= 0)
                {
                    SetKeyColor(i, keyIndex, eColors.Close);
                }
                else
                {
                    SetKeyColor(i, keyIndex, eColors.Invalid);
                    _Keys[keyIndex].Enabled = false;
                }
            }
        }

        private void SetKeyColor(int lblIndex, int keyIndex, eColors keyColor)
        {
            _Labels[_CurGuessRow, lblIndex].BackColor = _KeyColors[(int)keyColor];
            if ((_Keys[keyIndex].BackColor == _KeyColors[(int)eColors.Normal]) ||
                (_Keys[keyIndex].BackColor == _KeyColors[(int)eColors.Close]))
            {
                _Keys[keyIndex].BackColor = _KeyColors[(int)keyColor];
            }

            //if ((_Keys[index].BackColor == Color.White) || (_Keys[index].BackColor == Color.Yellow))
            //{
            //    _Keys[index].BackColor = color;
            //}
        }


        private void SetLabel(string msg)
        {
            SetLabel(msg, eLabelType.Normal);
        }

        private void SetLabel(string msg, eLabelType eType)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = _LabelColors[(int)eType];
        }

        private void ProcessKey(eKeys key)
        {
            if (IsGameOn())
            {
                if (_CurGuess.CurLetterPos < _cWORD_LENGTH)
                {
                    int curLetter = _CurGuess.CurLetterPos;
                    LowlightLetter(curLetter);
                    _CurGuess.SetChar(Strings.Chr(65 + (int)key));
                    _Labels[_CurGuessRow, curLetter].Text = _CurGuess.GetChar(curLetter).ToString();
                    HighlightLetter(_CurGuess.CurLetterPos);
                }
            }
        }

        private void HighlightLetter(int pos)
        {
            HighlightLetter(_CurGuessRow, pos, _cHighlightBorderStyle);
        }

        private void LowlightLetter(int pos)
        {
            HighlightLetter(_CurGuessRow, pos, _cLowlightBorderStyle);
        }

        private void HighlightLetter(int row, int pos, BorderStyle eStyle)
        {
            if (IsGameOn() && row == _CurGuessRow)
            {
                _Labels[row, pos].BorderStyle = eStyle;
            }
        }

        private void DeleteLastChar()
        {
            if (IsGameOn())
            {
                if (_CurGuessRow < _cMAX_GUESSES)
                {
                    if (_CurGuess.IsComplete())
                    {   // Clear the last box and keep the focus there
                        _Labels[_CurGuessRow, _CurGuess.CurLetterPos].Text = "";
                        _CurGuess.ClearLastChar();
                    }
                    else
                    {   // Clear the last character and go back one box
                        LowlightLetter(_CurGuess.CurLetterPos);
                        _CurGuess.ClearLastChar();
                        _Labels[_CurGuessRow, _CurGuess.CurLetterPos].Text = "";
                        HighlightLetter(_CurGuess.CurLetterPos);
                    }
                }
            }
        }

        private bool IsGameOn()
        {
            if (!_GameOn)
            {
                SetLabel(_cNEW_GAME, eLabelType.Green);
                return false;
            }
            return true;
        }

        #endregion

        #endregion
    }
}
