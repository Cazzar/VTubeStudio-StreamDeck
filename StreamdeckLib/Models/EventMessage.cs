using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace StreamDeckLib.Models
{
    public record EventMessage
    {
        public virtual string Event { get; init; } = default;
    }
}
