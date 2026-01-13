using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NBitcoin;

namespace CasperWallet.Cryptography
{
    /// <summary>
    /// Utility klasa do obsługi BIP39 (Mnemonic Code)
    /// https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki
    /// </summary>
    public static class Bip39Utils
    {
        private const int MnemonicEntropyBits = 256; // 24 słowa = 256 bitów entropii
        private const int Pbkdf2Iterations = 2048;   // Standard BIP39
        private const int SeedLengthBytes = 64;      // 512 bitów

        /// <summary>
        /// Generuje losową seed phrase (24 słowa)
        /// Equivalent TypeScript: generateSecretPhrase()
        /// </summary>
        /// <returns>Tablica 24 słów w formacie string[]</returns>
        public static string[] GenerateSecretPhrase()
        {
            try
            {
                // NBitcoin: Generuje mnemonic z 256 bitami entropii = 24 słowa
                var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);

                return mnemonic.Words;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Błąd podczas generowania seed phrase", ex);
            }
        }

        /// <summary>
        /// Konwertuje seed phrase na 512-bitowy seed
        /// Używa PBKDF2-HMAC-SHA512 (2048 iteracji)
        /// 
        /// Equivalent TypeScript: secretPhraseToSeed()
        /// </summary>
        /// <param name="secretPhrase">Tablica 24 słów</param>
        /// <returns>512-bitowy seed (64 bajty)</returns>
        public static byte[] SecretPhraseToSeed(string[] secretPhrase)
        {
            try
            {
                if (!ValidateSecretPhrase(secretPhrase))
                {
                    throw new ArgumentException("Secret phrase jest Invalid");
                }

                // Łączenie słów w single string oddzielone spacją
                var mnemonicString = string.Join(" ", secretPhrase);

                // BIP39 standard: seed = PBKDF2-HMAC-SHA512(mnemonic, salt="mnemonic", 2048 iterations)
                var salt = Encoding.UTF8.GetBytes("mnemonic");
                var passwordBytes = Encoding.UTF8.GetBytes(mnemonicString);

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    passwordBytes,
                    salt,
                    Pbkdf2Iterations,
                    HashAlgorithmName.SHA512))
                {
                    return pbkdf2.GetBytes(SeedLengthBytes);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Błąd podczas konwersji seed phrase na seed", ex);
            }
        }

        /// <summary>
        /// Waliduje czy seed phrase jest prawidłowy mnemonic BIP39
        /// Equivalent TypeScript: validateSecretPhrase()
        /// </summary>
        /// <param name="secretPhrase">Tablica słów do weryfikacji</param>
        /// <returns>true jeśli seed phrase jest Valid, false w przeciwnym razie</returns>
        public static bool ValidateSecretPhrase(string[] secretPhrase)
        {
            try
            {
                if (secretPhrase == null || secretPhrase.Length != 24)
                {
                    return false;
                }

                var mnemonicString = string.Join(" ", secretPhrase);

                // NBitcoin waliduje mnemonic automatycznie
                var mnemonic = new Mnemonic(mnemonicString, Wordlist.English);

                return mnemonic.IsValidChecksum;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Konwertuje seed phrase na entropy (entropia)
        /// Użyteczne dla enkodowania/dekodowania
        /// </summary>
        /// <param name="secretPhrase">Tablica 24 słów</param>
        /// <returns>Entropy w formacie bajtów</returns>
        public static byte[] EncodeSeed(string[] secretPhrase)
        {
            try
            {
                if (!ValidateSecretPhrase(secretPhrase))
                {
                    throw new ArgumentException("Secret phrase jest Invalid");
                }

                var mnemonicString = string.Join(" ", secretPhrase);
                var mnemonic = new Mnemonic(mnemonicString, Wordlist.English);

                return mnemonic.DeriveSeed();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Błąd podczas enkodowania seed", ex);
            }
        }

        /// <summary>
        /// Konwertuje entropy z powrotem na seed phrase
        /// </summary>
        /// <param name="entropy">Entropy w formacie bajtów</param>
        /// <returns>Tablica 24 słów</returns>
        public static string[] DecodeSeed(byte[] entropy)
        {
            try
            {
                var mnemonic = new Mnemonic(Wordlist.English, entropy);
                return mnemonic.Words;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Błąd podczas dekodowania seed", ex);
            }
        }
    }
}
