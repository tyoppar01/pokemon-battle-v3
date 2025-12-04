using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonBattle.Data.Entities {
    
    /**
        * Playable Pokemon Entity
        * Represents a Pokemon that can be chosen by users
    */

    [Table("PlayablePokemon")]
    public class PlayablePokemonEntity {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        
        public int BaseMaxHP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpeed { get; set; }
        
        [MaxLength(100)]
        public string NormalAttack { get; set; }
        
        [MaxLength(100)]
        public string SpecialAttack { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        public bool IsAvailable { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }

}
