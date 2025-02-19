# GC12 AutoClicker

GC12 AutoClicker is a powerful and versatile Windows desktop application built with WPF (Windows Presentation Foundation) that allows you to automate mouse clicks. It offers multiple clicking modes, customizable hotkeys, and a robust macro system for creating and executing complex click sequences.  It's designed to be user-friendly while providing advanced features for power users.

## Features

*   **Multiple Click Modes:**
    *   **Current Position:** Clicks at the current mouse cursor location.
    *   **Specific Position:** Clicks at fixed X and Y coordinates that you specify.
    *   **Macro:** Executes a predefined sequence of clicks, each with its own coordinates, duration, delay, and repetition count.

*   **Customizable Hotkeys:**
    *   **Start/Stop Hotkey (Default: F8):**  Starts and stops the clicking process.  You can change this hotkey in the application.
    *   **Capture Position Hotkey (Default: F5):**  Captures the current mouse cursor coordinates and sets them as the specific X and Y coordinates for clicking.  This is useful for quickly setting click locations.
    *   **Macro Capture Position Hotkey (F5):** *Within the Macro Creation/Editing window*, pressing F5 captures the current cursor position and populates the X and Y fields of the action being created/edited.
    *   **Add Action Hotkey (F9):**  *Within the Macro Creation/Editing window*, pressing F9 adds a new action (or edits the selected action) to the macro, using the values in the input fields.

*   **Macro System:**
    *   **Create, Edit, Load, and Export Macros:**  Easily create new macros, modify existing ones, load macros from `.gcam` files, and export macros to `.gcam` files.
    *   **Individual Macro Export:**  You can export individual macros. The exported file will have the name of the macro.
    *   **Macro Actions:** Each macro action consists of:
        *   **X and Y Coordinates:**  The location of the click.
        *   **Click Duration:** How long the mouse button is held down (in milliseconds).
        *   **Delay:** The time to wait *after* the click (and after any repetitions) before the next action (in milliseconds).
        *   **Repetitions:**  The number of times to repeat the click action *before* moving to the next action in the macro.
        *   **Action Reordering and Deletion:** Easily change the order of actions within a macro, or delete actions.

*   **Click Interval:**  Set the time delay between clicks (in milliseconds) for the Current Position and Specific Position modes.

*   **Click Type:** Choose between Left, Right, and Middle mouse button clicks.

*   **Number of Clicks:** Specify the number of clicks to perform (or set to 0 for infinite clicking).

*   **User-Friendly Interface:**  The WPF interface provides clear controls and visual feedback.

*   **Cursor Position Display:**  The application continuously displays the current X and Y coordinates of the mouse cursor.

*   **Instructions:** Built-in instructions are available via "Instructions" buttons in the main window and the macro creation/editing window.

## Installation

1.  **Download:** Download the latest release ZIP archive from the [Releases](https://github.com/GDcocos12/GC12-AutoClicker/releases) page.
2.  **Extract:** Extract the contents of the ZIP archive to a folder on your computer.
3.  **Run:** Double-click the `GC12_AutoClicker.exe` file to launch the application.  No installation is required; it's a standalone executable.

## Usage

### Basic Clicking (Current and Specific Position)

1.  **Set the Click Interval:** Enter the desired time between clicks (in milliseconds) in the "Interval (ms)" field.
2.  **Choose a Position Mode:**
    *   Select "Current" to click at the current cursor position.
    *   Select "Specific" to click at fixed coordinates.  Enter the X and Y coordinates in the corresponding fields.  Use the F5 hotkey to capture the current cursor position.
3.  **Set the Number of Clicks:** Enter the desired number of clicks in the "Clicks" field.  Use `0` for infinite clicks.
4.  **Select the Click Type:** Choose "Left", "Right", or "Middle" from the "Click Type" dropdown.
5.  **Enable Hotkey (Optional):** Click "Enable Hotkey" to activate the Start/Stop hotkey (default: F8).  You can change the hotkey by clicking in the "Hotkey" textbox and pressing the desired key.
6.  **Start Clicking:** Click the "Start" button or press the Start/Stop hotkey.
7.  **Stop Clicking:** Click the "Stop" button or press the Start/Stop hotkey.

### Macro Usage

1.  **Create a New Macro:**
    *   Click the "Create" button next to the Macro selection box.
    *   Enter a name for your macro in the "Macro Name" field.
    *   Add actions to the macro:
        *   Enter the X and Y coordinates, click duration, delay, and repetitions for each action.
        *   Click "Add Action" (or press F9) to add the action to the list.
        *   Use F5 to capture the current cursor position into the X and Y fields.
        *   Select an action in the list to edit it.  Click "Add Action" to save the changes.
        *   Use "Move Up", "Move Down", and "Delete Action" to manage the action list.
    *   Click "Save" to save the macro.
2.  **Load Existing Macros:**
    *   Click the "Load" button.
    *   Select a `.gcam` file containing your macros.  You can load either a file containing a single macro or a file containing multiple macros.
3.  **Select a Macro:** Choose the desired macro from the "Macro" dropdown list.
4.  **Edit a Macro:** Select the macro you want to edit, and then click the "Edit" button. This reopens Macro Creation/Editing Window.
5.  **Export a Macro:** Select the macro you want to export, and then click the "Export" button.  The macro will be saved to a `.gcam` file with the macro's name.
6. **Export all Macros:** Click "Export All Macros" button.
7.  **Start/Stop Macro Execution:**  Use the "Start" and "Stop" buttons (or the hotkey) just like with the other clicking modes. The selected macro's actions will be executed repeatedly until stopped.

## Building from Source

1.  **Clone the Repository:**
    ```bash
    git clone https://github.com/GDcocos12/GC12-AutoClicker.git
    ```
2.  **Open in Visual Studio:** Open the `GC12_AutoClicker.sln` file in Visual Studio (2019 or later recommended).
3.  **Build:** Build the solution (Build -> Build Solution).  The executable will be created in the `bin\Release` or `bin\Debug` folder.

## Dependencies

*   .NET Framework (version 4.7.2 or later is recommended, but it should work with 4.6.2 and above).  The correct version is likely already installed on most Windows systems.  If not, the application will prompt the user to install it.
*   System.Text.Json (included with .NET 5+; for older frameworks, it's usually included via NuGet).

## License

This project is licensed under the [MIT License](LICENSE) - see the `LICENSE` file for details.
