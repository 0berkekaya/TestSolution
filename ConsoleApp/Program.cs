using CoreLibrary;
class ConsoleAppProgram
{
    static void Main()
    {

        TransactionInformation response2 = TryCatch.Run(() =>
        {
            var berke = 24;
            string helloWorld = string.Empty;
            berke.ToString(string.Empty);
            helloWorld = "     Hello World       ".Trim().PadLeft(2, '0');

            throw new CustomExceptions.BerkeExcpt("Program.cs'de Satır 14'te hata verdim.");
        }, new TryCatchConfiguration() { LogErrors = false, ThrowOnException = false, CustomExceptionHandling = true });




        Console.ReadLine();
    }
}

