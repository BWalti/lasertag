<template>
  <li class="col-span-1 divide-y divide-gray-200 rounded-lg bg-white shadow">
    <div class="flex w-full items-center justify-between space-x-6 p-6">
      <div class="flex-1 truncate">
        <div class="flex items-center space-x-3">
          <h3 class="truncate text-sm font-medium text-gray-900">{{ name }}</h3>
          <span
            class="inline-block flex-shrink-0 rounded-full bg-green-100 px-2 py-0.5 text-xs font-medium text-green-800">{{
                role
            }}</span>
        </div>
        <p class="mt-1 truncate text-sm text-gray-500">{{ title }}</p>
      </div>
      <img class="h-10 w-10 flex-shrink-0 rounded-full bg-gray-300" :src="imageUrl" alt="" />
    </div>
    <div>
      <div class="-mt-px flex divide-x divide-gray-200">
        <div class="flex w-0 flex-1">
          <a :href="`mailto:${email}`"
            class="relative -mr-px inline-flex w-0 flex-1 items-center justify-center rounded-bl-lg border border-transparent py-4 text-sm font-medium text-gray-700 hover:text-gray-500">
            <EnvelopeIcon class="h-5 w-5 text-gray-400" aria-hidden="true" />
            <span class="ml-3">Email</span>
          </a>
        </div>
        <div class="-ml-px flex w-0 flex-1">
          <a :href="`tel:${telephone}`"
            class="relative inline-flex w-0 flex-1 items-center justify-center rounded-br-lg border border-transparent py-4 text-sm font-medium text-gray-700 hover:text-gray-500">
            <PhoneIcon class="h-5 w-5 text-gray-400" aria-hidden="true" />
            <span class="ml-3">Call</span>
          </a>
        </div>
      </div>
    </div>
  </li>
</template>

<script setup lang="ts">
import { EnvelopeIcon, PhoneIcon } from '@heroicons/vue/20/solid'

export interface Props {
  name: string,
  title: string,
  role: string,
  email: string,
  telephone: string,
  imageUrl: string,
}

defineProps<Props>();

</script>