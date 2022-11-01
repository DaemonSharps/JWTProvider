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

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    [NotMapped]
    public string FullName
        => string.Join(' ', FirstName, MiddleName, LastName);

    [Required]
    public Password Password { get; set; }

    public List<Session> Sessions { get; set; }
}
