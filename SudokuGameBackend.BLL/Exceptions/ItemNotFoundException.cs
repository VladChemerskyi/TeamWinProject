using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Exceptions
{
    public class ItemNotFoundException : ApplicationException
    {
        public ItemNotFoundException() { }
        public ItemNotFoundException(string message) : base(message) { }
    }
}
