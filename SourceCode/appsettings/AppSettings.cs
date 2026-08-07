using AdSnooperGui.common;
using AdSnooperGui.models;
using DomainAdSnooper.dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AdSnooperGui.appsettings
{
    public class AppSettings
    {

        public double A001_MainPosX { get; set; }
        public double A002_MainPosY { get; set; }
        public double A003_MainHeight { get; set; }
        public double A004_MainWidth { get; set; }
        public double A005_SearchPersonAreaWidth { get; set; }
        public double A006_PersonPropertiesAreaWidth { get; set; }
        public double A007_PersonSearchResultAreaHeight { get; set; }
        public double A008_SearchGroupAreaWidth { get; set; }
        public double A009_GroupSearchResultAreaWidth { get; set; }

        public string B001_Language { get; set; }
        public string B002_ExportSettingsGeneral { get; set; } // Einstellungen: Header, Delimiter, Outputformat, destination
        public string B003_ExportSettingsPersons { get; set; } // ob Felder angehakt sind (1) oder nicht (0)
        public string B004_ExportSettingsGroups { get; set; } // ob Felder angehakt sind (1) oder nicht (0)
        public string B005_ExportFilePersons { get; set; } //Pfad, wo Dateien exportiert werden
        public string B007_SearchPanelUserAttributes { get; set; } // Suchfelder für UserSearch aus Userattributes
        public string B008_SearchResultUserAttributes { get; set; }// Felder in Ergebnistabelle der UserSearch aus UserAttributes
        public string B009_AllUserAttributes { get; set; }//alle UserAttribute in Key-Value Properties-Tabelle
        public string B011_SearchPanelGroupAttributes { get; set; }//Suchfelder für GroupSearch aus GroupAttributes
        public string B012_SearchResultGroupAttributes { get; set; }//Felder in Ergebnistabelle der GroupSearch aus GroupAttributes
        public string B014_AllGroupAttributes { get; set; }//alle GroupAttributes

        
        public static readonly string APPLICATIONDATA_DIRNAME = "adsnooper";
        public static readonly string BATCH_DIRNAME = "batch";

        //------------------------
        private Dictionary<string, string> defaultValues = new Dictionary<string, string>();
        private string filePathUserSettings = null;
        private string filePathAppSettings = null;
        private AdSnooperGuiSettings aAdSnooperGuiSettings = null;
        private AdSnooperSettings aAdSnooperAppSettings = null;
        private int unsavedDataCounter = 0;
        private List<AdAttributeDto> _adAttributeListUser = null;
        private Dictionary<int,AdAttributeDto> _adAttributeDicUser = null;
        private List<AdAttributeDto> _adAttributeListGroup = null;
        private Dictionary<int,AdAttributeDto> _adAttributeDicGroup = null;
        private int[] searchPanelUserAttributes;
        private int[] searchResultUserAttributes;
        private int[] allUserAttributes;
        private int[] allGroupAttributes;
        private int[] memberOfGroupAttributes;
        private int[] searchPanelGroupAttributes;
        private int[] searchResultGroupAttributes;
        private int[] memberGroupAttributes;

        public AppSettings()
        {
            // Settings der App
            filePathAppSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adsnooper.xml");

            //User-Settings
            string appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATIONDATA_DIRNAME);
            if (!Directory.Exists(appdataPath))
            {
                Directory.CreateDirectory(appdataPath);
            }
            filePathUserSettings = Path.Combine(appdataPath, "config.xml");
        }

        /// <summary>
        /// Liefert die speziellen ExportParameter für Person.
        /// </summary>
        /// <returns></returns>
        public ExportJobParameterDto getExportParameterPerson()
        {
            ExportJobParameterDto result = new ExportJobParameterDto();
            result.dataType = ExportJobParameterDto.DATATYPE_PERSON;
            result.columnList = new string[adAttributeListUser.Count];
            result.columnSelectedList = new bool[adAttributeListUser.Count];
            int index = 0;
            string[] exportSettingsPerson = B003_ExportSettingsPersons.Split(',', StringSplitOptions.TrimEntries);
            foreach (AdAttributeDto attributeItem in getAllUserAttributesList())
            {
                result.columnList[index] = attributeItem.getLabel(getLanguageSetShort());
                if (index < exportSettingsPerson.Length && exportSettingsPerson[index].Equals("1"))
                    result.columnSelectedList[index] = true;
                else
                    result.columnSelectedList[index] = false;
                index++;
            }
            result.filePath = B005_ExportFilePersons;
            string[] exportSettingsGeneral = B002_ExportSettingsGeneral.Split(',', StringSplitOptions.TrimEntries);
            result.withHeader = exportSettingsGeneral[0].Equals("1")?true:false;
            result.delimiter = (EDelimiter)Enum.Parse(typeof(EDelimiter), exportSettingsGeneral[1]); 
            result.exportDestination = (EExportDestination)Enum.Parse(typeof(EExportDestination), exportSettingsGeneral[2]); 
            result.outputFormat = (EExportOutputFormat)Enum.Parse(typeof(EExportOutputFormat), exportSettingsGeneral[3]); 
            return result;
        }

        /// <summary>
        /// Liefert die speziellen ExportParameter für Group.
        /// </summary>
        /// <returns></returns>
        public ExportJobParameterDto getExportParameterGroup()
        {
            ExportJobParameterDto result = new ExportJobParameterDto();
            result.dataType = ExportJobParameterDto.DATATYPE_GROUP;
            result.columnList = new string[adAttributeListGroup.Count];
            result.columnSelectedList = new bool[adAttributeListGroup.Count];
            int index = 0;
            string[] exportSettingsGroup = B004_ExportSettingsGroups.Split(',', StringSplitOptions.TrimEntries);
            foreach (AdAttributeDto attributeItem in getAllGroupAttributesList())
            {
                result.columnList[index] = attributeItem.getLabel(getLanguageSetShort());
                if (index < exportSettingsGroup.Length && exportSettingsGroup[index].Equals("1"))
                    result.columnSelectedList[index] = true;
                else
                    result.columnSelectedList[index] = false;
                index++;
            }
            result.filePath = B005_ExportFilePersons;
            string[] exportSettingsGeneral = B002_ExportSettingsGeneral.Split(',', StringSplitOptions.TrimEntries);
            result.withHeader = exportSettingsGeneral[0].Equals("1") ? true : false;
            result.delimiter = (EDelimiter)Enum.Parse(typeof(EDelimiter), exportSettingsGeneral[1]);
            result.exportDestination = (EExportDestination)Enum.Parse(typeof(EExportDestination), exportSettingsGeneral[2]);
            result.outputFormat = (EExportOutputFormat)Enum.Parse(typeof(EExportOutputFormat), exportSettingsGeneral[3]);
            return result;
        }

        /// <summary>
        /// Liefert die allgemeinen ExportParameter.
        /// </summary>
        /// <returns></returns>
        public ExportJobParameterDto getExportParameter()
        {
            ExportJobParameterDto result = new ExportJobParameterDto();
            result.dataType = ExportJobParameterDto.DATATYPE_OTHER;
            result.filePath = B005_ExportFilePersons;
            string[] exportSettingsGeneral = B002_ExportSettingsGeneral.Split(',', StringSplitOptions.TrimEntries);
            result.withHeader = exportSettingsGeneral[0].Equals("1") ? true : false;
            result.delimiter = (EDelimiter)Enum.Parse(typeof(EDelimiter), exportSettingsGeneral[1]);
            result.exportDestination = (EExportDestination)Enum.Parse(typeof(EExportDestination), exportSettingsGeneral[2]);
            result.outputFormat = (EExportOutputFormat)Enum.Parse(typeof(EExportOutputFormat), exportSettingsGeneral[3]);
            return result;
        }

        /// <summary>
        /// Setzt die speziellen ExportParameter für Person.
        /// </summary>
        /// <param name="param"></param>
        public void setExportParameterPerson(ExportJobParameterDto param)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (bool item in param.columnSelectedList)
            {
                if (first) first = false;
                else sb.Append(',');
                sb.Append(item ? '1' : '0');
            }
            B003_ExportSettingsPersons = sb.ToString();
            sb.Clear();
            sb.Append(param.withHeader ? '1' : '0');
            sb.Append(',');
            sb.Append((int)param.delimiter);
            sb.Append(',');
            sb.Append((int)param.exportDestination);
            sb.Append(',');
            sb.Append((int)param.outputFormat);
            B002_ExportSettingsGeneral = sb.ToString();
            B005_ExportFilePersons = param.filePath;
        }

        /// <summary>
        /// Setzt die speziellen ExportParameter für Group.
        /// </summary>
        /// <param name="param"></param>
        public void setExportParameterGroup(ExportJobParameterDto param)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (bool item in param.columnSelectedList)
            {
                if (first) first = false;
                else sb.Append(',');
                sb.Append(item ? '1' : '0');
            }
            B004_ExportSettingsGroups = sb.ToString();
            sb.Clear();
            sb.Append(param.withHeader ? '1' : '0');
            sb.Append(',');
            sb.Append((int)param.delimiter);
            sb.Append(',');
            sb.Append((int)param.exportDestination);
            sb.Append(',');
            sb.Append((int)param.outputFormat);
            B002_ExportSettingsGeneral = sb.ToString();
            B005_ExportFilePersons = param.filePath;
        }

        /// <summary>
        /// Setzt die allgemeinen ExportParameter.
        /// </summary>
        /// <param name="param"></param>
        public void setExportParameter(ExportJobParameterDto param)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(param.withHeader ? '1' : '0');
            sb.Append(',');
            sb.Append((int)param.delimiter);
            sb.Append(',');
            sb.Append((int)param.exportDestination);
            sb.Append(',');
            sb.Append((int)param.outputFormat);
            B002_ExportSettingsGeneral = sb.ToString();
            B005_ExportFilePersons = param.filePath;
        }

        private AdSnooperGuiSettings buildDefault()
        {
            //AD-Attribute zuerst, weil sich diese dynamisch ändern könnten.
            //B009
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach(AdAttributeDto item in _adAttributeListUser)
            {
                if (first) first = false; else sb.Append(',');
                sb.Append(item.id);
            }
            defaultValues[nameof(B009_AllUserAttributes)] = sb.ToString();
            //B014
            first = true;
            sb.Clear();
            foreach (AdAttributeDto item in _adAttributeListGroup)
            {
                if (first) first = false; else sb.Append(',');
                sb.Append(item.id);
            }
            defaultValues[nameof(B014_AllGroupAttributes)] = sb.ToString();

            //Gui
            defaultValues.Add(nameof(A001_MainPosX), "400");
            defaultValues.Add(nameof(A002_MainPosY), "400");
            defaultValues.Add(nameof(A003_MainHeight), "700");
            defaultValues.Add(nameof(A004_MainWidth), "1100");
            defaultValues.Add(nameof(A005_SearchPersonAreaWidth), "250");
            defaultValues.Add(nameof(A006_PersonPropertiesAreaWidth), "380");
            defaultValues.Add(nameof(A007_PersonSearchResultAreaHeight), "390");
            defaultValues.Add(nameof(A008_SearchGroupAreaWidth), "320");
            defaultValues.Add(nameof(A009_GroupSearchResultAreaWidth), "440");
            defaultValues.Add(nameof(B001_Language), "English");
            
            //B002_ExportSettingsGeneral
            defaultValues.Add(nameof(B002_ExportSettingsGeneral), "1,1,1,1"); // Einstellungen: Header, Delimiter, Outputformat, destination

            //B003_ExportSettingsPersons
            sb.Clear();
            first = true;
            foreach (AdAttributeDto item in _adAttributeListUser)
            {
                if (first) first = false; else sb.Append(',');
                sb.Append('1');
            }
            defaultValues.Add(nameof(B003_ExportSettingsPersons), sb.ToString()); // ob Felder angehakt sind (1) oder nicht (0)

            //B004_ExportSettingsGroups
            sb.Clear();
            first = true;
            foreach (AdAttributeDto item in _adAttributeListGroup)
            {
                if (first) first = false; else sb.Append(',');
                sb.Append('1');
            }
            defaultValues.Add(nameof(B004_ExportSettingsGroups), sb.ToString()); // ob Felder angehakt sind (1) oder nicht (0)

            defaultValues.Add(nameof(B005_ExportFilePersons), Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            
            defaultValues.Add(nameof(B007_SearchPanelUserAttributes), "1,2,3,8");
            defaultValues.Add(nameof(B008_SearchResultUserAttributes), "1,4,2,3,8");
            defaultValues.Add(nameof(B011_SearchPanelGroupAttributes), "1");
            defaultValues.Add(nameof(B012_SearchResultGroupAttributes), "1,2");
            



            AdSnooperGuiSettings defaultSettings = new AdSnooperGuiSettings();
            defaultSettings.setting = new AdSnooperGuiSettingsSetting[defaultValues.Count];
            string currentValue;
            int idx = 0;
            foreach(string key in defaultValues.Keys)
            {
                AdSnooperGuiSettingsSetting settingEntry = new AdSnooperGuiSettingsSetting();
                defaultValues.TryGetValue(key, out currentValue);
                settingEntry.name = key;
                settingEntry.value = currentValue;
                defaultSettings.setting[idx++] = settingEntry;
            }
            return defaultSettings;
        }

        public List<AdAttributeDto> adAttributeListUser 
        {
            get { return _adAttributeListUser; }
        }

        public List<AdAttributeDto> adAttributeListGroup
        {
            get { return _adAttributeListGroup; }
        }

        /// <summary>
        /// Liefert Liste aller UserAttribute, die bei der Usersearch angeboten werden sollen.
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getSearchPanelUserAttributeList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in searchPanelUserAttributes) 
            { 
                result.Add(getAdAttributeOfUserById(id));
            }
            return result;
        }

        /// <summary>
        /// Liefert alle GroupAttribute, die bei der GroupSearch angeboten werden sollen
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getSearchPanelGroupAttributeList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in searchPanelGroupAttributes)
            {
                result.Add(getAdAttributeOfGroupById(id));
            }
            return result;
        }

        /// <summary>
        /// Liefert alle Attribute, die in der Suchergebnistabelle im User-Tab angezeigt werden sollen.
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getSearchResultUserAttributesList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in searchResultUserAttributes)
            {
                result.Add(getAdAttributeOfUserById(id));
            }
            return result;
        }

        /// <summary>
        /// Liegfert die Spaltenbreite der Tabelle PersonSearchResult.
        /// </summary>
        /// <returns></returns>
        public double[] getColumnSizesPersonSearchResult()
        {
            double[] result = new double[searchResultUserAttributes.Length + 1];//plus 1 für Zeilennr.
            for(int i=0;i<result.Length;i++)
                result[i] = 130.0;
            return result;
        }


        /// <summary>
        /// Liefert alle Attribute, die in der SuchergebnisMemberOfTabelle im User-Tab angezeigt werden sollen.
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getSearchResultUserMemberOfAttributesList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in memberOfGroupAttributes)
            {
                result.Add(getAdAttributeOfGroupById(id));
            }
            return result;
        }

        /// <summary>
        /// Liefert Liste aller UserAttribute
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getAllUserAttributesList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in allUserAttributes)
            {
                result.Add(getAdAttributeOfUserById(id));
            }
            return result;
        }

        /// <summary>
        /// Liefert Liste aller GroupAttribute
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getAllGroupAttributesList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in allGroupAttributes)
            {
                result.Add(getAdAttributeOfGroupById(id));
            }
            return result;
        }

        /// <summary>
        /// LIefert Liste der GroupAtttribute, die als Suchmaske angeboten werden sollen.
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getSearchPanelGroupAttributesList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in searchPanelGroupAttributes)
            {
                result.Add(getAdAttributeOfGroupById(id));
            }
            return result;
        }

        /// <summary>
        /// Liefert LIste der GroupAttribute, die in der Tabelle GroupSearchResult angezeigt werden soillen
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getSearchResultGroupAttributesList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in searchResultGroupAttributes)
            {
                result.Add(getAdAttributeOfGroupById(id));
            }
            return result;
        }

        /// <summary>
        /// Liefert Liste der UserAttribute, die bei GroupMember angezeigt werden sollen
        /// </summary>
        /// <returns></returns>
        public List<AdAttributeDto> getMemberGroupAttributesList()
        {
            List<AdAttributeDto> result = new List<AdAttributeDto>();
            foreach (int id in memberGroupAttributes)
            {
                result.Add(getAdAttributeOfUserById(id));
            }
            return result;
        }

        //------------------------------


        private AdAttributeDto getAdAttributeOfUserById(int id)
        {
            _adAttributeDicUser.TryGetValue(id, out AdAttributeDto adAttribute);
            return adAttribute;
        }

        private AdAttributeDto getAdAttributeOfGroupById(int id)
        {
            _adAttributeDicGroup.TryGetValue(id, out AdAttributeDto adAttribute);
            return adAttribute;
        }

        public string getLanguageSetShort()
        {
            if (B001_Language.Equals(ELanguageSet.German.ToString()))
                return ELanguageSetShort.DE.ToString();
            else return ELanguageSetShort.EN.ToString();
        }


        public void readSettings()
        {
            readSettings(filePathUserSettings);
        }

        public void readSettings(string pathUserSettingsFile)
        {
            //AppSettings
            try
            {
                if (File.Exists(filePathAppSettings))
                {
                    aAdSnooperAppSettings = readAppSettings();

                    _adAttributeListUser = new List<AdAttributeDto>();
                    _adAttributeDicUser = new Dictionary<int, AdAttributeDto>();
                    foreach (AdSnooperSettingsAdsettingsAttributesAttribute attributeItem in aAdSnooperAppSettings.adsettings.attributes.user)
                    {
                        AdAttributeDto attributeDto = new AdAttributeDto(attributeItem.id, attributeItem.name);
                        attributeDto.formaterName = attributeItem.formatername;
                        foreach (AdSnooperSettingsAdsettingsAttributesAttributeLabel labelItem in attributeItem.label)
                        {
                            attributeDto.addLabel(labelItem.lang, labelItem.value);
                        }
                        _adAttributeListUser.Add(attributeDto);
                        _adAttributeDicUser.Add(attributeDto.id, attributeDto);
                    }
                    _adAttributeListGroup = new List<AdAttributeDto>();
                    _adAttributeDicGroup = new Dictionary<int, AdAttributeDto>();
                    foreach (AdSnooperSettingsAdsettingsAttributesAttribute1 attributeItem in aAdSnooperAppSettings.adsettings.attributes.group)
                    {
                        AdAttributeDto attributeDto = new AdAttributeDto(attributeItem.id, attributeItem.name);
                        attributeDto.formaterName = attributeItem.formatername;
                        foreach (AdSnooperSettingsAdsettingsAttributesAttributeLabel1 labelItem in attributeItem.label)
                        {
                            attributeDto.addLabel(labelItem.lang, labelItem.value);
                        }
                        _adAttributeListGroup.Add(attributeDto);
                        _adAttributeDicGroup.Add(attributeDto.id, attributeDto);
                    }


                }
            }
            catch (Exception)
            {
                throw new Exception("Error while reading application settings in path " + filePathAppSettings);
            }


            //user settings
            try
            {
                aAdSnooperGuiSettings= buildDefault();
                if (File.Exists(pathUserSettingsFile))
                {
                    aAdSnooperGuiSettings = readXmlFileUserSettings(pathUserSettingsFile);
                    var dummy = buildDefault();
                }
                else
                    writeXmlFileUserSettings(aAdSnooperGuiSettings, pathUserSettingsFile);
                
            }
            catch (Exception)
            {
                //aAdSnooperGuiSettings = buildDefault();
                //writeXmlFileUserSettings(aAdSnooperGuiSettings, pathUserSettingsFile);
            }

            A001_MainPosX = double.Parse(getSettings(nameof(A001_MainPosX)).value);
            A002_MainPosY = double.Parse(getSettings(nameof(A002_MainPosY)).value);
            A003_MainHeight = double.Parse(getSettings(nameof(A003_MainHeight)).value);
            A004_MainWidth = double.Parse(getSettings(nameof(A004_MainWidth)).value);
            A005_SearchPersonAreaWidth = double.Parse(getSettings(nameof(A005_SearchPersonAreaWidth)).value);
            A006_PersonPropertiesAreaWidth = double.Parse(getSettings(nameof(A006_PersonPropertiesAreaWidth)).value);
            A007_PersonSearchResultAreaHeight = double.Parse(getSettings(nameof(A007_PersonSearchResultAreaHeight)).value);
            A008_SearchGroupAreaWidth = double.Parse(getSettings(nameof(A008_SearchGroupAreaWidth)).value);
            A009_GroupSearchResultAreaWidth = double.Parse(getSettings(nameof(A009_GroupSearchResultAreaWidth)).value);

            B001_Language = getSettings(nameof(B001_Language)).value;
            B002_ExportSettingsGeneral = getSettings(nameof(B002_ExportSettingsGeneral)).value;
            B003_ExportSettingsPersons = getSettings(nameof(B003_ExportSettingsPersons)).value;
            B004_ExportSettingsGroups = getSettings(nameof(B004_ExportSettingsGroups)).value;
            B005_ExportFilePersons = getSettings(nameof(B005_ExportFilePersons)).value;
            B007_SearchPanelUserAttributes = getSettings(nameof(B007_SearchPanelUserAttributes)).value;
            B008_SearchResultUserAttributes = getSettings(nameof(B008_SearchResultUserAttributes)).value;
            B009_AllUserAttributes = getSettings(nameof(B009_AllUserAttributes)).value;
            B011_SearchPanelGroupAttributes = getSettings(nameof(B011_SearchPanelGroupAttributes)).value;
            B012_SearchResultGroupAttributes = getSettings(nameof(B012_SearchResultGroupAttributes)).value;
            B014_AllGroupAttributes= getSettings(nameof(B014_AllGroupAttributes)).value;

            //einige Werte müssen geprüft und angepasst werden.
            if(B003_ExportSettingsPersons.Split(',').Length != _adAttributeListUser.Count())
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (AdAttributeDto item in _adAttributeListUser)
                {
                    if (first) first = false; else sb.Append(',');
                    sb.Append('1');
                }
                B003_ExportSettingsPersons = sb.ToString();
            }
            if (B004_ExportSettingsGroups.Split(',').Length != _adAttributeListGroup.Count())
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (AdAttributeDto item in _adAttributeListGroup)
                {
                    if (first) first = false; else sb.Append(',');
                    sb.Append('1');
                }
                B004_ExportSettingsGroups = sb.ToString();
            }
            if (B009_AllUserAttributes.Split(',').Length != _adAttributeListUser.Count())
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (AdAttributeDto item in _adAttributeListUser)
                {
                    if (first) first = false; else sb.Append(',');
                    sb.Append(item.id);
                }
                B009_AllUserAttributes = sb.ToString();
            }
            if (B014_AllGroupAttributes.Split(',').Length != _adAttributeListGroup.Count())
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (AdAttributeDto item in _adAttributeListGroup)
                {
                    if (first) first = false; else sb.Append(',');
                    sb.Append(item.id);
                }
                B014_AllGroupAttributes = sb.ToString();
            }

            searchPanelUserAttributes = convertCsvToIntArray(B007_SearchPanelUserAttributes);
            searchResultUserAttributes = convertCsvToIntArray(B008_SearchResultUserAttributes);
            allUserAttributes = convertCsvToIntArray(B009_AllUserAttributes);
            allGroupAttributes = convertCsvToIntArray(B014_AllGroupAttributes);
            searchPanelGroupAttributes = convertCsvToIntArray(B011_SearchPanelGroupAttributes);
            searchResultGroupAttributes = convertCsvToIntArray(B012_SearchResultGroupAttributes);
            
            
            StringBuilder sb2 = new StringBuilder();
            int value = 0;
            bool first1 = true;
            bool findError = false;
            foreach (string item in B007_SearchPanelUserAttributes.Split(','))
            {
                if(Int32.TryParse(item, out value))
                {
                    if (allUserAttributes.Contains(value))
                    {
                        if (first1) first1 = false; else sb2.Append(',');
                        sb2.Append(value);
                    }else
                        findError = true;
                }
            }
            if (findError)
                B007_SearchPanelUserAttributes = sb2.ToString();

            sb2.Clear();
            first1 = true;
            findError = false;
            foreach (string item in B008_SearchResultUserAttributes.Split(','))
            {
                if (Int32.TryParse(item, out value))
                {
                    if (allUserAttributes.Contains(value))
                    {
                        if (first1) first1 = false; else sb2.Append(',');
                        sb2.Append(value);
                    }
                    else
                        findError = true;
                }
            }
            if (findError)
                B008_SearchResultUserAttributes = sb2.ToString();

            sb2.Clear();
            first1 = true;
            findError = false;
            foreach (string item in B011_SearchPanelGroupAttributes.Split(','))
            {
                if (Int32.TryParse(item, out value))
                {
                    if (allGroupAttributes.Contains(value))
                    {
                        if (first1) first1 = false; else sb2.Append(',');
                        sb2.Append(value);
                    }
                    else
                        findError = true;
                }
            }
            if (findError)
                B011_SearchPanelGroupAttributes = sb2.ToString();

            sb2.Clear();
            first1 = true;
            findError = false;
            foreach (string item in B012_SearchResultGroupAttributes.Split(','))
            {
                if (Int32.TryParse(item, out value))
                {
                    if (allGroupAttributes.Contains(value))
                    {
                        if (first1) first1 = false; else sb2.Append(',');
                        sb2.Append(value);
                    }
                    else
                        findError = true;
                }
            }
            if (findError)
                B012_SearchResultGroupAttributes = sb2.ToString();

            searchPanelUserAttributes = convertCsvToIntArray(B007_SearchPanelUserAttributes);
            searchResultUserAttributes = convertCsvToIntArray(B008_SearchResultUserAttributes);
            searchPanelGroupAttributes = convertCsvToIntArray(B011_SearchPanelGroupAttributes);
            searchResultGroupAttributes = convertCsvToIntArray(B012_SearchResultGroupAttributes);
        }
              

        private int[] convertCsvToIntArray(string csv)
        {
            string[] data = csv.Split(',');
            int[] result = new int[data.Length];
            int i = 0;
            foreach (string item in data)
            {
                result[i++]= Convert.ToInt32(item);
            }
            return result;
        }

        public void writeSettings()
        {
            writeSettings(filePathUserSettings);
        }
        public void writeSettings(string path)
        {
            try
            {
                PropertyInfo[] myProperties = GetType().GetProperties();//liste meiner member
                foreach (string key in defaultValues.Keys)
                {
                    object? memberValue=null;
                    Type memberType=null;
                    foreach(PropertyInfo memberInfo in myProperties)
                    {
                        if (memberInfo.Name.Equals(key))
                        {
                            memberValue = memberInfo.GetValue(this);
                            memberType = memberInfo.PropertyType;
                            break;
                        }
                    }
                    if(memberType == typeof(Double))
                    {
                        getSettings(key).value = ((Double)memberValue).ToString();
                    }
                    else if (memberType == typeof(String))
                    {
                        getSettings(key).value = (string)memberValue;
                    }
                    else if (memberType == typeof(Boolean))
                    {
                        getSettings(key).value = ((Boolean)memberValue).ToString();
                    }
                }
                writeXmlFileUserSettings(aAdSnooperGuiSettings, path);
            }
            catch (Exception)
            {
            }
        }

        private AdSnooperGuiSettingsSetting getSettings(string paramName)
        {
            AdSnooperGuiSettingsSetting foundItem = null;
            foreach (AdSnooperGuiSettingsSetting item in aAdSnooperGuiSettings.setting)
            {
                if (item != null && item.name.Equals(paramName))
                {
                    foundItem = item;
                    break;
                }
            }
            if (foundItem == null)//add
            {
                defaultValues.TryGetValue(paramName, out string defaultValue);
                foundItem = new AdSnooperGuiSettingsSetting { name = paramName, value = defaultValue };
                addParam(foundItem);
            }
            return foundItem;
        }


        private void addParam(AdSnooperGuiSettingsSetting newParam)
        {
            AdSnooperGuiSettingsSetting[] currentData = aAdSnooperGuiSettings.setting;
            if(currentData.Length< defaultValues.Count)
            {
                //create new size and copy
                AdSnooperGuiSettingsSetting[] newData = new AdSnooperGuiSettingsSetting[defaultValues.Count];
                for(int x=0; x < aAdSnooperGuiSettings.setting.Length; x++)
                {
                    newData[x] = aAdSnooperGuiSettings.setting[x];
                }
                aAdSnooperGuiSettings.setting = newData;
            }
            for(int i=0;i< aAdSnooperGuiSettings.setting.Length; i++)
            {
                if (aAdSnooperGuiSettings.setting[i] == null)
                {
                    //add new
                    aAdSnooperGuiSettings.setting[i] = newParam;
                    break;
                }
            }
            
        }

        private AdSnooperSettings readAppSettings()
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(AdSnooperSettings));
                using (StreamReader streamReader = new StreamReader(filePathAppSettings))
                {
                    return (AdSnooperSettings)xmlserializer.Deserialize(streamReader);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error parsing xml file: {0}, {1}", filePathUserSettings, e.Message));
            }
        }

        private AdSnooperGuiSettings readXmlFileUserSettings(string path)
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(AdSnooperGuiSettings));
                using (StreamReader streamReader = new StreamReader(path))
                {
                    return (AdSnooperGuiSettings)xmlserializer.Deserialize(streamReader);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error parsing xml file: {0}, {1}", path, e.Message));
            }
        }

        private void writeXmlFileUserSettings(AdSnooperGuiSettings settings, string path)
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(AdSnooperGuiSettings));
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    xmlserializer.Serialize(streamWriter, settings);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error parsing xml file: {0}, {1}", filePathUserSettings, e.Message));
            }
        }

        /// <summary>
        /// Zählt den unsavedDataCounter hoch
        /// </summary>
        public void unsafedDataInc()
        {
            unsavedDataCounter++;
        }

        /// <summary>
        /// Zählt den unsavedDataCounter runter
        /// </summary>
        public void unsafedDataDec()
        {
            unsavedDataCounter--;
            if (unsavedDataCounter < 0) 
                unsavedDataCounter = 0;
        }
        public bool isUnsafedData()
        {
            return unsavedDataCounter != 0;
        }
    }






}
