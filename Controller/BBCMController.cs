using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBM.Data.DB2.Core;
using System.Data;
using System.Configuration;
using Basic;
using SQLUI;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using HIS_DB_Lib;
using System.Text.Json.Serialization;
using ServiceReference;
using MySql.Data.MySqlClient;
namespace DB2VM.Controller
{

    public class DrugInfoClass
    {
        [JsonPropertyName("DRG_CODE")]
        public string 藥品代碼 { get; set; }
        [JsonPropertyName("DRG_NAME")]
        public string 藥名 { get; set; }
        [JsonPropertyName("DRG_NOMENCLATURE")]
        public string 學名 { get; set; }
        [JsonPropertyName("DRG_SPEC")]
        public string 規格 { get; set; }
        [JsonPropertyName("DRG_UNIT")]
        public string 單位 { get; set; }
        [JsonPropertyName("DRG_BARCODE_STOCK")]
        public string 藥局條碼 { get; set; }
        [JsonPropertyName("DRG_SPEC_QTY")]
        //(整數七位小數三位)
        public double 醫囑轉換率 { get; set; }
        [JsonPropertyName("DRG_SPEC_UNIT")]
        public string 醫囑轉換單位 { get; set; }
        [JsonPropertyName("DRG_ATC_CODE")]
        public string ATC代碼 { get; set; }
        [JsonPropertyName("CHINESE_DESC")]
        public string CHINESE_DESC { get; set; }
    }
    public class DrugSpecInfoClass
    {
        [JsonPropertyName("DRUG_CODE")]
        public string 藥品代碼 { get; set; }
        [JsonPropertyName("CHINESE_DESC")]
        public string 管制級別 { get; set; }
        [JsonPropertyName("TYPE")]
        public string 類別 { get; set; }

    }
    public class DrugImgUrlClass
    {
        [JsonPropertyName("URL_MED_CODE")]
        public string 藥品代碼 { get; set; }
        [JsonPropertyName("URL_URL")]
        public string URL { get; set; }

    }

    
    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBCMController : ControllerBase
    {
    
        static public string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        [HttpGet]
        async public Task<string> Get(string? Code)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic(50000);
            returnData returnData = new returnData();
            List<ServerSettingClass> serverSettingClasses = ServerSettingClassMethod.WebApiGet($"{API_Server}");
            serverSettingClasses = serverSettingClasses.MyFind("Main", "網頁", "VM端");
            if (serverSettingClasses.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }

            ServerSettingClass serverSettingClass = serverSettingClasses[0];
            string Server = serverSettingClass.Server;
            string DB = serverSettingClass.DBName;
            string UserName = serverSettingClass.User;
            string Password = serverSettingClass.Password;
            uint Port = (uint)serverSettingClass.Port.StringToInt32();

            SQLControl sQLControl_UDSDBBCM = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);

            List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
            List<medClass> medClasses_cloud_buf = new List<medClass>();
            Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = medClasses_cloud.CoverToDictionaryByCode();

            List<object[]> list_medClasses_cloud_Add = new List<object[]>();
            List<object[]> list_medClasses_cloud_Replace = new List<object[]>();


            ServiceReference.ADCMedicineCabinetWCFServiceClient client = new ADCMedicineCabinetWCFServiceClient();
            string json_med = await client.DrugInfoAsync();
            string json_drugSpecInfo = await client.DrugSpecInfoAsync();
            string json_drugImgUrl = await client.DrugImgUrlAsync();
            List<DrugInfoClass> DrugInfoClasses = json_med.JsonDeserializet<List<DrugInfoClass>>();

            List<DrugSpecInfoClass> drugSpecInfoClasses = json_drugSpecInfo.JsonDeserializet<List<DrugSpecInfoClass>>();
            List<DrugSpecInfoClass> drugSpecInfoClasses_buf = new List<DrugSpecInfoClass>();
            System.Collections.Generic.Dictionary<string, List<DrugSpecInfoClass>> keyValuePairs_drugSpecInfo = drugSpecInfoClasses.CoverToDictionaryByCode();

            List<DrugImgUrlClass> drugImgUrlClasses = json_drugImgUrl.JsonDeserializet<List<DrugImgUrlClass>>();
            List<DrugImgUrlClass> drugImgUrlClasses_buf = new List<DrugImgUrlClass>();
            Dictionary<string, List<DrugImgUrlClass>> keyValuePairs_drugImgUrl = drugImgUrlClasses.CoverToDictionaryByCode();

