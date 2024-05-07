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


    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBCMController : ControllerBase
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
            [JsonPropertyName("DRG_CODE")]
            public string 藥品代碼 { get; set; }
            [JsonPropertyName("CHINESE_DESC")]
            public string 管制級別 { get; set; }

        }
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

            List<object[]> list_BBCM = sQLControl_UDSDBBCM.GetAllRows("medicine_page_cloud");
            List<object[]> list_BBCM_buf = new List<object[]>();
            List<object[]> list_BBCM_Add = new List<object[]>();
            List<object[]> list_BBCM_Replace = new List<object[]>();


            ServiceReference.ADCMedicineCabinetWCFServiceClient client = new ADCMedicineCabinetWCFServiceClient();
            string json_med = await client.DrugInfoAsync();
            string json_drugSpecInfo = await client.DrugSpecInfoAsync();

            List<DrugInfoClass> DrugInfoClasses = json_med.JsonDeserializet<List<DrugInfoClass>>();
            List<medClass> medClasses = new List<medClass>();

            for (int i = 0; i < DrugInfoClasses.Count; i++)
            {
                medClass medClass = new medClass();
                list_BBCM_buf = list_BBCM.GetRows((int)enum_雲端藥檔.藥品碼, DrugInfoClasses[i].藥品代碼.Trim());
                if (list_BBCM_buf.Count == 0)
                {
                    medClass.GUID = Guid.NewGuid().ToString();
                    string 管制級別 = "";
                    string 警訊藥品 = "";

                    if (DrugInfoClasses[i].藥品代碼 != null) medClass.藥品碼 = DrugInfoClasses[i].藥品代碼.Trim();
                    if (DrugInfoClasses[i].藥名 != null) medClass.藥品名稱 = DrugInfoClasses[i].藥名.Trim();
                    if (DrugInfoClasses[i].學名 != null) medClass.藥品學名 = DrugInfoClasses[i].學名.Trim();
                    if (DrugInfoClasses[i].ATC代碼 != null) medClass.料號 = DrugInfoClasses[i].ATC代碼.Trim();
                    if (DrugInfoClasses[i].單位 != null) medClass.包裝單位 = DrugInfoClasses[i].單位.Trim();

                    if (DrugInfoClasses[i].學名 != null)
                    {
                        if (medClass.藥品名稱.Contains("管1")) 管制級別 = "1";
                        else if (medClass.藥品名稱.Contains("管2")) 管制級別 = "2";
                        else if (medClass.藥品名稱.Contains("管3")) 管制級別 = "3";
                        else if (medClass.藥品名稱.Contains("管4")) 管制級別 = "4";
                        else 管制級別 = "N";
                        if (medClass.藥品名稱.Contains("●")) 警訊藥品 = true.ToString();
                        else 警訊藥品 = false.ToString();
                        medClass.管制級別 = 管制級別;
                        medClass.警訊藥品 = 警訊藥品;
                    }

                    string barcode = "";
                    if (DrugInfoClasses[i].藥局條碼 != null) barcode = DrugInfoClasses[i].藥局條碼.Trim();
                    medClass.Add_BarCode(barcode);
                    object[] value = medClass.ClassToSQL<medClass, enum_雲端藥檔>();
                    list_BBCM_Add.Add(value);
                }
                else
                {
                    object[] value = list_BBCM_buf[0];
                    bool flag_replace = false;
                    bool flag_barcode_replace = true;
                    string 藥品名稱 = value[(int)enum_雲端藥檔.藥品名稱].ObjectToString();
                    string 藥品學名 = value[(int)enum_雲端藥檔.藥品學名].ObjectToString();
                    string 料號 = value[(int)enum_雲端藥檔.料號].ObjectToString();
                    string 包裝單位 = value[(int)enum_雲端藥檔.包裝單位].ObjectToString();
                    string 管制級別 = "";
                    string 警訊藥品 = "";

                    if (藥品名稱 != null)
                    {
                        if (藥品名稱.Contains("管1"))
                        {
                            管制級別 = "1";
                        }
                        else if (藥品名稱.Contains("管2")) 管制級別 = "2";
                        else if (藥品名稱.Contains("管3")) 管制級別 = "3";
                        else if (藥品名稱.Contains("管4")) 管制級別 = "4";
                        else 管制級別 = "N";
                        if (藥品名稱.Contains("●")) 警訊藥品 = true.ToString();
                        else 警訊藥品 = false.ToString();

                    }


                    if (DrugInfoClasses[i].藥名 != null) if (藥品名稱 != DrugInfoClasses[i].藥名.Trim()) flag_replace = true;
                    if (DrugInfoClasses[i].學名 != null) if (藥品學名 != DrugInfoClasses[i].學名.Trim()) flag_replace = true;
                    if (DrugInfoClasses[i].ATC代碼 != null) if (料號 != DrugInfoClasses[i].ATC代碼.Trim()) flag_replace = true;
                    if (DrugInfoClasses[i].單位 != null) if (包裝單位 != DrugInfoClasses[i].單位.Trim()) flag_replace = true;
                    if (value[(int)enum_雲端藥檔.管制級別].ObjectToString() != 管制級別) flag_replace = true;
                    if (value[(int)enum_雲端藥檔.警訊藥品].ObjectToString() != 警訊藥品) flag_replace = true;
                    string barcode = "";
                    if (DrugInfoClasses[i].藥局條碼 != null) barcode = DrugInfoClasses[i].藥局條碼.Trim();

                    //List<string> list_barcode = value[(int)enum_雲端藥檔.藥品條碼2].ObjectToString().JsonDeserializet<List<string>>();
                    //if (list_barcode == null) list_barcode = new List<string>();
                    //list_barcode.Clear();
                    //for (int k = 0; k < list_barcode.Count; k++)
                    //{
                    //    if (list_barcode[k] == barcode)
                    //    {
                    //        flag_barcode_replace = false;
                    //        break;
                    //    }
                    //}
                    //if (barcode.StringIsEmpty()) flag_barcode_replace = false;
                    flag_barcode_replace = false;
                    if (flag_barcode_replace || flag_replace)
                    {
                        value[(int)enum_雲端藥檔.藥品碼] = DrugInfoClasses[i].藥品代碼.Trim();
                        if (DrugInfoClasses[i].藥名 != null) value[(int)enum_雲端藥檔.藥品名稱] = DrugInfoClasses[i].藥名.Trim();
                        if (DrugInfoClasses[i].學名 != null) value[(int)enum_雲端藥檔.藥品學名] = DrugInfoClasses[i].學名.Trim();
                        if (DrugInfoClasses[i].ATC代碼 != null) value[(int)enum_雲端藥檔.料號] = DrugInfoClasses[i].ATC代碼.Trim();
                        if (DrugInfoClasses[i].單位 != null) value[(int)enum_雲端藥檔.包裝單位] = DrugInfoClasses[i].單位.Trim();
                        value[(int)enum_雲端藥檔.管制級別] = 管制級別;
                        value[(int)enum_雲端藥檔.警訊藥品] = 警訊藥品;

                        //if (flag_barcode_replace)
                        //{
                        //    list_barcode.Add(barcode);
                        //}
                        value[(int)enum_雲端藥檔.藥品條碼2] = "";
                        list_BBCM_Replace.Add(value);
                    }

                }
            }

            if (list_BBCM_Add.Count > 0) sQLControl_UDSDBBCM.AddRows(null, list_BBCM_Add);
            //for (int i = 0; i < list_BBCM_Replace.Count; i++)
            //{
            //    string str = list_BBCM_Replace[i][(int)enum_雲端藥檔.中文名稱].ObjectToString();
            //    byte[] bytes = Encoding.Default.GetBytes(str);
            //    byte[] BIG5 = Encoding.Convert(Encoding.Default, Encoding.GetEncoding("BIG5"), bytes);//進行轉碼,參數1,來源編碼,參數二,目標編碼,參數三,欲編碼變
            //    string decodedString = Encoding.GetEncoding("BIG5").GetString(BIG5);
            //    list_BBCM_Replace[i][(int)enum_雲端藥檔.中文名稱] = decodedString;
            //}
            if (list_BBCM_Replace.Count > 0) sQLControl_UDSDBBCM.UpdateByDefulteExtra(null, list_BBCM_Replace);

            returnData.Code = 200;
            returnData.Result = $"取得藥檔完成! 共<{DrugInfoClasses.Count}>筆 ,新增<{list_BBCM_Add.Count}>筆,修改<{list_BBCM_Replace.Count}>筆";
            returnData.TimeTaken = myTimerBasic.ToString();
            return returnData.JsonSerializationt(true);
        }
    }
}
