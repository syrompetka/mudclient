using Adan.Client.Commands;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Model;
using Adan.Client.Common.Plugins;
using Adan.Client.Common.Settings;
using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils;
using Adan.Client.Common.ViewModel;
using Adan.Client.Controls;
using Adan.Client.Dialogs;
using Adan.Client.Model.ActionDescriptions;
using Adan.Client.Model.ActionParameters;
using Adan.Client.Model.Actions;
using Adan.Client.Model.ParameterDescriptions;
using Adan.Client.ViewModel;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
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
using Adan.Client.Resources.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Adan.Client.Common.Messages;

namespace Adan.Client
{
    using System.Xml.Serialization;
    using Settings;

    public partial class MainWindow
    {
        #region Constants and Fields

        private readonly IList<Window> _allWidgets = new List<Window>();
        private readonly IList<OutputWindow> _outputWindows = new List<OutputWindow>();
        private readonly IList<RootModel> _allRootModels = new List<RootModel>();

        private WindowState _nonFullScreenWindowState;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //Load all types for deserialization
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
                                typeof(ShowOutputWindowAction),
                                typeof(SendToWindowAction),
                                typeof(ToggleFullScreenModeAction),
                            };

            //Load plugins
            foreach (var plugin in PluginHost.Instance.AllPlugins)
            {
                foreach (var customType in plugin.CustomSerializationTypes)
                {
                    types.Add(customType);
                }
            }

            //Load settings
            Properties.Settings settings = Properties.Settings.Default;
            settings.Reload();

            SettingsHolder.Instance.Initialize((SettingsFolder)settings.SettingsFolder, types);
            SettingsHolder.Instance.ErrorOccurred += HandleSettingsError;

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
            actionDescriptions.Add(new ToggleFullScreenModeActionDescription(actionDescriptions));

            parameterDescriptions.Add(new TriggerOrCommandParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new VariableReferenceParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new MathExpressionParameterDescription(parameterDescriptions));
            parameterDescriptions.Add(new ConstantStringParameterDescription(parameterDescriptions));

            RootModel.AllActionDescriptions = actionDescriptions;
            RootModel.AllParameterDescriptions = parameterDescriptions;

            //Initialize themes and add their to menu
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
                .ContinueWith(t => Dispatcher.Invoke(initializationDalog.Close));
            initializationDalog.ShowDialog();

            //Initialize plugins
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

            //Initialize window's position
            Top = SettingsHolder.Instance.Settings.MainWindowTop;
            Left = SettingsHolder.Instance.Settings.MainWindowLeft;
            Width = SettingsHolder.Instance.Settings.MainWindowWidth;
            Height = SettingsHolder.Instance.Settings.MainWindowHeight;
            WindowState = SettingsHolder.Instance.Settings.MainWindowState;

