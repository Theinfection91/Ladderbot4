# Ladderbot4

## Documentation
[Extensive Documentation Found Here](https://github.com/Theinfection91/Ladderbot4/blob/main/Ladderbot4Doc.md)

## Releases
[Download Latest Version Here](https://github.com/Theinfection91/Ladderbot4/releases)

---

# Ladderbot 4.2.0 - What's New

## Simple Overview
### (Merge 0)
The first update to the **4.2.0 branch** introduces significant changes to support flexible league configurations and lays the groundwork for enhanced data handling. These changes include the decoupling of team sizes for leagues (XvX), a new `Data` class structure, and the replacement of several JSON files for improved organization and scalability. This marks the first step toward the fully realized 4.2.0 update.

### (Merge 1)
The second update to the **4.2.0 branch** introduces the **Modal Confirmation System**, which enhances critical operations by requiring users to confirm sensitive actions twice, ensuring case-sensitive accuracy. This system is applied to key operations like starting, ending, deleting leagues, and removing teams. Additional improvements include a more streamlined approach to handling the ladder lifecycle, with enhancements like resets for team stats at the start of a ladder and new results embeds at the end. A reusable handler class (`ModalInteractionHandlers`) was also created for better maintainability and scalability of modal processes. This update sets the stage for more interactive components in future releases.

### (Merge 2)
The third update introduces **Dynamic Challenge Rank Updates**, a feature that ensures challenge ranks remain accurate even after other teams report wins, causing rank changes. It also includes new methods for challenge rank validation and correction, improving the integrity of challenge data.

### (Merge 3)
The fourth update addresses **Duplicate Automated Messages** and improves JSON file handling, resolving issues where states were not properly loading after being cloned from the repository. Automated message processes now properly save and retrieve message IDs, preventing duplicates and ensuring a smoother user experience.

### (Merge 4)

The fifth update introduces a comprehensive custom autocomplete process for Ladderbot commands. The system dynamically pulls and filters relevant options, such as league names and team names, ensuring efficient and user-friendly command execution.

### (Merge 5)
The sixth update reimplements **Member Stats** and adds **Member Leaderboards**, introducing robust support for tracking individual member statistics and adding new commands for personalized data retrieval and leaderboards. Key features include `members_list.json` creation, real-time validation and updating of member profiles, and new methods for integrating stats into league operations.

### (Merge 6)
The seventh update to the **4.2.0 branch** introduces a **Level, XP, and Title System**, adding an engaging progression mechanic for members participating in league activities. Members now earn XP for various actions, including joining teams, participating in matches, and completing ladders. XP contributes to leveling up, unlocking unique titles that showcase achievements and milestones.

---

## (Merge 6 - Detailed) - (01/25/2025)

### Level, XP, and Title System

#### **Earning XP**
Members now gain XP for participating in league activities, encouraging consistent engagement and rewarding performance. The following activities contribute XP:

- **Joining or Retaining Teams**:
  - Small XP boost for registering as part of a new team or staying on a team when a ladder restarts.
- **Participating in Matches**:
  - **Win**: Moderate XP reward for the winning team.
  - **Loss**: Small XP reward for the losing team to encourage participation.
- **Winning a Ladder**:
  - Large XP reward for members of the championship team.
- ** Second or Third Place**:
- Moderate and Small XP reward for second and third place teams
- **Ladder Completion**:
  - Base XP awarded to all participants at the end of the ladder to celebrate their involvement.

#### **Titles and Progression**
Each member level unlocks a unique title, offering a sense of accomplishment. Titles are thematically designed and reflect progression milestones. Initial title levels include:

- **Level 1**: Novice  
- **Level 3**: Apprentice  
- **Level 5**: Challenger  
- **Level 7**: Contender  
- **Level 9**: Elite  
- **Level 12**: Champion  
- **Level 15**: Master  
- **Level 20**: Legend  

#### **Key Changes**
- **New Member Profile Fields**:
  - Added `Level`, `Experience`, and `Title` fields to member profiles.
- **Level Scaling**:
  - XP required to level up scales dynamically with a formula:  
    `50 * level^1.2`, ensuring a steady progression curve.
- **LevelGuide Class**:
  - Introduced a `LevelGuide` class to manage XP values for specific actions and title unlocks.

#### **Integration into League Operations**
- **XP Distribution Events**:
  - **Team Registration**: Awards XP when members join or retain teams during ladder restarts.
  - **Match Results**: Awards XP to winners and losers when a match is reported.
  - **Ladder Completion**: Awards XP to all participants, with bonus XP for first, second, and third-place teams.
- **Embeds Updated**:
  - Added XP, level, and title information to embeds for improved visibility and engagement.

#### **Future Enhancements**
- **Milestone Rewards**:
  - Potential to add milestone XP bonuses for major achievements, such as 100 matches played.
- **Dynamic Titles**:
  - Expansion of title themes and additional levels to support long-term progression.

---

## (Merge 5 - Detailed) - (01/22/2025)

### Member Stats Rework
- **New MembersListData Class**:
  - Introduced `MembersListData` to manage individual member profiles and stats.
  - Created methods to add members to teams, and update stats like wins, losses, matches, and league participation.
  - Data is now stored in `members_list.json`, ensuring persistence and scalability.

- **Validation and Synchronization**:
  - Added a `ValidateMembersListData` method to synchronize data at startup:
    - Checks all Discord IDs registered to leagues.
    - Automatically creates a profile for any missing member.
  - Ensures consistent data integrity across leagues and teams.

### Command Enhancements
- **New Member Commands**:
  - **`/member mystats`**:
    - Displays a personalized embed with the user's stats (e.g., wins, losses, total seasons).
    - Stats are now updated dynamically during league operations.
  - **`/member leaderboard`**:
    - Displays the top 25 members based on performance metrics.
    - Ephemeral message ensures user privacy during leaderboard retrieval.

- **Team Member Validation**:
  - Enhanced processes for adding members to teams:
    - Validates if a member is already on a team in the league.
    - Ensures team member limits are respected (e.g., for large teams of 21+ members).

### League Integration
- **Stat Tracking in League Operations**:
  - **Ending Ladders**:
    - League Championship stat increments for members of first-place teams.
    - Total Seasons stat updates for all league participants.
  - **Starting Ladders**:
    - Introduced logic to ensure persistent teams across seasons still update member stats correctly.
  - **Team Registration**:
    - Automatically registers members to profiles during team creation.

### Bug Fixes and Miscellaneous Updates
- **Stat Calculation Adjustments**:
  - Replaced "Total Teams" stat with "Total Seasons," reflecting the number of seasons completed by a member.
  - Ensured accurate stat updates when teams persist across multiple seasons.
- **Modal Confirmation Fixes**:
  - Updated `/member mystats` command to use ephemeral responses for better UX.

---

## (Merge 4 - Detailed) - (01/19/2025)

### League Name Autocomplete
- `/team register` now supports a custom autocomplete system for the `league_name` parameter:
  - Retrieves all league names dynamically.
  - Displays league names sorted alphabetically, filtering results as the user types.
  - Each league name is displayed with its format for better clarity (e.g., "1v1," "2v2").

### Team Name Autocomplete
- Commands now include autocomplete for team names with additional contextual details:
  - **`/challenge` and `/challenge admin`**:
    - Fetches team names dynamically for both `challenger_team` and `challenged_team` parameters.
    - Displays team names with their associated leagues for better usability.
  - **`/cancel`**:
    - Added autocomplete for both user-level and admin-level commands, ensuring accurate team selection when canceling challenges.
  - **Future Suggestions**:
    - Filter challenge-related options to show only teams eligible for the action.
    - Incorporate user-specific context to display relevant league/team options.

---

## (Merge 3 - Detailed) - (01/18/2025)

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

## (Merge 2 - Detailed) - (01/17/2025)

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

## (Merge 1 - Detailed) - (01/14/2025)

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

## (Merge 0 - Detailed) - (01/13/2025)

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
