using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuStandard
{
    public static class ExtensionMethods
    {
        public static T[] ToArray<T>(this T[,] twoDimensionalArray)
        {
            int index = 0;
            int width = twoDimensionalArray.GetLength(0);
            int height = twoDimensionalArray.GetLength(1);
            T[] single = new T[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    single[index] = twoDimensionalArray[x, y];
                    index++;
                }
            }
            return single;
        }
    }
}
