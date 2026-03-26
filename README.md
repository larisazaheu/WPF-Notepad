# WPF Notepad

Developed a modern text editor built using **C#**, **WPF**, and the **MVVM architecture pattern**.
It extends the classic Notepad experience with tabs, file explorer integration, and powerful text editing tools.

<img width="984" height="555" alt="notepad" src="https://github.com/user-attachments/assets/d97f1689-26aa-4eea-b055-e196ad9d4ac5" />

---

## Features

### File Management

* Create new files
* Open existing files
* Save / Save As
* Multiple tabs support
* Prompt to save unsaved changes
* Close individual or all tabs

---

### File Explorer

* Browse system drives and folders
* Open files directly from explorer
* Create new files inside folders
* Copy folder paths
* Copy & paste folders (recursive)

---

### Text Editing

* Copy / Cut / Paste
* Convert text:
  * Uppercase
  * Lowercase
* Remove empty lines
* Go to specific line
* Toggle read-only mode

---

### Search & Replace

* Find text in:
  * Current tab
  * All tabs
* Navigate between results
* Replace:
  * First occurrence
  * All occurrences
* Highlight matches in editor

---

### Architecture

The project follows the **MVVM (Model-View-ViewModel)** pattern:

#### Models

* `MyTabModel` – represents an open file/tab
* `FileDirectoryModel` – represents files/folders in explorer
* `DataContextTree` – manages system drives

#### ViewModels

* `MainViewModel` – core logic of the application
* `FindViewModel` – search functionality
* `ReplaceViewModel` – replace functionality

#### Services

* `FileService` – file operations (open/save)
* `SearchService` – search & replace logic

#### Utilities
* Custom `RelayCommand` implementation
* Visual tree helpers for active editor detection

---

## Technologies Used

* C#
* WPF (.NET)
* MVVM Pattern
* ObservableCollection
* ICommand

---

## 📄 License

This project is open-source and available under the MIT License.
