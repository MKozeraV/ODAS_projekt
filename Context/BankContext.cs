using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using ProjektODASAPI.Data;
using System.Text;

namespace ProjektODASAPI.Context
{
    public class BankContext : DbContext
    {
        public byte[] _encryptionKey = System.IO.File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "\\key");
        public byte[] _encryptionIV = System.IO.File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "\\iv");
        //public byte[] _encryptionKey = AesProvider.GenerateKey(AesKeySize.AES256Bits).Key;
        //public byte[] _encryptionIV = AesProvider.GenerateKey(AesKeySize.AES256Bits).IV;
        //private readonly byte[] _encryptionIV = AesProvider.GenerateKey(AesKeySize.AES256Bits).IV;
        //private readonly byte[] _encryptionKey = AesProvider.GenerateKey(AesKeySize.AES256Bits).Key;
        //private readonly byte[] _encryptionIV = AesProvider.GenerateKey(AesKeySize.AES256Bits).IV;
        //private readonly byte[] _encryptionKey = Encoding.ASCII.GetBytes("I00Tsbx+XIdUSGmYaIy2roSC9oH4M0bmg1E6cIJ3MgM=");
        //private readonly byte[] _encryptionIV = Encoding.ASCII.GetBytes("aHLrwICC8IseuzT8nHMJYA==");
        private readonly IEncryptionProvider _provider;

        public BankContext(DbContextOptions options) : base(options) 
        {
            this._provider = new AesProvider(this._encryptionKey, this._encryptionIV);
        }
        public DbSet<UserData> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(this._provider);
        }
    }
}