            for (int i = 0; i < DrugInfoClasses.Count; i++)
            {
                string pic_url = "";
                medClass medClass = new medClass();
                medClasses_cloud_buf = keyValuePairs_medClasses_cloud.SortDictionaryByCode(DrugInfoClasses[i].藥品代碼.Trim());
                drugSpecInfoClasses_buf = keyValuePairs_drugSpecInfo.SortDictionaryByCode(DrugInfoClasses[i].藥品代碼);
                drugImgUrlClasses_buf = keyValuePairs_drugImgUrl.SortDictionaryByCode(DrugInfoClasses[i].藥品代碼.Trim());
                if (drugImgUrlClasses_buf.Count > 0)
                {
                    pic_url = $"https://www.kmuh.org.tw/med/medimage/{drugImgUrlClasses_buf[0].URL}";
                }


                string 類別 = "西藥";
                if (medClasses_cloud_buf.Count == 0)
                {
                    medClass.GUID = Guid.NewGuid().ToString();
                    string 管制級別 = "";
                    string 警訊藥品 = "";

                    if (DrugInfoClasses[i].藥品代碼 != null) medClass.藥品碼 = DrugInfoClasses[i].藥品代碼.Trim();
                    if (DrugInfoClasses[i].藥名 != null) medClass.藥品名稱 = DrugInfoClasses[i].藥名.Trim();
                    if (DrugInfoClasses[i].學名 != null) medClass.藥品學名 = DrugInfoClasses[i].學名.Trim();
                    if (DrugInfoClasses[i].ATC代碼 != null) medClass.料號 = DrugInfoClasses[i].ATC代碼.Trim();
                    if (DrugInfoClasses[i].單位 != null) medClass.包裝單位 = DrugInfoClasses[i].單位.Trim();
                    medClass.中西藥 = 類別;

                    if (DrugInfoClasses[i].藥品代碼 != null)
                    {
                        if (medClass.藥品名稱.Contains("管1") || medClass.藥品名稱.Contains("管一"))
                        {
                            管制級別 = "1";
                            if (drugSpecInfoClasses_buf.Count > 0)
                            {
                                
                            }
                        }
                        else if (medClass.藥品名稱.Contains("管2") || medClass.藥品名稱.Contains("管一")) 管制級別 = "2";
                        else if (medClass.藥品名稱.Contains("管3") || medClass.藥品名稱.Contains("管一")) 管制級別 = "3";
                        else if (medClass.藥品名稱.Contains("管4") || medClass.藥品名稱.Contains("管一")) 管制級別 = "4";
                        else 管制級別 = "N";
                        if (medClass.藥品名稱.Contains("●")) 警訊藥品 = true.ToString();
                        else 警訊藥品 = false.ToString();
                        medClass.管制級別 = 管制級別;
                        medClass.警訊藥品 = 警訊藥品;
                        medClass.圖片網址 = pic_url;


                    }

                    string barcode = "";
                    if (DrugInfoClasses[i].藥局條碼 != null) barcode = DrugInfoClasses[i].藥局條碼.Trim();
                    medClass.Add_BarCode(barcode);
                    object[] value = medClass.ClassToSQL<medClass, enum_雲端藥檔>();
                    list_medClasses_cloud_Add.Add(value);
                }
                else
                {
                    medClass = medClasses_cloud_buf[0];
                    bool flag_replace = false;
                    bool flag_barcode_replace = false;
                    string 管制級別 = "";
                    string 警訊藥品 = "";
                    if (medClass.藥品名稱 != null)
                    {
                        if (medClass.藥品名稱.Contains("管1") || medClass.藥品名稱.Contains("管一"))
                        {
                            管制級別 = "1";

                        }
                        else if (medClass.藥品名稱.Contains("管2") || medClass.藥品名稱.Contains("管二")) 管制級別 = "2";
                        else if (medClass.藥品名稱.Contains("管3") || medClass.藥品名稱.Contains("管三")) 管制級別 = "3";
                        else if (medClass.藥品名稱.Contains("管4") || medClass.藥品名稱.Contains("管四")) 管制級別 = "4";
                        else 管制級別 = "N";
                        if (medClass.藥品名稱.Contains("●")) 警訊藥品 = true.ToString();
                        else 警訊藥品 = false.ToString();

                    }
                    if(medClass.藥品碼.Contains("QZYV"))
                    {

                    }
                    if (drugSpecInfoClasses_buf.Count > 0)
                    {
                        if (drugSpecInfoClasses_buf[0].類別 == "LASA")
                        {
                            
                        }
                    }

                    if (DrugInfoClasses[i].藥名 != null) if (medClass.藥品名稱 != DrugInfoClasses[i].藥名.Trim()) flag_replace = true;
                    if (DrugInfoClasses[i].學名 != null) if (medClass.藥品學名 != DrugInfoClasses[i].學名.Trim()) flag_replace = true;
                    if (DrugInfoClasses[i].ATC代碼 != null) if (medClass.料號 != DrugInfoClasses[i].ATC代碼.Trim()) flag_replace = true;
                    if (DrugInfoClasses[i].單位 != null) if (medClass.包裝單位 != DrugInfoClasses[i].單位.Trim()) flag_replace = true;
                    if (medClass.中西藥 != 類別) flag_replace = true;
                    if (medClass.管制級別 != 管制級別) flag_replace = true;
                    if (medClass.警訊藥品 != 警訊藥品) flag_replace = true;
                    if (medClass.類別 != 類別) flag_replace = true;
                    if(medClass.圖片網址 != pic_url) flag_replace = true;
           
                    string barcode = "";
                    if (DrugInfoClasses[i].藥局條碼 != null) barcode = DrugInfoClasses[i].藥局條碼.Trim();
                    if (medClass.IsHaveBarCode(barcode) == false)
                    {
                        medClass.Add_BarCode(barcode);
                        flag_barcode_replace = true;
                    }
                     flag_barcode_replace = false;
                    if (flag_barcode_replace || flag_replace)
                    {
                        medClass.藥品碼 = DrugInfoClasses[i].藥品代碼.Trim();
                        if (DrugInfoClasses[i].藥名 != null) medClass.藥品名稱= DrugInfoClasses[i].藥名.Trim();
                        if (DrugInfoClasses[i].學名 != null) medClass .藥品學名= DrugInfoClasses[i].學名.Trim();
                        if (DrugInfoClasses[i].ATC代碼 != null) medClass.料號 = DrugInfoClasses[i].ATC代碼.Trim();
                        if (DrugInfoClasses[i].單位 != null) medClass.包裝單位 = DrugInfoClasses[i].單位.Trim();
                        medClass .管制級別= 管制級別;
                        medClass .警訊藥品= 警訊藥品;
                        medClass.類別 = 類別;
                        medClass.圖片網址 = pic_url;
                        medClass.中西藥 = 類別;

                        object[] value = medClass.ClassToSQL<medClass, enum_雲端藥檔>();
                        list_medClasses_cloud_Replace.Add(value);
                    }

                }
            }

