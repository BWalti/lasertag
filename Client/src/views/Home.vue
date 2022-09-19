<template>
  <MainLayout>
    <div class="flex gap-2">
      <PrimaryButton @click="initGame">Init Game</PrimaryButton>
      <SecondaryButton @click="connectLasertag" v-if="isGameInInitializedState"
        >Connect LasertagSet</SecondaryButton
      >
      <SecondaryButton @click="createLobby" v-if="isGameInInitializedState"
        >Create Lobby</SecondaryButton
      >
      <SecondaryButton @click="startGame" v-if="isGameInLobbyState"
        >Start Game</SecondaryButton
      >
    </div>

    <div class="m-2 rounded-md border border-2 border-blue-400 p-4">
      <p>Game ID: {{ game?.gameId }}</p>
      <p>Game Status: {{ game?.status }}</p>
      <p>Round ID: {{ game?.activeRoundId }}</p>
    </div>

    <div class="bg-gray-100 p-4">
      <h3 class="mb-4 text-lg font-semibold">Connected Game Sets:</h3>

      <div
        v-for="gameSet in lasertagSets"
        :key="gameSet.id"
        class="my-4 rounded-md border-b border-gray-200 bg-white px-4 py-5 sm:px-6"
      >
        <h3 class="text-lg font-medium leading-6 text-gray-900">
          Name
          <span class="text-sm font-normal text-gray-400"
            >({{ gameSet.id }})</span
          >
        </h3>

        <span>- {{ gameSet.id }}</span>
        <span
          class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
          :class="{
            'bg-green-100': gameSet.isActive,
            'text-green-800': gameSet.isActive,
            'bg-red-100': !gameSet.isActive,
            'text-red-800': !gameSet.isActive,
          }"
          >{{ gameSet.isActive ? "Online" : "Offline" }}</span
        >

        <span
          class="inline-flex items-center rounded-full bg-gray-100 px-2.5 py-0.5 text-xs font-medium text-gray-800"
          >{{ gameSet.color }}</span
        >
        <SecondaryButton
          @click="activateGameSet(gameSet.id!)"
          class="m-2"
          v-if="isGameInLobbyState && !gameSet.isActive"
          >Activate LasertagSet</SecondaryButton
        >
      </div>
    </div>
  </MainLayout>
</template>

<script setup lang="ts">
import { ref, watch, computed } from "vue";

import MainLayout from "../layouts/mainLayout.vue";
import PrimaryButton from "../components/PrimaryButton.vue";
import SecondaryButton from "../components/SecondaryButton.vue";

import { Game as GameService } from "../services/Game";
import { Game, GameRound } from "../services/data-contracts";
import { lasertagApiHttpClient } from "../utils/httpClient";
import { GameRound as GameRoundService } from "../services/GameRound";

const game = ref<Game>();
const gameRound = ref<GameRound>();
const gameService = new GameService(lasertagApiHttpClient);
const gameRoundService = new GameRoundService(lasertagApiHttpClient);

const initGame = async function () {
  var result = await gameService.initCreate();
  game.value = result.output;
};

const connectLasertag = async function () {
  if (!isGameIdAvailable.value) {
    return;
  }

  const gameSetId = crypto.randomUUID();
  console.log(`Generated GameSet ID: ${gameSetId}`);

  var result = await gameService.connectCreate(game.value!.gameId!, gameSetId);
  game.value = result.output;
};

const createLobby = async function () {
  if (!isGameIdAvailable.value) {
    return;
  }

  var result = await gameService.createLobbyCreate(game.value!.gameId!);
  game.value = result.output;
};

const activateGameSet = async function (gameSetId: string) {
  if (!isGameIdAvailable.value) {
    return;
  }

  const playerId = crypto.randomUUID();
  var result = await gameService.activateCreate(
    game.value!.gameId!,
    gameSetId,
    playerId,
  );
  game.value = result.output;
};

const startGame = async function () {
  if (!isGameIdAvailable.value) {
    return;
  }

  var result = await gameService.startCreate(game.value!.gameId!);
  game.value = result.game?.output;
  gameRound.value = result.gameRound?.output;
};

const isGameIdAvailable = computed(() => {
  if (game && game.value && game.value.gameId) {
    return true;
  }

  return false;
});

const isGameInInitializedState = computed(() => {
  if (game && game.value && game.value.status == 0) {
    return true;
  }

  return false;
});

const isGameInLobbyState = computed(() => {
  if (game && game.value && game.value.status == 1) {
    return true;
  }

  return false;
});

const lasertagSets = computed(() => {
  const connected = game.value?.connectedGameSets;
  const groups = game.value?.gameSetGroups;
  const active = game.value?.activeGameSets;

  var obj: any = {};

  connected?.forEach((item) => {
    obj[item.id!] = {
      id: item.id,
      isActive: false,
      playerId: null,
    };
  });

  groups?.forEach((group) => {
    group.gameSets?.forEach((gameSet) => {
      obj[gameSet.id!].groupId = group.groupId;
      obj[gameSet.id!].color = group.color;
    });
  });

  active?.forEach((item) => {
    obj[item.gameSetId!].isActive = true;
    obj[item.gameSetId!].playerId = item.playerId!;
  });

  return obj;
});

watch(game, (newGame) => {
  console.log(newGame);
});
</script>
