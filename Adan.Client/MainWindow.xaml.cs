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
    using Adan.Client.Common;
    using AvalonDock;
    using Commands;
    using CommandSerializers;
    using Common.Commands;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Model;
    using Common.Networking;
    using Common.Plugins;
    using Common.Themes;
    using Common.Utils;
    using ConveyorUnits;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Dialogs;
    using MessageDeserializers;
    using Microsoft.Win32;
    using Model.ActionDescriptions;
    using Model.ActionParameters;
    using Model.Actions;
    using Model.ParameterDescriptions;
    using ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MessageConveyor _converyor;
        private readonly MainWindowModel _model;
        private readonly HotkeyCommand _hotkeyCommand = new HotkeyCommand();
        private readonly Queue<Message> _messageQueue = new Queue<Message>();
        private readonly object _messageQueueLockObject = new object();
        private readonly IList<DockableContent> _allWidgets = new List<DockableContent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var mccpClient = new MccpClient();

            _converyor = new MessageConveyor(mccpClient);
            var allVariables = SettingsHolder.Instance.Variables;
            var allGroups = SettingsHolder.Instance.Groups;

            var actionDescriptions = new List<ActionDescription>();
            var parameterDescriptions = new List<ParameterDescription>();

            actionDescriptions.Add(new SendTextActionDescription(parameterDescriptions, actionDescriptions));
            actionDescriptions.Add(new OutputToMainWindowActionDescription(parameterDescriptions, actionDescriptions));
            actionDescriptions.Add(new ClearVariableValueActionDescription(actionDescriptions, allVariables));
            actionDescriptions.Add(new ConditionalActionDescription(parameterDescriptions, actionDescriptions));
            actionDescriptions.Add(new DisableGroupActionDescription(allGroups, actionDescriptions));
            actionDescriptions.Add(new EnableGroupActionDescription(allGroups, actionDescriptions));
            actionDescriptions.Add(new SetVariableValueActionDescription(actionDescriptions, parameterDescriptions, allVariables));
            actionDescriptions.Add(new StartLogActionDescription(actionDescriptions, parameterDescriptions));
            actionDescriptions.Add(new StopLogActionDescription(actionDescriptions));

            parameterDescriptions.Add(new TriggerOrCommandParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new VariableReferenceParameterDescription(parameterDescriptions, allVariables));
            parameterDescriptions.Add(new MathExpressionParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new ConstantStringParameterDescription(parameterDescriptions));

            var rootModel = new RootModel(_converyor, allGroups, allVariables, actionDescriptions, parameterDescriptions);

            foreach (var plugin in PluginHost.Instance.Plugins)
            {
                foreach (var customType in plugin.CustomSerializationTypes)
                {
                    rootModel.CustomSerializationTypes.Add(customType);
                }
            }

            _converyor.AddConveyorUnit(new CommandSeparatorUnit(_converyor, rootModel));

            _converyor.AddCommandSerializer(new TextCommandSerializer(_converyor));
            _converyor.AddMessageDeserializer(new TextMessageDeserializer(_converyor));
            _converyor.AddMessageDeserializer(new ProtocolVersionMessageDeserializer(_converyor));

            _converyor.AddConveyorUnit(new SubstitutionUnit(_converyor, rootModel));
            _converyor.AddConveyorUnit(new AliasUnit(_converyor, rootModel));
            _converyor.AddConveyorUnit(new CommandRepeaterUnit(_converyor));
            _converyor.AddConveyorUnit(new HotkeyUnit(_converyor, rootModel));
            _converyor.AddConveyorUnit(new CommandMultiplierUnit(_converyor));
            _converyor.AddConveyorUnit(new TriggerUnit(_converyor, rootModel));
            _converyor.AddConveyorUnit(new HighlightUnit(_converyor, rootModel));
            _converyor.AddConveyorUnit(new LoggingUnit(_converyor));
            _converyor.AddConveyorUnit(new CommandsFromUserLineUnit(_converyor, rootModel));

            _converyor.MessageReceived += HandleMessageFromServer;

            Loaded += (o, e) => txtCommandInput.Focus();

            txtCommandInput.Conveyor = _converyor;

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

            Task.Factory.StartNew(() => PluginHost.Instance.InitializePlugins(_converyor, rootModel, initializationDalog.ViewModel, this))
                .ContinueWith(t => Dispatcher.Invoke((Action)initializationDalog.Close));
            initializationDalog.ShowDialog();

            foreach (var plugin in PluginHost.Instance.Plugins)
            {
                if (plugin.HasOptions)
                {
                    var menuItem = new MenuItem { Header = plugin.OptionsMenuItemText };

                    var pluginClosure = plugin;
                    menuItem.Click += (o, e) => pluginClosure.ShowOptionsDialog(this);
                    optionsMenuItem.Items.Add(menuItem);
                }
            }

            int maxProtocolVersion = PluginHost.Instance.Plugins.Max(plugin => plugin.RequiredProtocolVersion);
            _converyor.AddConveyorUnit(new ProtocolVersionUnit(_converyor, maxProtocolVersion));

            // _model = new MainWindowModel(allGroups, allVariables, rootModel);
            //_model = new MainWindowModel(rootModel.Groups, rootModel.Variables, rootModel);
            _model = new MainWindowModel(rootModel);

            DataContext = _model;

            dockManager.DeserializationCallback = HandleFindWidget;

            //Заглушка для нераспознанных комманд с CharCommands
            _converyor.AddConveyorUnit(new CapForLineCommandUnit(_converyor));

            _converyor.AddConveyorUnit(new VariableReplaceUnit(_converyor, rootModel));

            //Проверка коннекта к серверу для команд
            _converyor.AddConveyorUnit(new ConnectionUnit(_converyor));
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown([NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            _hotkeyCommand.Key = e.Key == Key.System ? e.SystemKey : e.Key;
            _hotkeyCommand.ModifierKeys = Keyboard.Modifiers;
            _hotkeyCommand.Handled = false;
            _converyor.PushCommand(_hotkeyCommand);

            if (_hotkeyCommand.Handled)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Up && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.ShowPreviousCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Down && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.ShowNextCommand();
                e.Handled = true;
            }

            //if (e.Key == Key.Enter && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            if (e.Key == Key.Enter && txtCommandInput.IsFocused)
            {
                txtCommandInput.SendCurrentCommand();
                e.Handled = true;
            }

            if (e.Key == Key.PageUp && Keyboard.Modifiers == 0)
            {
                mainOutputWindow.PageUp();
                e.Handled = true;
            }

            if (e.Key == Key.PageDown && Keyboard.Modifiers == 0)
            {
                mainOutputWindow.PageDown();
                e.Handled = true;
            }

            if (e.Key == Key.End && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                mainOutputWindow.ScrollToEnd();
                e.Handled = true;
            }

            //Очищение коммандной строки клавишей escape
            if (e.Key == Key.Escape && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.Clear();
                e.Handled = true;
            }

            if (!e.Handled)
            {
                base.OnPreviewKeyDown(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing([NotNull] CancelEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            _converyor.Dispose();
            base.OnClosing(e);
        }

        private void HandleMessageFromServer([NotNull] object sender, [NotNull] MessageReceivedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            lock (_messageQueueLockObject)
            {
                _messageQueue.Enqueue(e.Message);
            }

            Dispatcher.BeginInvoke((Action)ProcessMessageQueue);
        }

        private void ProcessMessageQueue()
        {
            IList<Message> messages;
            lock (_messageQueueLockObject)
            {
                messages = _messageQueue.ToList();
                _messageQueue.Clear();
            }

            if (messages.Count > 0)
            {
                mainOutputWindow.AddMessages(messages.OfType<OutputToMainWindowMessage>());
            }
        }

        private void HandleConnect([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            _converyor.PushCommand(new ConnectCommand(SettingsHolder.Instance.ConnectHostName, SettingsHolder.Instance.ConnectPort));
        }

        private void HandleDisconnect([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            _converyor.PushCommand(new DisconnectCommand());
        }

        private void HandleConnectionPreference([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var connectionDialogViewModel = new ConnectionDialogViewModel { HostName = SettingsHolder.Instance.ConnectHostName, Port = SettingsHolder.Instance.ConnectPort };
            var dialog = new ConnectionDialog { DataContext = connectionDialogViewModel, Owner = this };


            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                SettingsHolder.Instance.ConnectHostName = connectionDialogViewModel.HostName;
                SettingsHolder.Instance.ConnectPort = connectionDialogViewModel.Port;
            }
        }

        private void HandleExit([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            Close();
        }

        private void HandleEditGroups([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var groupEditDialog = new GroupsEditDialog { DataContext = _model.GroupsModel, Owner = this };
            groupEditDialog.ShowDialog();
            _model.RootModel.RecalculatedEnabledTriggersPriorities();
        }

        private void HandleEditTriggers([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var triggerEditDialog = new TriggersEditDialog { DataContext = new TriggersViewModel(_model.GroupsModel.Groups, _model.RootModel.AllActionDescriptions), Owner = this };
            triggerEditDialog.ShowDialog();
            _model.RootModel.RecalculatedEnabledTriggersPriorities();
        }

        private void HandleEditAliases([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var aliasesEditDialog = new AliasesEditDialog { DataContext = new AliasesViewModel(_model.GroupsModel.Groups, _model.RootModel.AllActionDescriptions), Owner = this };
            aliasesEditDialog.ShowDialog();
        }

        private void HandleEditHotKeys([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var hotKeysEditDialog = new HotkeysEditDialog { DataContext = new HotkeysViewModel(_model.GroupsModel.Groups, _model.RootModel.AllActionDescriptions), Owner = this };
            hotKeysEditDialog.ShowDialog();
        }

        private void HandleEditSubstitutions([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var substitutionsEditDialog = new SubstitutionsEditDialog { DataContext = new SubstitutionsViewModel(_model.GroupsModel.Groups), Owner = this };
            substitutionsEditDialog.ShowDialog();
        }

        private void HandleEditHighlights([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var highlightsEditDialog = new HighlightsEditDialog { DataContext = new HighlightsViewModel(_model.GroupsModel.Groups), Owner = this };
            highlightsEditDialog.ShowDialog();
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
                    Content = widgetDescription.Control
                };

                dockContent.FloatingWindowSizeToContent = widgetDescription.ResizeToContent
                                                              ? SizeToContent.WidthAndHeight
                                                              : SizeToContent.Manual;

                if (!string.IsNullOrEmpty(widgetDescription.Icon))
                {
                    dockContent.Icon = (ImageSource)FindResource(widgetDescription.Icon);
                }

                _allWidgets.Add(dockContent);

                var menuItem = new MenuItem { Header = widgetDescription.Description, Tag = dockContent };

                var visibleBinding = new Binding("IsVisible") { Source = dockContent, Mode = BindingMode.OneWay };
                menuItem.SetBinding(MenuItem.IsCheckedProperty, visibleBinding);
                menuItem.Click += HandleHideShowWidget;
                viewMenuItem.Items.Add(menuItem);
                dockContent.Show(dockManager);
            }

            var layoutFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adan client", "Settings", "Layout.xml");
            if (File.Exists(layoutFullPath))
            {
                dockManager.RestoreLayout(layoutFullPath);
            }

            Dispatcher.BeginInvoke((Action)BindDockumentPane);
        }

        private void BindDockumentPane()
        {
            var singleItemBinding = new Binding("HasSingleItem")
            {
                RelativeSource = RelativeSource.Self,
                Converter = new InverseBooleanConverter()
            };

            TreeHelper.FindVisualChildren<DocumentPane>(dockManager).First().SetBinding(Pane.ShowHeaderProperty, singleItemBinding);
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
                dockContent.Show();
            }
        }

        private void HandleFindWidget([NotNull] object sender, [NotNull] DeserializationCallbackEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            e.Content = _allWidgets.FirstOrDefault(w => w.Name == e.Name);
        }

        private void HandleWindowClosing([NotNull] object sender, [NotNull] CancelEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var layoutFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adan client", "Settings");
            if (!Directory.Exists(layoutFullPath))
            {
                Directory.CreateDirectory(layoutFullPath);
            }

            layoutFullPath = Path.Combine(layoutFullPath, "Layout.xml");
            dockManager.SaveLayout(layoutFullPath);

            txtCommandInput.SaveCurrentHistory();
        }

        private void HandleEditProfile([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var models = new ObservableCollection<ProfileViewModel>();

            foreach (string str in ProfileHolder.Instance.AllProfiles)
            {
                models.Add(new ProfileViewModel(str, str == "Default" ? true : false));
            }

            var profilesViewModel = new ProfilesViewModel(models, ProfileHolder.Instance.Name);
            var profileDialog = new ProfilesDialog() { DataContext = profilesViewModel, Owner = this };

            var resultDialog = profileDialog.ShowDialog();
        }

        private void HandleSaveProfile([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            SettingsHolder.Instance.Save();
        }

        private void HandleImportJmcConfig([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".set";
            fileDialog.Filter = "Config|*.set|All Files|*.*";
            fileDialog.Multiselect = false;

            var result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ProfileHolder.Instance.ImportJmcConfig(fileDialog.FileName, _model.RootModel);
            }
        }

        private void HandleEditOptions([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var model = new OptionsViewModel()
            {
                AutoClearInput = SettingsHolder.Instance.AutoClearInput,
                AutoReconnect = SettingsHolder.Instance.AutoReconnect,
                CommandChar = SettingsHolder.Instance.CommandChar,
                CommandDelimiter = SettingsHolder.Instance.CommandDelimiter,
                StartOfLine = SettingsHolder.Instance.CursorPosition == CursorPositionHistory.StartOfLine,
                EndOfLine = SettingsHolder.Instance.CursorPosition == CursorPositionHistory.EndOfLine,
                HistorySize = SettingsHolder.Instance.HistorySize.ToString(),
                MinLengthHistory = SettingsHolder.Instance.MinLengthHistory.ToString(),
                ScrollBuffer = SettingsHolder.Instance.ScrollBuffer.ToString(),
            };

            var optionsDialog = new OptionsDialog() { DataContext = model, Owner = this };
            var dialogResult = optionsDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                SettingsHolder.Instance.AutoClearInput = model.AutoClearInput;
                SettingsHolder.Instance.AutoReconnect = model.AutoReconnect;
                SettingsHolder.Instance.CommandChar = model.CommandChar;
                SettingsHolder.Instance.CommandDelimiter = model.CommandDelimiter;
                if (model.StartOfLine)
                    SettingsHolder.Instance.CursorPosition = CursorPositionHistory.StartOfLine;
                else
                    SettingsHolder.Instance.CursorPosition = CursorPositionHistory.EndOfLine;

                int val;
                if(int.TryParse(model.HistorySize, out val))
                    SettingsHolder.Instance.HistorySize = val;

                if (int.TryParse(model.MinLengthHistory, out val))
                    SettingsHolder.Instance.MinLengthHistory = val;

                if(int.TryParse(model.ScrollBuffer, out val))
                    SettingsHolder.Instance.ScrollBuffer = val;
            }
        }
    }
}
