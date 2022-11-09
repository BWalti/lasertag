// stores/counter.js
import { defineStore } from 'pinia'
import { Game } from "../services/data-contracts";
import { Api } from "../services/Api";
import { lasertagApiHttpClient } from "../utils/httpClient";

const api = new Api(lasertagApiHttpClient);

export const useGameStore = defineStore('game', {
  state: () => {
    return {
      game: undefined as Game | undefined
    }
  },
  // could also be defined as
  // state: () => ({ count: 0 })
  actions: {
    async initServer() {
      var result = await api.initGame();

      if (result.success) {
        this.game = result.output;
      }
    },
    async registerGameSet() {
      if (!this.game?.gameId) {
        return;
      }


      const gameSetId = crypto.randomUUID();
      console.log(`Generated GameSet ID: ${gameSetId}`);

      var result = await api.registerGameSet(this.game!.gameId!, gameSetId, { isTargetOnly: false });
      if (result.success) {
        this.game = result.output;
      }
    },

    async toggleGameSetConnectivity(gameSetId: string | undefined) {
      if (!this.game?.gameId || !gameSetId) {
        return;
      }

      var gameSet = this.game.gameSets?.find(gs => gs.id == gameSetId);
      if (!gameSet) {
        return;
      }

      var result = !gameSet.isOnline
        ? await api.connectGameSet(this.game!.gameId!, gameSetId)
        : await api.disconnectGameSet(this.game!.gameId!, gameSetId);
      if (result.success) {
        this.game = result.output;
      }

    }
  },
})