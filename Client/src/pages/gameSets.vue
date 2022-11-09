<route lang="json">
{
  "meta": {
    "title": "GameSets",
    "priority": 20
  }
}
</route>
    
<template>
  <MainLayout>
    <PrimaryButton @click="game.registerGameSet">Register GameSet</PrimaryButton>

    <GridList class="m-4">
      <GridListGameSetItem v-for="gameSet in game.game?.gameSets" :name="gameSet.name" rank="Basis"
        :is-online="gameSet.isOnline" @connect="() => game.toggleGameSetConnectivity(gameSet.id)" avatar="empty"
        :key="gameSet.id">
      </GridListGameSetItem>
    </GridList>
  </MainLayout>
</template>


<script setup lang="ts">
import { useGameStore } from '../stores/game'
const game = useGameStore();

</script>
