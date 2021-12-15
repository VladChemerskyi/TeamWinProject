using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Exceptions
{
    public class GameSessionException : ApplicationException
    {
        public GameSessionException() { }
        public GameSessionException(string message) : base(message) { }
        public GameSessionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
