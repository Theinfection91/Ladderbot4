# Ladderbot4
## Releases
[Download v4.1.0-Ladderbot4 for Windows x64](https://github.com/Theinfection91/Ladderbot4/releases/download/v4.1.0/v4.1.0-Ladderbot4-win_x64.zip)

## Overview
**Ladderbot4** is an advanced, feature-rich Discord bot designed to manage competitive ladders across various leagues of different division types with enhanced flexibility, scalability, and user interaction. Built in C#, this version builds on the foundation of **Ladderbot3.0**, emphasizing modularity, performance, and a more engaging user experience.

The bot also includes an intuitive and dynamic **ladder tournament system** designed to provide a continuous ranking experience. Unlike traditional tournament formats (e.g., single elimination or best-of matches), **Ladderbot4** uses a **ladder-based** system where teams can challenge one another based on rank. Teams climb the ladder by defeating those above them, while teams that lose are pushed down, creating a constantly shifting and dynamic ranking environment. This structure ensures a continuous competition where the focus is on consistent performance over time, not a single match outcome.

### **(NEW 12-28-2024)** Introducing the "LeagueMatrix" v4.1.0 Update for Ladderbot4
This update introduces the **LeagueMatrix**, a major overhaul that allows **multiple leagues** to be managed simultaneously with each one being a different division (1v1, 2v2, or 3v3). The **LeagueMatrix** enables greater flexibility by decoupling division and league management, allowing administrators to easily create, manage, and update multiple leagues within the same division type if needed. This development is perfect for large environments with many teams, offering improved scalability and smoother operations.

### SlashCommands
In addition, **Ladderbot4** now features **SlashCommands**, streamlining user interactions, reducing errors, and providing a more intuitive experience. Unlike previous versions, the bot now sends **Discord embeds** in responses rather than plain text, enhancing readability and user engagement.

![SlashCommand Example 1](examples/example1.png)

![SlashCommand Example 2](examples/example2.png)

With the **LeagueMatrix** update, **Ladderbot4** significantly enhances its administrative controls, team management, and challenge systems, making it even easier for admins to track rankings, manage teams, and handle challenges dynamically. While **Ladderbot4** is still under active development, it provides a solid foundation with many exciting new features and plans for future improvements.

For any questions, suggestions, or issues, feel free to submit them via the **Issues** tab or message me on Discord at `Theinfection1991`.

---

## Features

### New/In-Progress Functionalities:

- **(NEW) LeagueMatrix Update (Implemented)**:
  - **Ladderbot4** introduces the **LeagueMatrix** update, a feature designed to significantly improve the management and flexibility of competitive ladders.
  - This update enables the bot to handle **multiple leagues** simultaneously, each representing different divisions (e.g., 1v1, 2v2, 3v3) without being limited by static divisions. Each league can now have its own set of teams, standings, and challenges.
  - **LeagueMatrix** will allow admins to:
    - Create and manage multiple leagues of the same division type (e.g., multiple 1v1 leagues).
    - Dynamically adjust the system for better scalability as the number of leagues and teams increases.
    - Easily add, remove, or update leagues on the fly.
  - This new architecture provides greater flexibility for managing competitive environments and scaling up to support more users and teams.

- **(NEW) Git Backup Storage (Implemented And Working, Read [Documentation](https://github.com/Theinfection91/Ladderbot4/blob/main/Ladderbot4Doc.md) for Full Instructions)**:
  - The ability to use an owned GitHub Repo as a backup storage hub for the .json database files that are used for teams, challenges, states, and history.
  
- **(NEW) Embed Output (Implemented)**:
  - Previous versions only used strings in Markdown format. **Ladderbot4** now returns embed objects as output to improve the overall readability for users.

### Returning Features from Ladderbot3.0:

- **Ladder Management**:
  - Admins can start and end ladders for specific leagues.

- **Team Management**:
  - Admin commands for registering and removing teams in specific leagues.

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

1. **Stat Tracking and Achievements** (Began Background Work):
   - Track team and individual stats, possible stat ideas:
     - Number of matches played.
     - Rank changes during a ladder's lifetime.
     - Player achievements, such as participating in a champion team.
   - Implement achievements tied to Discord IDs and store data in the bot's database.

2. **Logging System** (Enhanced):
   - Plans to integrate bot commands to filter and view logs dynamically in Discord.
   - Logs command invocations, parameters, and errors.

3. **Match History**:
   - Storing previous match results in the database to cross-reference for later use in case messages or channels are deleted. Match history will also be used for writing tests.

4. **Creative Additions**:
   - Open to suggestions for new features or improvements to the bot's current functionality.

---

## Rules of the Ladder System

### Team Rank Assignments
1. **New Teams**:
   - Teams are assigned ranks based on the order they are created in a league.
     - First team: Rank 1.
     - Second team: Rank 2.
     - Subsequent teams are assigned incrementally lower ranks.
   
2. **Leagues and Divisions**:
   - The bot creates leagues that supports ladder tournaments for the following divisions:
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

## Setting Up Ladderbot4

### Discord Bot Token
**Ladderbot4** now has a setup process that checks the Discord Bot Token length. If the token isn't the correct length, it prompts you to enter a valid token in the Command Line Interface. If a token of correct length is entered but is actually incorrect, you will need to manually change it in `Settings/config.json` until the setup process is fully completed.

### Guild ID for SlashCommands 
The setup process also asks for the correct Guild ID and provides a list showing all servers the bot is connected to. Enter the correct Guild ID for your server to enable Slash Commands. If an incorrect one is entered, you can use `/set_guild_id` or `/sgid` as a regular command in Discord after the bot runs to dynamically grab the ID. Then restart the bot, and Slash Commands should begin working for that server.

### Git Backup Feature
During setup, you'll be prompted to use an owned GitHub repo as a backup system for all the JSON database files. The setup process will ask for a **GitHub Personal Access Token (PAT)**, which you can generate through GitHub, and the **HTTPS link** to your GitHub repository.

[Read Documentation For More Help](https://github.com/Theinfection91/Ladderbot4/blob/main/Ladderbot4Doc.md)
