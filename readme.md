# Test task for Sigma Software
## Project overview

The main goal was to create a single-page website with vue.js 3 and .Net that saves forms to Azure Blob Storage. During this project, I developed a user-friendly form (like a register form) with different field types and validation. I also developed two services at the backend that send saved forms to Blob storage.

## Frontend

### Tech stack

- Vue.js 3
- Vite


The main page has a couple of reusable components with validation
- Text input field - First name, Last name, Email, Password, Confirm Password
- Dropdown - Gender
- File upload - Avatar


> Fields like Email, Password, Confirm Password has a validation.

Email
- Required: Email field cannot be empty
- Format: Must match standard email format (xxx@xxx.xxx)

Password
- Required: Password field cannot be empty
- Length: Must be at least 8 characters long
- Content: Must contain at least one letter (Latin or Cyrillic)
- Content: Must contain at least one digit

Confirm Password
- Required: Confirm password field cannot be empty
- Match: Must match the password field value

----
    ⛔ Uploader has max size validation for 5 mb. 
    To pretend to upload large file to blob storage

---
# Backend
### Tech stack

- .Net 7.0
-  Azure.Storage.Blobs


*For this task, I used UploadFormDto.cs to transfer data from the frontend to the backend. The backend has two services: AzureBlobService for uploading images and JSON files (forms) to blob and FormService for managing mapping from DTO to SaveForm that inherits FormModel.cs, hashing passwords, and transferring data to AzureBlobService.*


