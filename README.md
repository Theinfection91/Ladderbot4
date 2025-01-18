# Ladderbot4

## Documentation
[Extensive Documentation Found Here](https://github.com/Theinfection91/Ladderbot4/blob/main/Ladderbot4Doc.md)

## Releases
[Download Latest Version Here](https://github.com/Theinfection91/Ladderbot4/releases)

---

# Ladderbot 4.2.0 - What's New

## Overview
### (Merge 0)
The first update to the **4.2.0 branch** introduces significant changes to support flexible league configurations and lays the groundwork for enhanced data handling. These changes include the decoupling of team sizes for leagues (XvX), a new `Data` class structure, and the replacement of several JSON files for improved organization and scalability. This marks the first step toward the fully realized 4.2.0 update.

### (Merge 1)
The second update to the **4.2.0 branch** introduces the **Modal Confirmation System**, which enhances critical operations by requiring users to confirm sensitive actions twice, ensuring case-sensitive accuracy. This system is applied to key operations like starting, ending, deleting leagues, and removing teams. Additional improvements include a more streamlined approach to handling the ladder lifecycle, with enhancements like resets for team stats at the start of a ladder and new results embeds at the end. A reusable handler class (`ModalInteractionHandlers`) was also created for better maintainability and scalability of modal processes. This update sets the stage for more interactive components in future releases.

### (Merge 2)
The third update introduces **Dynamic Challenge Rank Updates**, a feature that ensures challenge ranks remain accurate even after other teams report wins, causing rank changes. It also includes new methods for challenge rank validation and correction, improving the integrity of challenge data.

### (Merge 3)
The fourth update addresses **Duplicate Automated Messages** and improves JSON file handling, resolving issues where states were not properly loading after being cloned from the repository. Automated message processes now properly save and retrieve message IDs, preventing duplicates and ensuring a smoother user experience.

---

## What's New in 4.2.0 (Merge 3) - (01/18/2025)

### Bug Fixes and Enhancements

#### **Duplicate Automated Messages**
- Added message ID fields to the `State` model, enabling the bot to track and edit existing automated messages for standings, challenges, and teams.
- Automated messages are no longer duplicated. The bot now:
  - Saves the message ID upon sending a new message.
  - Edits the old message on startup instead of creating duplicates.
- This improvement applies to all automated message types (standings, challenges, and teams).

#### **JSON File Handling Fixes**
- Fixed an issue where database JSON files were not loading after being cloned from the repository. This ensures seamless operation even after repo updates.

---

## What's New in 4.2.0 (Merge 2) - (01/17/2025)

### Dynamic Challenge Rank Updates

#### **Challenge Rank Validation**
- Created a new `IsChallengeRankCorrect` method in `ChallengeManager` to validate whether a challenge's rank matches the team's current rank in standings.
- Added logic to:
  - Automatically correct mismatched ranks during `ReportWin` operations.
  - Ensure rank integrity for both user commands and admin overrides.

#### **Refactored Rank Correction Process**
- Moved challenge rank correction logic into `ChallengeManager` for better modularity and maintainability.
- Added functionality to check and correct ranks for teams with ongoing challenges, dynamically resolving discrepancies caused by reported wins affecting standings.

---

## What's New in 4.2.0 (Merge 1) - (01/14/2025)

### Feature Enhancements

#### **Modal Confirmation System**
- Introduced a robust Modal Confirmation process to ensure accuracy and prevent unintended actions when performing critical operations. Users are required to confirm sensitive inputs twice in a case-sensitive manner.
- Key operations updated to use the Modal Confirmation system:
  - **Start Ladder**: Users must input the league name twice, ensuring precise matches with the database before starting a new ladder.
  - **End Ladder**: Users must input the league name twice, ensuring precise matches with the database to prevent errors when ending a ladder.
  - **Delete League**: Similar confirmation logic applied to prevent accidental deletion of entire leagues.
  - **Remove Team**: Enhanced safeguards added using modal confirmation to handle team removal responsibly.

#### **Ladder Lifecycle Enhancements**
- **Start Ladder**:
  - Team statistics (wins, losses, streaks) are reset to zero at the start of a new ladder to ensure a fresh slate.
  - The modal process for Start Ladder includes case-sensitive league name validation.
- **End Ladder**:
  - New results embed added to display standings when a ladder is ended.
  - Challenges are cleared upon ladder termination to finalize the league state.
  - Future feature: Consider exporting standings as a PDF for archival purposes.

#### **Reusable Modal Handlers**
- Created a centralized `ModalInteractionHandlers` class for managing modal interactions:
  - Supports consistent handling of ladder start/end, league deletion, and team removal processes.
  - Reduces redundant code and enhances maintainability by utilizing shared logic for modal validation and confirmation.

#### **Interactive UX Framework**
- Established the foundation for future interactive components (e.g., buttons).
- The modal framework offers scalability for adding new features and interactive processes.

---

## What's New in 4.2.0 (Merge 0) - (01/13/2025)

### Feature Enhancements

#### **Decoupling Team Sizes for XvX Leagues**
- The static structure of predefined division types (1v1, 2v2, 3v3) has been replaced with flexible, customizable XvX leagues.
- Users can now define leagues with any team size, such as 4v4, 5v5, or other configurations.
- The system dynamically manages leagues of various team sizes concurrently, improving scalability and allowing for diverse league setups.

#### **Improved Data Handling**
- Introduced a new base `Data` class in the codebase, simplifying the management of JSON-based data files. This structure improves consistency and maintainability.
- Certain data files now inherit from this base class, enabling more efficient data processing and extensibility for future updates.

#### **New JSON File Structure**
- Several JSON files have been replaced to align with the new data handling system:
  - **`leagues.json` → `league_registry.json`**  
    The new file structure enhances the storage and retrieval of league data, supporting the flexible XvX configuration.
  - **`challenges.json` → `challenges_hub.json`**  
    Challenge data is now organized for better integration with dynamic league management.
  - **`states.json` → `states_atlas.json`**  
    State tracking data has been restructured for improved clarity and future scalability.
- **Note for Users**:  
  - These changes will automatically generate new files. The old files will no longer be used but can be retained as backups if needed.

#### **Settings Compatibility**
- The `SettingsData` class has been renamed to `SettingsVaultData` internally.  
  - **User Impact**: No changes to the `config.json` file in the `Settings` folder. The transition is seamless for end-users.


---

## Next Steps
This merge represents the foundation of the 4.2.0 update. Future updates will build upon these changes, introducing features such as:
- **Member profile stats.**
- **Autocomplete suggestions for league names.**
- **Additional bug fixes and enhancements.**

For detailed documentation on setup and configuration, refer to the [Ladderbot4 Documentation](https://github.com/Theinfection91/Ladderbot4/blob/main/Ladderbot4Doc.md).


## Features

### Core Functionalities:
- **Dynamic League Management**:
  - Manage multiple leagues across various league formats (e.g., 1v1, 3v3, 5v5, 20v20).
  - Flexibly create and manage leagues of the same or different types.

- **Team Management**:
  - Commands to register, remove, or adjust team settings within leagues.

- **Challenge System**:
  - Teams can issue challenges to climb ranks based on dynamic ladder rules.
  - Admin controls for overriding or canceling challenges as needed.

- **Git Backup System**:
  - Seamlessly backs up JSON data to a GitHub repository for reliable storage and easy recovery.

- **Embed-Based Responses**:
  - Enhanced readability through rich Discord embed formatting.

---

## Rules of the Ladder System

### Team Rank Assignments
1. **New Teams**:
   - Assigned ranks based on creation order within a league.

2. **Rank Adjustments**:
   - Teams can challenge up to two ranks above them and swap ranks based on match outcomes.

3. **Challenge Limitations**:
   - Teams are limited to one active challenge at a time.

---

## Setting Up Ladderbot4

### Discord Bot Token
The bot requires a valid Discord Bot Token for operation. During the setup process, invalid tokens will prompt an error message, and you'll need to provide the correct token.

### Git Backup Integration
To use the Git backup feature, you'll need:
- A **GitHub Personal Access Token (PAT)**.
- The **HTTPS link** to your GitHub repository.

Upon setup, the bot initializes by cloning the repository and using the JSON files as its initial database.

[Detailed Setup Instructions](https://github.com/Theinfection91/Ladderbot4/blob/main/Ladderbot4Doc.md)
