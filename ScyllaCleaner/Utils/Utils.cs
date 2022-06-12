using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Utils {
    internal static class Utils {
        public static string GetNTAccountSecIdentifier() {
            NTAccount nTAccount = new NTAccount(WindowsIdentity.GetCurrent().Name);
            SecurityIdentifier securityIdentifier = (SecurityIdentifier)nTAccount.Translate(typeof(SecurityIdentifier));

            return securityIdentifier.ToString();
        }

        public static bool ContainsCaseInsensitive(this string source, string substring) {
            return source?.IndexOf(substring, StringComparison.OrdinalIgnoreCase) > -1;
        }

        public static string CalculateMD5(string filename) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(filename)) {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}