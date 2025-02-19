using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GC12_AutoClicker
{
    public partial class MacroCreationWindow : Window
    {
        public Macro Macro { get; private set; }
        private bool _isEditing = false;

        public MacroCreationWindow(Macro macroToEdit = null)
        {
            InitializeComponent();

            if (macroToEdit != null)
            {
                Macro = new Macro
                {
                    Name = macroToEdit.Name,
                    Actions = new ObservableCollection<MacroAction>(macroToEdit.Actions.Select(a => new MacroAction
                    {
                        X = a.X,
                        Y = a.Y,
                        ClickDuration = a.ClickDuration,
                        Delay = a.Delay,
                        Repetitions = a.Repetitions
                    }))
                };
                MacroNameTextBox.Text = macroToEdit.Name;
                _isEditing = true;
                Title = "Edit Macro";
                SaveButton.Content = "Save Changes";
            }
            else
            {
                Macro = new Macro();
                Title = "Create Macro";
                SaveButton.Content = "Save";
            }

            DataContext = this;
            ActionsListView.ItemsSource = Macro.Actions;

            ActionsListView.SelectionChanged += ActionsListView_SelectionChanged;

            this.KeyDown += MacroCreationWindow_KeyDown;
        }

        private void MacroCreationWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F9)
            {
                AddActionButton_Click(this, new RoutedEventArgs());
            }
        }

        private void AddActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(XTextBox.Text, out int x) ||
                !int.TryParse(YTextBox.Text, out int y) ||
                !int.TryParse(DurationTextBox.Text, out int duration) || duration < 0 ||
                !int.TryParse(DelayTextBox.Text, out int delay) || delay < 0 ||
                !int.TryParse(RepetitionsTextBox.Text, out int repetitions) || repetitions <= 0)
            {
                MessageBox.Show("Please enter valid values for all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ActionsListView.SelectedItem == null)
            {
                Macro.Actions.Add(new MacroAction { X = x, Y = y, ClickDuration = duration, Delay = delay, Repetitions = repetitions });
            }
            else //Edit selected
            {
                MacroAction selectedAction = (MacroAction)ActionsListView.SelectedItem;
                selectedAction.X = x;
                selectedAction.Y = y;
                selectedAction.ClickDuration = duration;
                selectedAction.Delay = delay;
                selectedAction.Repetitions = repetitions;

                int index = Macro.Actions.IndexOf(selectedAction);
                if (index != -1)
                {
                    Macro.Actions[index] = selectedAction;

                    var updatedActions = new ObservableCollection<MacroAction>(Macro.Actions);
                    ActionsListView.ItemsSource = updatedActions;
                    Macro.Actions = updatedActions;
                    ActionsListView.SelectedItem = selectedAction;
                }

            }

            XTextBox.Clear();
            YTextBox.Clear();

        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MacroNameTextBox.Text))
            {
                MessageBox.Show("Please enter a macro name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Macro.Actions.Count == 0)
            {
                MessageBox.Show("Please add actions to the macro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Macro.Name = MacroNameTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ActionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActionsListView.SelectedItem != null)
            {
                MacroAction selectedAction = (MacroAction)ActionsListView.SelectedItem;
                XTextBox.Text = selectedAction.X.ToString();
                YTextBox.Text = selectedAction.Y.ToString();
                DurationTextBox.Text = selectedAction.ClickDuration.ToString();
                DelayTextBox.Text = selectedAction.Delay.ToString();
                RepetitionsTextBox.Text = selectedAction.Repetitions.ToString();
            }
            else
            {
                XTextBox.Clear();
                YTextBox.Clear();
                DurationTextBox.Clear();
                DelayTextBox.Clear();
                RepetitionsTextBox.Clear();
            }
            UpdateButtonsState();
        }

        private void InstructionsButton_Click(object sender, RoutedEventArgs e)
        {
            string instructions =
                "Macro Creation/Editing Instructions:\n\n" +
                "1. Macro Name: Enter a name for your macro.\n" +
                "2. Actions List: Displays the sequence of actions in the macro.\n" +
                "3. X, Y: Enter the coordinates for each click.\n" +
                "4. Duration (ms): How long the mouse button should be held down for each click.\n" +
                "5. Delay (ms): The time to wait *after* the click (and after any repetitions) before the next action.\n" +
                "6. Repeats: Number of times to repeat the click.\n" +
                "7. Add Action: Adds the defined action to the macro, or edits selected action.\n" +
                "   - To Edit an Action:\n" +
                "     - Click on the action in the Actions List.\n" +
                "     - The X, Y, Duration, Delay, and Repeats fields will be populated with the selected action's values.\n" +
                "     - Modify the values as needed.\n" +
                "     - Click \"Add Action\" to update the selected action with the new values.\n" +
                "8. Capture Position (F5): Press F5 while this window is active to capture the current cursor coordinates and fill in the X and Y fields.\n" +
                "9. Save/Save Changes: Saves the new macro or updates the existing macro.\n" +
                "10. Cancel: Discards changes and closes the window.\n" +
                "------------------\n" +
                "Made by GDCocos12";
            MessageBox.Show(instructions, "Instructions", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActionsListView.SelectedItem != null)
            {
                Macro.Actions.Remove((MacroAction)ActionsListView.SelectedItem);
            }
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActionsListView.SelectedItem != null)
            {
                int selectedIndex = ActionsListView.SelectedIndex;
                if (selectedIndex > 0)
                {
                    Macro.Actions.Move(selectedIndex, selectedIndex - 1);
                    ActionsListView.SelectedIndex = selectedIndex - 1;
                }
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActionsListView.SelectedItem != null)
            {
                int selectedIndex = ActionsListView.SelectedIndex;
                if (selectedIndex < Macro.Actions.Count - 1)
                {
                    Macro.Actions.Move(selectedIndex, selectedIndex + 1);
                    ActionsListView.SelectedIndex = selectedIndex + 1;
                }
            }
        }

        private void UpdateButtonsState()
        {
            bool isItemSelected = ActionsListView.SelectedItem != null;
            DeleteActionButton.IsEnabled = isItemSelected;
            MoveUpButton.IsEnabled = isItemSelected;
            MoveDownButton.IsEnabled = isItemSelected;
        }
    }
}
