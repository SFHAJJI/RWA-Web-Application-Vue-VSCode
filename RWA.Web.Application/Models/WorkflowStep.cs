using System;
using System.ComponentModel.DataAnnotations;

namespace RWA.Web.Application.Models
{
    public class WorkflowStep
    {
        [Key]
        public int Id { get; set; }

    // Optional UserId removed for global workflow support. Keep for backward compatibility but allow null.
    public string? UserId { get; set; }

    [Required]
    public string StepName { get; set; }

    [Required]
    public string Status { get; set; }

    public string DataPayload { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
