# TeamWinProject

## Sudoku App

The main idea of the Sudoku App is provide to users the ability to solve sudoku puzzles of several difficulty levels and compete with other users in solving them.

## Functional specification
The app should provide the following functionality:
* Generating sudoku puzzles
* Possibility to choose a puzzle difficulty level
* Single game mode
* Duel game mode (two players solve the puzzle at the same time)
* Free game mode (results reached in this game mode will not be recorded)
* User personal puzzle solving statistics
* Global ranking (users can compare their results with other players)
* User profile editing and deleting
* Simple and clear user interface

## Identity Management
For identity management, we will use Firebase Authentication.
It has a simple API, has integration with Flutter and .NET, and provides several sign-in providers. Also, Firebase provides a lot of other useful services as Analytics A/B testing, and so on.

## General Architecture Information
The app will have the classic client-server model. Here is common architecture diagram: ![common architecture diagram](https://github.com/VladChemerskyi/TeamWinProject/blob/main/docs/common_architecture_diagram.png?raw=true)

The mockups of future app are in separate Folder (*docs/Mockups*)

Database scheme: https://dbdiagram.io/d/617b186cfa17df5ea67414c1
