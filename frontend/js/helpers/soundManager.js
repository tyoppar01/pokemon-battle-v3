/**
 * Sound Manager - Pokemon Style Audio Effects
 */
export class SoundManager {
    constructor() {
        this.sounds = {};
        this.enabled = true;
        this.volume = 0.3;
        this.initializeSounds();
    }

    /**
     * Initialize sound effects using Web Audio API
     */
    initializeSounds() {
        // Create AudioContext
        this.audioContext = null;
        
        // Initialize on first user interaction (required by browsers)
        document.addEventListener('click', () => {
            if (!this.audioContext) {
                this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
            }
        }, { once: true });
    }

    /**
     * Generate a beep sound with Pokemon-style characteristics
     * @param {number} frequency - Frequency in Hz
     * @param {number} duration - Duration in seconds
     * @param {string} type - Waveform type
     */
    playBeep(frequency = 800, duration = 0.1, type = 'square') {
        if (!this.enabled || !this.audioContext) return;

        try {
            const oscillator = this.audioContext.createOscillator();
            const gainNode = this.audioContext.createGain();

            oscillator.type = type;
            oscillator.frequency.setValueAtTime(frequency, this.audioContext.currentTime);

            // Pokemon-style envelope (quick attack, quick decay)
            gainNode.gain.setValueAtTime(this.volume, this.audioContext.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, this.audioContext.currentTime + duration);

            oscillator.connect(gainNode);
            gainNode.connect(this.audioContext.destination);

            oscillator.start(this.audioContext.currentTime);
            oscillator.stop(this.audioContext.currentTime + duration);
        } catch (error) {
            console.error('Error playing sound:', error);
        }
    }

    /**
     * Play button click sound (menu selection)
     */
    playButtonClick() {
        this.playBeep(1200, 0.05, 'square');
    }

    /**
     * Play hover sound (menu hover)
     */
    playHover() {
        this.playBeep(900, 0.03, 'square');
    }

    /**
     * Play confirm sound (action confirmed)
     */
    playConfirm() {
        if (!this.enabled || !this.audioContext) return;
        
        // Two-tone confirm sound
        this.playBeep(800, 0.08, 'square');
        setTimeout(() => this.playBeep(1000, 0.08, 'square'), 80);
    }

    /**
     * Play cancel/back sound
     */
    playCancel() {
        this.playBeep(600, 0.12, 'square');
    }

    /**
     * Play selection change sound (for dropdowns)
     */
    playSelect() {
        this.playBeep(1000, 0.05, 'square');
    }

    /**
     * Play error sound
     */
    playError() {
        if (!this.enabled || !this.audioContext) return;
        
        // Low buzz for error
        this.playBeep(200, 0.15, 'sawtooth');
    }

    /**
     * Play battle start sound
     */
    playBattleStart() {
        if (!this.enabled || !this.audioContext) return;
        
        // Rising tone
        const oscillator = this.audioContext.createOscillator();
        const gainNode = this.audioContext.createGain();

        oscillator.type = 'square';
        oscillator.frequency.setValueAtTime(400, this.audioContext.currentTime);
        oscillator.frequency.exponentialRampToValueAtTime(800, this.audioContext.currentTime + 0.3);

        gainNode.gain.setValueAtTime(this.volume, this.audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, this.audioContext.currentTime + 0.3);

        oscillator.connect(gainNode);
        gainNode.connect(this.audioContext.destination);

        oscillator.start(this.audioContext.currentTime);
        oscillator.stop(this.audioContext.currentTime + 0.3);
    }

    /**
     * Play attack sound
     */
    playAttack() {
        if (!this.enabled || !this.audioContext) return;
        
        // Sharp attack sound
        this.playBeep(1500, 0.08, 'sawtooth');
        setTimeout(() => this.playBeep(1200, 0.08, 'sawtooth'), 50);
    }

    /**
     * Play damage sound
     */
    playDamage() {
        if (!this.enabled || !this.audioContext) return;
        
        // Descending tone
        const oscillator = this.audioContext.createOscillator();
        const gainNode = this.audioContext.createGain();

        oscillator.type = 'sawtooth';
        oscillator.frequency.setValueAtTime(800, this.audioContext.currentTime);
        oscillator.frequency.exponentialRampToValueAtTime(200, this.audioContext.currentTime + 0.15);

        gainNode.gain.setValueAtTime(this.volume * 0.7, this.audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, this.audioContext.currentTime + 0.15);

        oscillator.connect(gainNode);
        gainNode.connect(this.audioContext.destination);

        oscillator.start(this.audioContext.currentTime);
        oscillator.stop(this.audioContext.currentTime + 0.15);
    }

    /**
     * Toggle sound on/off
     */
    toggle() {
        this.enabled = !this.enabled;
        return this.enabled;
    }

    /**
     * Set volume (0.0 to 1.0)
     */
    setVolume(volume) {
        this.volume = Math.max(0, Math.min(1, volume));
    }
}

// Export singleton instance
export const soundManager = new SoundManager();
