namespace CoreLibrary
{
    public static class TryCatch
    {
        /// <summary>
        /// Belirtilen bir işlevi çalıştırır ve işlem bilgisini döndürür.
        /// </summary>
        /// <typeparam name="T">Dönüş değeri türü.</typeparam>
        /// <param name="methodFuncT">Çalıştırılacak işlev.</param>
        /// <param name="tryCatchConfiguration">Try-Catch ayarları (isteğe bağlı).</param>
        /// <returns>İşlem bilgisi.</returns>
        public static TransactionInformation<T> Run<T>(Func<T> methodFuncT, TryCatchConfiguration? tryCatchConfiguration = null)
        {
            // Try-Catch ayarlarını varsayılan değerle başlat
            tryCatchConfiguration ??= new TryCatchConfiguration();

            // İşlem bilgisi oluştur
            TransactionInformation<T> process = new TransactionInformation<T>();

            try
            {
                // İşlevi çağır ve dönüş değerini ayarla
                process.Result = methodFuncT();
                // İşlem başarılı olduğunu işaretle
                process.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                // Hata durumunda istisna bilgisini ve işlem durumunu ayarla
                process.Error = ExceptionDetailsBuilder(ex);
                process.IsSuccessful = false;

                // Hata günlüğünü kaydetme ayarı aktifse
                if (tryCatchConfiguration.LogErrors)
                {
                    // Hata günlüğünü kaydetme işlemleri burada gerçekleştirilebilir
                }

                if (tryCatchConfiguration.CustomExceptionHandling) CustomExceptionHandler(ex);
            }
            // İşlem bilgisini döndür
            return process;
        }

        /// <summary>
        /// Belirtilen bir işlevi çalıştırır ve işlem bilgisini döndürür.
        /// </summary>
        /// <typeparam name="T">Dönüş değeri türü.</typeparam>
        /// <param name="methodRequest">Çalıştırılacak işlev.</param>
        /// <param name="tryCatchConfiguration">Try-Catch ayarları (isteğe bağlı).</param>
        /// <returns>İşlem bilgisi.</returns>
        public static TransactionInformation<T> Run<T>(Func<TransactionInformation<T>> methodRequest, TryCatchConfiguration? tryCatchConfiguration = null)
        {
            // Try-Catch ayarlarını varsayılan değerle başlat
            tryCatchConfiguration ??= new TryCatchConfiguration();

            // İşlem bilgisi nesnesini tanımla ve başlangıçta null olarak ayarla
            TransactionInformation<T>? request = null;

            try
            {
                // İşlevi çağır ve işlem bilgisini al
                request = methodRequest();
                // İşlem başarılı olduğunu işaretle
                request.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                // İşlem bilgisi nesnesi henüz oluşturulmamışsa yeni bir nesne oluştur
                request ??= new TransactionInformation<T>();

                // Hata durumunda istisna bilgisini ve işlem durumunu ayarla
                request.Error = ExceptionDetailsBuilder(ex);
                request.IsSuccessful = false;

                // Hata günlüğünü kaydetme ayarı aktifse
                if (tryCatchConfiguration.LogErrors)
                {
                    // Hata günlüğünü kaydetme işlemleri burada gerçekleştirilebilir
                }
                if (tryCatchConfiguration.CustomExceptionHandling) CustomExceptionHandler(ex);

                // Throw ayarı aktifse istisnayı tekrar fırlat
                if (tryCatchConfiguration.ThrowOnException) throw;
            }

            // İşlem bilgisini döndür
            return request;
        }

        /// <summary>
        /// Belirtilen bir işlemi gerçekleştirir ve işlem bilgisini döndürür.
        /// </summary>
        /// <param name="methodAction">Gerçekleştirilecek işlem.</param>
        /// <param name="tryCatchConfiguration">Try-Catch ayarları (isteğe bağlı).</param>
        /// <returns>İşlem bilgisi.</returns>
        public static TransactionInformation Run(Action methodAction, TryCatchConfiguration? tryCatchConfiguration = null)
        {
            // Try-Catch ayarlarını varsayılan değerle başlat
            tryCatchConfiguration ??= new TryCatchConfiguration();

            // İşlem bilgisi nesnesini oluştur
            TransactionInformation request = new TransactionInformation();

            try
            {
                // İşlemi gerçekleştir
                methodAction();
                // İşlem başarılı olduğunu işaretle
                request.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                // Hata durumunda işlem bilgisini ve durumunu ayarla
                request.IsSuccessful = false;
                request.Error = ExceptionDetailsBuilder(ex);

                // Hata günlüğünü kaydetme ayarı aktifse
                if (tryCatchConfiguration.LogErrors)
                {
                    // Hata günlüğünü kaydetme işlemleri burada gerçekleştirilebilir
                }

                if (tryCatchConfiguration.CustomExceptionHandling) CustomExceptionHandler(ex);

                // Throw ayarı aktifse istisnayı tekrar fırlat
                if (tryCatchConfiguration.ThrowOnException) throw;
            }

            // İşlem bilgisini döndür
            return request;
        }

