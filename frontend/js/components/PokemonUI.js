/**
 * UI Component - Pokemon Viewing
 */
export class PokemonUI {
    /**
     * Show view Pokemon screen
     * @param {Array} pokemonList 
     */
    static async show(pokemonList) {
        document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
        document.getElementById('viewPokemonScreen').classList.add('active');
        
        const pokemonListEl = document.getElementById('pokemonList');
        
        if (pokemonList.length === 0) {
            pokemonListEl.innerHTML = '<p style="color: #666; text-align: center; padding: 40px;">No Pokemon available.</p>';
        } else {
            pokemonListEl.innerHTML = pokemonList.map((pokemon, index) => `
                <div class="pokemon-card">
                    <div class="pokemon-header">${index + 1}. ${pokemon.name}</div>
                    <div class="pokemon-type">Type: ${pokemon.type}</div>
                    <div class="pokemon-stats">
                        <div class="stat-row">HP: ${pokemon.baseMaxHP}</div>
                        <div class="stat-row">Attack: ${pokemon.baseAttack}</div>
                        <div class="stat-row">Defense: ${pokemon.baseDefense}</div>
                        <div class="stat-row">Speed: ${pokemon.baseSpeed}</div>
                    </div>
                    <div class="pokemon-attacks">
                        <div class="attack-label">Attacks:</div>
                        <div class="attack-item">• ${pokemon.normalAttack}</div>
                        <div class="attack-item special">• ${pokemon.specialAttack}</div>
                    </div>
                </div>
            `).join('');
        }
    }
}
