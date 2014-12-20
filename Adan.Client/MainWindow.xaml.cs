using Adan.Client.Commands;
using Adan.Client.CommandSerializers;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Model;
using Adan.Client.Common.Plugins;
using Adan.Client.Common.Settings;
using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils;
using Adan.Client.Common.ViewModel;
using Adan.Client.Controls;
using Adan.Client.ConveyorUnits;
using Adan.Client.Dialogs;
using Adan.Client.MessageDeserializers;
using Adan.Client.Model.ActionDescriptions;
using Adan.Client.Model.ActionParameters;
using Adan.Client.Model.Actions;
using Adan.Client.Model.ParameterDescriptions;
using Adan.Client.Properties;
using Adan.Client.ViewModel;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.AvalonDock.Themes;

namespace Adan.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindowEx.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants and Fields

        private WindowState nonFullScreenWindowState;
        private IList<LayoutContent> _allWidgets = new List<LayoutContent>();
        private IList<OutputWindow> _outputWindows = new List<OutputWindow>();
        private IList<Hotkey> _globalHotkeys = new List<Hotkey>();
        private double nonFullScreenWindowWidth;
        private double nonFullScreenWindowHeight;

        #endregion

        #region Constructors
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

            foreach (var plugin in PluginHost.Instance.AllPlugins)
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

            MessageConveyor.AddMessageDeserializer(new TextMessageDeserializer());
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
                _themesMenuItem.Items.Add(menuItem);
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
                    _optionsMenuItem.Items.Insert(0, menuItem);

                    if (_optionsSeparator.Visibility != Visibility.Visible)
                        _optionsSeparator.Visibility = Visibility.Visible;
                }
            }

            MessageConveyor.AddConveyorUnit(new ProtocolVersionUnit());
            MessageConveyor.AddConveyorUnit(new CommandRepeaterUnit());
            MessageConveyor.AddConveyorUnit(new CapForLineCommandUnit());
            MessageConveyor.AddConveyorUnit(new ConnectionUnit());

            Top = SettingsHolder.Instance.Settings.MainWindowTop;
            Left = SettingsHolder.Instance.Settings.MainWindowLeft;
            Width = SettingsHolder.Instance.Settings.MainWindowWidth;
            Height = SettingsHolder.Instance.Settings.MainWindowHeight;
            WindowState = SettingsHolder.Instance.Settings.MainWindowState;

            _dockManager.ActiveContentChanged += _dockManager_ActiveContentChanged;
            _dockManager.Theme = new ExpressionDarkTheme();
        }

        #endregion

        #region Layouts

        private void HandleThemeChange([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var newTheme = (ThemeDescription)((MenuItem)sender).Tag;
            ThemeManager.Instance.SwitchToTheme(newTheme);
            PluginHost.Instance.ApplyAdditionalPluginMergeDictionaries();

            foreach (MenuItem item in _themesMenuItem.Items)
            {
                item.IsChecked = item.Tag == ThemeManager.Instance.ActiveTheme;
            }
        }

        private void HandleDockManagerLoaded([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            LoadLayout();

            foreach (var widget in PluginHost.Instance.Widgets)
            {
                if (!_allWidgets.Any(x => x.ContentId == widget.Name))
                {
                    CreateWidget(widget);
                }
            }

            if (_outputWindows.Count == 0)
            {
                CreateOutputWindow("Default", "Default" + Guid.NewGuid().ToString("N"));
            }

            _dockManager.ActiveContent = _outputWindows.FirstOrDefault().VisibleControl;
        }

        private void LoadLayout()
        {
            var layoutFullPath = Path.Combine(SettingsHolder.Instance.Folder, "Settings", "Layout.xml");
            if (File.Exists(layoutFullPath))
            {
                try
                {
                    var serializer = new XmlLayoutSerializer(_dockManager);
                    serializer.LayoutSerializationCallback += LayoutSerializationCallback;
                    serializer.Deserialize(layoutFullPath);
                }
                catch (Exception ex) 
                {
                    ErrorLogger.Instance.Write(string.Format("Error load layout: {0}", ex.ToString()));
                }
            }
        }

        private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs args)
        {
            if (args.Model.ContentId.StartsWith("Plugin"))
            {
                var widgetDescription = PluginHost.Instance.Widgets.FirstOrDefault(x => x.Name == args.Model.ContentId);
                if (widgetDescription != null)
                {
                    args.Content = widgetDescription.Control;
                    _allWidgets.Add(args.Model);

                    var menuItem = new MenuItem()
                    {
                        Header = widgetDescription.Description,
                        Tag = args.Model,
                    };

                    var visibleBinding = new Binding("IsVisible")
                    {
                        Source = args.Model,
                        Mode = BindingMode.OneWay,
                    };

                    menuItem.SetBinding(MenuItem.IsCheckedProperty, visibleBinding);
                    menuItem.Click += HandleHideShowWidget;
                    _viewMenuItem.Items.Add(menuItem);
                }
                else
                {
                    args.Cancel = true;
                }
            }
            else
            {
                var outputWindow = new OutputWindow(this, args.Model.Title);
                outputWindow.Uid = args.Model.ContentId;
                outputWindow.DockContent = args.Model;

                _outputWindows.Add(outputWindow);
                args.Content = outputWindow.VisibleControl;

                args.Model.Closed += OnOutputWindowClosed;

                var menuItem = new MenuItem()
                {
                    Header = outputWindow.Name,
                    Name = outputWindow.Uid,
                };

                menuItem.Click += (s, e) => { ShowOutputWindow((string)((MenuItem)s).Header); };

                if (_windowMenuItem.Items.Count > 0)
                    _windowMenuItem.Items.Insert(0, menuItem);
                else
                    _windowMenuItem.Items.Add(menuItem);

                if (_windowSeparator.Visibility != Visibility.Visible)
                    _windowSeparator.Visibility = Visibility.Visible;

                PluginHost.Instance.OutputWindowCreated(outputWindow);

                if (SettingsHolder.Instance.Settings.AutoConnect)
                {
                    outputWindow.RootModel.PushCommandToConveyor(
                        new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
                }
            }
        }

        private void CreateWidget(WidgetDescription widgetDescription)
        {
            var anchorable = new LayoutAnchorable()
            {
                CanAutoHide = false,
                CanFloat = true,
                CanHide = true,
                Title = widgetDescription.Description,
                Content = widgetDescription.Control,
                ContentId = widgetDescription.Name,
                FloatingHeight = widgetDescription.Height,
                FloatingWidth = widgetDescription.Width,
                FloatingLeft = widgetDescription.Left,
                FloatingTop = widgetDescription.Top,                
            };

            if (!string.IsNullOrEmpty(widgetDescription.Icon))
            {
                try
                {
                    anchorable.IconSource = (ImageSource)FindResource(widgetDescription.Icon);
                }
                catch (Exception)
                { }
            }

            var menuItem = new MenuItem()
            {
                Header = widgetDescription.Description,
                Tag = anchorable,
            };

            var visibleBinding = new Binding("IsVisible")
            {
                Source = anchorable,
                Mode = BindingMode.OneWay,
            };

            menuItem.SetBinding(MenuItem.IsCheckedProperty, visibleBinding);
            menuItem.Click += HandleHideShowWidget;
            _viewMenuItem.Items.Add(menuItem);

            _allWidgets.Add(anchorable);

            anchorable.AddToLayout(_dockManager, AnchorableShowStrategy.Most);
            ((LayoutAnchorablePane)anchorable.Parent).FloatingHeight = widgetDescription.Height;
            ((LayoutAnchorablePane)anchorable.Parent).FloatingWidth = widgetDescription.Width;
            ((LayoutAnchorablePane)anchorable.Parent).FloatingLeft = widgetDescription.Left;
            ((LayoutAnchorablePane)anchorable.Parent).FloatingTop = widgetDescription.Top;
            anchorable.Float();
        }

        private void CreateOutputWindow(string name, string uid)
        {
            OutputWindow outputWindow = new OutputWindow(this, name);
            _outputWindows.Add(outputWindow);
            outputWindow.Uid = uid;

            LayoutAnchorable anchorable = new LayoutAnchorable()
            {
                CanAutoHide = false,
                CanClose = true,
                CanFloat = true,
                CanHide = false,
                Title = name,
                Content = outputWindow.VisibleControl,
                ContentId = uid,
                FloatingHeight = 600,
                FloatingWidth = 800,
            };

            outputWindow.DockContent = anchorable;

            anchorable.Closed += OnOutputWindowClosed;

            var menuItem = new MenuItem()
            {
                Header = outputWindow.Name,
                Name = outputWindow.Uid,
            };

            menuItem.Click += (s, e) => { ShowOutputWindow((string)((MenuItem)s).Header); };

            if (_windowMenuItem.Items.Count > 0)
                _windowMenuItem.Items.Insert(0, menuItem);
            else
                _windowMenuItem.Items.Add(menuItem);

            if (_windowSeparator.Visibility != Visibility.Visible)
                _windowSeparator.Visibility = Visibility.Visible;

            PluginHost.Instance.OutputWindowCreated(outputWindow);

            var firstDocumentPane = _dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                firstDocumentPane.Children.Add(anchorable);
            }

            if (SettingsHolder.Instance.Settings.AutoConnect)
            {
                outputWindow.RootModel.PushCommandToConveyor(
                    new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
            }
        }

        #endregion

        #region Windows interaction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void ShowOutputWindow(string name)
        {
            OutputWindow outputWindowToSelect = null;
            if (string.IsNullOrEmpty(name))
            {
                var activeContent = _dockManager.ActiveContent as MainOutputWindowNative;
                if (activeContent != null)
                {
                    var currentWindow = _outputWindows.FirstOrDefault(x => x.Uid == activeContent.RootModel.Uid);
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
                else if(_outputWindows.Count > 0)
                {
                    outputWindowToSelect = _outputWindows[0];
                }
            }
            else
            {
                outputWindowToSelect = _outputWindows.FirstOrDefault(x => x.Name == name);
            }

            if (outputWindowToSelect != null)
            {
                _dockManager.ActiveContent = outputWindowToSelect.VisibleControl;
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
                try
                {
                    action.Execute(outputWindow.RootModel, actionExecutionContext);
                }
                catch (Exception)
                { }
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
                    try
                    {
                        action.Execute(outputWindow.RootModel, actionExecutionContext);
                    }
                    catch (Exception)
                    { }
                }
            }
        }        

        private void OnOutputWindowClosed(object sender, EventArgs e)
        {
            var dockable = (LayoutAnchorable)sender;
            var outputWindow = _outputWindows.FirstOrDefault(output => output.Uid == dockable.ContentId);

            if (outputWindow != null)
            {
                outputWindow.Save();
                PluginHost.Instance.OutputWindowClose(outputWindow);
                outputWindow.Dispose();
                _outputWindows.Remove(outputWindow);

                foreach (var item in _windowMenuItem.Items)
                {
                    var menuItem = item as MenuItem;
                    if (menuItem != null && menuItem.Name == outputWindow.Uid)
                    {
                        _windowMenuItem.Items.Remove(menuItem);
                        break;
                    }
                }

                if (_windowMenuItem.Items.Count <= 2)
                    _windowSeparator.Visibility = Visibility.Collapsed;
            }
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

                    CreateOutputWindow(name, name + Guid.NewGuid().ToString("N"));
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error add new window: {0}\r\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private void _dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            var activeContent = _dockManager.ActiveContent as MainOutputWindowNative;
            if (activeContent != null)
            {
                var dockContent = _outputWindows.FirstOrDefault(x => { return x.Uid == activeContent.RootModel.Uid; });
                if (dockContent != null)
                {
                    dockContent.Focus();
                }
            }
        }

        #endregion

        #region Hotkeys

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
                var activeContent = _dockManager.ActiveContent as MainOutputWindowNative;
                if (activeContent != null)
                {
                    var outputWindow = _outputWindows.FirstOrDefault(x => { return x.Uid == activeContent.RootModel.Uid; });

                    if (outputWindow != null)
                    {
                        foreach (var action in hotkey.Actions)
                        {
                            try
                            {
                                if (action.IsGlobal)
                                    action.Execute(null, null);
                                else if (outputWindow.RootModel != null)
                                    action.Execute(outputWindow.RootModel, ActionExecutionContext.Empty);
                            }
                            catch (Exception)
                            { }

                        }
                    }
                }

                e.Handled = true;
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

        #endregion

        #region Buttons

        private void HandleAbout([NotNull]object sender, [NotNull]RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var aboutDialog = new AboutDialog() { Owner = this, }.ShowDialog();
        }

        private void HandleConnect([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var activeContent = _dockManager.ActiveContent as MainOutputWindowNative;
            if (activeContent != null)
            {
                var outputWindow = _outputWindows.FirstOrDefault(x => { return x.Uid == activeContent.RootModel.Uid; });
                if (outputWindow == null)
                {
                    CreateOutputWindow("Default", Guid.NewGuid().ToString("N"));
                }

                outputWindow.RootModel.PushCommandToConveyor(
                        new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
            }
        }

        private void HandleConnectAll([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (_outputWindows.Count == 0)
                CreateOutputWindow("Default", Guid.NewGuid().ToString("N"));

            foreach (OutputWindow window in _outputWindows)
                window.RootModel.PushCommandToConveyor(new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
        }

        private void HandleDisconnect([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var activeContent = _dockManager.ActiveContent as MainOutputWindowNative;
            if (activeContent != null)
            {
                var outputWindow = _outputWindows.FirstOrDefault(x => { return x.Uid == activeContent.RootModel.Uid; });

                if (outputWindow != null)
                    outputWindow.RootModel.PushCommandToConveyor(new DisconnectCommand());
            }
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

        private void HandleHideShowWidget([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var dockContent = (LayoutAnchorable)((MenuItem)sender).Tag;
            if (!dockContent.IsHidden)
            {
                dockContent.Hide();
            }
            else
            {
                dockContent.Show();
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

            profileDialog.Show();
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

        #endregion

        #region Logging
        
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

        #endregion

        /// <summary>
        /// Toggles the full screen mode.
        /// </summary>
        public void ToggleFullScreenMode()
        {
            if (this.WindowStyle == WindowStyle.None)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = nonFullScreenWindowState;
                ResizeMode = ResizeMode.CanResizeWithGrip;
                Width = nonFullScreenWindowWidth;
                Height = nonFullScreenWindowHeight;
                _mainMenu.Visibility = Visibility.Visible;
            }
            else
            {
                nonFullScreenWindowHeight = Height;
                nonFullScreenWindowWidth = Width;
                nonFullScreenWindowState = WindowState;
                Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                Top = 0;
                Left = 0;
                WindowState = System.Windows.WindowState.Normal;
                WindowStyle = WindowStyle.None;
                ResizeMode = System.Windows.ResizeMode.NoResize;
                _mainMenu.Visibility = Visibility.Collapsed;
            }
        }        

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing([NotNull] CancelEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            SaveAllSettings();

            base.OnClosing(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveAllSettings()
        {
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
                if (!File.Exists(layoutFullPath))
                    File.Delete(layoutFullPath);

                new XmlLayoutSerializer(_dockManager).Serialize(layoutFullPath);
            }
            catch (Exception ex)
            {
                ErrorLogger.Instance.Write(string.Format("Error save layout:{0}\r\n{1}", ex.Message, ex.StackTrace));
            }

            try
            {
                foreach (var outputWindow in _outputWindows)
                {
                    outputWindow.Save();
                }

                SettingsHolder.Instance.Settings.MainWindowTop = (int)Top;
                SettingsHolder.Instance.Settings.MainWindowLeft = (int)Left;
                SettingsHolder.Instance.Settings.MainWindowWidth = (int)Width;
                SettingsHolder.Instance.Settings.MainWindowHeight = (int)Height;
                SettingsHolder.Instance.Settings.MainWindowState = WindowState;

                SettingsHolder.Instance.SaveAllSettings();
            }
            catch (Exception ex)
            {
                ErrorLogger.Instance.Write(string.Format("Error save settings:{0}\r\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
