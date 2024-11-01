using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle.src
{
    internal interface IWordAPI
    {
        internal string Word { get; }
        internal string Error { get; }

        internal Task<bool> GetNewWordAsync();
        internal Task<bool> IsValidWordAsync(string word, string errMsg = "");


    }
}
