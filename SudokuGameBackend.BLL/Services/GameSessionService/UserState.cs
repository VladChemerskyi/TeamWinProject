using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Services
{
    class UserState
    {
        public bool IsReady { get; set; }
        public DateTime? FinishTime { get; set; }
        public string ConnectionId { get; set; }
        public int CompletionPercent { get; set; }
        public bool ResultWasCreated { get; set; }
    }
}
