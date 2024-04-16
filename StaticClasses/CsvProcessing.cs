using System.Runtime.CompilerServices;
using System;
using System.IO;

namespace StaticClasses
{
    public static class CsvProcessing
    {
        private static string? _fPath;
        /// <summary>
        /// This property set path value.
        /// </summary>
        public static string FPath
        {
            get
            {
                // Checking path and return its value
                if (CheckPath(_fPath))
                    return _fPath;
                else
                    throw new ArgumentNullException();
            }
            set
            {
                // Checking path and set value
                if (CheckPath(value))
                    _fPath = value;
                else
                    throw new ArgumentNullException();
            }
        }
        /// <summary>
        /// This method checks path and throws exception if it is incorrect.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool CheckPath(string? path)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            
            // Allowing files with only csv extension.
            if (path is null || !File.Exists(path) || path.IndexOfAny(invalidPathChars) != -1 || path[^4..] != ".csv")
            {
                throw new ArgumentNullException();
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// This method reads a file and returns an array of table rows
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string[] Read()
        {
            // Initial names of table columns.
            string[] alphabetEng = { "ID", "LibraryName", "AdmArea", "District", "Address", "NumberOfAccessPoints", "WiFiName", "CoverageArea", "FunctionFlag", "AccessFlag", "Password", "Latitude_WGS84", "Longitude_WGS84", "global_id", "geodata_center", "geoarea" };
            string[] alphabetRus = { "Код", "Наименование библиотеки", "Административный округ", "Район", "Адрес", "Количество точек доступа", "Имя Wi-Fi сети", "Зона покрытия, в метрах", "Признак функционирования", "Условия доступа", "Пароль", "Широта в WGS-84", "Долгота в WGS-84", "global_id", "geodata_center", "geoarea" };
            
            //Reading all file lines into an array.
            string[] tableRows = File.ReadAllLines(_fPath);
                
            // Checking data in the file.
            // Number of table rows should be >= 3 because a file must include two columns' name rows and at least one row with data.
            if (tableRows is null || tableRows.Length < 3 || tableRows[0].Split(";")[..^1].Length != 16)
            {
                throw new ArgumentNullException();
            }

            for (int i = 0; i < tableRows.Length; i++)
            {
                // Checking length of every row. It mast be equal to the first two lines. Without last element because ir is \n.
                if (tableRows[i].Split(";")[..^1].Length != tableRows[0].Split(";")[..^1].Length)
                {
                    throw new ArgumentNullException();
                }
                // Checking data format. Every element's first and last symbol mast be a quote.
                for (int j = 0; j < tableRows[i].Split(";")[..^1].Length; j++)
                {
                    if (tableRows[i].Split(";")[j][0] != '\"' || tableRows[i].Split(";")[j][^1] != '\"')
                    {
                        throw new ArgumentNullException();
                    }
                }
            }
            // Checking first two rows, every one must consists of initial names of columns.
            for (int j = 0; j < tableRows[0].Split(';')[..^1].Length; j++)
            {
                if (tableRows[0].Split(";")[j].Trim('\"') != alphabetEng[j] || tableRows[1].Split(";")[j].Trim('\"') != alphabetRus[j])
                {
                    throw new ArgumentNullException();
                }
            }

            return tableRows;
        }
        /// <summary>
        /// This method writes the table into the user's file.
        /// </summary>
        /// <param name="tableRow"></param>
        /// <param name="nPath"></param>
        /// <param name="exit"></param>
        public static void Write(string tableRow, string? nPath, out bool exit)
        {
            // Variable to exit if it will be false.
            exit = true;

            char[] invalidPathChars = Path.GetInvalidPathChars();

            // This cycle doesn't allow to do next step until user enters correct path
            while (true)
            {
                // Checking path. If it is incorrect reveal interface menu with options.
                if (nPath == null || nPath.IndexOfAny(invalidPathChars) != -1 || nPath.Length <= 0 || nPath == " " || nPath[^4..] != ".csv")
                {
                    int num = Interface.CheckFileName();
                    // If user wants to enter file's name again.
                    if (num == 1)
                    {
                        nPath = Console.ReadLine();
                        continue;
                    }
                    else
                    // If user wants to create a file with data in the same directory
                    if (num == 2)
                    {
                        int i = 1;
                        // Checking is a file with the same name exists.
                        while (File.Exists($"table{i}.csv"))
                        {
                            i++;
                        }
                        nPath = $"table{i}.csv";
                        // Creating a new file with a correct name "table" and counter number.
                        try
                        {
                            File.WriteAllText(nPath, tableRow);
                            break;
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
                            Interface.PrintColor("Error while writing data in the file. Please try again.", ConsoleColor.Red);
                            continue;
                        }
                        catch (ArgumentException)
                        {
                            Interface.PrintColor("Wrong file name.Please try again.", ConsoleColor.Red);
                            continue;
                        }
                    }
                    // If user wants to exit the program.
                    else
                    {
                        exit = false;
                        break;

                    }

                }
                // If file name is correct appending table in existing file or creating a new one and writing data there.
                else
                {
                    try
                    {
                        File.AppendAllText(nPath, tableRow);
                        break;
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
                        Interface.PrintColor("Error while writing data in the file. Please try again.", ConsoleColor.Red);
                        continue;
                    }
                    catch (ArgumentException)
                    {
                        Interface.PrintColor("Wrong file name.Please try again.", ConsoleColor.Red);
                        continue;
                    }
                }
            }
        }
        /// <summary>
        /// This method rewrite the initial file with new data.
        /// </summary>
        /// <param name="resTable"></param>
        public static void Write(string[] resTable)
        {
            //No checking because fPath has been already checked.
            for (int i = 0; i < resTable.Length; i++)
            {
                // Rewriting the file with the first row of the table.
                if (i == 0)
                    File.WriteAllText(_fPath, resTable[i]);
                // The rest of rows appending to the firs one.
                else
                    File.AppendAllText(_fPath, resTable[i]);
            }
        }
    }
}