            _dockManager.ActiveContentChanged += _dockManager_ActiveContentChanged;
            _dockManager.Theme = new AvalonDockDarkTheme();
        }

        #endregion

        private void HandleSettingsError(object sender, SettingsErrorEventArgs e)
        {
            var activeContent = _dockManager.ActiveContent as MainOutputWindow;
            if (activeContent != null)
            {
                var outputWindow = _outputWindows.FirstOrDefault(x => { return x.Uid == activeContent.RootModel.Uid; });

                if (outputWindow != null)
                {
                    outputWindow.RootModel.PushMessageToConveyor(new ErrorMessage(e.Message));
                }
            }
            else if (_outputWindows.Count > 0)
            {
                _outputWindows.First().RootModel.PushMessageToConveyor(new ErrorMessage(e.Message));
            }
        }


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

            foreach (var widget in PluginHost.Instance.Widgets)
            {
                _allWidgets.Add(CreateWidget(widget));
            }

            LoadLayout();

            if (_outputWindows.Count == 0)
            {
                CreateOutputWindow("Default", "a" + Guid.NewGuid().ToString("N"));
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
                    ErrorLogger.Instance.Write(string.Format("Error loading layout: {0}", ex));
                }
            }

            var widgetLayoutFullPath = Path.Combine(SettingsHolder.Instance.Folder, "Settings", "WidgetLayout.xml");
            if (File.Exists(widgetLayoutFullPath))
            {
                try
                {
                    using (var stream = File.OpenRead(widgetLayoutFullPath))
                    {

                        var serializer = new XmlSerializer(typeof(WidgetLayout));
                        var widgetLayout = (WidgetLayout)serializer.Deserialize(stream);
                        foreach (var widgetLayoutItem in widgetLayout.Widgets)
                        {
                            var widgetWindow = _allWidgets.FirstOrDefault(w => w.Tag != null && w.Tag is WidgetDescription && ((WidgetDescription)w.Tag).Name == widgetLayoutItem.WidgetName);
                            if (widgetWindow != null)
                            {
                                var widgetDescription = (WidgetDescription)widgetWindow.Tag;
                                widgetWindow.Visibility = widgetLayoutItem.Visible ? Visibility.Visible : Visibility.Collapsed;
                                widgetWindow.Left = widgetLayoutItem.Left;
                                widgetWindow.Top = widgetLayoutItem.Top;
                                if (!widgetDescription.ResizeToContent)
                                {
                                    widgetWindow.Height = widgetLayoutItem.Height;
                                    widgetWindow.Width = widgetLayoutItem.Width;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error loading widget layout: {0}", ex));
                }
            }
        }

        private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs args)
        {
            if (args.Model.ContentId.StartsWith("Plugin"))
            {
                args.Cancel = true;
            }
            else
            {
                var outputWindow = new OutputWindow(this, args.Model.Title, _allRootModels)
                {
                    Uid = args.Model.ContentId,
                    DockContent = args.Model
                };

                _outputWindows.Add(outputWindow);
                args.Content = outputWindow.VisibleControl;

                args.Model.Closed += OnOutputWindowClosed;

                var menuItem = new MenuItem
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

                PluginHost.Instance.OutputWindowCreated(outputWindow.RootModel);

                if (SettingsHolder.Instance.Settings.AutoConnect)
                {
                    outputWindow.RootModel.PushCommandToConveyor(
                        new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
                }
            }
        }

        private Window CreateWidget(WidgetDescription widgetDescription)
        {
            Window widgedWindow;
            if (widgetDescription.ResizeToContent)
            {
                widgedWindow = new AutoSizableWidgetWindow
                {
                    Title = widgetDescription.Description,
                    Left = widgetDescription.Left,
                    Top = widgetDescription.Top,
                    Content = widgetDescription.Control,
                    Tag = widgetDescription,
                };
            }
            else
            {
                widgedWindow = new WidgetWindow
                {
                    Height = widgetDescription.Height,
                    Width = widgetDescription.Width,
                    Title = widgetDescription.Description,
                    Left = widgetDescription.Left,
                    Top = widgetDescription.Top,
                    Content = widgetDescription.Control,
                    Tag = widgetDescription,
                };
            }

            widgedWindow.Owner = this;
            var menuItem = new MenuItem()
            {
                Header = widgetDescription.Description,
                Tag = widgedWindow,
            };

            var visibleBinding = new Binding("IsVisible")
            {
                Source = widgedWindow,
                Mode = BindingMode.OneWay,
            };

            menuItem.SetBinding(MenuItem.IsCheckedProperty, visibleBinding);
            menuItem.Click += HandleHideShowWidget;
            _viewMenuItem.Items.Add(menuItem);

            widgedWindow.Show();
            return widgedWindow;
        }

        private void CreateOutputWindow(string name, string uid)
        {
            OutputWindow outputWindow = new OutputWindow(this, name, _allRootModels);
            _outputWindows.Add(outputWindow);
            outputWindow.Uid = uid;

            LayoutAnchorable anchorable = new LayoutAnchorable
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

            var menuItem = new MenuItem
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

            PluginHost.Instance.OutputWindowCreated(outputWindow.RootModel);

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
                var activeContent = _dockManager.ActiveContent as MainOutputWindow;
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
                else if (_outputWindows.Count > 0)
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

        private void OnOutputWindowClosed(object sender, EventArgs e)
        {
            var dockable = (LayoutAnchorable)sender;
            var outputWindow = _outputWindows.FirstOrDefault(output => output.Uid == dockable.ContentId);

            if (outputWindow != null)
            {
                outputWindow.Save();
                PluginHost.Instance.OutputWindowClose(outputWindow.RootModel);
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

                    //Name of MenuItem cannot start with number
                    CreateOutputWindow(name, "a" + Guid.NewGuid().ToString("N"));
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error add new window: {0}\r\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private void _dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            var activeContent = _dockManager.ActiveContent as MainOutputWindow;
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

        private void HandleGlobalProfile([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var globalProfileDialog = new ProfileOptionsEditDialog("Global")
            {
                DataContext = new ProfileOptionsViewModel("Global", SettingsHolder.Instance.Settings.GlobalGroups),
                Owner = this,
            };

            globalProfileDialog.Show();
            globalProfileDialog.Closed += (s, a) =>
            {
                SettingsHolder.Instance.SaveCommonSettings();
            };
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

            var activeContent = _dockManager.ActiveContent as MainOutputWindow;
            if (activeContent != null)
            {
                var outputWindow = _outputWindows.FirstOrDefault(x => { return x.Uid == activeContent.RootModel.Uid; });
                if (outputWindow == null)
                {
                    CreateOutputWindow("Default", "a" + Guid.NewGuid().ToString("N"));
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
                CreateOutputWindow("Default", "a" + Guid.NewGuid().ToString("N"));

            foreach (OutputWindow window in _outputWindows)
                window.RootModel.PushCommandToConveyor(new ConnectCommand(SettingsHolder.Instance.Settings.ConnectHostName, SettingsHolder.Instance.Settings.ConnectPort));
        }

        private void HandleDisconnect([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var activeContent = _dockManager.ActiveContent as MainOutputWindow;
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

            var dockContent = (Window)((MenuItem)sender).Tag;
            if (dockContent.Visibility == Visibility.Visible)
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
                SelectedFont = SettingsHolder.Instance.Settings.MUDFontName,
                SelectedFontSize = SettingsHolder.Instance.Settings.MUDFontSize,
                SelectedFontWeight = SettingsHolder.Instance.Settings.MudFontWeight,
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
                SettingsHolder.Instance.Settings.ColorTheme = model.SelectedTheme.Name;
                ThemeManager.Instance.ActiveTheme = model.SelectedTheme;
                SettingsHolder.Instance.Settings.MUDFontName = model.SelectedFont;
                SettingsHolder.Instance.Settings.MUDFontSize = model.SelectedFontSize;
                SettingsHolder.Instance.Settings.MudFontWeight = model.SelectedFontWeight;

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

        /// <summary>
        /// Toggles the full screen mode.
        /// </summary>
        public void ToggleFullScreenMode()
        {
            if (WindowStyle == WindowStyle.None)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = _nonFullScreenWindowState;
                _mainMenu.Visibility = Visibility.Visible;
            }
            else
            {
                _nonFullScreenWindowState = WindowState;
                WindowState = System.Windows.WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
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
            PluginHost.Instance.Dispose();
            base.OnClosing(e);
        }

        public void SaveAllSettings()
        {
            try
            {
                if (WindowStyle == WindowStyle.None)
                {
                    ToggleFullScreenMode();
                }

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

            var widgetLayoutFullPath = Path.Combine(SettingsHolder.Instance.Folder, "Settings", "WidgetLayout.xml");
            try
            {
                var widgetLayout = new WidgetLayout { Widgets = new List<WidgetLayoutItem>() };
                foreach (var widget in _allWidgets)
                {
                    var widgetDescription = (WidgetDescription)widget.Tag;
                    var widgetLayoutItem = new WidgetLayoutItem
                    {
                        WidgetName = widgetDescription.Name,
                        Top = widget.Top,
                        Left = widget.Left,
                        Height = widget.Height,
                        Width = widget.Width,
                        Visible = widget.Visibility == Visibility.Visible,
                    };
                    widgetLayout.Widgets.Add(widgetLayoutItem);
                }
                using (var stream = File.Open(widgetLayoutFullPath, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(WidgetLayout));
                    serializer.Serialize(stream, widgetLayout);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Instance.Write(string.Format("Error saving widget layout: {0}", ex));
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
