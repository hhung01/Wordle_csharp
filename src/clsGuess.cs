using System.Diagnostics;

namespace Wordle.src
{
    internal class clsGuess
    {
        #region . Variables, Constants, and other stuff .

        private readonly int _WordLength;
        private char[] _Guess = new char[5];

        #endregion

        #region . Constructor .

        internal clsGuess(int wordLength)
        {
            _WordLength = wordLength;
            CurLetterPos = 0;
            ClearGuess();
        }

        #endregion

        #region . Public Properties .

        internal string Word
        {
            get
            {
                string retValue = "";
                for (int i = 0; i < _WordLength; i++)
                {
                    retValue += _Guess[i];
                }
                return retValue.ToUpper();
            }
        }

        internal int CurLetterPos { get; set; }

        #endregion

        #region . Public Functions .

        internal bool IsComplete()
        {
            for (int i = 0; i < _WordLength; i++)
            {
                if (_Guess[i] == ' ')
                {
                    return false;
                }
            }
            return true;
        }

        internal void SetChar(char ch)
        {
            SetChar(ch, CurLetterPos);
            IncrementPos();
        }

        internal void SetChar(char ch, int index)
        {
            if (IsIndexValid(index) && (!String.IsNullOrEmpty(ch.ToString())))
            {
                if (index == _WordLength - 1)
                {
                    if (_Guess[index] == ' ')
                    {
                        _Guess[index] = ch;
                    }
                }
                else
                {
                    _Guess[index] = ch;
                }
            }
        }

        internal char GetChar(int index)
        {
            if (IsIndexValid(index))
            {
                return _Guess[index];
            }
            return ' ';
        }

        internal void ClearLastChar()
        {
            if (IsIndexValid(CurLetterPos))
            {
                if (CurLetterPos == _WordLength - 1)
                {
                    if (_Guess[CurLetterPos] == ' ')
                    {
                        DecrementPos();
                        _Guess[CurLetterPos] = ' ';
                    }
                    else
                    {
                        _Guess[CurLetterPos] = ' ';
                    }
                }
                else
                {
                    DecrementPos();
                    _Guess[CurLetterPos] = ' ';
                }
            }
        }

        internal void ClearGuess()
        {
            _Guess = new char[_WordLength];
            for (int i = 0; i < _WordLength; i++)
            {
                _Guess[i] = ' ';
            }
            CurLetterPos = 0;
        }

        #endregion

        #region . Private Class Functions .

        private bool IsIndexValid(int index)
        {
            return (0 <= index) && (index < _WordLength);
        }
        private void DecrementPos()
        {
            if (CurLetterPos > 0)
            {
                CurLetterPos--;
            }
        }

        private void IncrementPos()
        {
            if (CurLetterPos < (_WordLength - 1))
            {
                CurLetterPos++;
            }
        }

        #endregion
    }
}
