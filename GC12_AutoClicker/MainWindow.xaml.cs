using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GC12_AutoClicker
{
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private bool _isClicking = false;
        private CancellationTokenSource _cts;
        private int _clickInterval = 100;
        private int _xPos = 0;
        private int _yPos = 0;
        private ClickMode _clickMode = ClickMode.CurrentPosition;
        private int _numberOfClicks = 0;
        private bool _hotkeyEnabled = false;
        private Key _startStopHotkey = Key.F8;
        private Key _capturePositionHotkey = Key.F5;
        private ClickType _clickType = ClickType.Left;
        private GlobalHotkey _startStopGlobalHotkey;
        private GlobalHotkey _capturePositionGlobalHotkey;
        private GlobalHotkey _captureMacroPositionGlobalHotkey;
        private int _clicksPerformed = 0;
        private DispatcherTimer _cursorPositionTimer;
        private const int CursorRepositionInterval = 50;
        private ObservableCollection<Macro> _macros = new ObservableCollection<Macro>();
        private Macro _selectedMacro;

        public enum ClickType
        {
            Left,
            Right,
            Middle
        }

        public enum ClickMode
        {
            CurrentPosition,
            SpecificPosition,
            Macro
        }

        public MainWindow()
        {
            InitializeComponent();
            IntervalTextBox.Text = _clickInterval.ToString();
            XPosTextBox.Text = _xPos.ToString();
            YPosTextBox.Text = _yPos.ToString();
            ClicksTextBox.Text = _numberOfClicks.ToString();
            HotkeyTextBox.Text = _startStopHotkey.ToString();
            ClickTypeComboBox.ItemsSource = Enum.GetValues(typeof(ClickType));
            ClickTypeComboBox.SelectedItem = _clickType;

            CurrentPositionRadioButton.IsChecked = _clickMode == ClickMode.CurrentPosition;
            SpecificPositionRadioButton.IsChecked = _clickMode == ClickMode.SpecificPosition;
            MacroRadioButton.IsChecked = _clickMode == ClickMode.Macro;

            _cursorPositionTimer = new DispatcherTimer();
            _cursorPositionTimer.Interval = TimeSpan.FromMilliseconds(100);
            _cursorPositionTimer.Tick += CursorPositionTimer_Tick;
            _cursorPositionTimer.Start();

            EnableHotkeys();
            MacroComboBox.ItemsSource = _macros;

            UpdateUIState();
            Closing += MainWindow_Closing;
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnregisterHotkeys();
            _cursorPositionTimer.Stop();
        }

        private void UnregisterHotkeys()
        {
            _startStopGlobalHotkey?.Unregister();
            _capturePositionGlobalHotkey?.Unregister();
            _captureMacroPositionGlobalHotkey?.Unregister();
            _startStopGlobalHotkey = null;
            _capturePositionGlobalHotkey = null;
            _captureMacroPositionGlobalHotkey = null;
        }

        private void UpdateUIState()
        {
            IntervalTextBox.IsEnabled = !_isClicking && _clickMode != ClickMode.Macro;
            XPosTextBox.IsEnabled = !_isClicking && _clickMode == ClickMode.SpecificPosition;
            YPosTextBox.IsEnabled = !_isClicking && _clickMode == ClickMode.SpecificPosition;
            ClicksTextBox.IsEnabled = !_isClicking;
            CurrentPositionRadioButton.IsEnabled = !_isClicking;
            SpecificPositionRadioButton.IsEnabled = !_isClicking;
            MacroRadioButton.IsEnabled = !_isClicking;
            ClickTypeComboBox.IsEnabled = !_isClicking;
            HotkeyTextBox.IsEnabled = !_isClicking;
            StartButton.IsEnabled = !_isClicking;
            StopButton.IsEnabled = _isClicking;
            MacroComboBox.IsEnabled = !_isClicking && _clickMode == ClickMode.Macro;
            CreateMacroButton.IsEnabled = !_isClicking && _clickMode == ClickMode.Macro;
            LoadMacroButton.IsEnabled = !_isClicking && _clickMode == ClickMode.Macro;
            ExportMacroButton.IsEnabled = !_isClicking && _clickMode == ClickMode.Macro;
            EditMacroButton.IsEnabled = !_isClicking && _clickMode == ClickMode.Macro && _selectedMacro != null;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clickMode != ClickMode.Macro && (!int.TryParse(IntervalTextBox.Text, out _clickInterval) || _clickInterval <= 0))
            {
                MessageBox.Show("Invalid click interval. Please enter a positive integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_clickMode == ClickMode.SpecificPosition &&
               (!int.TryParse(XPosTextBox.Text, out _xPos) || !int.TryParse(YPosTextBox.Text, out _yPos)))
            {
                MessageBox.Show("Invalid X or Y coordinates. Please enter integers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(ClicksTextBox.Text, out _numberOfClicks) || _numberOfClicks < 0)
            {
                MessageBox.Show("Invalid number of clicks. Please enter 0 for infinite, or a positive integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _clickType = (ClickType)ClickTypeComboBox.SelectedItem;

            _isClicking = true;
            _cts = new CancellationTokenSource();
            UpdateUIState();

            _clicksPerformed = 0;

            try
            {
                await ClickLoop(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
            finally
            {
                _isClicking = false;
                _cts.Dispose();
                _cts = null;
                UpdateUIState();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private async Task ClickLoop(CancellationToken cancellationToken)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            if (_clickMode == ClickMode.Macro)
            {
                if (_selectedMacro == null)
                {
                    MessageBox.Show("Select a macro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _cts.Cancel();
                    return;
                }
                await ExecuteMacro(_selectedMacro, cancellationToken);
            }
            else
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_clickMode == ClickMode.SpecificPosition)
                    {
                        if (stopwatch.ElapsedMilliseconds >= CursorRepositionInterval)
                        {
                            SetCursorPos(_xPos, _yPos);
                            stopwatch.Restart();
                        }
                    }
                    else
                    {
                        if (GetCursorPos(out POINT currentPoint))
                        {
                            _xPos = currentPoint.X;
                            _yPos = currentPoint.Y;
                        }
                        else
                        {
                            MessageBox.Show("Failed to get current cursor position.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            _cts.Cancel();
                            return;
                        }
                    }

                    switch (_clickType)
                    {
                        case ClickType.Left:
                            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)_xPos, (uint)_yPos, 0, 0);
                            break;
                        case ClickType.Right:
                            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (uint)_xPos, (uint)_yPos, 0, 0);
                            break;
                        case ClickType.Middle:
                            mouse_event(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP, (uint)_xPos, (uint)_yPos, 0, 0);
                            break;
                    }

                    _clicksPerformed++;
                    if (_numberOfClicks > 0 && _clicksPerformed >= _numberOfClicks)
                    {
                        _cts.Cancel();
                        return;
                    }

                    try
                    {
                        await Task.Delay(_clickInterval, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                }
            }
        }

        private async Task ExecuteMacro(Macro macro, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var action in macro.Actions)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    for (int i = 0; i < action.Repetitions; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        SetCursorPos(action.X, action.Y);

                        switch (_clickType)
                        {
                            case ClickType.Left:
                                mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)action.X, (uint)action.Y, 0, 0);
                                await Task.Delay(action.ClickDuration, cancellationToken);
                                mouse_event(MOUSEEVENTF_LEFTUP, (uint)action.X, (uint)action.Y, 0, 0);
                                break;
                            case ClickType.Right:
                                mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)action.X, (uint)action.Y, 0, 0);
                                await Task.Delay(action.ClickDuration, cancellationToken);
                                mouse_event(MOUSEEVENTF_RIGHTUP, (uint)action.X, (uint)action.Y, 0, 0);
                                break;
                            case ClickType.Middle:
                                mouse_event(MOUSEEVENTF_MIDDLEDOWN, (uint)action.X, (uint)action.Y, 0, 0);
                                await Task.Delay(action.ClickDuration, cancellationToken);
                                mouse_event(MOUSEEVENTF_MIDDLEUP, (uint)action.X, (uint)action.Y, 0, 0);
                                break;
                        }
                        _clicksPerformed++;
                        if (_numberOfClicks > 0 && _clicksPerformed >= _numberOfClicks)
                        {
                            _cts.Cancel();
                            return;
                        }
                        if (i < action.Repetitions - 1)
                        {
                            await Task.Delay(action.Delay, cancellationToken);
                        }

                    }
                    await Task.Delay(action.Delay, cancellationToken);
                }
            }
        }

        private void CurrentPositionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _clickMode = ClickMode.CurrentPosition;
            UpdateUIState();
        }

        private void SpecificPositionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _clickMode = ClickMode.SpecificPosition;
            UpdateUIState();
        }

        private void MacroRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _clickMode = ClickMode.Macro;
            UpdateUIState();
        }

        private void EnableHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (!_hotkeyEnabled)
            {
                EnableHotkeys();
            }
            else
            {
                _startStopGlobalHotkey.Unregister();
                _capturePositionGlobalHotkey?.Unregister();
                _hotkeyEnabled = false;
                HotkeyButton.Content = "Enable Hotkey";
            }
        }

        private void EnableHotkeys()
        {
            try
            {
                _startStopGlobalHotkey = new GlobalHotkey(ModifierKeys.None, _startStopHotkey, this, OnStartStopHotkeyActivated);
                if (_startStopGlobalHotkey.Register())
                {
                    _hotkeyEnabled = true;
                    HotkeyButton.Content = "Disable Hotkey";
                }
                else
                {
                    MessageBox.Show("Can't register hotkey for start/stop. It's possible that the hotkey is being already used by other application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _capturePositionGlobalHotkey = new GlobalHotkey(ModifierKeys.None, _capturePositionHotkey, this, OnCapturePositionHotkeyActivated);
            }
            catch (Exception ex)
            {
                //Do nothing lol, idc
            }
        }


        private void RegisterHotkey(ref GlobalHotkey hotkey, Key key, Action action)
        {
            hotkey = new GlobalHotkey(ModifierKeys.None, key, this, action);
        }

        private void OnStartStopHotkeyActivated()
        {
            if (_isClicking)
            {
                _cts?.Cancel();
            }
            else
            {
                StartButton_Click(this, new RoutedEventArgs());
            }
        }

        private void OnCapturePositionHotkeyActivated()
        {
            if (GetCursorPos(out POINT currentPoint))
            {
                SpecificPositionRadioButton.IsChecked = true;
                XPosTextBox.Text = currentPoint.X.ToString();
                YPosTextBox.Text = currentPoint.Y.ToString();
                _xPos = currentPoint.X;
                _yPos = currentPoint.Y;
            }
            else
            {
                MessageBox.Show("Failed to get current cursor position.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            _startStopHotkey = key;
            HotkeyTextBox.Text = key.ToString();
        }

        private void CursorPositionTimer_Tick(object sender, EventArgs e)
        {
            if (GetCursorPos(out POINT currentPoint))
            {
                CursorPositionLabel.Content = $"X: {currentPoint.X}, Y: {currentPoint.Y}";
            }
        }

        private void CreateMacroButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMacroWindow(null);
        }

        private void EditMacroButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMacro != null)
            {
                OpenMacroWindow(_selectedMacro);
            }
        }

        private void OpenMacroWindow(Macro macroToEdit)
        {
            var macroWindow = new MacroCreationWindow(macroToEdit);

            RegisterHotkey(ref _captureMacroPositionGlobalHotkey, _capturePositionHotkey, () =>
            {
                if (macroWindow.IsActive && GetCursorPos(out POINT currentPoint))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        macroWindow.XTextBox.Text = currentPoint.X.ToString();
                        macroWindow.YTextBox.Text = currentPoint.Y.ToString();
                    });
                }
            });


            if (macroWindow.ShowDialog() == true)
            {
                if (macroToEdit == null)
                {
                    _macros.Add(macroWindow.Macro);
                    _selectedMacro = macroWindow.Macro;
                    MacroComboBox.SelectedItem = _selectedMacro;
                }
                else
                {
                    int index = _macros.IndexOf(macroToEdit);
                    if (index != -1)
                    {
                        _macros[index] = macroWindow.Macro;
                        _selectedMacro = macroWindow.Macro;
                        MacroComboBox.SelectedItem = _selectedMacro;
                    }
                }
            }
            _captureMacroPositionGlobalHotkey?.Unregister();
            _captureMacroPositionGlobalHotkey = null;
            UpdateUIState();
        }

        private void MacroComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMacro = MacroComboBox.SelectedItem as Macro;
            UpdateUIState();
        }

        private const string MacrosFileExtension = ".gcam";

        private void LoadMacros()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = MacrosFileExtension,
                Filter = $"Macro Files (*{MacrosFileExtension})|*{MacrosFileExtension}|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (!openFileDialog.FileName.EndsWith(MacrosFileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Invalid file format. Please select a file with the {MacrosFileExtension} extension.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);

                    try
                    {
                        var loadedMacros = JsonSerializer.Deserialize<List<Macro>>(json);
                        _macros.Clear();
                        foreach (var macro in loadedMacros)
                        {
                            _macros.Add(macro);
                        }
                        MacroComboBox.ItemsSource = _macros;
                        if (_macros.Count > 0)
                        {
                            MacroComboBox.SelectedIndex = 0;
                        }
                        return;
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var loadedMacro = JsonSerializer.Deserialize<Macro>(json);
                            if (loadedMacro != null)
                            {
                                _macros.Add(loadedMacro);
                                MacroComboBox.ItemsSource = _macros;
                                MacroComboBox.SelectedItem = loadedMacro;
                            }
                            return;
                        }
                        catch (JsonException ex)
                        {
                            MessageBox.Show($"Error parsing JSON: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found: {ex.FileName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (DirectoryNotFoundException ex)
                {
                    MessageBox.Show($"Directory not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"I/O Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access Denied: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveMacros(string filePath)
        {
            if (_macros.Count == 0) return;

            try
            {
                string json = JsonSerializer.Serialize(_macros);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving macros: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMacroButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMacros();
        }

        private void ExportMacroButton_Click(object sender, RoutedEventArgs e)
        {
            if (_macros.Count == 0)
            {
                MessageBox.Show("No macros to export.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = MacrosFileExtension,
                Filter = $"Macro Files (*{MacrosFileExtension})|*{MacrosFileExtension}|All files (*.*)|*.*",
                FileName = "macros"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                if (!filePath.EndsWith(MacrosFileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    filePath += MacrosFileExtension;
                }
                SaveMacros(filePath);
            }
        }

        private void InstructionsButton_Click(object sender, RoutedEventArgs e)
        {
            string instructions =
                "AutoClicker Instructions:\n\n" +
                "1. Interval (ms): Set the time between clicks in milliseconds.\n" +
                "2. Position:\n" +
                "   - Current: Clicks at the current cursor position.\n" +
                "   - Specific: Clicks at the specified X and Y coordinates.\n" +
                "   - Macro: Executes a predefined sequence of clicks (see Macro section below).\n" +
                "3. X/Y: Enter the coordinates for the Specific position mode.\n" +
                "4. Clicks:  Enter the number of clicks to perform (0 for infinite).\n" +
                "5. Click Type: Select Left, Right, or Middle mouse button.\n" +
                "6. Hotkey:  The key to start/stop the clicking (default: F8).  Press the desired key in the box.\n" +
                "   - Enable/Disable Hotkey button:  Activates or deactivates the hotkey.\n" +
                "7.  Capture Position Hotkey (F5): When using Specific Position, press F5 to capture the current cursor coordinates.\n" +
                "8. Macro:\n" +
                "   - Select a macro from the dropdown list.\n" +
                "   - Create:  Opens the Macro Creation window to define a new macro.\n" +
                "   - Load:  Loads macros from a .gcam file.\n" +
                "   - Export: Saves the current macros to a .gcam file.\n" +
                "   - Edit: Opens the Macro Creation window to modify the selected macro.\n" +
                "9. Start: Begins the clicking process.\n" +
                "10. Stop: Halts the clicking process.\n\n" +
                "Macro Creation/Editing:\n\n" +
                "1. Macro Name: Enter a name for your macro.\n" +
                "2. Actions List: Displays the sequence of actions in the macro.\n" +
                "3. X, Y: Enter the coordinates for each click.\n" +
                "4. Duration (ms): How long the mouse button should be held down for each click.\n" +
                "5. Delay (ms):  The time to wait *after* the click (and after any repetitions) before the next action.\n" +
                "6. Repeats: Number of times to repeat the click.\n" +
                "7. Add Action:  Adds the defined action to the macro.\n" +
                "8. Capture Position(F5):  Press F5 while this window is active to capture the current cursor coordinates and fill in the X and Y fields.\n" +
                "9. Save/Save Changes:  Saves the new macro or updates the existing macro.\n" +
                "10. Cancel: Discards changes and closes the window.\n" +
                "\nImportant Notes on Hotkeys:\n" +
                "- The hotkeys are global and will override other applications' hotkeys while enabled.\n" +
                "- Be careful when choosing hotkeys to avoid conflicts with other frequently used shortcuts.\n" +
                "- If Hotkey is already used - hotkey will try to register anyway. If it is not possible - exception will be thrown.\n" +
                "------------------\n" +
                "Made by GDCocos12";

            MessageBox.Show(instructions, "Instructions", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
