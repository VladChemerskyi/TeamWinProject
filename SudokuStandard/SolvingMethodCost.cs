using System;
using System.Collections.Generic;
using System.Text;
using Kermalis.SudokuSolver.Core;

namespace SudokuStandard
{
    public struct MethodCost
    {
        public int FirstUseCost { get; private set; }
        public int SubsequentUsesCost { get; private set; }
        private MethodCost(int firstUseCost, int subsequentUsesCost)
        {
            FirstUseCost = firstUseCost;
            SubsequentUsesCost = subsequentUsesCost;
        }
        public static MethodCost FromSolvingMethod(SolvingMethod solvingMethod)
        {
            MethodCost methodCost;
            switch(solvingMethod)
            {
                case SolvingMethod.SingleCandidate:
                    methodCost = new MethodCost(100, 100);
                    break;
                case SolvingMethod.SinglePosition:
                    methodCost = new MethodCost(100, 100);
                    break;
                case SolvingMethod.CandidateLines:
                    methodCost = new MethodCost(350, 200);
                    break;
                case SolvingMethod.NakedPair:
                    methodCost = new MethodCost(750, 500);
                    break;
                case SolvingMethod.HiddenPair:
                    methodCost = new MethodCost(1500, 1200);
                    break;
                case SolvingMethod.NakedTriple:
                    methodCost = new MethodCost(2000, 1400);
                    break;
                case SolvingMethod.HiddenTriple:
                    methodCost = new MethodCost(2400, 1600);
                    break;
                case SolvingMethod.XWing:
                    methodCost = new MethodCost(2800, 1600);
                    break;
                case SolvingMethod.XYChain:
                    methodCost = new MethodCost(4200, 2100);
                    break;
                case SolvingMethod.NakedQuadruple:
                    methodCost = new MethodCost(5000, 4000);
                    break;
                case SolvingMethod.HiddenQuadruple:
                    methodCost = new MethodCost(7000, 5000);
                    break;
                case SolvingMethod.Swordfish:
                    methodCost = new MethodCost(8000, 6000);
                    break;
                default:
                    methodCost = new MethodCost(0, 0);
                    break;
            }
            return methodCost;
        }
    }
}
