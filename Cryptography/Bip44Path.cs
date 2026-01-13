namespace CasperWallet.Cryptography
{
    /// <summary>
    /// Utility klasa do generowania BIP44 ścieżek derywacji
    /// https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki
    /// 
    /// Format: m/44'/coin_type'/account'/change/address_index
    /// Casper Network coin type = 506
    /// </summary>
    public static class Bip44Path
    {
        /// <summary>
        /// Coin type dla Casper Network
        /// https://github.com/satoshilabs/slips/blob/master/slip-0044.md
        /// </summary>
        private const uint CasperNetworkCoinType = 506;

        /// <summary>
        /// Generuje BIP44 ścieżkę derywacji dla Casper Network
        /// 
        /// Equivalent TypeScript: getBip44Path()
        /// </summary>
        /// <param name="index">Indeks konta (0, 1, 2, ...)</param>
        /// <returns>Ścieżka w formacie "m/44'/506'/0'/0/index"</returns>
        public static string GetBip44Path(int index)
        {
            // m = Master key
            // 44' = BIP44 (hardened, stąd apostrof)
            // 506' = Casper Network coin type (hardened)
            // 0' = Account 0 (hardened)
            // 0 = External chain (non-hardened)
            // index = Address index (non-hardened)

            return $"m/44'/{CasperNetworkCoinType}'/0'/0/{index}";
        }

        /// <summary>
        /// Konwertuje BIP44 ścieżkę na tablicę indeksów do derywacji
        /// </summary>
        /// <param name="path">Ścieżka w formacie "m/44'/506'/0'/0/0"</param>
        /// <returns>Tablica uint[] indeksów z informacją o hardening'u</returns>
        public static uint[] ParseBip44Path(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("m"))
            {
                throw new ArgumentException("Invalid BIP44 path format");
            }

            var parts = path.Substring(2).Split('/'); // Pomiń "m/"
            var indices = new uint[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                uint index;

                // Hardened key jeśli kończy się apostrofem
                if (part.EndsWith("'"))
                {
                    part = part.TrimEnd('\'');
                    index = uint.Parse(part) + 0x80000000; // Hardened flag
                }
                else
                {
                    index = uint.Parse(part);
                }

                indices[i] = index;
            }

            return indices;
        }
    }
}
