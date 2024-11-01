using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Wordle.src
{
    internal class clsWordAPI
    {
        /* This Class uses 2 APIs to retrieve and validate words
         * 
         * The 1st API (random-word-api.herokuapp.com) is used to generate a random 5 letter word.
         * 
         * The 2nd API (datamuse.com) is used to validate a given word.
         * 
         * datamuse.com does not have an API that returns random words.  This is why, a separate API
         * is used for that purpose.  Because of this, once we get the random word, we need to validate
         * it against datamuse to verify that it recognizes it as a valid word.  Otherwise, if the
         * player guessed correctly, datamuse.com might return fail if it's not in its dictionary.
         * 
         * By having a separate class handle word generation and validation, this will allow easy
         * plug and play if a different API is used.
        */

        #region . Constants, Variables, and other stuff .

        private const string _cDEFAULT_WORD = "GUESS";

        private const string _cRANDOM_WORD_API = "https://random-word-api.herokuapp.com/word?length={0}";
        private const string _cVALIDATE_WORD_API = "https://api.datamuse.com/words?sp={0}&max=1";

        private const string _cERROR_GET_NEW_WORD = "Error retrieving new word from the Internet!";
        private const string _cERROR_VALIDATING_WORD = "Unable to validate word.  Try again.";

        private readonly int _MaxWordLength;
        private HttpClient _httpClient = new HttpClient();

        #endregion

        #region . Constructors .

        internal clsWordAPI(int maxWordLength)
        {
            _MaxWordLength = maxWordLength;
            Word = "";
            Error = "";
        }

        #endregion

        #region . Public Properties .

        // public string IWordAPI.Word {  get; private set; }

        internal string Word { get; private set; }

        internal string Error { get; private set; }

        #endregion

        #region . Public Functions .

        internal async Task<bool> GetNewWordAsync()
        {
            Word = "";
            Error = "";
            try
            {
                do
                {
                    Word = await GetRandomWordAsync(_MaxWordLength);
                } while (!(await IsValidWordAsync(Word)));
                return true;
            }
            catch (Exception)
            {
                Word = _cDEFAULT_WORD;
            }
            return false;
        }

        internal async Task<bool> IsValidWordAsync(string word, string errMsg = "")
        {
            Error = "";
            try
            {
                string apiUrl = string.Format(_cVALIDATE_WORD_API, word);
                var response = await _httpClient.GetStringAsync(apiUrl);
                var jsonArray = JArray.Parse(response);
                if ((jsonArray.Count > 0) && (jsonArray[0]["word"].ToString().ToUpper() == word.ToUpper()))
                {
                    return true;
                }
                Error = errMsg;
            }
            catch (Exception)
            {
                Error = _cERROR_VALIDATING_WORD;
                throw;
            }
            return false;
        }

        #endregion

        #region . Private Functions .

        private async Task<string> GetRandomWordAsync(int length)
        {
            try
            {
                string apiUrl = string.Format(_cRANDOM_WORD_API, length);
                var response = await _httpClient.GetStringAsync(apiUrl);
                var jsonArray = JArray.Parse(response);
                return jsonArray[0].ToString().ToUpper();
            }
            catch (Exception)
            {
                Error = _cERROR_GET_NEW_WORD;
                throw;
            }
        }

        #endregion

    }
}
