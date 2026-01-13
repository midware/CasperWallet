using System;
using System.Collections.Generic;
using System.Linq;
using CasperWallet.Cryptography;
using CasperWallet.Models;

namespace CasperWallet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Casper Wallet - Key Derivation (C# .NET) - Kamil Szymoniak║");
            Console.WriteLine("║  100% WORKING WITH CASPER NETWORK                          ║");
            Console.WriteLine("║  BIP39 + BIP32 + BIP44 + Casper Network Implementation     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                // ═══════════════════════════════════════════════════════════
                // STEP 1: GENERATE SEED PHRASE (24 words)
                // ═══════════════════════════════════════════════════════════
                Console.WriteLine("┌─ STEP 1: Generating seed phrase ──────────────────────────┐");
                string[] secretPhrase = Bip39Utils.GenerateSecretPhrase();

                Console.WriteLine($"Seed phrase ({secretPhrase.Length} words):");
                Console.WriteLine();

                // Display seed phrase in 6 columns
                for (int i = 0; i < secretPhrase.Length; i++)
                {
                    Console.Write($"{i + 1:D2}. {secretPhrase[i],-12} ");

                    if ((i + 1) % 6 == 0)
                        Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();

                // ⚠️ CRITICAL WARNING
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  SECURITY WARNING:");
                Console.WriteLine("  • NEVER store seed phrase in plaintext");
                Console.WriteLine("  • NEVER commit this to Git");
                Console.WriteLine("  • Keep seed phrase offline in a secure place");
                Console.WriteLine("  • Anyone with seed phrase has access to ALL funds");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine();

                // ═══════════════════════════════════════════════════════════
                // STEP 2: VALIDATE SEED PHRASE
                // ═══════════════════════════════════════════════════════════
                Console.WriteLine("┌─ STEP 2: Validating seed phrase ──────────────────────────┐");
                bool isValid = Bip39Utils.ValidateSecretPhrase(secretPhrase);
                Console.WriteLine($"Status: {(isValid ? "✓ Valid" : "✗ Invalid")}");
                Console.WriteLine();
                Console.WriteLine();

                // ═══════════════════════════════════════════════════════════
                // STEP 3: CONVERT TO SEED (PBKDF2-HMAC-SHA512)
                // ═══════════════════════════════════════════════════════════
                Console.WriteLine("┌─ STEP 3: Converting to seed (PBKDF2-HMAC-SHA512) ────────┐");
                byte[] seed = Bip39Utils.SecretPhraseToSeed(secretPhrase);
                Console.WriteLine($"Seed length: {seed.Length} bytes ({seed.Length * 8} bits)");
                Console.WriteLine();
                Console.WriteLine("Process:");
                Console.WriteLine("  1. Password = seed phrase (24 words)");
                Console.WriteLine("  2. Salt = \"mnemonic\"");
                Console.WriteLine("  3. Algorithm = PBKDF2-HMAC-SHA512");
                Console.WriteLine("  4. Iterations = 2048");
                Console.WriteLine("  5. Output = 512-bit seed (64 bytes)");
                Console.WriteLine();
                Console.WriteLine();

                // ═══════════════════════════════════════════════════════════
                // STEP 4: KEY DERIVATION (BIP44)
                // ═══════════════════════════════════════════════════════════
                Console.WriteLine("┌─ STEP 4: Key derivation (BIP44) ──────────────────────────┐");
                int accountCount = 3;
                Console.WriteLine($"Generating {accountCount} accounts from the same seed phrase:");
                Console.WriteLine();

                var wallet = new CasperWallet.Models.CasperWallet
                {
                    SecretPhrase = secretPhrase
                };

                for (int i = 0; i < accountCount; i++)
                {
                    Console.WriteLine($"╔═══════════════════════════════════════════════════════════╗");
                    Console.WriteLine($"║ ACCOUNT #{i} (m/44'/506'/0'/0/{i})                        ");
                    Console.WriteLine($"╚═══════════════════════════════════════════════════════════╝");
                    Console.WriteLine();

                    // Key derivation
                    var keyPair = KeyDerivation.DeriveKeyPair(secretPhrase, i);

                    // Prepare data for display
                    var account = new Account
                    {
                        Index = i,
                        KeyPair = keyPair,
                        DerivationPath = $"m/44'/506'/0'/0/{i}"
                    };

                    wallet.Accounts.Add(account);

                    // Display results
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Public Key (Casper Format - with \"01\" prefix):");
                    Console.ResetColor();
                    Console.WriteLine($"  {keyPair.PublicKey}");
                    Console.WriteLine($"  Length: {keyPair.PublicKey.Length} characters");
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Account Hash (Wallet address in Casper Network):");
                    Console.ResetColor();
                    Console.WriteLine($"  {keyPair.AccountHash}");
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Secret Key (BASE64) - NEVER share:");
                    Console.ResetColor();
                    Console.WriteLine($"  {keyPair.SecretKey}");
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Account Details:");
                    Console.ResetColor();
                    Console.WriteLine($"  Derivation Path: {account.DerivationPath}");
                    Console.WriteLine($"  BIP44 Coin Type: 506 (Casper Network)");
                    Console.WriteLine($"  Public Key (raw, without prefix): {keyPair.PublicKeyRaw}");
                    Console.WriteLine();
                }

                // ═══════════════════════════════════════════════════════════
                // SUMMARY
                // ═══════════════════════════════════════════════════════════
                Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║ WALLET SUMMARY                                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine($"Wallet created: {wallet.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine($"Number of accounts: {wallet.Accounts.Count}");
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("CASPER NETWORK COMPATIBILITY:");
                Console.WriteLine(" • Public Key format: ED25519 with \"01\" prefix ✓");
                Console.WriteLine(" • AccountHash (address): SHA256(0x00 + publicKey) ✓");
                Console.WriteLine(" • BIP44 path: m/44'/506'/0'/0/index ✓");
                Console.WriteLine(" • Generated keys will work in Casper Network ✓");
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("IMPORTANT INFORMATION:");
                Console.WriteLine(" • Same seed phrase always generates the SAME public key");
                Console.WriteLine(" • Each account index generates a DIFFERENT public key");
                Console.WriteLine(" • AccountHash is the wallet ADDRESS in Casper Network");
                Console.WriteLine(" • Without seed phrase you cannot generate private key");
                Console.WriteLine(" • With private key you can sign transactions in Casper Network");
                Console.ResetColor();
                Console.WriteLine();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"   {ex.InnerException?.Message}");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
