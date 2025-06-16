using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManager.API.Models;

public partial class Task
{
    [Key]
    public int TaskId { get; set; }

    public int EmployeeId { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateOnly? DueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Tasks")]
    public virtual Employee Employee { get; set; } = null!;
}
