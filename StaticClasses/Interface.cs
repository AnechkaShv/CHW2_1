using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StaticClasses
{
    public static class Interface
    {
        /// <summary>
        /// This method prints the menu of all processes.
        /// </summary>
        public static void PrintMenu(string[] elems, int row, int column, int idx)
        {

            Console.SetCursorPosition(column, row);

            for (int i = 0; i < elems.Length; i++)
            {
                // Setting color of current menu item.
                if (i + 1 == idx)
                {
                    Console.BackgroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(elems[i]);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        /// <summary>
        /// This menu implements all menu actions.
        /// </summary>
        /// <param name="tableValues"></param>
        /// <param name="columnNames"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string[][]? ActMenu(string[][] tableValues, string[] columnNames, out int idx)
        {
            // Array of menu items.
            string[] menuItems = new string[] { "1. Make a selection by value AdmArea", "2. Make a selection by value WifiName", "3. Make a selection by values FunctionFlag and AccessFlag", "4. Sort the table by value LibraryName (alphabetically)", "5. Sort the table by value CoverageArea(sort descending)", "6. Exit the program" };

            string[][]? resTable = null;

            // Cycle of repeating to process errors.
            while (true)
            {
                Console.WriteLine("Use up/down keys to chose menu item.");

                // Cursor of current place.
                int row = Console.CursorTop;
                int column = Console.CursorLeft;
                // Default index of menu.
                idx = 1;


                // Indicator of returning to the menu is selected value is incorrect.
                bool flag = true;

                // Cycle of repeating to show interactive menu.
                while (true)
                {
                    // The variable to break the cycle after choosing menu item.
                    bool isExit = true;

                    PrintMenu(menuItems, row, column, idx);

                    switch (Console.ReadKey(true).Key)
                    {
                        // When user enters down key.
                        case ConsoleKey.DownArrow:
                            // Checking if it is the last element.
                            if (idx < menuItems.Length)
                                // Moving to the next item.
                                idx++;
                            break;
                        // When user enters up key.
                        case ConsoleKey.UpArrow:
                            // Checking if it is the first and minimal element.
                            if (idx > 1)
                                // Moving to the previous item.
                                idx--;
                            break;
                        // When user chooses the item.
                        case ConsoleKey.Enter:
                            // Initializing the result table according to user's choice.
                            resTable = idx switch
                            {
                                1 => SelectInterface(tableValues, columnNames, "AdmArea", ref flag),
                                2 => SelectInterface(tableValues, columnNames, "WiFiName", ref flag),
                                3 => SelectInterface(tableValues, columnNames, "FunctionFlag", ref flag, "AccessFlag"),
                                4 => DataProcessing.Sort(tableValues, Array.IndexOf(columnNames, "LibraryName"), 1),
                                5 => DataProcessing.Sort(tableValues, Array.IndexOf(columnNames, "CoverageArea"), 2),
                                _ => null
                            };
                            // Exiting interactive menu.
                            isExit = false;
                            break;

                    }
                    if (!isExit)
                        break;
                }

                // If user decided to return to the menu after enterring wrong value going to naext iteration of cycle.
                if (!flag)
                    continue;
                return resTable;
            }
        }
        /// <summary>
        /// This method implements selection interface and returns selected table.
        /// </summary>
        /// <param name="tableValues"></param>
        /// <param name="columnNames"></param>
        /// <param name="name1"></param>
        /// <param name="flag"></param>
        /// <param name="name2"></param>
        /// <returns></returns>
        public static string[][]? SelectInterface(string[][] tableValues, string[] columnNames, string name1, ref bool flag, string name2 = "")
        {
            int num;

            string[][]? resTable;
            // This cycle doesn't allow to do next step until user enters right data.
            while (true)
            {
                Console.WriteLine("Enter values to make a selection");
                Console.Write("Value 1: ");

                // Getting selection value from user.
                string? value1 = Console.ReadLine();
                // Checking first value.
                if(value1 == null || value1.Length <= 0) 
                {
                    PrintColor("Wrong value. Please try again.", ConsoleColor.Red);
                    continue;
                }
                string? value2;
                // If it is the selection of two columns getting second value.
                if (name2 != "")
                {
                    Console.WriteLine();
                    Console.Write("Value 2: ");
                    value2 = Console.ReadLine();
                }
                // If it is the selection of one column equating values.
                else
                {
                    value2 = value1;
                }
                // Checking values
                if (value1 == null || value2 == null || value1.Length <= 0 || value2.Length <= 0)
                {
                    PrintColor("Wrong value. Please try again.", ConsoleColor.Red);
                    continue;
                }
                // Getting the result table if it is one column selection.
                if (name2 == "")
                {
                    resTable = DataProcessing.Select(tableValues, Array.IndexOf(columnNames, name1), value1);
                }
                // Getting the result table if it is two columns selection.
                else
                {
                    resTable = DataProcessing.Select(tableValues, Array.IndexOf(columnNames, name1), value1, Array.IndexOf(columnNames, name2), value2);
                }
                // If there are no user's value in the column(s) than resTable = null  and printing menu of actions.
                if (resTable == null)
                {
                    PrintEmptyOutput();
                    while (!int.TryParse(Console.ReadLine(), out num) || (num != 1 && num != 2))
                    {
                        ErrorMessage();
                    }
                    // If user wants to enter value(s) again.
                    if (num == 1)
                    {
                        continue;
                    }
                    // If user wants to return to the main menu.
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                break;
            }
            return resTable;
        }
        /// <summary>
        /// This method prints menu of actions if selection data doesn't exist in the table.
        /// </summary>
        public static void PrintEmptyOutput()
        {
            PrintColor("This value doesn't exist in the table. Do you want to try again?", ConsoleColor.Yellow);
            PrintColor("1. Yes", ConsoleColor.Yellow);
            PrintColor("2. No. Return to the menu", ConsoleColor.Yellow);
        }
        /// <summary>
        /// This method returns message when user enters wrong menu item.
        /// </summary>
        public static void ErrorMessage() => PrintColor("Wrong number. Please try again.", ConsoleColor.Red);

        /// <summary>
        /// This method prints menu of actions if file name is wrong and returns selected menu item.
        /// </summary>
        /// <returns></returns>
        public static int CheckFileName()
        {
            PrintColor("Wrong file name. Do you want to enter another file name or save data near to executable file with name \"table(Number of such a file)\", for exanple \"table1\"", ConsoleColor.Yellow);
            PrintColor("1. Enter another file name", ConsoleColor.Yellow);
            PrintColor("2. Save data near to the executable file with name \"table(Number of such a file)\"", ConsoleColor.Yellow);
            PrintColor("3. Exit the program", ConsoleColor.Yellow);
            int num;
            while(!int.TryParse(Console.ReadLine(), out num)|| (num!=1 && num!=2 && num!=3))
            {
                ErrorMessage();
            }
            return num;

        }
        /// <summary>
        /// This method prints the table in console.
        /// </summary>
        /// <param name="resTable"></param>
        /// <param name="columnNames"></param>
        public static void PrintTable(string[][] resTable, string[][] columnNames)
        {
            // Array of the maximal length of element in every row.
            int[] maxElems = new int[resTable[0].Length];
            for (int i = 0; i < columnNames.Length; i++)
            {
                for (int j = 0; j < columnNames[i].Length; j++)
                {
                    //Maximal length of element between two column names rows in each column.
                    maxElems[j] = Math.Max(maxElems[j], columnNames[i][j].Length);
                }
            }
            // Searching maximal length of column's element among all rows. 
            for (int i = 0; i < resTable.Length; i++)
            {
                for (int j = 0; j < resTable[i].Length; j++)
                {
                    maxElems[j] = Math.Max(resTable[i][j].Length, maxElems[j]);
                }
            }
            for (int i = 0; i < columnNames.Length; i++, Console.WriteLine())
            {
                for (int j = 0; j < columnNames[i].Length; j++)
                {
                    // Checking every element
                    if (columnNames[i][j] != null && columnNames[i][j].Length>0)
                    {
                        // Printing column names with spaces.
                        Console.Write(columnNames[i][j] + new string(' ', maxElems[j] - columnNames[i][j].Length+2));
                    }
                }
            }
            for (int i = 0; i < resTable.Length; i++, Console.WriteLine())
            {
                // Number of empty elements in each row.
                int counter = 0;
                for (int j = 0; j < resTable[i].Length; j++)
                {
                    if (resTable[i][j] == " " || resTable[i][j] is null)
                    {
                        counter += 1;
                    }
                }
                // Printing elements of row only if the row isn't empty.
                if (counter != resTable[i].Length)
                {
                    for (int j = 0; j < resTable[i].Length; j++)
                    {
                        // Checking elements.
                        if (resTable[i][j] != null && resTable[i][j] != " " && resTable[i][j].Length > 0)
                        {
                            // Printing elements of the table with right number of spaces.
                            if (j != resTable[i].Length - 1)
                            {
                                Console.Write(resTable[i][j] + new string(' ', maxElems[j] - resTable[i][j].Length + 2));
                            }
                            // Printing last element without spaces.
                            else
                            {
                                Console.Write(resTable[i][j]);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This method implements menu of actions before saving file and returns selected menu item.
        /// </summary>
        /// <returns></returns>
        public static int SaveFile()
        {
            int num;
            Console.WriteLine("The data is going to be saved. Choose the way of saving please.");
            Console.WriteLine("1. I want to save it in my file.");
            Console.WriteLine("2. Save it instead of the initial file.");
            Console.WriteLine("3. Exit the program without saving");
            while(!int.TryParse(Console.ReadLine(), out num) || (num!=1 && num!=2 && num!=3))
            {
                ErrorMessage();
            }
            return num;
        }
        /// <summary>
        /// This method changes color of a string.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void PrintColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
