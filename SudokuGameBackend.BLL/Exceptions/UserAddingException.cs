using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Exceptions
{
    public class UserAddingException : ApplicationException
    {
        public UserAddingException() { }
        public UserAddingException(string message) : base(message) { }
        public UserAddingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
