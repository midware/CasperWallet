namespace CasperWallet.Models
{
    /// <summary>
    /// Para kluczy kryptograficznych (public + private) - CASPER NETWORK COMPATIBLE
    /// </summary>
    public class KeyPair
    {
        /// <summary>
        /// Klucz publiczny w formacie HEX z Casper Network prefix'em
        /// Format: "01" + 64 znaki (66 znaków total dla ED25519)
        /// "01" = ED25519 algorithm tag
        /// Używany do generowania adresu portfela (AccountHash)
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Klucz prywatny w formacie BASE64 (raw 32 bajty)
        /// Używany do podpisywania transakcji
        /// ⚠️ NIGDY nie dziel się tym kluczem!
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Klucz publiczny w formacie RAW HEX (bez prefix'u)
        /// Używany wewnętrznie do obliczania AccountHash
        /// </summary>
        public string PublicKeyRaw { get; set; }

        /// <summary>
        /// Account hash dla Casper Network (32 bajty SHA256)
        /// Format: "account-hash-" + hex string
        /// To jest ADRES portfela w Casper Network
        /// </summary>
        public string AccountHash { get; set; }
    }

    /// <summary>
    /// Pojedyncze konto w portfelu (jedno z wielu możliwych kont z tego samego seed phrase)
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Indeks konta (0, 1, 2, ...)
        /// Każdy indeks generuje inny adres z tego samego seed phrase
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Para kluczy dla tego konta
        /// </summary>
        public KeyPair KeyPair { get; set; }

        /// <summary>
        /// BIP44 ścieżka derywacji
        /// np. m/44'/506'/0'/0/0
        /// </summary>
        public string DerivationPath { get; set; }

        /// <summary>
        /// Publiczny klucz w formacie Casper Network (z prefix'em "01")
        /// </summary>
        public string PublicKey => KeyPair?.PublicKey;

        /// <summary>
        /// Adres portfela (AccountHash) w Casper Network
        /// Format: "account-hash-[32-byte-hex]"
        /// </summary>
        public string AccountAddress => KeyPair?.AccountHash;
    }

    /// <summary>
    /// Reprezentacja całego portfela Casper
    /// </summary>
    public class CasperWallet
    {
        /// <summary>
        /// Seed phrase (24 słowa)
        /// ⚠️ NIGDY nie przechowuj tego w plaintext w bazie danych!
        /// </summary>
        public string[] SecretPhrase { get; set; }

        /// <summary>
        /// Lista kont derive'owanych z tego seed phrase
        /// </summary>
        public List<Account> Accounts { get; set; } = new List<Account>();

        /// <summary>
        /// Timestamp utworzenia portfela
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
