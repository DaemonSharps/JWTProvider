using System;
using Infrastructure.Extentions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Common
{
    /// <summary>
    /// Помошник при хэщировании строк
    /// </summary>
    public class StringHasher
    {
        private readonly int _saltSize = 16;
        private readonly int _hashSize = 20;
        private readonly int _iterations = 10000;
        private string _stringToHash;

        public StringHasher(string value)
        {
            _stringToHash = value;
        }

        /// <summary>
        /// Хэшировать строку
        /// </summary>
        /// <param name="stringSalt">Строковое представление значения перед хэшированным паролем</param>
        /// <param name="pepper">Строка добавляемая к паролю перед хэшированием</param>
        /// <param name="iterations">Количество инераций для хэширования</param>
        /// <param name="hashSize">Размер получаемого хэша</param>
        /// <returns>Хэшированная строка</returns>
        public string Hash(string stringSalt, string pepper, int iterations, int hashSize)
        {
            if (string.IsNullOrEmpty(stringSalt))
                throw new ArgumentNullException(stringSalt);

            var salt = stringSalt.ToByteArray();

            if (!string.IsNullOrEmpty(pepper))
                _stringToHash += pepper;

            var hash = new Rfc2898DeriveBytes(_stringToHash, salt, iterations);
            var bytes = hash.GetBytes(hashSize);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Хэшировать строку
        /// </summary>
        /// <param name="salt">Строковое представление значения перед хэшированным паролем</param>
        /// <param name="pepper">Строка добавляемая к паролю перед хэшированием</param>
        /// <returns>Хэшированная строка</returns>
        public string Hash(string salt, string pepper)
        {
            return Hash(salt, pepper, _iterations, _hashSize);
        }

        /// <summary>
        /// Хэшировать строку
        /// </summary>
        /// <param name="salt">Строковое представление значения перед хэшированным паролем</param>
        /// <returns>Хэшированная строка</returns>
        public string Hash(string salt)
        {
            return Hash(salt, null);
        }

        /// <summary>
        /// Хэшировать строку
        /// </summary>
        /// <returns>Хэшированная строка</returns>
        public string Hash()
        {
            byte[] salt = new byte[_saltSize];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            var stringSalt = Convert.ToString(salt);
            return Hash(stringSalt);
        }
    }
}
