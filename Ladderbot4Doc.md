# Ladderbot4 Documentation

Welcome to the **Ladderbot4 Documentation**! This guide is designed to help users and administrators set up and fully utilize Ladderbot4 for managing competitive ladders and tournaments within a Discord server. Whether you're configuring the bot for the first time or using its extensive features, this documentation will provide clear instructions and examples to guide you every step of the way.

## Features
Ladderbot4 offers powerful tools for managing competitive ladders, including:
- Dynamic team management and ranking systems.
- Automated challenge and match result reporting.
- Comprehensive statistics tracking for teams and individual members.
- Achievements, historical match data, and much more!

## Getting Started
This documentation will guide you through the following essential setup steps:
1. **Configuring the Bot**:
   - Setting up the bot's Discord token.
   - Configuring the GitHub Personal Access Token (PAT).
   - Providing the Git HTTPS URL path for backups.
   - Setting the Guild ID to enable slash commands.

2. **Using Commands**:
   - Overview of user commands for interacting with ladders and teams.
   - Explanation of admin commands for advanced configuration and management.
   - Detailed mechanics for each command, including examples and expected outputs.

---

With this guide, you'll be ready to set up Ladderbot4 in your Discord server and make full use of its capabilities. Let's get started!

---

## Configuring The Bot
How to set the Discord Token, Git PAT Token and HTTPS Url Path, and the Guild ID for SlashCommands:

---

## Using Commands
Extensive information and how to use the various commands in Ladderbot4:

## Team Slash Commands

### `/team register`

**Description**:  
Registers a new team in the specified division. This command can only be used by administrators.

#### **Command Syntax**:

```/team register <teamName> <division> <member1> [member2] [member3]```

#### **Parameters**:
- **teamName**: The unique name of the team to register.
- **division**: The division type: `1v1`, `2v2`, or `3v3`.
- **member1**: The first member of the team.
- **member2** (optional): The second member, required for `2v2` and `3v3`.
- **member3** (optional): The third member, required for `3v3`.

#### **How It Works**:
1. Validates the team name and division type.
2. Confirms the correct number of members for the division.
3. Ensures members are not already in another team in the same division.
4. Creates the team, assigns it a rank, and saves it to the database.
5. Provides a success or error response.

#### **Example**:
**Command**:  

```/team register "Team Phoenix" 2v2 @Player1 @Player2```

**Response**:  
A success embed showing:
- Team Name: Team Phoenix
- Division: 2v2
- Rank: Assigned automatically
- Members: Player1, Player2

---
