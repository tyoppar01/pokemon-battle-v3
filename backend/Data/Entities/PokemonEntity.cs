using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonBattle.Data.Entities {
    
    [Table("Pokemon")]
    public class PokemonEntity {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        
        public int MaxHitPoint { get; set; }
        public int CurrentHitPoint { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Level { get; set; }
        public int Speed { get; set; }
        
        [MaxLength(100)]
        public string SpecialSkill { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }

}
