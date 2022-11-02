using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataBase.Interfaces;

namespace Infrastructure.DataBase.Entities;

public class OperatingSystemType : ITypeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Code { get; set; }

    public List<Session> Sessions { get; set; }
}

