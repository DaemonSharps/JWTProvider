using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase.Entities;

public class User : Timestamp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    /// <summary> Имя </summary>
    public string FirstName { get; set; }

    /// <summary> Фамилия </summary>
    public string LastName { get; set; }

    /// <summary> Отчество </summary>
    public string Patronymic { get; set; }

    [NotMapped]
    public string FullName
        => string.Join(' ', FirstName, LastName, Patronymic);

    [Required]
    public Password Password { get; set; }

    public List<Session> Sessions { get; set; }
}
