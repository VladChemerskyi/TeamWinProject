using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Exceptions
{
    public class RatingServiceException : ApplicationException
    {
        public RatingServiceException() { }
        public RatingServiceException(string message) : base(message) { }
        public RatingServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
