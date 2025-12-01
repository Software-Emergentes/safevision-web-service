using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeVisionPlatform.IAM.Domain.Model.Entities
{
    public class MfaCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Propiedad de navegación
        public virtual Aggregates.User? User { get; set; }
    }
}

