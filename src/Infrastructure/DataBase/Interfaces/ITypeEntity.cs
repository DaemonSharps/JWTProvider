using System;
namespace Infrastructure.DataBase.Interfaces;

public interface ITypeEntity
{
    /// <summary>
    /// Код типа объекта
    /// </summary>
    public string Code { get; set; }
}

