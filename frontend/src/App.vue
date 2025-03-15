<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import BaseInput from './components/BaseInput.vue'
import Dropdown from './components/Dropdown.vue'
import Uploader from './components/Uploader.vue'

interface FormData {
  firstName: string
  lastName: string
  gender: string
  email: string
  password: string
  confirmPassword: string
  avatar: File[] | null
}

interface FormErrors {
  firstName?: string
  lastName?: string
  gender?: string
  email?: string
  password?: string
  confirmPassword?: string
  avatar?: string
}

//validation schema
const validationSchema = {
  firstName: (value: string): string | true => {
    if (!value) return 'First name is required'
    return true
  },
  lastName: (value: string): string | true => {
    if (!value) return 'Last name is required'
    return true
  },
  gender: (value: string): string | true => {
    if (!value) return 'Gender is required'
    return true
  },
  email: (value: string): string | true => {
    if (!value) return 'Email is required'

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(value)) return 'Unsuitable email format'

    return true
  },
  password: (value: string): string | true => {
    if (!value) return 'Password is required'

    if (value.length < 8) return 'Password must be at least 8 characters long'
    if (!/[A-Za-zА-Яа-я]/.test(value)) return 'Password must contain at least one letter'
    if (!/\d/.test(value)) return 'Password must contain at least one digit'

    return true
  },
  confirmPassword: (value: string, formData: FormData): string | true => {
    if (!value) return 'Confirm password is required'
    if (value !== formData.password) return 'Passwords do not match'
    return true
  },
  avatar: (value: File[] | null): string | true => {
    if (!value || value.length === 0) return 'Avatar is required'
    return true
  },
}

// reactive form data
const formData = reactive<FormData>({
  firstName: '',
  lastName: '',
  gender: '',
  email: '',
  password: '',
  confirmPassword: '',
  avatar: null,
})

// Реактивные ошибки
const errors = reactive<FormErrors>({})

// Check if the form is valid
const isFormValid = computed(() => {
  return (
    !Object.values(errors).some((error) => error !== undefined && error !== '') &&
    formData.firstName &&
    formData.lastName &&
    formData.gender &&
    formData.email &&
    formData.password &&
    formData.confirmPassword &&
    formData.avatar
  )
})

const validateField = (fieldName: keyof FormData, value: any) => {
  if (fieldName === 'confirmPassword') {
    const result = validationSchema[fieldName](value, formData)
    errors[fieldName] = result === true ? '' : result
  } else if (validationSchema[fieldName]) {
    const result = validationSchema[fieldName](value)
    errors[fieldName] = result === true ? '' : result
  }
}

// Validate the form
const validateForm = () => {
  let isValid = true

  ;(Object.keys(validationSchema) as Array<keyof FormData>).forEach((field) => {
    if (field === 'confirmPassword') {
      const result = validationSchema[field](formData[field], formData)
      errors[field] = result === true ? '' : result
      if (result !== true) isValid = false
    } else {
      // @ts-ignore
      const result = validationSchema[field](formData[field])
      errors[field] = result === true ? '' : result
      if (result !== true) isValid = false
    }
  })

  return isValid
}

const updateFirstName = (value: string) => {
  formData.firstName = value
  validateField('firstName', value)
}

const updateLastName = (value: string) => {
  formData.lastName = value
  validateField('lastName', value)
}

const updateGender = (option: { value: string }) => {
  formData.gender = option.value
  validateField('gender', option.value)
}

const updateEmail = (value: string) => {
  formData.email = value
  validateField('email', value)
}

const updatePassword = (value: string) => {
  formData.password = value
  validateField('password', value)
  if (formData.confirmPassword) {
    validateField('confirmPassword', formData.confirmPassword)
  }
}

const updateConfirmPassword = (value: string) => {
  formData.confirmPassword = value
  validateField('confirmPassword', value)
}

const handleFilesChange = (files: File[]) => {
  formData.avatar = files
  validateField('avatar', files)
}

