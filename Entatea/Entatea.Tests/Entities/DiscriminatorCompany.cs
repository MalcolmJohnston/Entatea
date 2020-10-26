using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("Discriminators")]
    public class DiscriminatorCompany
    {
        [KeyType(KeyType.Identity)]
        [Column("DiscriminatorId")]
        public int CompanyId { get; set; }

        [Discriminator(DiscriminatorType.Company)]
        public DiscriminatorType DiscriminatorType { get; set; }

        public string Name { get; set; }
    }
}
