# Ladderbot4

## Overview
**Ladderbot4** is a robust and feature-rich Discord bot designed for managing competitive ladders in various divisions. This C# implementation significantly enhances performance and maintainability over its predecessor, Ladderbot3.0, leveraging dependency injection, a more modular architecture, and improved command handling like **SlashCommands** to add a more interactive experience and reduce errors.

SlashCommand Example 1: ![SlashCommand Example 1](examples/example1.png)

SlashCommand Example 2: ![SlashCommand Example 2](examples/example2.png)

Ladderbot4 is still under active development, but it offers a stable foundation with exciting new features and improvements to come over previous versions. Feedback and suggestions are always welcome to further refine and expand its capabilities.

For any questions, suggestions, or issues, feel free to submit them via the **Issues** tab or message me on Discord at `Theinfection1991`.

---

## Rules of the Ladder System

### Team Rank Assignments
1. **New Teams**:
   - Teams are assigned ranks based on the order they are created in a division.
     - First team: Rank 1.
     - Second team: Rank 2.
     - Subsequent teams are assigned incrementally lower ranks.
   
2. **Divisions**:
   - The bot supports concurrent ladders for the following divisions:
     - **1v1**
     - **2v2**
     - **3v3**

### Challenges and Rank Changes
1. **Who Can Be Challenged**:
   - Teams **cannot challenge below their rank**.
   - Teams may only challenge **up to 2 ranks above them**.
     - Example: Rank 4 can challenge Rank 3 and Rank 2, but not Rank 1.

2. **Challenge Outcomes**:
   - **If the challenger wins**:
     - The challenger team takes the rank of the defeated team.
     - The defeated team is pushed **down one rank**, and all other teams below them are adjusted accordingly.
     - Example:
       - Rank 4 defeats Rank 2 → Rank 4 becomes Rank 2, Rank 2 becomes Rank 3, and Rank 3 becomes Rank 4.
   - **If the challenger loses**:
     - No rank changes occur.
     - The standings remain the same.

3. **Rank Swapping**:
   - If the challenger defeats the team directly above them, their ranks are swapped.
     - Example: Rank 4 defeats Rank 3 → Rank 4 becomes Rank 3, and Rank 3 becomes Rank 4.

4. **Challenge Limitations**:
   - Teams may only have **one active challenge** at a time.

---

## Features

### Key Functionalities:

- **Ladder Management**:
  - Admins can start and end ladders for specific divisions (e.g., 1v1, 2v2, 3v3).

- **Team Management**:
  - Admin commands for registering and removing teams in specific divisions.

- **Challenge System**:
  - Allows teams to issue challenges, manage their status, and track results.
  - Admin overrides for manually creating or canceling challenges.

- **Match Reporting**:
  - Command-based reporting for match results that automatically update standings and rankings.
  - Admin commands to adjust results, ranks, and other metrics manually.

- **Standings, Challenges, Teams Channels**:
  - Post real-time standings and active challenges to designated channels.
  - Automatically update standings and challenge information.

- **Administrative Controls**:
  - Comprehensive admin controls for managing ladders, teams, ranks, wins, and losses.

---

## Planned Features

1. **Stat Tracking and Achievements**:
   - Track team and individual stats, possible stat ideas:
     - Number of matches played.
     - Rank changes during a ladder's lifetime.
     - Player achievements, such as participating in a champion team.
   - Implement achievements tied to Discord IDs and store data in the bot's database.

2. **Match History**:
   - Storing previous match results in the database to cross reference for later use in case messages or channels are deleted. I also plan to use Match History for writing tests.

3. **Logging System** (Enhanced):
   - Plans to integrate bot commands to filter and view logs dynamically in Discord.
   - Logs command invocations, parameters, and errors
     
5. **Creative Additions**:
   - Open to suggestions for new features or improvements to the bot's current functionality.

---

## Setting Up Ladderbot4

### Discord Bot Token
**Ladderbot4** now has a setup process when ran that checks the Discord Bot Token length and has you enter it in into the Command Line Interface if it isn't a certain amount of characters. If a token of correct length is entered but is actually incorrect, then manually change it in Settings/config.json for now until I finish the setup process completely. The setup also asks for the correct Guild ID and gives a list showing all servers the bot is connected from. Enter in the correct Guild ID for your Server to enable Slash Commands. If an incorrect one is entered, you can use /set_guild_id or /sgid as a regular command in discord after the bot runs to dynamically grab the Id, then restart the bot and Slash Commands should begin working for that Server.
