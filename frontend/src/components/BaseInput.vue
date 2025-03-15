<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  placeholder: String,
  type: String,
  value: String,
  error: String,
})

const emit = defineEmits(['input'])

const hiddenPassword = ref(true)
const tempType = computed(() => {
  if (props.type === 'password') {
    return hiddenPassword.value ? 'password' : 'text'
  }
  return props.type
})

function togglePasswordVisibility() {
  hiddenPassword.value = !hiddenPassword.value
}

function handleInput(event) {
  emit('input', event.target.value)
}
</script>

<template>
  <div class="input_field">
    <div class="input-wrapper">
      <input :type="tempType" :placeholder="placeholder" :value="value" @input="handleInput" />
      <img
        v-if="props.type === 'password'"
        :src="hiddenPassword ? '/public/imgs/icons/hide.png' : '/public/imgs/icons/visible.png'"
        alt="Toggle password visibility"
        @click="togglePasswordVisibility"
      />
    </div>
    <div class="error_text" v-if="error">
      {{ error }}
    </div>
  </div>
</template>

<style scoped>
.input-wrapper {
  display: flex;
  flex-direction: row;
  gap: 5px;
  border: 1px solid #ccc;
  border-radius: 5px;
  padding: 15px 10px;
  width: 100%;
}

.input_field {
  width: 100%;
}

input {
  border: none;
  width: 100%;
  background-color: transparent;
  color: white;
}

input:focus {
  outline: none;
}

img {
  cursor: pointer;
  width: 20px;
  height: 20px;
  filter: invert(1);
}

.error_text {
  color: red;
  font-size: 12px;
  margin-top: 5px;
}
</style>
