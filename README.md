# Ladderbot4

## Documentation
[Extensive Documentation Found Here](https://github.com/Theinfection91/Ladderbot4/blob/main/Ladderbot4Doc.md)

## Releases
[Download Latest Version Here](https://github.com/Theinfection91/Ladderbot4/releases)

---

## Overview
**Ladderbot4** is an advanced Discord bot designed for competitive ladder management, enabling dynamic league creation, flexible division handling, and a robust user experience. Built in C#, it offers powerful tools for managing team-based tournaments with features like Git-based backup storage, SlashCommands, and a dynamic ladder-based ranking system.

This bot uses a **ladder tournament system** where teams challenge others to climb ranks, creating a continuously evolving competitive environment that rewards consistent performance.

---

## What's New in v4.1.1

### Feature Enhancements

### Git Backup Rework
- **Interactive Database Overwrite Prompt**:
  - During the cloning process, the bot now prompts users to decide whether to overwrite the local database with the cloned backup repository:
    ```plaintext
    [DateTime] - GitBackupManager - Do you want to use the newly cloned backup data from the repository as your Database? 
    Yes is typically the answer here. NOTE - This will overwrite data currently present in your JSON files in 'Database'. This can not be reversed.
    
    HINT: If the files in your backup repo online are more up-to-date than your local files in the 'Database' folder, input Y.
    If your JSON files in the 'Database' folder are more up-to-date than the files in your backup repo online, input N.
    ```
  - Acceptable inputs:
    - `Y`: Copies the newly cloned files from `BackupRepo` to the `Database` folder.
    - `N`: Keeps the local files intact. Users are warned that this may cause discrepancies if the local data is outdated.
    - Invalid inputs are met with a retry prompt.

- **Backup Manager Trigger Fix**:
  - Resolved an issue where the `BackupManager` was not triggered when channel IDs were set (e.g., `/set standings_channel_id`). Now, `states.json` is backed up immediately after such changes.

### Post Leagues Command
- **New `/post leagues` Command**:
  - Posts all leagues in the database, displaying each league and the teams within it.
  - Optionally, filter by division type (e.g., `1v1`, `2v2`, `3v3`) to show leagues of a specific type.
  - Useful for quickly sharing league data in Discord channels with clear, formatted embeds.
  - Will be adding the ability to assign this to a channel like standings/challenges/teams in future updates.

### Administrative Updates
- **Admin Privileges Required**:
  - Running the bot now requires **Administrator permissions** to prevent potential errors during operations.

- **Embedded Debugging Information**:
  - The .pdb file is now embedded into the .exe and .dll files, streamlining deployments and reducing clutter.
    
- **Streamlined Git Cloning Workflow**:
  - Enhanced user clarity during the backup cloning process to prevent unintended data overwrites or loss.

### Bug Fixes
- **Challenge Restrictions**:
  - Fixed a bug where admins could adjust team ranks while challenges were active. The bot now blocks this action and provides instructions to resolve or cancel the challenge first.

- **Improved Backup Logic**:
  - Addressed inconsistencies in event-driven backups to ensure all relevant data changes are saved to the online repository.

- **General Stability Improvements**:
  - Various performance optimizations and minor bug fixes for smoother operations.
    
---

## Features

### Core Functionalities:
- **Dynamic League Management**:
  - Manage multiple leagues across various division types (e.g., 1v1, 2v2, 3v3).
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
