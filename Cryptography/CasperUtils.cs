using System;
using System.Security.Cryptography;

namespace CasperWallet.Cryptography
{
    /// <summary>
    /// Utility funkcje specyficzne dla Casper Network
    /// https://docs.casper.network/concepts/accounts-and-keys
    /// </summary>
    public static class CasperUtils
    {
        /// <summary>
        /// ED25519 algorithm tag dla Casper Network
        /// </summary>
        private const byte ED25519_TAG = 0x01;

        /// <summary>
        /// Account tag dla AccountHash
        /// </summary>
        private const byte ACCOUNT_HASH_TAG = 0x00;

        /// <summary>
        /// Dodaje ED25519 type prefix do public key
        /// Casper Network wymaga: "01" + public key (64 znaki)
        /// </summary>
        /// <param name="publicKeyHex">Public key w formacie HEX (64 znaki)</param>
        /// <returns>Public key z prefix'em "01" (66 znaków)</returns>
        public static string AddED25519Prefix(string publicKeyHex)
        {
            if (string.IsNullOrWhiteSpace(publicKeyHex) || publicKeyHex.Length != 64)
            {
                throw new ArgumentException(
                    "Public key musi być 64-znakowym HEX string'iem (32 bajty)");
            }

            return "01" + publicKeyHex;
        }

        /// <summary>
        /// Generuje AccountHash dla Casper Network
        /// AccountHash = SHA256(concat(tag=0x00, public_key_bytes))
        /// Format wyniku: "account-hash-" + 64-znakowy HEX
        /// </summary>
        /// <param name="publicKeyRawHex">Raw public key HEX (64 znaki, BEZ prefix'u)</param>
        /// <returns>AccountHash w formacie "account-hash-..."</returns>
        public static string GenerateAccountHash(string publicKeyRawHex)
        {
            if (string.IsNullOrWhiteSpace(publicKeyRawHex) || publicKeyRawHex.Length != 64)
            {
                throw new ArgumentException(
                    "Public key musi być 64-znakowym HEX string'iem (32 bajty)");
            }

            try
            {
                // Konwersja HEX string na bajty
                byte[] publicKeyBytes = HexStringToBytes(publicKeyRawHex);

                // Casper Network: AccountHash = SHA256(tag + publicKey)
                byte[] hashInput = new byte[publicKeyBytes.Length + 1];
                hashInput[0] = ACCOUNT_HASH_TAG;  // ← POPRAWIONE: przypisanie do elementu [0]
                Array.Copy(publicKeyBytes, 0, hashInput, 1, publicKeyBytes.Length);

                // Obliczenie SHA256 hash
                using (var sha256 = SHA256.Create())
                {
                    byte[] accountHashBytes = sha256.ComputeHash(hashInput);
                    string accountHashHex = BytesToHexString(accountHashBytes);

                    return "account-hash-" + accountHashHex;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Błąd podczas generowania AccountHash", ex);
            }
        }

        /// <summary>
        /// Konwertuje HEX string na array bajtów
        /// </summary>
        /// <param name="hex">HEX string (format: "a1b2c3d4...")</param>
        /// <returns>Array bajtów</returns>
        public static byte[] HexStringToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex) || hex.Length % 2 != 0)
            {
                throw new ArgumentException("HEX string musi mieć parzystą liczbę znaków");
            }

            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                string byteString = hex.Substring(i, 2);
                bytes[i / 2] = Convert.ToByte(byteString, 16);
            }

            return bytes;
        }

        /// <summary>
        /// Konwertuje array bajtów na HEX string
        /// </summary>
        /// <param name="bytes">Array bajtów</param>
        /// <returns>HEX string (lowercase)</returns>
        public static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException("Bytes array nie może być pusty");
            }

            return BitConverter.ToString(bytes)
                .Replace("-", "")
                .ToLowerInvariant();
        }
    }
}
