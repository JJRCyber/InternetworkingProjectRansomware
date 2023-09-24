using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UTSRansomware
{
    internal class Decryptor
    {

        private byte[] key;
        private byte[] iv;

        public Decryptor(byte[] key, byte[] iv) 
        {
            this.key = key;
            this.iv = iv;
        }

        public void DecryptDirectory(string parentDirectory)
        {
            string[] filePaths = Directory.GetFiles(parentDirectory);
            string[] directoryPaths = Directory.GetDirectories(parentDirectory);

            foreach (string directoryPath in directoryPaths)
            {
                DecryptDirectory(directoryPath);
            }


            foreach (var filePath in filePaths)
            {
                DecryptFile(filePath);
            }
        }

        public void DecryptFile(string encryptedFilePath)
        {
            // Instantiate Aes and configure its settings
            using (Aes aesAlg = Aes.Create())
            {
                string tempFilePath = encryptedFilePath + ".temp";
                aesAlg.Mode = CipherMode.CBC; // CBC mode
                aesAlg.KeySize = 256;         // 256-bit key
                aesAlg.BlockSize = 128;       // 128-bit block size
                aesAlg.IV = iv;               // Initialization Vector
                aesAlg.Key = key;             // Encryption Key

                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Open file streams to read the encrypted file and output the decrypted file
                using (FileStream fsEncrypted = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
                using (FileStream fsDecrypted = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                using (CryptoStream cs = new CryptoStream(fsEncrypted, decryptor, CryptoStreamMode.Read))
                {
                    // Read encrypted file and write decrypted bytes to output file
                    int data;
                    while ((data = cs.ReadByte()) != -1)
                    {
                        fsDecrypted.WriteByte((byte)data);
                    }
                }
                File.Delete(encryptedFilePath);
                File.Move(tempFilePath, encryptedFilePath);
                Console.WriteLine("Decrypted: " + encryptedFilePath);
            }
        }
    }
}
