<route lang="json">
{
  "meta": {
    "title": "Server",
    "priority": 30
  }
}
</route>
    
<template>
  <MainLayout>

    <PrimaryButton @click="infrastructure.initServer">Init Game</PrimaryButton>

    <div class="m-2 rounded-md border border-2 border-blue-400 p-4">
      <p>Game ID: {{ infrastructure.game?.gameId }}</p>
      <p>Game Status: {{ infrastructure.game?.status }}</p>
      <p>Round ID: {{ infrastructure.game?.activeRoundId }}</p>
    </div>

  </MainLayout>
</template>

<script setup lang="ts">
import { useGameStore } from '../stores/game'
const infrastructure = useGameStore();
</script>
