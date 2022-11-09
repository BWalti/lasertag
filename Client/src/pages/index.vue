<route lang="json">
{
  "meta": {
    "title": "Dashboard",
    "priority": 10
  }
}
</route>
  
<template>
  <MainLayout>
    <div class="flex gap-2">
      <SecondaryButton @click="registerLasertag" v-if="isGameInInitializedState">Register LasertagSet</SecondaryButton>
      <SecondaryButton @click="createLobby" v-if="isGameInInitializedState">Create Lobby</SecondaryButton>
      <SecondaryButton @click="startGame" v-if="isGameInLobbyState">Start Game</SecondaryButton>
    </div>


    <GridList class="m-4">
      <GridListPersonItem v-for="p in people" :name="p.name" :email="p.email" :image-url="p.imageUrl" :role="p.role"
        :telephone="p.telephone" :title="p.title" :key="p.email"></GridListPersonItem>
    </GridList>

    <div class="bg-gray-100 p-4">
      <h3 class="mb-4 text-lg font-semibold">Connected Game Sets:</h3>

      <div class="grid grid-cols-1 md:grid-cols-2 items-stretch gap-2">

        <div v-for="gameSet in lasertagSets" :key="gameSet.id" class="relative rounded-md border-b p-4 bg-white">
          <h3 class="text-lg font-medium leading-6 text-gray-900 rounded p-2" :class="{
            'bg-white': gameSet.color === undefined,
            'bg-red-100': gameSet.color == 'Red',
            'bg-blue-100': gameSet.color == 'Blue',
            'bg-yellow-100': gameSet.color == 'Yellow',
            'bg-green-100': gameSet.color == 'Green',
            'bg-turquoise-100': gameSet.color == 'Turquoise',
            'bg-pink-100': gameSet.color == 'Pink',
            'bg-violet-100': gameSet.color == 'Violet',
            'bg-white-100': gameSet.color == 'White',
            'text-black-800': gameSet.color == 'White',
          }">
            {{ gameSet.name }}
          </h3>
          <p class="text-sm font-normal text-gray-400">({{ gameSet.id }})</p>

          <div class="absolute top-3 right-2 rounded-full w-16 px-2.5 py-0.5 text-xs font-medium border" :class="{
            'bg-green-100': gameSet.isActive,
            'text-green-800': gameSet.isActive,
            'border-greem-800': gameSet.isActive,
            'bg-red-100': !gameSet.isActive,
            'text-red-800': !gameSet.isActive,
            'border-red-800': !gameSet.isActive,
          }">{{ gameSet.isActive ? "Online" : "Offline" }}</div>


          <SecondaryButton @click="activateGameSet(gameSet.id!)" class="m-2"
            v-if="isGameInLobbyState && !gameSet.isActive">Activate LasertagSet</SecondaryButton>

          <div v-if="isGameInGameStartedState">
            <SecondaryButton @click="fire(gameSet.id!, gameSet.fireAt)" class="m-2">Fire At</SecondaryButton>
            <input type="text" v-model="gameSet.fireAt" />
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script setup lang="ts">
import { Api } from "../services/Api";
import { Game, GameRound } from "../services/data-contracts";
import { lasertagApiHttpClient } from "../utils/httpClient";

const people = [
  {
    name: 'Jane Cooper',
    title: 'Regional Paradigm Technician',
    role: 'Admin',
    email: 'janecooper@example.com',
    telephone: '+1-202-555-0170',
    imageUrl:
      'https://images.unsplash.com/photo-1494790108377-be9c29b29330?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=4&w=256&h=256&q=60',
  },
  // More people...
]

const game = ref<Game>();
const gameRound = ref<GameRound>();
const api = new Api(lasertagApiHttpClient);

const initGame = async function () {
  var result = await api.initGame();
  game.value = result.output;
};

const registerLasertag = async function () {
  if (!isGameIdAvailable.value) {
    return;
  }

  const gameSetId = crypto.randomUUID();
  console.log(`Generated GameSet ID: ${gameSetId}`);

  var result = await api.registerGameSet(game.value!.gameId!, gameSetId, { isTargetOnly: false });
  game.value = result.output;
};

const createLobby = async function () {
  if (!isGameIdAvailable.value) {
    return;
  }

  var result = await api.createGameRound(game.value!.gameId!);
  game.value = result.output;
};

const activateGameSet = async function (gameSetId: string) {
  if (!isGameIdAvailable.value) {
    return;
  }

  var result = await api.connectGameSet(
    game.value!.gameId!,
    gameSetId
  );
  game.value = result.output;
};

const startGame = async function () {
  if (!isGameIdAvailable.value) {
    return;
  }

  var result = await api.startGameRound(game.value!.gameId!);
  game.value = result.game?.output;
  gameRound.value = result.gameRound?.output;
};

const fire = async function (sourceId: string, targetId: string) {
  var result = await api.gameRoundLasertagSetShotFiredCreate(gameRound.value!.id!, sourceId);
  gameRound.value = result.output;

  if (targetId) {
    var result = await api.gameRoundLasertagSetGotHitFromLasertagSetCreate(gameRound.value!.id!, sourceId, targetId);
    gameRound.value = result.output;
  }
};

const isGameIdAvailable = computed(() => {
  if (game && game.value && game.value.gameId) {
    return true;
  }

  return false;
});

const isGameInInitializedState = computed(() => {
  if (game && game.value && game.value.status == "Initialized") {
    return true;
  }

  return false;
});
const isGameInLobbyState = computed(() => {
  if (game && game.value && game.value.status == "LobyOpened") {
    return true;
  }

  return false;
});

const isGameInGameStartedState = computed(() => {
  if (game && game.value && game.value.status == "GameStarted") {
    return true;
  }

  return false;
});

const lasertagSets = computed(() => {
  const connected = game.value?.gameSets;
  // const groups = game.value?.gameSetGroups;
  // const active = game.value?.activeGameSets;

  var obj: any = {};

  connected?.forEach((item) => {
    obj[item.id!] = {
      id: item.id,
      isActive: false,
      playerId: null,
      name: item.name,
    };
  });

  // groups?.forEach((group) => {
  //   group.gameSets?.forEach((gameSet) => {
  //     obj[gameSet.id!].groupId = group.groupId;
  //     obj[gameSet.id!].color = group.color;
  //   });
  // });

  // active?.forEach((item) => {
  //   obj[item.gameSetId!].isActive = true;
  //   obj[item.gameSetId!].playerId = item.playerId!;
  // });

  return obj;
});

watch(game, (newGame) => {
  console.log(newGame);
});
</script>
