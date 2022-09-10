using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase.Entities;

public class Session
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid RefreshToken { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid UserId { get; set; }

    public User User { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid AppId { get; set; }

    public App App { get; set; }

    public string IP { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid OperationSystemTypeId { get; set; }

    public OperationSystemType OperationSystemType { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreationDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset LastUpdate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DateTimeOffset FinishDate { get; } = DateTimeOffset.UtcNow.AddDays(5);
}
