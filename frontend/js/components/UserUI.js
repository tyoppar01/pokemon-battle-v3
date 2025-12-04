/**
 * UI Component - User Management Screens
 */
export class UserUI {
    /**
     * Show player selection screen
     */
    static showSelectPlayers() {
        document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
        document.getElementById('selectPlayersScreen').classList.add('active');
        
        // Reset selections and clear previews
        document.getElementById('player1Select').value = '';
        document.getElementById('player2Select').value = '';
        document.getElementById('player1Preview').innerHTML = '<p style="color: #666;">Select a trainer to see their team</p>';
        document.getElementById('player2Preview').innerHTML = '<p style="color: #666;">Select a trainer to see their team</p>';
    }

    /**
     * Show create user screen
     */
    static showCreateUser() {
        document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
        document.getElementById('createUserScreen').classList.add('active');
        // Reset form
        document.getElementById('newTrainerName').value = '';
        document.getElementById('newTrainerGender').value = 'Male';
        document.querySelectorAll('.pokemon-option').forEach(opt => opt.classList.remove('selected'));
        document.getElementById('teamCounter').textContent = '0';
        document.getElementById('teamDisplay').innerHTML = '';
    }

    /**
     * Show view users screen
     * @param {Array} users 
     */
    static showViewUsers(users) {
        document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
        document.getElementById('viewUsersScreen').classList.add('active');
        
        const usersList = document.getElementById('usersList');
        
        if (users.length === 0) {
            usersList.innerHTML = '<p style="color: #666; text-align: center; padding: 40px;">No trainers found. Create one first!</p>';
        } else {
            usersList.innerHTML = users.map(user => `
                <div class="user-card">
                    <div class="user-header">${user.name}</div>
                    <div class="user-gender">Gender: ${user.gender}</div>
                    <div class="pokemon-list">
                        <div style="color: #ffff00; margin-bottom: 10px;">Pokemon Team (${user.pokemon.length}):</div>
                        ${user.pokemon.map((p, i) => `
                            <div class="pokemon-item">  ✓ ${p.name} (${p.type}) - Level ${p.level}</div>
                        `).join('')}
                    </div>
                </div>
            `).join('');
        }
    }

    /**
     * Populate user selection dropdowns
     * @param {Array} users 
     */
    static populateUserSelects(users) {
        const player1Select = document.getElementById('player1Select');
        const player2Select = document.getElementById('player2Select');

        [player1Select, player2Select].forEach(select => {
            select.innerHTML = '<option value="">Select trainer...</option>';
            users.forEach(user => {
                const option = document.createElement('option');
                option.value = user.id;
                option.textContent = `${user.name} (${user.pokemon.length} Pokemon)`;
                select.appendChild(option);
            });
        });
    }

    /**
     * Show player preview
     * @param {number} playerNum 
     * @param {Object} user 
     */
    static showPlayerPreview(playerNum, user) {
        const preview = document.getElementById(`player${playerNum}Preview`);
        
        // Always clear the preview first
        preview.innerHTML = '';
        
        if (!user || !user.pokemon || user.pokemon.length === 0) {
            preview.innerHTML = '<p style="color: #666;">No Pokemon available</p>';
            return;
        }

        preview.innerHTML = `
            <div style="color: #ffff00; margin-bottom: 10px;">${user.name}'s Team:</div>
            ${user.pokemon.map(p => `
                <div style="padding: 5px 0; color: #00ff00;">• ${p.name} (Lv.${p.level})</div>
            `).join('')}
        `;
    }

    /**
     * Update team display during Pokemon selection
     * @param {Array} selectedTeam 
     */
    static updateTeamDisplay(selectedTeam) {
        document.getElementById('teamCounter').textContent = selectedTeam.length;
        document.getElementById('teamDisplay').innerHTML = selectedTeam
            .map(name => `<div style="color: #00ff00; padding: 5px;">✓ ${name}</div>`)
            .join('');
    }
}
