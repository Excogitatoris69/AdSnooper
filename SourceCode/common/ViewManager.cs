using AdSnooperGui.appsettings;
using AdSnooperGui.models;
using AdSnooperGui.viewmodels;
using AdSnooperGui.views;
using CoreAdSnooper.interfaces;
using DomainAdSnooper.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdSnooperGui.common
{

    public class ViewManager
    {

        //public MainWindow? mainWindow{ get; set; }
        public IDataService dataService { get; set; }
        public AppSettings appSettings { get; set; }

        private Dictionary<string, WindowSizeSettings> windowSizeSettingsDic = null; // WindowName, Settings
        private SettingsWindow? settingsWindow = null;
        private SettingsWindowVM? settingsWindowVM = null;
        private ExportWindow? exportWindow = null;
        private ExportWindowVM? exportWindowVM = null;
        private GroupDiffWindow? groupDiffWindow = null;
        private GroupDiffWindowVM? groupDiffWindowVM = null;

        private MainWindow? mainWindow = null;
        private MainWindowVM? mainWindowVM = null;


        private CustomMessageBox? customMessageBoxWindow = null;
        private CustomMessageBoxVM? customMessageBoxWindowVM = null;
        private CustomInputBox? customInputBoxWindow = null;
        private CustomInputBoxVM? customInputBoxWindowVM = null;


        public event EventHandler OnAllWindowsCloseEvent;

        public ViewManager()
        {
            windowSizeSettingsDic = new Dictionary<string, WindowSizeSettings>();
        }



        /// <summary>
        /// Sendet an alle windows, dass
        /// alle Fenster schließen müssen.
        /// </summary>
        public void sendAllWindowsCloseEvent()
        {
            OnAllWindowsCloseEvent?.Invoke(this, new EventArgs());
        }

        public void openSettingswindow()
        {
            if (mainWindow == null) throw new ArgumentNullException("MainWindow is null");
            if (settingsWindow == null)
            {
                if (settingsWindowVM == null)
                    settingsWindowVM = new SettingsWindowVM(appSettings, dataService, this);
                settingsWindow = new SettingsWindow(settingsWindowVM);
                settingsWindow.Left = mainWindow.Left + 100;
                settingsWindow.Top = mainWindow.Top + 100;
                settingsWindow.Width = 600;
                settingsWindow.Height = 350;
                settingsWindow.Closed += (s, e) =>
                {
                    settingsWindow.Owner = null;
                    settingsWindow = null;
                    settingsWindowVM = null;
                };
                settingsWindow.Owner = mainWindow;
                settingsWindow.ShowDialog();
            }
        }

        public void openExportWindow(ExportJobParameterDto parameter)
        {
            if (mainWindow == null) throw new ArgumentNullException("MainWindow is null");
            if (exportWindow == null)
            {
                if (exportWindowVM == null)
                    exportWindowVM = new ExportWindowVM(dataService, parameter, appSettings);
                exportWindow = new ExportWindow(exportWindowVM);
                exportWindow.Left = mainWindow.Left + 100;
                exportWindow.Top = mainWindow.Top + 100;
                exportWindow.Width = 700;
                exportWindow.Height = 350;
                exportWindow.Closed += (s, e) =>
                {
                    exportWindow.Owner = null;
                    exportWindow = null;
                    exportWindowVM = null;
                };
                exportWindow.Owner = mainWindow;
                exportWindow.ShowDialog();
            }
        }

        public void openGroupDiffWindow(GroupDiffWindowParameter parameter)
        {
            if (mainWindow == null) throw new ArgumentNullException("MainWindow is null");
            if (groupDiffWindow == null)
            {
                if (groupDiffWindowVM == null)
                    groupDiffWindowVM = new GroupDiffWindowVM(parameter, appSettings);

                WindowSizeSettings windowSizeSettings = getWindowSizeSettings("GroupDiffWindow", mainWindow.Left + 100, mainWindow.Top + 100, 700, 350);
                groupDiffWindowVM.windowLeft = windowSizeSettings.windowLeft;
                groupDiffWindowVM.windowTop = windowSizeSettings.windowTop;
                groupDiffWindowVM.windowWidth = windowSizeSettings.windowWidth;
                groupDiffWindowVM.windowHeight = windowSizeSettings.windowHeight;
                groupDiffWindow = new GroupDiffWindow(groupDiffWindowVM);
                groupDiffWindow.Closed += (s, e) =>
                {
                    setWindowSizeSettings("GroupDiffWindow", groupDiffWindowVM.windowLeft, groupDiffWindowVM.windowTop, groupDiffWindowVM.windowWidth, groupDiffWindowVM.windowHeight);
                    groupDiffWindow.Owner = null;
                    groupDiffWindow = null;
                    groupDiffWindowVM = null;
                };
                groupDiffWindow.Owner = mainWindow;
                groupDiffWindow.ShowDialog();
            }
        }


        
        public void openMainWindow()
        {
            if (mainWindowVM == null)
                mainWindowVM = new MainWindowVM(dataService, this, appSettings);
            mainWindowVM.windowLeft = appSettings.A001_MainPosX;
            mainWindowVM.windowTop = appSettings.A002_MainPosY;
            mainWindowVM.windowWidth = appSettings.A004_MainWidth;
            mainWindowVM.windowHeight = appSettings.A003_MainHeight;
            mainWindow = new MainWindow(mainWindowVM);
            mainWindow.Show(); // not modal
        }

        /// <summary>
        /// Liefert die gespeicherten SizeSettings des Window mit dem gesuchten Namen.
        /// Existieren noich keine, werden sie neu angelegt.
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        private WindowSizeSettings getWindowSizeSettings(string windowName, double left, double top, double width, double height)
        {
            WindowSizeSettings windowSizeSettings = null;
            bool r = windowSizeSettingsDic.TryGetValue(windowName, out windowSizeSettings);
            if (!r)
            {
                windowSizeSettings = new WindowSizeSettings();
                windowSizeSettings.windowLeft = left;
                windowSizeSettings.windowTop = top;
                windowSizeSettings.windowWidth = width;
                windowSizeSettings.windowHeight = height;
                windowSizeSettingsDic.Add(windowName, windowSizeSettings);
            }
            return windowSizeSettings;
        }

        /// <summary>
        /// Setzt die SizeSettings des Window.
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void setWindowSizeSettings(string windowName, double left, double top, double width, double height)
        {
            WindowSizeSettings windowSizeSettings = null;
            bool r = windowSizeSettingsDic.TryGetValue(windowName, out windowSizeSettings);
            if (r)
            {
                windowSizeSettings.windowLeft = left;
                windowSizeSettings.windowTop = top;
                windowSizeSettings.windowWidth = width;
                windowSizeSettings.windowHeight = height;
            }
        }



        public void showMesgBoxError(CustomMessageBoxParameter parameter)
        {
            //MessageBox.Show(messageText, title, MessageBoxButton.OK, MessageBoxImage.Error);
            if (mainWindow == null) throw new ArgumentNullException("MainWindow is null");
            if (customMessageBoxWindow == null)
            {
                if (customMessageBoxWindowVM == null)
                    customMessageBoxWindowVM = new CustomMessageBoxVM(parameter, appSettings);
                customMessageBoxWindowVM.isProgressBarVisibility = parameter.isProgressBarVisibility;
                customMessageBoxWindowVM.progressBarValue = parameter.progressBarValue;

                customMessageBoxWindow = new CustomMessageBox(customMessageBoxWindowVM);
                customMessageBoxWindow.Left = mainWindow.Left + 200;
                customMessageBoxWindow.Top = mainWindow.Top + 200;
                customMessageBoxWindow.Width = parameter.windowWidth!=0? parameter.windowWidth:300;
                customMessageBoxWindow.Height = parameter.windowHeight!=0 ? parameter.windowHeight:150;
                customMessageBoxWindow.Closed += (s, e) =>
                {
                    customMessageBoxWindow.Owner = null;
                    customMessageBoxWindow = null;
                    customMessageBoxWindowVM = null;
                };
                customMessageBoxWindow.Owner = mainWindow;
                customMessageBoxWindow.ShowDialog();
            }
        }

        public void showInputBox(CustomMessageBoxParameter parameter)
        {
            if (mainWindow == null) throw new ArgumentNullException("MainWindow is null");
            if (customInputBoxWindow == null)
            {
                if (customInputBoxWindowVM == null)
                    customInputBoxWindowVM = new CustomInputBoxVM(parameter, appSettings);
                customInputBoxWindow = new CustomInputBox(customInputBoxWindowVM);
                customInputBoxWindow.Left = mainWindow.Left + 200;
                customInputBoxWindow.Top = mainWindow.Top + 200;
                customInputBoxWindow.Width = parameter.windowWidth != 0 ? parameter.windowWidth : 300;
                customInputBoxWindow.Height = parameter.windowHeight != 0 ? parameter.windowHeight : 150;
                customInputBoxWindow.Closed += (s, e) =>
                {
                    customInputBoxWindow.Owner = null;
                    customInputBoxWindow = null;
                    customInputBoxWindowVM = null;
                };
                customInputBoxWindow.Owner = mainWindow;
                customInputBoxWindow.ShowDialog();
            }
        }

    }
}