const handleUploadError = (error: string) => {
  console.error('Ошибка загрузки:', error)
  errors.avatar = error
}

const isSubmitting = ref(false)
const submitError = ref('')
const submitSuccess = ref(false)

const submitForm = async () => {
  const isValid = validateForm()

  if (isValid) {
    try {
      isSubmitting.value = true
      submitError.value = ''

      const formDataToSend = new FormData()

      formDataToSend.append('firstName', formData.firstName)
      formDataToSend.append('lastName', formData.lastName)
      formDataToSend.append('gender', formData.gender)
      formDataToSend.append('email', formData.email)
      formDataToSend.append('password', formData.password)

      if (formData.avatar && formData.avatar.length > 0) {
        formData.avatar.forEach((file, index) => {
          formDataToSend.append(`avatar`, file)
        })
      }

      const response = await fetch('http://localhost:5295/Form', {
        method: 'POST',
        body: formDataToSend,
      })

      if (!response.ok) {
        const errorData = await response.json()
        throw new Error(errorData.message || 'Error')
      }

      const data = await response.json()
      console.log('Success:', data)

      submitSuccess.value = true
    } catch (error) {
      console.error('An error occurred while sending the form:', error)
      submitError.value = error instanceof Error ? error.message : 'Unrecognized error'
    } finally {
      isSubmitting.value = false
    }
  } else {
    console.error('Form has a erros:', errors)
  }
}

const genderOptions = [
  { label: 'Male', value: 'male' },
  { label: 'Female', value: 'female' },
  { label: 'Other', value: 'other' },
]
</script>

<template>
  <div class="container">
    <div class="left-side_banner" />
    <div class="form_container">
      <div class="form_content">
        <div class="user_name">
          <BaseInput
            placeholder="First name"
            type="text"
            :value="formData.firstName"
            @input="updateFirstName"
            :error="errors.firstName"
          />
          <BaseInput
            placeholder="Last name"
            type="text"
            :value="formData.lastName"
            @input="updateLastName"
            :error="errors.lastName"
          />
        </div>

        <BaseInput
          placeholder="Email"
          type="text"
          :value="formData.email"
          @input="updateEmail"
          :error="errors.email"
        />

        <BaseInput
          placeholder="Password"
          type="password"
          :value="formData.password"
          @input="updatePassword"
          :error="errors.password"
        />

        <BaseInput
          placeholder="Confirm Password"
          type="password"
          :value="formData.confirmPassword"
          @input="updateConfirmPassword"
          :error="errors.confirmPassword"
        />

        <Dropdown
          v-model="formData.gender"
          :options="genderOptions"
          @change="updateGender"
          :error="errors.gender"
          placeholder="Select gender"
        />

        <Uploader
          v-model:files="formData.avatar"
          :multiple="false"
          :max-size="2"
          :max-files="1"
          @update:files="handleFilesChange"
          @upload-error="handleUploadError"
          :error="errors.avatar"
        />

        <button class="submit_button" @click="submitForm" :disabled="!isFormValid">Submit</button>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.container {
  display: flex;
  flex-direction: row;
  height: 100vh;
  background-color: black;
}

.left-side_banner,
.form_container {
  width: 50%;
  min-height: 100vh;
}

.left-side_banner {
  background-image: url('/public/imgs/banner.png');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  background-color: rgb(24 24 27);
  border-right: 1px solid #cccccc17;
}

.form_container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: hsl(240 10% 3.9%);

  .form_content {
    display: flex;
    flex-direction: column;
    gap: 20px;
    width: 50%;

    .user_name {
      display: flex;
      flex-direction: row;
      gap: 20px;
    }
  }

  .submit_button {
    padding: 10px 20px;
    background-color: #171717;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    border: 1px solid #cccccc10;
    transition: background-color 0.3s ease;

    &:hover {
      background-color: #2a2a2a;
    }

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  }
}
</style>
