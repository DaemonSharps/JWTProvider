namespace Infrastructure.CustomAttributes.Swagger
{
    /// <summary>
    /// Атрибут для методов "Запросов"
    /// </summary>
    public class QuerryAttribute : CQRSAttribute
    {
        public QuerryAttribute()
        {
            Name = Constants.SwaggerTagNames.Queries;
        }
    }

    public class QuerryAttributeFilter : CQRSAttributeFilter<QuerryAttribute> { }
}
