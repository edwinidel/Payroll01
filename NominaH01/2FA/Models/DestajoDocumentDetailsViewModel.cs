using _2FA.Data.Entities;

namespace _2FA.Models
{
    public class DestajoDocumentDetailsViewModel
    {
        public DestajoDocumentEntity Document { get; set; } = new DestajoDocumentEntity();
        public DestajoProductionEntity NewProduction { get; set; } = new DestajoProductionEntity();
        public IEnumerable<DestajoProductionEntity> ExistingProductions { get; set; } = Array.Empty<DestajoProductionEntity>();
    }
}
