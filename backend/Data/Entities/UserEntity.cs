using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonBattle.Data.Entities {
    
    [Table("Users")]
    public class UserEntity {
        [Key]
        public string Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string Gender { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public ICollection<PokemonEntity> Pokemon { get; set; }
    }

}
