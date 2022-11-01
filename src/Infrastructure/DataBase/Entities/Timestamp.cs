using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase.Entities;

public class Timestamp
{
    public DateTimeOffset CreationDate { get; set; }

    public DateTimeOffset LastUpdate { get; set; }

    public DateTimeOffset? FinishDate { get; set; }
}

