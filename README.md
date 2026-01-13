# 🔐 Casper Wallet Key Derivation - C# .NET

<div align="center">

![.NET Version](https://img.shields.io/badge/.NET-6.0%2B-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)
![Casper Network](https://img.shields.io/badge/Casper-Network-FF0000)
![BIP39](https://img.shields.io/badge/BIP39-✓-success)
![BIP44](https://img.shields.io/badge/BIP44-✓-success)
![Ed25519](https://img.shields.io/badge/Ed25519-✓-success)

**Complete implementation of Casper Wallet key generation with seed phrase in C# .NET**

[Documentation](#-documentation) •
[Installation](#-installation) •
[Examples](#-usage-examples) •
[Security](#-security)

</div>

---

## 📋 Table of Contents

- [About](#-about)
- [Features](#-features)
- [Requirements](#-requirements)
- [Installation](#-installation)
- [Usage Examples](#-usage-examples)
- [Project Structure](#-project-structure)
- [How It Works](#-how-it-works)
- [Security](#-security)
- [Standards & Compatibility](#-standards--compatibility)
- [References](#-references)
- [License](#-license)

---

## 🎯 About

Implementation of **HD Casper Wallet (Hierarchical Deterministic)** for **Casper Network** in C# .NET by Kamil Szymoniak.

### ✨ Key Features:

- ✅ **100% Casper Network compatible** - generated keys work immediately
- ✅ **BIP39** - generation and validation of 24-word seed phrases
- ✅ **BIP32** - hierarchical key derivation (HD Wallet)
- ✅ **BIP44** - standard derivation path (`m/44'/506'/0'/0/index`)
- ✅ **Ed25519** - native Casper Network cryptographic keys
- ✅ **AccountHash** - automatic wallet address generation
- ✅ **Multi-account** - unlimited accounts from single seed phrase

---

## 🚀 Features

| Feature | Description |
|---------|-------------|
| **Seed Phrase Generation** | 24-word mnemonic (256 bits entropy) |
| **BIP39 Validation** | Seed phrase checksum verification |
| **Key Derivation** | BIP44 path: `m/44'/506'/0'/0/index` |
| **Ed25519 KeyPair** | Public key + Private key (32 bytes each) |
| **AccountHash** | Wallet address `account-hash-...` |
| **Multi-account Support** | Multiple accounts from same seed phrase |

---

## 📦 Requirements

- **.NET 6.0+** (or newer)
- **NuGet Packages:**
  - `NBitcoin` - BIP39/BIP32/BIP44 implementation
  - `Chaos.NaCl.Standard` - Ed25519 cryptography

---

## 🔧 Installation

### 1. Clone the repository:

```bash
git clone https://github.com/midware/CasperWallet.git
cd CasperWallet
2. Install dependencies:
bash
dotnet add package NBitcoin
dotnet add package Chaos.NaCl.Standard
3. Build the project:
bash
dotnet build
4. Run:
bash
dotnet run
💻 Usage Examples
Example 1: Generate a New Wallet
csharp
using CasperWallet.Cryptography;
using CasperWallet.Models;

// Generate new 24-word seed phrase
string[] secretPhrase = Bip39Utils.GenerateSecretPhrase();

// Display seed phrase
Console.WriteLine("Seed phrase (save in a secure place!):");
for (int i = 0; i < secretPhrase.Length; i++)
{
    Console.WriteLine($"{i + 1:D2}. {secretPhrase[i]}");
}

// Generate first account (index 0)
var keyPair = KeyDerivation.DeriveKeyPair(secretPhrase, 0);

Console.WriteLine($"Public Key: {keyPair.PublicKey}");
Console.WriteLine($"Account Hash: {keyPair.AccountHash}");
Console.WriteLine($"Secret Key: {keyPair.SecretKey}");
Output:

text
Seed phrase:
01. abandon
02. ability
03. able
...
24. zoo

Public Key: 0178c6514e2c5c4611280a06c0495a553335a3b4303ab2f01f9e002ef2beedc7eb
Account Hash: account-hash-8edfcb6300127287c35398001ea9d81d500c159642f8fd1312cd74aff2951918
Secret Key: A3Y1kFE9dGjMQ7oO79wEfbFSnR4ft6Hs/n8miVL0vIs=
Example 2: Generate Multiple Accounts
csharp
// Generate 3 accounts from the same seed phrase
int accountCount = 3;

for (int i = 0; i < accountCount; i++)
{
    var keyPair = KeyDerivation.DeriveKeyPair(secretPhrase, i);
    
    Console.WriteLine($"\n=== ACCOUNT #{i} ===");
    Console.WriteLine($"BIP44 Path: m/44'/506'/0'/0/{i}");
    Console.WriteLine($"Public Key: {keyPair.PublicKey}");
    Console.WriteLine($"Account Hash: {keyPair.AccountHash}");
}
Example 3: Validate Existing Seed Phrase
csharp
string[] existingSeedPhrase = new[] 
{
    "abandon", "ability", "able", "about", "above", "absent",
    "absorb", "abstract", "absurd", "abuse", "access", "accident",
    "account", "accuse", "achieve", "acid", "acoustic", "acquire",
    "across", "act", "action", "actor", "actress", "actual"
};

// Validate
bool isValid = Bip39Utils.ValidateSecretPhrase(existingSeedPhrase);

if (isValid)
{
    Console.WriteLine("✓ Seed phrase is valid");
    
    // Generate keys
    var keyPair = KeyDerivation.DeriveKeyPair(existingSeedPhrase, 0);
    Console.WriteLine($"Account Hash: {keyPair.AccountHash}");
}
else
{
    Console.WriteLine("✗ Seed phrase is invalid");
}
Example 4: Create Complete Wallet
csharp
// Create wallet with multiple accounts
var wallet = new CasperWallet
{
    SecretPhrase = Bip39Utils.GenerateSecretPhrase(),
    CreatedAt = DateTime.UtcNow
};

// Add 5 accounts
for (int i = 0; i < 5; i++)
{
    var keyPair = KeyDerivation.DeriveKeyPair(wallet.SecretPhrase, i);
    
    wallet.Accounts.Add(new Account
    {
        Index = i,
        KeyPair = keyPair,
        DerivationPath = $"m/44'/506'/0'/0/{i}",
        AccountAddress = keyPair.AccountHash
    });
}

Console.WriteLine($"Wallet created: {wallet.CreatedAt}");
Console.WriteLine($"Number of accounts: {wallet.Accounts.Count}");

foreach (var account in wallet.Accounts)
{
    Console.WriteLine($"\nAccount #{account.Index}:");
    Console.WriteLine($"  Address: {account.AccountAddress}");
}
📁 Project Structure
text
CasperWallet/
├── Models/
│   └── Types.cs                    # Data models (KeyPair, Account, Wallet)
├── Cryptography/
│   ├── Bip39Utils.cs               # BIP39 seed phrase generation & validation
│   ├── Bip44Path.cs                # BIP44 derivation paths
│   ├── CasperUtils.cs              # Casper Network utilities (AccountHash)
│   └── KeyDerivation.cs            # Main key derivation logic
├── Program.cs                      # Usage example
├── CasperWallet.csproj
└── README.md

🔬 How It Works
Key Derivation Process:
text
┌─────────────────────────────────────────────────────────────┐
│ STEP 1: Seed Phrase (24 words)                             │
│ "abandon ability able about above absent absorb ..."       │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ STEP 2: PBKDF2-HMAC-SHA512                                 │
│ -  Password: seed phrase                                     │
│ -  Salt: "mnemonic"                                          │
│ -  Iterations: 2048                                          │
│ -  Output: 512-bit seed (64 bytes)                          │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ STEP 3: BIP32 Master Key                                    │
│ ExtKey.FromSeed() → Master Extended Key                     │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ STEP 4: BIP44 Derivation                                    │
│ m/44'/506'/0'/0/0                                           │
│  │   │   │  │  │                                            │
│  │   │   │  │  └─ Address index (0, 1, 2, ...)            │
│  │   │   │  └──── External chain (0 = receive)            │
│  │   │   └─────── Account index (0)                        │
│  │   └─────────── Casper coin type (506)                   │
│  └─────────────── BIP44 standard                           │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ STEP 5: Ed25519 KeyPair Generation                         │
│ -  Private Key: 32 bytes                                     │
│ -  Public Key: 32 bytes                                      │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ STEP 6: Casper Network Format                              │
│ -  Public Key: "01" + 64 HEX chars (66 total)              │
│ -  AccountHash: SHA256(0x00 + publicKey)                    │
│ -  Secret Key: BASE64 encoded                               │
└─────────────────────────────────────────────────────────────┘
🔒 Security
⚠️ CRITICAL WARNINGS:
❌ DON'T	✅ ALWAYS DO
❌ Don't hardcode seed phrase in code	✅ Generate seed phrase offline
❌ Don't log seed phrase to console	✅ Encrypt before storing in database
❌ Don't send seed phrase via email	✅ Use Azure Key Vault / HSM
❌ Don't commit seed phrase to Git	✅ Store in secure location
❌ Don't store in plaintext	✅ Regularly test recovery process
🛡️ Best Practices:
csharp
// ✅ CORRECT: Encrypt before storing
byte[] encryptedSeed = EncryptSeedPhrase(secretPhrase, userPassword);
database.SaveEncryptedSeed(userId, encryptedSeed);

// ✅ CORRECT: Clear memory after use
Array.Clear(secretPhrase, 0, secretPhrase.Length);
Array.Clear(privateKeyBytes, 0, privateKeyBytes.Length);

// ✅ CORRECT: Use secure key storage
var keyVault = new AzureKeyVaultClient();
await keyVault.SetSecretAsync("casper-wallet-seed", encryptedSeed);
📚 Standards & Compatibility
Implemented Standards:
Standard	Specification	Status
BIP39	Mnemonic Code	✅ 100%
BIP32	Hierarchical Deterministic Wallets	✅ 100%
BIP44	Multi-Account Hierarchy	✅ 100%
SLIP-44	Coin Type 506	✅ Casper
Ed25519	RFC 8410	✅ 100%
Casper Network Compatibility:
✅ Ed25519 Public Key Format - prefix 01 + 32 bytes

✅ AccountHash Generation - SHA256(0x00 + publicKey)

✅ BIP44 Coin Type - 506 (official coin type for Casper)

✅ Deterministic Generation - same seed = same keys

🔗 References
Documentation:
Casper Network Docs: https://docs.casper.network

Casper Accounts & Keys: https://docs.casper.network/concepts/accounts-and-keys

BIP39 Standard: https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki

BIP44 Standard: https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki

SLIP-44 Coin Types: https://github.com/satoshilabs/slips/blob/master/slip-0044.md

Ed25519 Specification: https://datatracker.ietf.org/doc/html/rfc8410

Libraries:
NBitcoin: https://github.com/MetacoSA/NBitcoin

Chaos.NaCl: https://github.com/CodesInChaos/Chaos.NaCl

Casper JavaScript SDK: https://github.com/make-software/casper-wallet

🧪 Testing
Verify Key Correctness:
csharp
// Test 1: Determinism
var keyPair1 = KeyDerivation.DeriveKeyPair(seedPhrase, 0);
var keyPair2 = KeyDerivation.DeriveKeyPair(seedPhrase, 0);

Assert.Equal(keyPair1.PublicKey, keyPair2.PublicKey);
Assert.Equal(keyPair1.SecretKey, keyPair2.SecretKey);

// Test 2: Key Lengths
Assert.Equal(66, keyPair1.PublicKey.Length);  // "01" + 64 chars
Assert.Equal(64, keyPair1.PublicKeyRaw.Length);  // 32 bytes = 64 HEX
Assert.True(keyPair1.AccountHash.StartsWith("account-hash-"));

// Test 3: Ed25519 Prefix
Assert.StartsWith("01", keyPair1.PublicKey);
🤝 Contributing
Contributions are welcome! If you'd like to help:

Fork the repository

Create a branch

Commit your changes

Push to the branch

Open a Pull Request

📝 License
This project is licensed under the MIT License.

👤 Author: Kamil Szymoniak
Created for the Casper Network community

GitHub: @midware

LinkedIn: https://pl.linkedin.com/in/kamil-szymoniak

🙏 Acknowledgments
Casper Network Team - for documentation and SDK

NBitcoin Contributors - for excellent BIP39/BIP32/BIP44 library

Chaos.NaCl - for Ed25519 implementation

⭐ If this project helped you, leave a star! ⭐

Made with ❤️ for Casper Network Community
