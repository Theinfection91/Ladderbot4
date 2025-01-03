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

### Git Backup Rework
- **Seamless Multi-Machine Integration**:
  - JSON data cloned from the backup repository now serves as the initial database when the bot is set up with the same token and Git configuration across different machines.
  - This ensures seamless continuity and data integrity in multi-machine setups.
  
- **Streamlined Backup Initialization**:
  - The bot automatically fetches and applies the latest backup during the setup process, minimizing manual configuration.

### Administrative Changes
- **Admin Privileges Required**:
  - Running the bot now requires **Administrator permissions** to prevent potential errors during operations.

- **Embedded Debugging Information**:
  - The `.pdb` file is now embedded into the `.exe` and `.dll` files, streamlining deployments and reducing clutter.

### Bug Fixes
- **Challenge Restrictions**:
  - Fixed an issue where admins could adjust team ranks while a challenge was active. The bot now prevents this action and sends an error embed instructing users to resolve or cancel the challenge first.
  
- **General Stability Improvements**:
  - Various minor bugs and inconsistencies have been resolved to improve overall performance and reliability.

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
