using AdAdapter;
using AdAdapterSimu;
using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.models;
using AdSnooperGui.Properties;
using AdSnooperGui.viewmodels;
using AdSnooperGui.views;
using CoreAdSnooper.interfaces;
using CoreAdSnooper.services;
using DomainAdSnooper.dto;
using ExportAdapter;
using FileManagementAdapter;
using Microsoft.Extensions.DependencyInjection;
using SqlParserAdapter;
using System.Collections.Generic;
using System.IO;
using System.Windows;
namespace AdSnooperGui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider serviceProvider;
    private bool appSettingAdAdapterSimulation = true;
    private ViewManager aViewManager = null;
    private AppSettings appSettings = null;
    private IDataService dataService = null;
    private IAdAdapter aAdAdapter;
    private IFileManagementAdapter aFileManagementAdapter = null;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        //doesn't work
        //var vCulture = new CultureInfo("en-US");
        //Thread.CurrentThread.CurrentCulture = vCulture;
        //Thread.CurrentThread.CurrentUICulture = vCulture;
        //CultureInfo.DefaultThreadCurrentCulture = vCulture;
        //CultureInfo.DefaultThreadCurrentUICulture = vCulture;

        //read settings
        appSettings = new AppSettings();
        appSettings.readSettings();

        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ViewManager>();
        serviceCollection.AddSingleton<SettingsWindowVM>();
        serviceCollection.AddSingleton<IExportAdapter, ExportAdapterImpl>();
        serviceCollection.AddSingleton<IFileManagementAdapter, FileManagementAdapterImpl>();
        serviceCollection.AddSingleton<IDataService, DataServiceImpl>();
        serviceCollection.AddSingleton<ISqlQueryParser, SqlQueryParserImpl>();

        //if (appSettings.C001_AdAdapterSimulation)
        //    serviceCollection.AddSingleton<IAdAdapter,AdAdapterSimuImpl>();
        //else
        //{
        //}
        serviceCollection.AddSingleton<IAdAdapter, AdAdapterImpl>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        
        //string appPath = System.AppContext.BaseDirectory;
        //appPath = AppDomain.CurrentDomain.BaseDirectory;
        //appPath = Directory.GetCurrentDirectory();
        //appPath = Environment.CurrentDirectory;
        //appPath = this.GetType().Assembly.Location;

        aViewManager = serviceProvider.GetRequiredService<ViewManager>();
        aViewManager.appSettings = appSettings;
        dataService =  serviceProvider.GetRequiredService<IDataService>();
        aViewManager.dataService = dataService;

        aFileManagementAdapter =  serviceProvider.GetRequiredService<IFileManagementAdapter>();
        aFileManagementAdapter.queryFileBaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppSettings.APPLICATIONDATA_DIRNAME, AppSettings.BATCH_DIRNAME);
        
        init();

        //MainWindow aMainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        //aViewManager.mainWindow = aMainWindow;
        //aMainWindow.Show();
        dataService.startBackgroundTaskGenerateGroupnameCache();
        aViewManager.openMainWindow();
        checkLicense();
        
    }

    private void init()
    {
        aAdAdapter = serviceProvider.GetRequiredService<IAdAdapter>();
        aAdAdapter.adUserAttributeList = appSettings.adAttributeListUser;
        aAdAdapter.adGroupAttributeList = appSettings.adAttributeListGroup;
        initSqlEditorSettings();
        dataService.init();
    }

    private void initSqlEditorSettings()
    {

        dataService.adAttributeListGroup = appSettings.adAttributeListGroup;
        dataService.adAttributeListUser = appSettings.adAttributeListUser;

        dataService.sqlAdPersonFieldnameDescriptorList = new List<SqlAdFieldnameDescriptor>();
        dataService.sqlAdGroupFieldnameDescriptorList = new List<SqlAdFieldnameDescriptor>();

        //person
        foreach (AdAttributeDto attribute in appSettings.adAttributeListUser)
        {
            dataService.sqlAdPersonFieldnameDescriptorList.Add(new SqlAdFieldnameDescriptor(attribute));
        }
        //group
        foreach (AdAttributeDto attribute in appSettings.adAttributeListGroup)
        {
            dataService.sqlAdGroupFieldnameDescriptorList.Add(new SqlAdFieldnameDescriptor(attribute));
        }


    }


    private void checkLicense()
    {
        

        



    }

    protected override async void OnExit(ExitEventArgs e)
    {
        //await appHost.StopAsync();
        base.OnExit(e);
        appSettings.writeSettings();
        dataService.saveCache();
    }

}

