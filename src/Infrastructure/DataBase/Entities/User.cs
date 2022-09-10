using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase.Entities;

public class User : Infrastructure.Entities.User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Password Password { get; set; }

    public List<Session> Sessions { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreationDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset LastUpdate { get; set; }

    public DateTimeOffset FinishDate { get; set; }
}
