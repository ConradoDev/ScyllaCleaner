using Microsoft.Win32.SafeHandles;
using Spotify.Enums;
using System;
using System.IO;
using System.Reflection;

namespace Spotify.Utils {
    internal class Logger {
        /// <summary>
        /// Allocates a console instance to the running process
        /// </summary>
        public static void CreateLoggerInstance() {
            NativeImport.NativeImport.AllocConsole();

            var outFile = NativeImport.NativeImport.CreateFile("CONOUT$", NativeImport.NativeImport.ConsolePropertyModifiers.GENERIC_WRITE
                                                            |  NativeImport.NativeImport.ConsolePropertyModifiers.GENERIC_READ,
                                                               NativeImport.NativeImport.ConsolePropertyModifiers.FILE_SHARE_WRITE,
                                                            0, NativeImport.NativeImport.ConsolePropertyModifiers.OPEN_EXISTING, /*FILE_ATTRIBUTE_NORMAL*/0, 0);

            var safeHandle = new SafeFileHandle(outFile, true);

            NativeImport.NativeImport.SetStdHandle(-11, outFile);

            FileStream fs = new FileStream(safeHandle, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs) { AutoFlush = true };

            Console.SetOut(writer);

            if (NativeImport.NativeImport.GetConsoleMode(outFile, out var cMode)) NativeImport.NativeImport.SetConsoleMode(outFile, cMode | 0x0200);

            Console.Title = $"Log Window - {Assembly.GetExecutingAssembly().GetName().Name}";
        }

        /// <summary>
        /// Logs the given string to the console
        /// </summary>
        /// <param name="dataToLog"></param>
        /// <param name="severity"></param>
        public static string Log(object str, LogLevel FormatColor, bool Newline = true) {
            if (NativeImport.NativeImport.GetConsoleWindow() != IntPtr.Zero) {
                var ConsoleColour = Console.ForegroundColor;

                string Prefix = "";
                string Sufix = "";
                if (Newline) {
                    Prefix = $"[{DateTime.Now.ToString("h:mm:ss")} - {Assembly.GetExecutingAssembly().GetName().Name}] => ";
                    Sufix = "\n";
                }

                switch (FormatColor) {
                    case LogLevel.Debug:
                        ConsoleColour = ConsoleColor.Cyan;
                        break;
                    case LogLevel.Error:
                        ConsoleColour = ConsoleColor.DarkRed;
                        break;
                    case LogLevel.Warn:
                        ConsoleColour = ConsoleColor.Magenta;
                        break;
                    case LogLevel.Success:
                        ConsoleColour = ConsoleColor.DarkGreen;
                        break;
                    case LogLevel.Info:
                        ConsoleColour = ConsoleColor.DarkYellow;
                        break;
                    case LogLevel.Neutral:
                        ConsoleColour = ConsoleColor.Gray;
                        break;
                    case LogLevel.Hex:
                        ConsoleColour = ConsoleColor.DarkGray;
                        break;
                    default:
                        // Default color
                        break;
                }

                Console.ForegroundColor = ConsoleColour;

                string Format = str.ToString();

                if (String.IsNullOrEmpty(Format)) {
                    Console.Write($"{Prefix} StringNullOrEmpty Occured at LogService.Log {Sufix}");
                    return $"{Prefix} StringNullOrEmpty Occured at LogService.Log {Sufix}";
                }

                Console.Write($"{Prefix}{Format}{Sufix}");
                return $"{Prefix}{Format}{Sufix}";
            } else {
                //MessageBox.Show(null, "Error: There is no debug console running!", $"{Assembly.GetExecutingAssembly().GetName().Name}");
                return "Error: There is no debug console running!";
            }
        }
    }
}