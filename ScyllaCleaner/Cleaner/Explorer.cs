using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Shell32;
using Spotify.Enums;
using Spotify.Utils;

namespace Spotify.Cleaner {
    internal class Explorer {

        private static readonly Dictionary<KnownFolder, Guid> _guids = new() {
            [KnownFolder.Contacts]      = new("56784854-C6CB-462B-8169-88E350ACB882"),
            [KnownFolder.Downloads]     = new("374DE290-123F-4565-9164-39C4925E467B"),
            [KnownFolder.Favorites]     = new("1777F761-68AD-4D8A-87BD-30B759FA33DD"),
            [KnownFolder.Links]         = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968"),
            [KnownFolder.SavedGames]    = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
            [KnownFolder.SavedSearches] = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA")
        };

        /// <summary>
        /// Removes the file or folder on specified path.
        /// </summary>
        public static bool Rmv(string path) {
            FileAttributes attr = File.GetAttributes(path);

            var flag = false;

            try {
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) Directory.Delete(path);
                else File.Delete(path);      

                flag = !flag;
            } 
            catch (DirectoryNotFoundException) {
                return flag;
            } 
            catch (FileNotFoundException) {
                return flag;
            }

            return flag;
        }

        public static void RmvAll() {
            var paths = new List<string>() {
                GetPath(KnownFolder.Downloads),
                GetPath(KnownFolder.Favorites),
                GetPath(KnownFolder.Links),
                $@"C:\$Recycle.Bin\{Utils.Utils.GetNTAccountSecIdentifier()}", //Recycle Bin
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent" //Recent
            };

            if (paths.Count == 0) {
                Logger.Log($"No Directory was Specified.", LogLevel.Error);
                return;
            }

            Logger.Log($"Found <{paths.Count}> Directories.", LogLevel.Info);

            var pathidx = 0;
            foreach (var path in paths) {
                pathidx++;

                Logger.Log($"{pathidx}. Searching for Possible Scylla Files Inside '{path}'.", LogLevel.Info);

                //Directory doesn't Exists.
                if (!Directory.Exists(path)) continue;

                string[] files = Directory.GetFiles(path);

                //Directory is Empty.
                if (files.Length == 0) {
                    Logger.Log($"Nothing was Found.", LogLevel.Hex);
                    continue;
                }

                var idx = 0;

                foreach (string file in files) {
                    //Look only to the files that contains "scy" string unless the specified path is the Recycle bin.
                    Wishlist.List.ForEach(x => {
                        if (file.ContainsCaseInsensitive(x)) {
                            idx++;
                            var status = Rmv(file);

                            if (status) Logger.Log($"Removed '{file}'.", Enums.LogLevel.Hex);
                            else Logger.Log($"Error While Trying to Remove '{file}'.", Enums.LogLevel.Error);
                        }
                    });
                }

                if (idx == 0)
                    Logger.Log($"Nothing was Found.", LogLevel.Hex);
                else
                    Logger.Log($"Removed {idx} Related Files.", LogLevel.Success);
            }
        }

        public static string GetPath(KnownFolder knownFolder) {
            return NativeImport.NativeImport.SHGetKnownFolderPath(_guids[knownFolder], 0);
        }
    }
}
