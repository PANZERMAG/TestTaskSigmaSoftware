<template>
  <div class="dropdown-container">
    <button @click="toggleDropdown" class="dropdown-toggle">
      {{ selectedOption || placeholder }}
      <span class="dropdown-arrow" :class="{ 'arrow-up': isOpen }">▼</span>
    </button>

    <div v-if="isOpen" class="dropdown-menu">
      <div
        v-for="option in options"
        :key="option.value"
        @click="selectOption(option)"
        class="dropdown-item"
        :class="{ selected: option.value === selectedValue }"
      >
        {{ option.label }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
const props = defineProps({
  options: {
    type: Array,
    required: true,
  },
  modelValue: {
    type: [String, Number, Object],
    default: null,
  },
  placeholder: {
    type: String,
    default: 'Select your gender',
  },
})

const emit = defineEmits(['update:modelValue', 'change'])

const isOpen = ref(false)
const selectedValue = ref(props.modelValue)

// Computed property to get the selected option's label
const selectedOption = computed(() => {
  const option = props.options.find((opt) => opt.value === selectedValue.value)
  return option ? option.label : null
})

function toggleDropdown() {
  isOpen.value = !isOpen.value
}

function selectOption(option) {
  selectedValue.value = option.value
  emit('update:modelValue', option.value)
  emit('change', option)
  isOpen.value = false
}

// Close dropdown when clicking outside
function handleClickOutside(event) {
  const dropdown = event.target.closest('.dropdown-container')
  if (!dropdown) {
    isOpen.value = false
  }
}

// Add/remove event listener when component is mounted/unmounted
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.dropdown-container {
  position: relative;
  width: 100%;
}

.dropdown-toggle {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  padding: 15px 10px;
  background-color: transparent;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  text-align: left;
  width: 100%;
  color: #ccc;
}

.dropdown-arrow {
  font-size: 10px;
  transition: transform 0.2s ease;
}

.arrow-up {
  transform: rotate(180deg);
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  width: 100%;
  max-height: 200px;
  overflow-y: auto;
  background-color: black;
  border: 1px solid #ddd;
  border-top: none;
  border-radius: 0 0 4px 4px;
  z-index: 10;
}

.dropdown-item {
  padding: 10px 15px;
  cursor: pointer;
}

.dropdown-item:hover {
  background-color: #171717;
}

.dropdown-item.selected {
  background-color: #171717;
}
</style>
