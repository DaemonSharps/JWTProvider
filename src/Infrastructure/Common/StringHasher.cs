﻿using System;
using System.Security.Cryptography;
using Infrastructure.Extentions;

namespace Infrastructure.Common
{
    /// <summary>
    /// Помошник при хэщировании строк
    /// </summary>
    public class StringHasher
    {
        private readonly int _saltSize = 16;
        private readonly int _hashSize = 32;
        private readonly int _iterations = 10000;
        private string _stringToHash;

        public StringHasher(string stringToHash)
        {
            ArgumentNullException.ThrowIfNull(stringToHash);
            _stringToHash = stringToHash;
        }

        /// <summary>
        /// Хэшировать строку
        /// </summary>
        /// <param name="stringSalt">Строковое представление значения перед хэшированным паролем</param>
        /// <param name="pepper">Строка добавляемая к паролю перед хэшированием</param>
        /// <param name="iterations">Количество инераций для хэширования</param>
        /// <param name="hashSize">Размер получаемого хэша</param>
        /// <returns>Хэшированная строка</returns>
        public string Hash(string stringSalt, int iterations, int hashSize, string pepper = null)
        {
            ArgumentNullException.ThrowIfNull(stringSalt);

            var salt = $"{stringSalt}{new byte[8]}".ToByteArray();

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
        public string Hash(string salt, string pepper = null)
        {
            return Hash(salt, _iterations, _hashSize, pepper);
        }

        /// <summary>
        /// Хэшировать строку
        /// </summary>
        /// <returns>Хэшированная строка</returns>
        public string Hash()
        {
            var salt = new byte[_saltSize];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(salt);
            }

            var stringSalt = Convert.ToString(salt);
            return Hash(stringSalt);
        }
    }
}
