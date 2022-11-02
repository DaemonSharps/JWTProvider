using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase.Entities;

public class Session : Timestamp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid RefreshToken { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid UserId { get; set; }

    public User User { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid? AppId { get; set; }

    public App App { get; set; }

    public string IP { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid? OperatingSystemTypeId { get; set; }

    public OperatingSystemType OperatingSystemType { get; set; }
}
