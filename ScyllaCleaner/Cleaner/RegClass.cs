using Microsoft.Win32;
using Spotify.Enums;
using Spotify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Cleaner {
    static class RegClass {

        static List<string> HKEY_CURRENT_USER = new List<string>() {
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Compatibility Assistant\Store",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FeatureUsage\ShowJumpView",
            @"SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache"
        };

        static List<string> HKEY_USERS = new List<string>() {
            $@"{Utils.Utils.GetNTAccountSecIdentifier()}_Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache",
            $@"{Utils.Utils.GetNTAccountSecIdentifier()}\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FeatureUsage\AppSwitched",
            $@"{Utils.Utils.GetNTAccountSecIdentifier()}\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FeatureUsage\AppLaunch",
            $@"{Utils.Utils.GetNTAccountSecIdentifier()}\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths"
        };

        public static void RmvAllRegistryKeys() {
            var idx = 0;
            var del = 0;

            foreach (var path in HKEY_CURRENT_USER) {
                idx++;
                var rmv = 0;

                try {
                    RegistryKey rk = Registry.CurrentUser.OpenSubKey(path, true);

                    if (rk != null) {
                        Logger.Log($"{idx}. Searching for Possible Scylla Keys Inside '{rk.Name}'.", LogLevel.Info);
                        var values = rk.GetValueNames();
                        if (values.Length > 0) {
                            foreach (var item in values) {
                                Wishlist.List.ForEach(x => {
                                    if (item.ContainsCaseInsensitive(x)) {
                                        rk.DeleteValue(item);
                                        Logger.Log($"Removed Value {item} from Registry.", LogLevel.Hex);
                                        rmv++;
                                        del++;
                                    }
                                });
                            }
                        }

                        var keys = rk.GetSubKeyNames();
                        if (keys.Length > 0) {
                            foreach (var item in keys) {
                                Wishlist.List.ForEach(x => {
                                    if (item.ContainsCaseInsensitive(x)) {
                                        rk.DeleteSubKeyTree(item);
                                        Logger.Log($"Removed Key {item} from Registry.", LogLevel.Hex);
                                        rmv++;
                                        del++;
                                    }
                                });
                            }
                        }

                        if (rmv == 0) Logger.Log($"Nothing was Found.", LogLevel.Hex);
                        
                    } else Logger.Log($"Key Path Doesn't Exist. {path}", LogLevel.Error);
                } 
                catch (UnauthorizedAccessException) { Logger.Log("Unauthorized Access.", LogLevel.Error); }
                catch (ArgumentException) { /* Key Doesn't Found. */ } 
                catch (Exception exc) { Logger.Log("\n" + exc.ToString() + "\n", LogLevel.Error); }
            }

            foreach (var path in HKEY_USERS) {
                idx++;
                var rmv = 0;

                try {
                    RegistryKey rk = Registry.Users.OpenSubKey(path, true);

                    if (rk != null) {
                        Logger.Log($"{idx}. Searching for Possible Scylla Keys Inside '{rk.Name}'.", LogLevel.Info);
                        var values = rk.GetValueNames();
                        if (values.Length > 0) {
                            foreach (var item in values) {
                                Wishlist.List.ForEach(x => {
                                    if (item.ContainsCaseInsensitive(x)) {
                                        rk.DeleteValue(item);
                                        Logger.Log($"Removed Value {item} from Registry.", LogLevel.Hex);
                                        rmv++;
                                        del++;
                                    }
                                });
                            }
                        }

                        var keys = rk.GetSubKeyNames();
                        if (keys.Length > 0) {
                            foreach (var item in keys) {
                                Wishlist.List.ForEach(x => {
                                    if (item.ContainsCaseInsensitive(x)) {
                                        rk.DeleteSubKeyTree(item);
                                        Logger.Log($"Removed Key {item} from Registry.", LogLevel.Hex);
                                        rmv++;
                                        del++;
                                    }
                                });
                            }
                        }

                        if (rmv == 0) Logger.Log($"Nothing was Found.", LogLevel.Hex);

                    } else Logger.Log($"Key Path Doesn't Exist. {path}", LogLevel.Error);
                } 
                catch (UnauthorizedAccessException) { Logger.Log("Unauthorized Access.", LogLevel.Error); } 
                catch (ArgumentException) { /* Key Doesn't Found. */ }
                catch (Exception exc) { Logger.Log("\n" + exc.ToString() + "\n", LogLevel.Error); }
            }

            if (del != 0) Logger.Log($"Cleaned {del} Items.", LogLevel.Success);
            else Logger.Log($"Nothing Found (Already Cleaned).", LogLevel.Success);          
        }
    }
}