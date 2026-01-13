using System;
using System.Collections.Generic;
using CasperWallet.Models;
using NBitcoin;
using Chaos.NaCl;

namespace CasperWallet.Cryptography
{
    /// <summary>
    /// Główna klasa obsługująca derywację kluczy HD (Hierarchical Deterministic)
    /// 
    /// Equivalent TypeScript: deriveKeyPair() z casper-wallet
    /// </summary>
    public static class KeyDerivation
    {
        /// <summary>
        /// GŁÓWNA FUNKCJA: Derywuje parę kluczy (public + private) dla danego indeksu konta
        /// 
        /// 
        /// Łańcuch transformacji:
        /// Seed Phrase → PBKDF2 → Master Seed → BIP32 HD → BIP44 Derivation → 
        /// ED25519 Signing → ED25519 prefix + AccountHash
        /// </summary>
        /// <param name="secretPhrase">Tablica 24 słów seed phrase</param>
        /// <param name="index">Indeks konta (0, 1, 2, ...)</param>
        /// <returns>KeyPair zawierająca publicKey (HEX z "01" prefix'em), secretKey (BASE64) i AccountHash</returns>
        /// <exception cref="ArgumentException">Gdy seed phrase jest Invalid</exception>

        public static CasperWallet.Models.KeyPair DeriveKeyPair(string[] secretPhrase, int index)
        {
            try
            {
                // KROK 1: Walidacja seed phrase
                if (!Bip39Utils.ValidateSecretPhrase(secretPhrase))
                {
                    throw new ArgumentException("Secret phrase jest invalid");
                }

                // KROK 2: Konwersja seed phrase na 512-bitowy seed (PBKDF2)
                byte[] seed = Bip39Utils.SecretPhraseToSeed(secretPhrase);

                // KROK 3: BIP32 derywacja ścieżki
                var mnemonicString = string.Join(" ", secretPhrase);
                var mnemonic = new Mnemonic(mnemonicString, Wordlist.English);
                var masterKey = mnemonic.DeriveExtKey();

                // KROK 4: Derywacja zgodnie z BIP44 (m/44'/506'/0'/0/index)
                string bip44Path = Bip44Path.GetBip44Path(index);
                uint[] pathIndices = Bip44Path.ParseBip44Path(bip44Path);

                ExtKey derivedKey = masterKey;
                foreach (uint pathIndex in pathIndices)
                {
                    derivedKey = derivedKey.Derive(pathIndex);
                }

                // KROK 5: Pobierz 32-bajtowy seed dla Ed25519
                byte[] privateKeyBytes = derivedKey.PrivateKey.ToBytes();

                // WAŻNE: Używamy tylko pierwszych 32 bajtów jako seed dla Ed25519
                byte[] ed25519Seed = new byte[32];
                Array.Copy(privateKeyBytes, 0, ed25519Seed, 0, 32);

                // KROK 6: Generuj Ed25519 keypair z seed
                byte[] publicKeyBytes;
                byte[] expandedPrivateKey;

                Ed25519.KeyPairFromSeed(out publicKeyBytes, out expandedPrivateKey, ed25519Seed);

                // KROK 7: Konwersja formatów
                string publicKeyRawHex = BytesToHex(publicKeyBytes);

                // Weryfikacja: Ed25519 public key to zawsze 32 bajty = 64 znaki HEX
                if (publicKeyRawHex.Length != 64)
                {
                    throw new InvalidOperationException(
                        $"Ed25519 public key ma nieprawidłową długość: {publicKeyRawHex.Length} (oczekiwano 64)");
                }

                // Dodaj Ed25519 prefix "01" dla Casper Network
                string publicKeyWithPrefix = CasperUtils.AddED25519Prefix(publicKeyRawHex);

                // Generuj AccountHash dla Casper Network
                string accountHash = CasperUtils.GenerateAccountHash(publicKeyRawHex);

                // Ed25519 private key to pierwsze 32 bajty expanded key
                byte[] ed25519PrivateKey = new byte[32];
                Array.Copy(expandedPrivateKey, 0, ed25519PrivateKey, 0, 32);

                string secretKeyBase64 = BytesToBase64(ed25519PrivateKey);

                return new CasperWallet.Models.KeyPair
                {
                    PublicKey = publicKeyWithPrefix,        // "01" + 64 znaki (66 total)
                    PublicKeyRaw = publicKeyRawHex,         // Raw 64 znaki
                    SecretKey = secretKeyBase64,            // BASE64 encoded
                    AccountHash = accountHash               // "account-hash-..."
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Błąd podczas derywacji klucza dla indeksu {index}", ex);
            }
        }

        /// <summary>
        /// Derywuje wiele kont (konta 0, 1, 2, ...) z tego samego seed phrase
        /// </summary>
        /// <param name="secretPhrase">Tablica 24 słów seed phrase</param>
        /// <param name="accountCount">Ile kont do wygenerowania</param>
        /// <returns>Lista KeyPair dla każdego konta</returns>
        public static List<CasperWallet.Models.KeyPair> DeriveMultipleKeyPairs(
            string[] secretPhrase,
            int accountCount)
        {
            var keyPairs = new List<CasperWallet.Models.KeyPair>();

            for (int i = 0; i < accountCount; i++)
            {
                var keyPair = DeriveKeyPair(secretPhrase, i);
                keyPairs.Add(keyPair);
            }

            return keyPairs;
        }

        /// <summary>
        /// Konwertuje bajty na HEX string (lowercase)
        /// </summary>
        private static string BytesToHex(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException("Bytes array cannot be null or empty");
            }

            return BitConverter.ToString(bytes)
                .Replace("-", "")
                .ToLowerInvariant();
        }

        /// <summary>
        /// Konwertuje bajty na BASE64 string
        /// </summary>
        private static string BytesToBase64(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException("Bytes array cannot be null or empty");
            }

            return Convert.ToBase64String(bytes);
        }
    }
}
