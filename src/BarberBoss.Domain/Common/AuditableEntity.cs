namespace BarberBoss.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public void Touch() => UpdatedAt = DateTime.UtcNow;
}
