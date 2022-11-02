using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataBase.Interfaces;

namespace Infrastructure.DataBase.Entities;

public class App : ITypeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Code { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid AppTypeId { get; set; }

    public AppType AppType { get; set; }

    public List<Session> Sessions { get; set; }
}

