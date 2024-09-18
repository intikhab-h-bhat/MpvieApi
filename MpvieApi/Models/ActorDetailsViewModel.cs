using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MpvieApi.Models
{
    public class ActorDetailsViewModel:ActorViewModel
    {
        public string[] Movies { get; set; }
    }
}
