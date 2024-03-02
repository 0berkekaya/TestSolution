namespace CoreLibrary
{
    public class CustomExceptions
    {
        /// <summary>
        /// Buradaki hatalar alındığı anda içerisindeki işlemler gerçekleştirilir.
        /// </summary>
        public class BerkeExcpt : CustomException
        {
            public BerkeExcpt(string? message = null) : base(message) { }
        }

        public class SemihExcpt : CustomException
        {

        }

        public class TalhaExcpt : CustomException
        {

        }

    }
    public class CustomException : Exception
    {
        private string DefaultMessage = "Bu hata mesajı otomatik olarak üretilmiştir.";
        public CustomException() { }
        public CustomException(string? message = null) : base(message)
        {
            if (message.IsNullOrEmpty().NotThis()) workingMethod(message);
            else Console.WriteLine(DefaultMessage);
        }

        private void workingMethod(string message)
        {
            Console.WriteLine(message);
        }
    }
}