            if (list_medClasses_cloud_Add.Count > 0) sQLControl_UDSDBBCM.AddRows(null, list_medClasses_cloud_Add);   
            if (list_medClasses_cloud_Replace.Count > 0) sQLControl_UDSDBBCM.UpdateByDefulteExtra(null, list_medClasses_cloud_Replace);

            returnData.Code = 200;
            returnData.Result = $"取得藥檔完成! 共<{DrugInfoClasses.Count}>筆 ,新增<{list_medClasses_cloud_Add.Count}>筆,修改<{list_medClasses_cloud_Replace.Count}>筆";
            returnData.TimeTaken = myTimerBasic.ToString();
            return returnData.JsonSerializationt(true);
        }


    }

    public static class DrugSpecInfoMethod
    {
        static public System.Collections.Generic.Dictionary<string, List<DrugSpecInfoClass>> CoverToDictionaryByCode(this List<DrugSpecInfoClass> drugSpecInfoClasses)
        {
            Dictionary<string, List<DrugSpecInfoClass>> dictionary = new Dictionary<string, List<DrugSpecInfoClass>>();

            foreach (var item in drugSpecInfoClasses)
            {
                string key = item.藥品代碼;

                // 如果字典中已經存在該索引鍵，則將值添加到對應的列表中
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(item);
                }
                // 否則創建一個新的列表並添加值
                else
                {
                    List<DrugSpecInfoClass> values = new List<DrugSpecInfoClass> { item };
                    dictionary[key] = values;
                }
            }

            return dictionary;
        }
        static public List<DrugSpecInfoClass> SortDictionaryByCode(this System.Collections.Generic.Dictionary<string, List<DrugSpecInfoClass>> dictionary, string code)
        {
            if (dictionary.ContainsKey(code))
            {
                return dictionary[code];
            }
            return new List<DrugSpecInfoClass>();
        }
       
    }
    public static class DrugImgUrlMethod
    {
        static public System.Collections.Generic.Dictionary<string, List<DrugImgUrlClass>> CoverToDictionaryByCode(this List<DrugImgUrlClass> drugImgUrlClasses)
        {
            Dictionary<string, List<DrugImgUrlClass>> dictionary = new Dictionary<string, List<DrugImgUrlClass>>();

            foreach (var item in drugImgUrlClasses)
            {
                string key = item.藥品代碼;
                if (key.StringIsEmpty()) continue;
                // 如果字典中已經存在該索引鍵，則將值添加到對應的列表中
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(item);
                }
                // 否則創建一個新的列表並添加值
                else
                {
                    List<DrugImgUrlClass> values = new List<DrugImgUrlClass> { item };
                    dictionary[key] = values;
                }
            }

            return dictionary;
        }
        static public List<DrugImgUrlClass> SortDictionaryByCode(this System.Collections.Generic.Dictionary<string, List<DrugImgUrlClass>> dictionary, string code)
        {
            if (dictionary.ContainsKey(code))
            {
                return dictionary[code];
            }
            return new List<DrugImgUrlClass>();
        }

    }
}
