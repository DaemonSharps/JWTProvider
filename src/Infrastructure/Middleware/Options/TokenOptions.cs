namespace Infrastructure.Middleware
{
    public class TokenOptions
    {
        /// <summary>
        /// Секция настроек
        /// </summary>
        public const string Section = "Token";

        /// <summary>
        /// Ключ для генерации токена доступа
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Униальный ключ издателя токена
        /// </summary>
        public string Issuer { get; set; }
    }
}