        /// <summary>
        /// Bir istisna nesnesinden ayrıntılı hata bilgisini oluşturur.
        /// </summary>
        /// <param name="exception">İstisna nesnesi.</param>
        /// <returns>Oluşturulan özel istisna nesnesi.</returns>
        private static TransactionError ExceptionDetailsBuilder(Exception exception)
        {
            // İstisna mesajı ve stack trace bilgisini al
            string exceptionMessage = exception.Message;
            string? stackTrace = exception.StackTrace;

            // Oluşturulacak özel istisna nesnesi
            TransactionError exceptionDetail = new TransactionError
            {
                Message = exceptionMessage,
                Details = new List<ExceptionDetail>()
            };

            // Stack trace varsa işle
            if (!string.IsNullOrWhiteSpace(stackTrace))
            {
                // Hata bilgilerini saklamak için bir sözlük kullan
                Dictionary<int, HashSet<string>> errorInformations = new Dictionary<int, HashSet<string>>();

                // Stack trace satırlarını işle
                string[] lines = stackTrace.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    int lineIndex = trimmedLine.IndexOf("line");
                    if (lineIndex != -1)
                    {
                        // Hata sınıf adını ve satır numarasını al
                        string errorClassName = trimmedLine.Substring(0, trimmedLine.IndexOf(" in ")).Trim(' ', '.', ':').Replace("at ", "");
                        int lineNumberStart = lineIndex + "line".Length;
                        int lineNumberEnd = trimmedLine.IndexOf(' ', lineNumberStart);
                        if (lineNumberEnd == -1)
                            lineNumberEnd = trimmedLine.Length;

                        int errorLineNumber = int.Parse(trimmedLine.Substring(lineNumberStart + 1));

                        // Hata sınıf adını ve satır numarasını sakla
                        if (!errorInformations.ContainsKey(errorLineNumber))
                            errorInformations[errorLineNumber] = new HashSet<string>();

                        errorInformations[errorLineNumber].Add(errorClassName);
                    }
                }

                // Hata bilgilerini özel istisna detaylarına ekle
                foreach (var errorDetail in errorInformations)
                {
                    exceptionDetail.Details.Add(new ExceptionDetail
                    {
                        Row = errorDetail.Key,
                        Class = string.Join(", ", errorDetail.Value)
                    });
                }
            }

            // Oluşturulan özel istisna nesnesini döndür
            return exceptionDetail;
        }


        /// <summary>
        /// Bu metot aldığı hata parametresi ile switch-case'den geçerek ilgili hataya özel işlemlerini yapar.
        /// </summary>
        /// <param name="ex"></param>
        private static void CustomExceptionHandler(Exception ex)
        {
            //Console.WriteLine("Bu mesaj custom try-catch metotları için özel hata yakalayıcı kullanılarak yakalanmıştır.");

            switch (ex)
            {
                case AggregateException:
                    Console.WriteLine("Bu hata sistem tarafından gerçekleştirilmiştir.");
                    break;

                case CustomExceptions.BerkeExcpt:
                    Console.WriteLine("Burada istediğimiz işlemleri yapabiliriz.");
                    break;


                default:
                    // Default Yapılandırma.
                    Console.WriteLine("Bu hata türü için özel bir ayar yapılandırılmamış.");
                    break;
            }
        }
    }
    /// <summary>
    /// Try-Catch bloklarındaki davranışı yapılandırmak için kullanılan sınıf.
    /// </summary>
    public class TryCatchConfiguration
    {
        /// <summary>
        /// Bir istisna oluştuğunda, try-catch bloğundaki kodun istisnayı tekrar fırlatıp fırlatmayacağını belirten bir değer.
        /// Varsayılan değer: false.
        /// </summary>
        public bool ThrowOnException { get; set; } = false;

        /// <summary>
        /// Hata günlüğüne kaydetme işleminin yapıp yapılmayacağını belirten bir değer.
        /// Varsayılan değer: false.
        /// </summary>
        public bool LogErrors { get; set; } = false;

        /// <summary>
        /// Özel olarak hatalın kendilerine has işlemlerin yapılması için bir değer.
        /// </summary>
        public bool CustomExceptionHandling { get; set; } = false;
    }
}
