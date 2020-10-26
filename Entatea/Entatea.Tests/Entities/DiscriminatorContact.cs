using Entatea.Annotations;

namespace Entatea.Tests.Entities
{
    [Table("Discriminators")]
    public class DiscriminatorContact
    {
        [KeyType(KeyType.Identity)]
        [Column("DiscriminatorId")]
        public int ContactId { get; set; }

        [Discriminator(DiscriminatorType.Contact)]
        public DiscriminatorType DiscriminatorType { get; set; }

        public string Name { get; set; }
    }
}
