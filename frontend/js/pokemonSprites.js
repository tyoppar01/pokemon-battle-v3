// pokemonSprites.js - Pokemon Sprite URLs
export { PokemonSprites };

class PokemonSprites {
    static getSpriteUrl(pokemonName) {
        const sprites = {
            'Pikachu': 'js/images/pikachu.png',
            'Charmander': 'js/images/chamander.png',
            'Squirtle': 'js/images/squirtle.png',
            'Bulbasaur': 'js/images/bulbusaur.png',
            'Gengar': 'js/images/gengar.jpeg',
            'Mewtwo': 'js/images/mewtwo.png',
            'Snorlax': 'js/images/snorl.jpeg',
            'Aron': 'js/images/aron.jpg'
        };
        return sprites[pokemonName] || 'https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/0.png';
    }
}
