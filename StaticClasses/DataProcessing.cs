using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StaticClasses
{
    public static class DataProcessing
    {
        /// <summary>
        /// This method devides table's rows into the array of arrays.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static string[][] InitData(string[] rows)
        {
            string[][] tableValues = new string[rows.Length][];
            for (int i = 0; i < rows.Length; i++)
            {
                // Without last element of a row because it is "\n".
                tableValues[i] = rows[i].Split(";")[..^1];
                for (int j = 0; j < tableValues[i].Length; j++)
                {
                    // Deleting quotes of each element.
                    tableValues[i][j] = tableValues[i][j].Trim('"');
                }
            }
            return tableValues;
        }
        /// <summary>
        /// This method makes a selection according to user's value and returns the result table
        /// </summary>
        /// <param name="tableValues"></param>
        /// <param name="indexColumn1"></param>
        /// <param name="value1"></param>
        /// <param name="indexColumn2"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static string[][]? Select(string[][] tableValues, int indexColumn1, string value1, int indexColumn2 = 0, string value2 = "")
        {
            // Quantity of suitable elements.
            int counter = 0;

            string[][]? selectedTable;
            // If user has chosen in menu to select only one column value2 and value1 will be the same.
            if (value2 == "")
            {
                indexColumn2 = indexColumn1;
                value2 = value1;
            }
            // Comparing every element in the selected column with user's value.
            for (int i = 0; i < tableValues.Length; i++)
            {
                if (tableValues[i][indexColumn1] == value1 && tableValues[i][indexColumn2] == value2)
                {
                    counter += 1;
                }
            }
            // If there are no suitable elements returns null.
            if (counter == 0)
            {
                selectedTable = null;
                return selectedTable;
            }
            // Size of the result array of arrays is equal suitable elements = counter.
            selectedTable = new string[counter][];

            // Index of result table's row to write result there.
            int idxElem = 0;
            for (int i = 0; i < tableValues.Length; i++)
            {
                // Comparing every element in the selected column with user's value and filling the result table.
                if (tableValues[i][indexColumn1] == value1 && tableValues[i][indexColumn2] == value2)
                {
                    selectedTable[idxElem] = tableValues[i];
                    idxElem++;
                }
            }
            return selectedTable;
        }
        /// <summary>
        /// This method sorts the table and return sorted array of arrays.
        /// </summary>
        /// <param name="tableValues"></param>
        /// <param name="indexColumn"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static string[][] Sort(string[][] tableValues, int indexColumn, int idx)
        {
            string[][] sortedTable = tableValues;
            // Bubble sorting rows according to the column.
            for (int i = 0; i < sortedTable.Length - 1; i++)
            {
                for (int j = 0; j < sortedTable.Length - i - 1; j++)
                {
                    // Index of the first column to sort(user's menu number 4).
                    if (idx == 1)
                    {
                        // Alphabetical comparison.
                        if (String.Compare(sortedTable[j][indexColumn], sortedTable[j + 1][indexColumn]) > 0 || sortedTable[j + 1][indexColumn] == " ")
                        {
                            string temp = sortedTable[j + 1][indexColumn];
                            sortedTable[j + 1][indexColumn] = sortedTable[j][indexColumn];
                            sortedTable[j][indexColumn] = temp;

                        }
                    }
                    // Index of the second column to sort(user's menu number 5).
                    else
                    {
                        // Descending comperison.
                        if (String.Compare(sortedTable[j][indexColumn], sortedTable[j + 1][indexColumn]) < 0 || sortedTable[j][indexColumn] == " ")
                        {
                            string temp = sortedTable[j + 1][indexColumn];
                            sortedTable[j + 1][indexColumn] = sortedTable[j][indexColumn];
                            sortedTable[j][indexColumn] = temp;

                        }
                    }
                }
            }
            return sortedTable;
        }
        /// <summary>
        /// This method returnes array of string which format is the same as the initial table's one.
        /// </summary>
        /// <param name="resTable"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static string[] FormatTable(string[][] resTable, string[][] columnNames)
        {
            // Result array of strings. +2 because first two rows with columns name must be added.
            string[] resRows = new string[resTable.Length+2];

            for (int i = 0; i < resTable.Length; i++)
            {
                for (int j = 0; j < resTable[i].Length; j++)
                {
                    // Formatting two rows of column names.
                    if(i == 0 || i == 1)
                    {
                        columnNames[i][j] = $"\"{columnNames[i][j]}\"";
                    }
                    // Formatting the rest of rows.
                    resTable[i][j] = $"\"{resTable[i][j]}\"";
                }
                // Adding two rows of column names.
                if(i == 0 || i == 1)
                {
                    resRows[i] = String.Join(';', columnNames[i]) + ";\n";
                }
                // Adding other rows. +2 because first and second rows have been already filled.
                resRows[i+2] = String.Join(';', resTable[i]) + ";\n";
            }
            return resRows;
        }
        /// <summary>
        /// This methodreturns the string of the whole table.
        /// </summary>
        /// <param name="resTable"></param>
        /// <returns></returns>
        public static string FormatTable(string[] resTable)
        {
            StringBuilder sb = new();
            for (int i = 0;i < resTable.Length;i++)
            {
                sb.Append(resTable[i]);
            }
            return sb.ToString();
        }
    }
}
