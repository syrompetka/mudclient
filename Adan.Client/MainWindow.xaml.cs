// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Adan.Client.Common.Controls;
    using AvalonDock;
    using Commands;
    using CommandSerializers;
    using Common.Commands;
    using Common.Conveyor;
    using Common.Model;
    using Common.Plugins;
    using Common.Themes;
    using ConveyorUnits;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Dialogs;
    using MessageDeserializers;
    using Model.ActionDescriptions;
    using Model.ActionParameters;
    using Model.Actions;
    using Model.ParameterDescriptions;
    using ViewModel;
    using Adan.Client.Common.ViewModel;
    using Adan.Client.Common.Settings;
    using Adan.Client.Properties;
    using Adan.Client.Common.Utils;
    using Adan.Client.Utils;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private WindowState _nonFullScreenWindowState;
        private IList<DockableContent> _allWidgets = new List<DockableContent>();
        private IList<OutputWindow> _outputWindows = new List<OutputWindow>();
        private IList<Hotkey> _globalHotkeys = new List<Hotkey>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var types = new List<Type>
                            {
                                typeof(SendTextAction),
                                typeof(OutputToMainWindowAction),
                                typeof(ClearVariableValueAction),
                                typeof(ConditionalAction),
                                typeof(DisableGroupAction),
                                typeof(EnableGroupAction),
                                typeof(SetVariableValueAction),
                                typeof(StartLogAction),
                                typeof(StopLogAction),
                                typeof(TriggerOrCommandParameter),
                                typeof(VariableReferenceParameter),
                                typeof(MathExpressionParameter),
                                typeof(ConstantStringParameter),
                                typeof(SendTextOneParameterAction),
                                typeof(ShowOutputWindowAction),
                                typeof(SendToWindowAction),
                                //typeof(ToggleFullScreenModeAction),
                            };

            foreach (var plugin in PluginHost.Instance.Plugins)
            {
                foreach (var customType in plugin.CustomSerializationTypes)
                {
                    types.Add(customType);
                    RootModel.CustomSerializationTypes.Add(customType);
                }
            }

            Settings settings = Settings.Default;
            settings.Reload();

            SettingsHolder.Instance.Initialize((SettingsFolder)settings.SettingsFolder, types);

            _globalHotkeys = SettingsHolder.Instance.Settings.GlobalHotkeys;

            var actionDescriptions = new List<ActionDescription>();
            var parameterDescriptions = new List<ParameterDescription>();

            actionDescriptions.Add(new SendTextActionDescription(parameterDescriptions, actionDescriptions));
            actionDescriptions.Add(new OutputToMainWindowActionDescription(parameterDescriptions, actionDescriptions));
            actionDescriptions.Add(new ClearVariableValueActionDescription(actionDescriptions));
            actionDescriptions.Add(new ConditionalActionDescription(parameterDescriptions, actionDescriptions));
            actionDescriptions.Add(new DisableGroupActionDescription(actionDescriptions));
            actionDescriptions.Add(new EnableGroupActionDescription(actionDescriptions));
            actionDescriptions.Add(new SetVariableValueActionDescription(actionDescriptions, parameterDescriptions));
            actionDescriptions.Add(new StartLogActionDescription(actionDescriptions, parameterDescriptions));
            actionDescriptions.Add(new StopLogActionDescription(actionDescriptions));
            actionDescriptions.Add(new ShowOutputWindowActionDescription(actionDescriptions));
            actionDescriptions.Add(new SendToWindowActionDescription(actionDescriptions));
            //actionDescriptions.Add(new ToggleFullScreenModeActionDescription(actionDescriptions));

            parameterDescriptions.Add(new TriggerOrCommandParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new VariableReferenceParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new MathExpressionParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new ConstantStringParameterDescription(parameterDescriptions));

            RootModel.AllActionDescriptions = actionDescriptions;
            RootModel.AllParameterDescriptions = parameterDescriptions;

            MessageConveyor.AddCommandSerializer(new TextCommandSerializer());

            // MessageConveyor.AddMessageDeserializer(new TextMessageDeserializer());
            MessageConveyor.AddMessageDeserializer(new TextMessageDeserializerEx());
            MessageConveyor.AddMessageDeserializer(new ProtocolVersionMessageDeserializer());

            MessageConveyor.AddConveyorUnit(new CommandSeparatorUnit());
            MessageConveyor.AddConveyorUnit(new CommandsFromUserLineUnit());
            MessageConveyor.AddConveyorUnit(new VariableReplaceUnit());
            MessageConveyor.AddConveyorUnit(new CommandMultiplierUnit());
            MessageConveyor.AddConveyorUnit(new SubstitutionUnit());
            MessageConveyor.AddConveyorUnit(new TriggerUnit());
            MessageConveyor.AddConveyorUnit(new AliasUnit());
            MessageConveyor.AddConveyorUnit(new HotkeyUnit());
            MessageConveyor.AddConveyorUnit(new HighlightUnit());
            MessageConveyor.AddConveyorUnit(new LoggingUnit(this));
            MessageConveyor.AddConveyorUnit(new ShowMainOutputUnit(this));
            MessageConveyor.AddConveyorUnit(new SendToWindowUnit(this));
            //MessageConveyor.AddConveyorUnit(new ToggleFullScreenModeUnit(this));

            foreach (var themeDescription in ThemeManager.Instance.AvailableThemes)
            {
                var menuItem = new MenuItem
                {
                    Header = themeDescription.Name,
                    Tag = themeDescription,
                    IsChecked = themeDescription == ThemeManager.Instance.ActiveTheme
                };
                menuItem.Click += HandleThemeChange;
                themesMenuItem.Items.Add(menuItem);
            }

            var initializationDalog = new PluginInitializationStatusDialog
            {
                ViewModel = new InitializationStatusModel()
            };

            Task task = Task.Factory.StartNew(() => PluginHost.Instance.InitializePlugins(initializationDalog.ViewModel, this))
                .ContinueWith(t => Dispatcher.Invoke((Action)initializationDalog.Close));
            initializationDalog.ShowDialog();

            foreach (var plugin in PluginHost.Instance.Plugins)
            {
                if (plugin.HasOptions)
                {
                    var menuItem = new MenuItem { Header = plugin.OptionsMenuItemText };

                    var pluginClosure = plugin;
                    menuItem.Click += (o, e) => pluginClosure.ShowOptionsDialog(this);
                    optionsMenuItem.Items.Insert(0, menuItem);

                    if (optionsSeparator.Visibility != Visibility.Visible)
                        optionsSeparator.Visibility = Visibility.Visible;
                }
            }

            int maxProtocolVersion = PluginHost.Instance.Plugins.Count > 0 ? PluginHost.Instance.Plugins.Max(plugin => plugin.RequiredProtocolVersion) : 1;
            MessageConveyor.AddConveyorUnit(new ProtocolVersionUnit(maxProtocolVersion));

            MessageConveyor.AddConveyorUnit(new CommandRepeaterUnit());

            //Заглушка для нераспознанных комманд
            MessageConveyor.AddConveyorUnit(new CapForLineCommandUnit());

            //Проверка коннекта к серверу для команд
            MessageConveyor.AddConveyorUnit(new ConnectionUnit());

            foreach (var uid in SettingsHolder.Instance.Settings.MainOutputs)
            {
                CreateNewOutputWindow(uid.Substring(0, uid.Length - 32), uid);
            }

            Top = SettingsHolder.Instance.Settings.MainWindowTop;
            Left = SettingsHolder.Instance.Settings.MainWindowLeft;
            Width = SettingsHolder.Instance.Settings.MainWindowWidth;
            Height = SettingsHolder.Instance.Settings.MainWindowHeight;
            WindowState = SettingsHolder.Instance.Settings.MainWindowState;
        }

        private void CreateNewOutputWindow(string name, string uid)
        {
            OutputWindow outputWindow = new OutputWindow(this, name);
            _outputWindows.Add(outputWindow);
            outputWindow.Uid = uid;

            DockableContent dockable = new DockableContent()
            {
                Name = uid,
                Title = name,
                Content = outputWindow.VisibleControl,
                HideOnClose = false,
            };

            outputWindow.DockContent = dockable;
            dockable.Closed += OnOutputWindowClosed;

            var menuItem = new MenuItem()
            {
                Header = outputWindow.Name,
                Name = outputWindow.Uid,
            };

            if (windowMenuItem.Items.Count > 0)
                windowMenuItem.Items.Insert(0, menuItem);
            else
                windowMenuItem.Items.Add(menuItem);

            if (windowSeparator.Visibility != Visibility.Visible)
                windowSeparator.Visibility = Visibility.Visible;

            PluginHost.Instance.OutputWindowCreated(outputWindow);

            if (dockManager.MainDocumentPane != null)
                dockManager.MainDocumentPane.Items.Add(dockable);
            else
                dockable.Show(dockManager);

            if (SettingsHolder.Instance.Settings.AutoConnect)
            {
                outputWindow.RootModel.PushCommandToConveyor(
                    new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
            }
        }

        private void OnOutputWindowClosed(object sender, EventArgs e)
        {
            var dockable = (DockableContent)sender;
            var outputWindow = _outputWindows.FirstOrDefault(output => output.Uid == dockable.Name);

            if (outputWindow != null)
            {
                outputWindow.Save();
                PluginHost.Instance.OutputWindowClose(outputWindow);
                outputWindow.Dispose();
                _outputWindows.Remove(outputWindow);

                foreach (var floatingWindow in dockManager.FloatingWindows)
                {
                    if (!floatingWindow.HostedPane.HasItems)
                    {
                        try
                        {
                            floatingWindow.Close();
                        }
                        catch { }
                    }
                }

                foreach (var item in windowMenuItem.Items)
                {
                    var menuItem = item as MenuItem;
                    if (menuItem != null && menuItem.Name == outputWindow.Uid)
                    {
                        windowMenuItem.Items.Remove(menuItem);
                        break;
                    }
                }

                if (windowMenuItem.Items.Count <= 2)
                    windowSeparator.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void ShowOutputWindow(string name)
        {
            OutputWindow outputWindowToSelect;
            if (string.IsNullOrEmpty(name))
            {
                var currentWindow = _outputWindows.FirstOrDefault(w => w.DockContent.IsKeyboardFocusWithin);
                if (currentWindow == null)
                {
                    return;
                }

                var currentWindowIndex = _outputWindows.IndexOf(currentWindow);
                if (currentWindowIndex < 0)
                {
                    return;
                }

                if (currentWindowIndex == _outputWindows.Count - 1)
                {
                    currentWindowIndex = 0;
                }
                else
                {
                    currentWindowIndex++;
                }

                outputWindowToSelect = _outputWindows[currentWindowIndex];
            }
            else
            {
                outputWindowToSelect = _outputWindows.FirstOrDefault(output => output.Name == name);
            }

            if (outputWindowToSelect != null)
            {
                dockManager.ActiveContent = outputWindowToSelect.DockContent;
                outputWindowToSelect.Focus();
            }
        }

        /// <summary>
        /// Sends to window.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="actionsToExecute">The actions to execute.</param>
        /// <param name="actionExecutionContext">The action execution context.</param>
        public void SendToWindow(string name, IEnumerable<ActionBase> actionsToExecute, ActionExecutionContext actionExecutionContext)
        {
            var outputWindow = _outputWindows.FirstOrDefault(w => w.Name == name);
            if (outputWindow == null)
            {
                return;
            }

            foreach (var action in actionsToExecute)
            {
                action.Execute(outputWindow.RootModel, actionExecutionContext);
            }
        }

        /// <summary>
        /// Sends to all windows.
        /// </summary>
        /// <param name="actionsToExecute">The actions to execute.</param>
        /// <param name="actionExecutionContext">The action execution context.</param>
        public void SendToAllWindows(IEnumerable<ActionBase> actionsToExecute, ActionExecutionContext actionExecutionContext)
        {
            foreach (var outputWindow in _outputWindows)
            {
                foreach (var action in actionsToExecute)
                {
                    action.Execute(outputWindow.RootModel, actionExecutionContext);
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown([NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            CheckGlobalHotkeys(e);

            if (!e.Handled)
                base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void CheckGlobalHotkeys(KeyEventArgs e)
        {
            var hotkeyCommand = new HotkeyCommand()
            {
                Key = e.Key == Key.System ? e.SystemKey : e.Key,
                ModifierKeys = Keyboard.Modifiers,
                Handled = false,
            };

            var hotkey = _globalHotkeys.FirstOrDefault(hot => hot.Key == hotkeyCommand.Key && hot.ModifierKeys == hotkeyCommand.ModifierKeys);
            if (hotkey != null)
            {
                var outputWindow = _outputWindows.FirstOrDefault(output => output.DockContent.IsKeyboardFocusWithin);

                if (outputWindow != null)
                {
                    foreach (var action in hotkey.Actions)
                    {
                        if (action.IsGlobal)
                            action.Execute(null, null);
                        else if (outputWindow.RootModel != null)
                            action.Execute(outputWindow.RootModel, ActionExecutionContext.Empty);
                    }
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing([NotNull] CancelEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnClosing(e);

            try
            {
                //if (WindowStyle == WindowStyle.None)
                //{
                //    ToggleFullScreenMode();
                //}

                var layoutFullPath = Path.Combine(SettingsHolder.Instance.Folder, "Settings");
                if (!Directory.Exists(layoutFullPath))
                {
                    Directory.CreateDirectory(layoutFullPath);
                }

                layoutFullPath = Path.Combine(layoutFullPath, "Layout.xml");
                if (File.Exists(layoutFullPath))
                    File.Delete(layoutFullPath);

                dockManager.SaveLayout(layoutFullPath);

                SettingsHolder.Instance.Settings.MainOutputs.Clear();

                foreach (var outputWindow in _outputWindows)
                {
                    SettingsHolder.Instance.Settings.MainOutputs.Add(outputWindow.Uid);
                    outputWindow.Save();
                    outputWindow.Dispose();
                }

                SettingsHolder.Instance.Settings.MainWindowTop = (int)Top;
                SettingsHolder.Instance.Settings.MainWindowLeft = (int)Left;
                SettingsHolder.Instance.Settings.MainWindowWidth = (int)Width;
                SettingsHolder.Instance.Settings.MainWindowHeight = (int)Height;
                SettingsHolder.Instance.Settings.MainWindowState = WindowState;

                SettingsHolder.Instance.SaveAllSettings();

                PluginHost.Instance.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error save settings");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="rootModel"></param>
        public void StartLogging(string logName, RootModel rootModel)
        {
            var outputWindow = _outputWindows.FirstOrDefault(wind => wind.Uid == rootModel.Uid);

            if (outputWindow != null)
            {
                outputWindow.StartLogging(logName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public void StopLogging(RootModel rootModel)
        {
            var outputWindow = _outputWindows.FirstOrDefault(wind => wind.Uid == rootModel.Uid);

            if (outputWindow != null)
            {
                outputWindow.StopLogging();
            }
        }

        private void HandleConnect([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var outputWindow = _outputWindows.FirstOrDefault(output => output.DockContent.IsKeyboardFocusWithin);
            if (outputWindow == null)
            {
                CreateNewOutputWindow("Default", Guid.NewGuid().ToString("N"));
            }

            outputWindow.RootModel.PushCommandToConveyor(
                    new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
        }

        private void HandleConnectAll([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (_outputWindows.Count == 0)
                CreateNewOutputWindow("Default", Guid.NewGuid().ToString("N"));

            foreach (OutputWindow window in _outputWindows)
                window.RootModel.PushCommandToConveyor(new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
        }

        private void HandleDisconnect([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var outputWindow = _outputWindows.FirstOrDefault(output => output.DockContent.IsKeyboardFocusWithin);

            if (outputWindow != null)
                outputWindow.RootModel.PushCommandToConveyor(new DisconnectCommand());
        }

        private void HandleDisconnectAll([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            foreach (OutputWindow window in _outputWindows)
                window.RootModel.PushCommandToConveyor(new DisconnectCommand());
        }

        private void HandleConnectionPreference([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var connectionDialogViewModel = new ConnectionDialogViewModel
            {
                HostName = SettingsHolder.Instance.Settings.ConnectHostName,
                Port = SettingsHolder.Instance.Settings.ConnectPort
            };

            var dialog = new ConnectionDialog { DataContext = connectionDialogViewModel, Owner = this };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                SettingsHolder.Instance.Settings.ConnectHostName = connectionDialogViewModel.HostName;
                SettingsHolder.Instance.Settings.ConnectPort = connectionDialogViewModel.Port;
            }
        }

        private void HandleExit([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            Close();
        }

        private void HandleThemeChange([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var newTheme = (ThemeDescription)((MenuItem)sender).Tag;
            ThemeManager.Instance.SwitchToTheme(newTheme);
            PluginHost.Instance.ApplyAdditionalPluginMergeDictionaries();

            foreach (MenuItem item in themesMenuItem.Items)
            {
                item.IsChecked = item.Tag == ThemeManager.Instance.ActiveTheme;
            }
        }

        private void HandleDockManagerLoaded([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            foreach (var widgetDescription in PluginHost.Instance.AllWidgets)
            {
                var dockContent = new DockableContent
                {
                    Title = widgetDescription.Description,
                    Name = widgetDescription.Name,
                    Content = widgetDescription.Control,
                    HideOnClose = true,
                };

                dockContent.FloatingWindowSizeToContent = widgetDescription.ResizeToContent
                                                              ? SizeToContent.WidthAndHeight
                                                              : SizeToContent.Manual;

                if (!string.IsNullOrEmpty(widgetDescription.Icon))
                {
                    dockContent.Icon = (ImageSource)FindResource(widgetDescription.Icon);
                }

                _allWidgets.Add(dockContent);

                var menuItem = new MenuItem()
                {
                    Header = widgetDescription.Description,
                    Tag = dockContent
                };

                var visibleBinding = new Binding("State")
                {
                    Source = dockContent,
                    Mode = BindingMode.OneWay,
                    Converter = new DockableContentStateToBoolConverter(),
                };

                menuItem.SetBinding(MenuItem.IsCheckedProperty, visibleBinding);
                menuItem.Click += HandleHideShowWidget;
                viewMenuItem.Items.Add(menuItem);

                dockContent.Show(dockManager);
            }

            LoadLayout();

            if (SettingsHolder.Instance.Settings.MainOutputs.Count == 0)
            {
                CreateNewOutputWindow("Default", "Default" + Guid.NewGuid().ToString("N"));
            }
        }

        private void LoadLayout()
        {
            var layoutFullPath = Path.Combine(SettingsHolder.Instance.Folder, "Settings", "Layout.xml");
            if (File.Exists(layoutFullPath))
            {
                try
                {
                    dockManager.RestoreLayout(layoutFullPath);
                }
                catch (Exception) { }
            }
        }

        private void HandleHideShowWidget([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var dockContent = (DockableContent)((MenuItem)sender).Tag;
            if (dockContent.State != DockableContentState.Hidden)
            {
                dockContent.Hide();
            }
            else
            {
                dockContent.Show(dockManager);
            }
        }

        private void HandleEditProfiles([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var models = new ObservableCollection<ProfileViewModel>();

            foreach (string str in SettingsHolder.Instance.AllProfiles)
            {
                models.Add(new ProfileViewModel(str, str == "Default" ? true : false));
            }

            var profilesViewModel = new ProfilesEditViewModel(models, models[0].NameProfile);
            var profileDialog = new ProfilesEditDialog() { DataContext = profilesViewModel, Owner = this };

            var resultDialog = profileDialog.ShowDialog();
        }

        private void HandleEditOptions([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var model = new OptionsViewModel()
            {
                AutoClearInput = SettingsHolder.Instance.Settings.AutoClearInput,
                AutoReconnect = SettingsHolder.Instance.Settings.AutoReconnect,
                CommandChar = SettingsHolder.Instance.Settings.CommandChar,
                CommandDelimiter = SettingsHolder.Instance.Settings.CommandDelimiter,
                StartOfLine = SettingsHolder.Instance.Settings.CursorPosition == CursorPositionHistory.StartOfLine,
                EndOfLine = SettingsHolder.Instance.Settings.CursorPosition == CursorPositionHistory.EndOfLine,
                HistorySize = SettingsHolder.Instance.Settings.CommandsHistorySize.ToString(),
                MinLengthHistory = SettingsHolder.Instance.Settings.MinLengthHistory.ToString(),
                ScrollBuffer = SettingsHolder.Instance.Settings.ScrollBuffer.ToString(),
                SettingsFolder = SettingsHolder.Instance.SettingsFolder == SettingsFolder.DocumentsAndSettings,
                AutoConnect = SettingsHolder.Instance.Settings.AutoConnect,
            };

            var optionsDialog = new OptionsDialog() { DataContext = model, Owner = this };
            var dialogResult = optionsDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                SettingsHolder.Instance.Settings.AutoClearInput = model.AutoClearInput;
                SettingsHolder.Instance.Settings.AutoReconnect = model.AutoReconnect;
                SettingsHolder.Instance.Settings.CommandChar = model.CommandChar;
                SettingsHolder.Instance.Settings.CommandDelimiter = model.CommandDelimiter;
                SettingsHolder.Instance.Settings.AutoConnect = model.AutoConnect;

                if (model.StartOfLine)
                    SettingsHolder.Instance.Settings.CursorPosition = CursorPositionHistory.StartOfLine;
                else
                    SettingsHolder.Instance.Settings.CursorPosition = CursorPositionHistory.EndOfLine;

                if (model.SettingsFolder)
                    SettingsHolder.Instance.SettingsFolder = SettingsFolder.DocumentsAndSettings;
                else
                    SettingsHolder.Instance.SettingsFolder = SettingsFolder.ProgramFolder;

                int val;
                if (int.TryParse(model.HistorySize, out val))
                    SettingsHolder.Instance.Settings.CommandsHistorySize = val;

                if (int.TryParse(model.MinLengthHistory, out val))
                    SettingsHolder.Instance.Settings.MinLengthHistory = val;

                if (int.TryParse(model.ScrollBuffer, out val))
                    SettingsHolder.Instance.Settings.ScrollBuffer = val < 100000 ? val : 100000;

                SettingsHolder.Instance.SaveCommonSettings();
            }
        }

        private void HandleGlobalHotkeys([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var globalHotkeysDialog = new GlobalHotkeysEditDialog()
            {
                DataContext = new GlobalHotkeysViewModel(_globalHotkeys, RootModel.AllActionDescriptions),
                Owner = this,
            };

            globalHotkeysDialog.ShowDialog();
            SettingsHolder.Instance.SaveCommonSettings();
        }

        private void HandleAddNewWindow([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            var profiles = new ObservableCollection<ProfileViewModel>();

            foreach (string str in SettingsHolder.Instance.AllProfiles)
            {
                profiles.Add(new ProfileViewModel(str, str == "Default" ? true : false));
            }

            if (profiles.Count == 0)
            {
                MessageBox.Show(this, "Create profile first", "Error");
                return;
            }

            var chooseViewModel = new ProfileChooseViewModel(profiles, profiles[0].NameProfile);

            var chooseDialog = new ProfilesChooseDialog()
            {
                DataContext = chooseViewModel,
                Owner = this,
            };

            var result = chooseDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    var name = chooseViewModel.SelectedProfile.NameProfile;

                    CreateNewOutputWindow(name, name + Guid.NewGuid().ToString("N"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Error");
                }
            }
        }

        /// <summary>
        /// Toggles the full screen mode.
        /// </summary>
        public void ToggleFullScreenMode()
        {
            if (this.WindowStyle == WindowStyle.None)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = _nonFullScreenWindowState; ;
                mainMenu.Visibility = Visibility.Visible;
            }
            else
            {
                mainMenu.Visibility = Visibility.Visible;
            }
            else
            {
                _nonFullScreenWindowState = WindowState;
                WindowState = System.Windows.WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                mainMenu.Visibility = Visibility.Collapsed;
            }
        }
    }
}
