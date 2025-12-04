/**
 * Pokemon Handler - Manages Pokemon display and viewing
 */
import { PokemonUI } from '../components/index.js';

export class PokemonHandler {
    constructor(app) {
        this.app = app;
    }

    /**
     * Show view Pokemon screen
     */
    async showViewPokemonScreen() {
        console.log('showViewPokemonScreen called');
        try {
            const allPokemon = await this.app.apiService.getAllPlayablePokemon();
            console.log('Pokemon loaded:', allPokemon);
            await PokemonUI.show(allPokemon);
            console.log('PokemonUI.show called');
        } catch (error) {
            console.error('Failed to load Pokemon:', error);
            await PokemonUI.show([]);
        }
    }
}
