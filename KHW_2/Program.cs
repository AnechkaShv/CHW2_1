// Второй вариант решения, когда исключения отлавливаются в месте вызова, а не в каждом методе. Вариант не по тз, но на семинаре сказали, что так правильнее.
using StaticClasses;
using System.Threading.Channels;

namespace KHW_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // For right encoding only on devices with english orientation.
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Console.InputEncoding = System.Text.Encoding.GetEncoding("utf-16");

            // Variable to exit on every step.
            bool exit = true;

            // Only for Windows to add an ability to scroll console window.
            //Console.BufferWidth = 1024;

            //Cycle of repeating
            do
            {
                while (true)
                {
                    Console.WriteLine("Enter an absolute path to the csv-file.");
                    try
                    {
                        // Changing static field using static property
                        CsvProcessing.FPath = Console.ReadLine();

                        // Reading table rows from the file to the srray of strings.
                        string[] tableRows = CsvProcessing.Read();

                        Interface.PrintColor("Your file is correct. Congratulations!", ConsoleColor.Green);

                        // Deviding strings into the array of elments' arrays.
                        string[][] tableValues = DataProcessing.InitData(tableRows);

                        // English names of columns.
                        string[] columnNames = tableValues[0];


                        int menuItem;
                        // Working with menu and user actions.
                        // Working only with elements without column names.
                        string[][]? resTable = Interface.ActMenu(tableValues[2..], columnNames, out menuItem);
                        // If user wants to exit the programm
                        if (menuItem == 6)
                        {
                            exit = false;
                            break;
                        }
                        // If user hasn't entered 6 to exit but result table is null.
                        else if (resTable == null)
                            throw new ArgumentNullException();

                        // Printing the result table.
                        Interface.PrintTable(resTable, tableValues[..2]);

                        // Asking how to save file.
                        int numMenu = Interface.SaveFile();
                        // If user wants to enter file name and save the result table there.
                        if (numMenu == 1)
                        {
                            Console.WriteLine("Enter your file name.");
                            string? nPath = Console.ReadLine();

                            // Adding the table to the file without column names if the file exists.
                            if(File.Exists(nPath) && File.ReadAllLines(nPath).Length !=0)
                                CsvProcessing.Write(DataProcessing.FormatTable(DataProcessing.FormatTable(resTable, tableValues[..2])[2..]), nPath, out exit);
                            // Adding the whole table if the file doesn't exist.
                            else
                                CsvProcessing.Write(DataProcessing.FormatTable(DataProcessing.FormatTable(resTable, tableValues[..2])), nPath, out exit);
                            Interface.PrintColor($"Data has been successfully saved in the file {nPath}", ConsoleColor.Green);
                            break;
                        }
                        // If user wants to save data in the initial file indtead of existed table.
                        else if (numMenu == 2)
                        {
                            CsvProcessing.Write(DataProcessing.FormatTable(resTable, tableValues[..2]));
                            Interface.PrintColor($"Data has been successfully saved in the file: {CsvProcessing.FPath}", ConsoleColor.Green);
                            break;
                        }
                        // If user wants to exit program without saving data.
                        else
                        {
                            exit = false;
                            break;
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        Interface.PrintColor("Wrong file. Please try again.", ConsoleColor.Red);
                        continue;
                    }
                    catch (PathTooLongException)
                    {
                        Interface.PrintColor("Your file name is too long. Please try again.", ConsoleColor.Red);
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Interface.PrintColor("Wrong file path. Please try again", ConsoleColor.Red);
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Interface.PrintColor("This file can be only read. Please try again.", ConsoleColor.Red);
                        continue;
                    }
                    catch (IOException)
                    {
                        Interface.PrintColor("Error while opening file. Please ry again", ConsoleColor.Red);
                        continue;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Interface.PrintColor("Error while working with arrays. Please try again", ConsoleColor.Red);
                        continue;
                    }
                    catch(ArgumentException)
                    {
                        Interface.PrintColor("Error while working with file. Please try again.", ConsoleColor.Red);
                        continue;
                    }
                }
                // If exit has been never wanted by user offering to continue or finish.
                if(exit)
                    Console.WriteLine("Enter any key to restart. To exit enter Escape.");
            } while (exit && Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}