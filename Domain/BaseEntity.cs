using System;

namespace GerenciadorDeLivraria.Domain
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

        public void TouchUpdated() => UpdatedAt = DateTime.UtcNow;
    }
}
