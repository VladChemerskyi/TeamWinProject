using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Exceptions
{
    class MatchmakerHubException : ApplicationException
    {
        public MatchmakerHubException() { }
        public MatchmakerHubException(string message) : base(message) { }
    }
}
