<template>
  <div class="image-upload-container">
    <input
      type="file"
      ref="fileInput"
      @change="handleFileChange"
      accept="image/*"
      class="file-input"
    />

    <div
      class="drop-area"
      :class="{
        'drag-over': isDragging,
        'has-preview': hasPreview && !isDragging,
        'error': error
      }"
      @click="triggerFileInput"
    >
      <div v-if="hasPreview" class="preview-container">
        <div class="image-preview">
          <img :src="previewUrl" alt="Preview" />
          <button type="button" class="remove-btn" @click.stop="removeImage">×</button>
        </div>
      </div>

      <div v-else class="upload-content">
        <div class="upload-icon">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
            <polyline points="17 8 12 3 7 8"></polyline>
            <line x1="12" y1="3" x2="12" y2="15"></line>
          </svg>
        </div>
        <div class="upload-text">
          <p>Drag & Drop your file here</p>
          <span>or</span>
          <p class="browse-text">Choose your file</p>
        </div>
      </div>

      <div v-if="isDragging" class="drag-overlay">
        <div class="drag-text">Drop your file here</div>
      </div>
    </div>

    <div v-if="validationError || error" class="validation-error">
      {{ validationError || error }}
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'

const props = defineProps({
  files: {
    type: Array,
    default: () => []
  },
  multiple: {
    type: Boolean,
    default: false
  },
  maxSize: {
    type: Number,
    default: 5,
  },
  maxFiles: {
    type: Number,
    default: 1,
  },
  accept: {
    type: Array,
    default: () => ['image/jpeg', 'image/png', 'image/gif', 'image/webp'],
  },
  error: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:files', 'upload-error'])

const fileInput = ref(null)
const isDragging = ref(false)
const validationError = ref('')
const previewUrl = ref('')

let dragCounter = 0

const hasPreview = computed(() => !!previewUrl.value)

// Watch for changes in the files prop
watch(() => props.files, (newFiles) => {
  if (newFiles && newFiles.length > 0) {
    updatePreview(newFiles[0])
  } else {
    previewUrl.value = ''
  }
}, { immediate: true, deep: true })

function updatePreview(file) {
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      previewUrl.value = e.target.result
    }
    reader.readAsDataURL(file)
  }
}

function triggerFileInput() {
  fileInput.value.click()
}

function handleFileChange(event) {
  const file = event.target.files[0]
  if (file) {
    processFile(file)
  }
  event.target.value = ''
}

// Drag event handlers
function handleDragEnter(event) {
  event.preventDefault()
  event.stopPropagation()
  dragCounter++
  isDragging.value = true
}

function handleDragOver(event) {
  event.preventDefault()
  event.stopPropagation()
  isDragging.value = true
}

function handleDragLeave(event) {
  event.preventDefault()
  event.stopPropagation()
  dragCounter--
  if (dragCounter === 0) {
    isDragging.value = false
  }
}

function handleDrop(event) {
  event.preventDefault()
  event.stopPropagation()
  dragCounter = 0
  isDragging.value = false

  const files = Array.from(event.dataTransfer.files).filter((file) =>
    file.type.startsWith('image/'),
  )

  if (files.length === 0) {
    validationError.value = 'Supported formats: jpg, jpeg, png, gif, webp'
    emit('upload-error', 'Supported formats: jpg, jpeg, png, gif, webp')
    return
  }

  processFile(files[0]) // Process only the first file
}

// Process the uploaded file
function processFile(file) {
  validationError.value = ''

  // Check file type
  if (!file.type.startsWith('image/')) {
    validationError.value = 'Unsupported format'
    emit('upload-error', 'Unsupported format')
    return
  }

  // Check file size
  if (file.size > props.maxSize * 1024 * 1024) {
    validationError.value = `Max file size: ${props.maxSize}Mb`
    emit('upload-error', `Max file size: ${props.maxSize}Mb`)
    return
  }

  // Update files
  updatePreview(file)
  emit('update:files', [file])
}

function removeImage() {
  previewUrl.value = ''
  emit('update:files', [])
}

function handleFilesEvent() {
  emit('update:files', props.files)
}

onMounted(() => {
  // Initialize preview if files exist
  if (props.files && props.files.length > 0) {
    updatePreview(props.files[0])
  }

  const element = document.querySelector('.drop-area')
  if (element) {
    element.addEventListener('dragenter', handleDragEnter)
    element.addEventListener('dragover', handleDragOver)
    element.addEventListener('dragleave', handleDragLeave)
    element.addEventListener('drop', handleDrop)

    // Add listeners to document for better reliability
    document.addEventListener('dragover', (e) => {
      e.preventDefault()
      e.stopPropagation()
    })

    document.addEventListener('drop', (e) => {
      e.preventDefault()
      e.stopPropagation()
    })
  }
})

// Remove event listeners when component unmounts
onUnmounted(() => {
  const element = document.querySelector('.drop-area')
  if (element) {
    element.removeEventListener('dragenter', handleDragEnter)
    element.removeEventListener('dragover', handleDragOver)
    element.removeEventListener('dragleave', handleDragLeave)
    element.removeEventListener('drop', handleDrop)

    document.removeEventListener('dragover', (e) => {
      e.preventDefault()
      e.stopPropagation()
    })

    document.removeEventListener('drop', (e) => {
      e.preventDefault()
      e.stopPropagation()
    })
  }
})
</script>

<style scoped>
.image-upload-container {
  width: 100%;
  margin-bottom: 1rem;
}

.file-input {
  display: none;
}

.drop-area {
  width: 100%;
  min-height: 200px;
  border: 2px dashed #ccc;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  padding: 20px;
  box-sizing: border-box;
}

.drop-area:hover {
  border-color: #999;
}

.drag-over {
  border-color: #4a90e2;
  background-color: rgba(74, 144, 226, 0.05);
}

.has-preview {
  border-style: solid;
  padding: 10px;
  min-height: auto;
}

.error {
  border-color: #e74c3c;
}

.upload-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  color: #666;
}

.upload-icon {
  color: #999;
  margin-bottom: 10px;
}

.upload-text {
  text-align: center;
}

.browse-text {
  color: #4a90e2;
  font-weight: 500;
  margin-top: 5px;
}

.preview-container {
  display: flex;
  justify-content: center;
  width: 100%;
}

.image-preview {
  position: relative;
  width: 100px;
  height: 100px;
  border-radius: 4px;
  overflow: hidden;
}

.image-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.remove-btn {
  position: absolute;
  top: 5px;
  right: 5px;
  background: rgba(0, 0, 0, 0.5);
  color: white;
  border: none;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 14px;
  padding: 0;
  line-height: 1;
}

.remove-btn:hover {
  background: rgba(0, 0, 0, 0.7);
}

.drag-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(74, 144, 226, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 6px;
  z-index: 5;
}

.drag-text {
  color: #4a90e2;
  font-weight: 500;
  font-size: 1.1em;
}

.validation-error {
  color: #e74c3c;
  font-size: 0.85em;
  margin-top: 5px;
}
</style>