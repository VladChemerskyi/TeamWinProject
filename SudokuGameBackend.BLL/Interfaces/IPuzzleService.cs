using SudokuGameBackend.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IPuzzleService
    {
        Task<List<RegularSolvedSudokuDto>> GetRegularPuzzle(GameMode gameMode);
    }
}
