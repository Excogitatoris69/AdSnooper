using AdSnooperGui.appsettings;
using AdSnooperGui.common;
using AdSnooperGui.models;
using AdSnooperGui.Properties;
using CoreAdSnooper.interfaces;
using CoreAdSnooper.services;
using DomainAdSnooper.dto;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;


namespace AdSnooperGui.viewmodels
{

    public class SettingsWindowVM : BasicVM
    {
        public List<LanguageListItem> languageList { get; set; }
        public List<GeneralListItem> licenseTypeList { get; set; }
        public int languageListSelectedIndex { get; set; }
        public int licenseTypeListSelectedIndex { get; set; }
        public string versionString { get; set; }
        public string authorString { get; set; }
        public string contactString { get; set; }
        public string licenseCustomerString { get; set; }
        public string licenseAddressString { get; set; }
        public string licenseVolumeSizeString { get; set; }
        public string licenseTypeString { get; set; }
        public string licenseExpireDateString { get; set; }

        public string orderCustomerString { get; set; }
        public string orderAddressStreetString { get; set; }
        public string orderAddressCityString { get; set; }
        public string orderVolumeSizeString { get; set; }
        public string pathOrderFile { get; set; }
        public string purchasingText { get; set; }
        public string warningMessageText { get; set; }
        
        


        public Visibility volumeSizeVisibility { get; set; }
        public Visibility orderVolumeSizeVisibility { get; set; }
        public Visibility expireDateVisibility { get; set; }
        public Visibility warningMessageVisibility { get; set; }

        private AppSettings _appSettings = null;
        private AdSnooperLicenseInfoDto _licenseInfo = null;
        private IDataService dataService = null;
        private ViewManager viewManager = null;

        public SettingsWindowVM(AppSettings appSettings, IDataService dataService, ViewManager viewManager)
        {
            this._appSettings = appSettings;
            this.dataService = dataService;
            this._licenseInfo = dataService.licenseManager.licenseInfo;
            this.viewManager = viewManager;
            init();
        }

        private void init()
        {

            if (_appSettings.B001_Language == ELanguageSet.German.ToString())
            {
                var vCulture = new CultureInfo("de-DE");
                Resources.Culture = vCulture;
            }
            else
            {
                var vCulture = new CultureInfo("en-US");
                Resources.Culture = vCulture;
            }
            //Marker Versionsnr. anpassen
            windowTitle = Properties.Resources.L030_Settings + " - AD Snooper";
            versionString = "AD Snooper V1.1 Build: 26.07.2026-2114";
            authorString = "created by Oliver Matle";
            contactString = "Internet: www.xxx.de       e-Mail: xxx@yyy.de";
            purchasingText = Resources.M018_PurchasingIsEasy;

            //if (!string.IsNullOrEmpty(_licenseInfo.customerAddress))
            licenseCustomerString = _licenseInfo.customerName;
            licenseAddressString = _licenseInfo.customerAddress;
            licenseTypeString = _licenseInfo.licenseType.ToString();

            if (_licenseInfo.isLicenseValid)
                warningMessageVisibility = Visibility.Hidden;
            else
            {
                warningMessageText = Resources.M022_WarningLicenseFileInvalid;
                warningMessageVisibility = Visibility.Visible;
            }

            if (_licenseInfo.licenseType == ELicenseType.ExpireDate)
            {
                licenseExpireDateString = _licenseInfo.expireTime.ToString("dd.MM.yyyy");
                expireDateVisibility = Visibility.Visible;
                warningMessageText = Resources.M023_WarningLicenseLimited;
                warningMessageVisibility = Visibility.Visible;
            }
            else
            {
                licenseExpireDateString = "";
                expireDateVisibility = Visibility.Hidden;
            }

            if (_licenseInfo.licenseType == ELicenseType.Volume)
            {
                licenseVolumeSizeString = _licenseInfo.volumeSize;
                volumeSizeVisibility = Visibility.Visible;
            }
            else
            {
                licenseVolumeSizeString = "";
                volumeSizeVisibility = Visibility.Hidden;
            }

                /*
                if (_licenseInfo.licenseType == ELicenseType.Test)
                {
                }
                if (_licenseInfo.licenseType == ELicenseType.ExpireDate)
                {
                    licenseTypeString = "** Time limited Shareware **";
                    licenseExpireDateString = string.Format("Expire time: {0}", _licenseInfo.expireTime.ToString("dd.MM.yyyy"));
                    if (!string.IsNullOrEmpty(_licenseInfo.customerName)) 
                        licenseCustomerString = string.Format("Customer: {0}", _licenseInfo.customerName);
                }
                if (_licenseInfo.licenseType == ELicenseType.SingleUser)
                {
                    licenseTypeString = "** SingleUser-License **";
                    licenseCustomerString = string.Format("Customer: {0}", _licenseInfo.customerName);
                }
                if (_licenseInfo.licenseType == ELicenseType.Volume)
                {
                    licenseTypeString = "** Volume-License **";
                    licenseCustomerString = string.Format("Customer: {0}", _licenseInfo.customerName);
                    licenseVolumeSizeString = string.Format("VolumeSize: {0}", _licenseInfo.volumeSize);
                }
                */

            languageList = new List<LanguageListItem>();
            languageList.Add(new LanguageListItem(Properties.Resources.L031_English, ELanguageSet.English));
            languageList.Add(new LanguageListItem(Properties.Resources.L032_German, ELanguageSet.German));
            if(_appSettings.B001_Language == ELanguageSet.English.ToString())
                languageListSelectedIndex = 0;
            else
                languageListSelectedIndex = 1;

            licenseTypeList = new List<GeneralListItem>();
            licenseTypeList.Add(new GeneralListItem(Resources.L089_SingleUser, "SingleUser"));
            licenseTypeList.Add(new GeneralListItem(Resources.L090_ExpireDate, "ExpireDate"));
            licenseTypeList.Add(new GeneralListItem(Resources.L091_Volume, "Volume"));
            licenseTypeListSelectedIndex = 0;
            orderVolumeSizeVisibility = Visibility.Hidden;


        }

        public void onLicenssTypeChanged()
        {
            
        }

        public void onLanguageChanged(LanguageListItem item)
        {
            if(item.languageSet == ELanguageSet.English)
            {
                _appSettings.B001_Language = ELanguageSet.English.ToString();
                languageListSelectedIndex = 0;
            }
            if (item.languageSet == ELanguageSet.German)
            {
                _appSettings.B001_Language = ELanguageSet.German.ToString();
                languageListSelectedIndex = 1;
            }
        }

        public void onActionWriteOrderfile()
        {
            //check
            bool check_ok = true;
            
        }

        public void onActionImportOrderfile()
        {
            bool result = showFileSelectDialog(Resources.M016_ImportOrderResponse, "AdSnooperLicenseOrderResponse.json");
            if (result)
            {
                
            }
        }

        private bool showFileSelectDialog(string title, string filename)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = title;
            openFileDialog.Filter = "Order file (*.json)|*.json";
            if (pathOrderFile == null)
            {
                openFileDialog.FileName = filename;
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                openFileDialog.FileName = filename;
                openFileDialog.InitialDirectory = Path.GetDirectoryName(pathOrderFile);
            }
            if (openFileDialog.ShowDialog() == true)
            {
                pathOrderFile = openFileDialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool showFileSaveDialog(string title, string filename)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = title;
            saveFileDialog.Filter = "Order file (*.json)|*.json";
            if (pathOrderFile == null)
            {
                saveFileDialog.FileName = filename;
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                saveFileDialog.FileName = filename;
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(pathOrderFile);
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                pathOrderFile = saveFileDialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
