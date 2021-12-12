using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Exceptions
{
    class GameHubException : ApplicationException
    {
        public GameHubException() { }
        public GameHubException(string message) : base(message) { }
    }
}
