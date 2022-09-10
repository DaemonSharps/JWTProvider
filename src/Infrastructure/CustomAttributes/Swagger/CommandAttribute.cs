namespace Infrastructure.CustomAttributes.Swagger
{
    /// <summary>
    /// Атрибут для методов "Команд"
    /// </summary>
    public class CommandAttribute : CQRSAttribute
    {
        public CommandAttribute()
        {
            Name = Constants.Swagger.SwaggerTagNames.Commands;
        }
    }

    public class CommandAttributeFilter : CQRSAttributeFilter<CommandAttribute> { }
}
