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
using HIS_DB_Lib;
using ServiceReference;
using System.Text.Json.Serialization;

namespace DB2VM
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBARController : ControllerBase
    {


        public class Order_DBVM_Class
        {     
            [JsonPropertyName("MNO_FUNCTION_CODE")]
            //(AF:新增/UM:修改/DF:刪除-要重印首頁)
            public string 修改狀態 { get; set; }
            [JsonPropertyName("MNO_DATE")]
            public double 調劑日期 { get; set; }
            [JsonPropertyName("MNO_NO_KIND")]
            //(M:混有針劑/I:純針劑/P:磨粉/C:化療)
            public string 領藥號類別 { get; set; }
            [JsonPropertyName("MNO_NO")]
            public double 領藥序號 { get; set; }
            [JsonPropertyName("MNO_CHART_NO")]
            public string 病歷號碼 { get; set; }
            [JsonPropertyName("PATIENT_NAME")]
            public string 病人姓名 { get; set; }
            [JsonPropertyName("BIRTH_DATE")]
            public string 生日 { get; set; }
            [JsonPropertyName("SEX")]
            public string 性別 { get; set; }
            [JsonPropertyName("MNO_DEPT")]
            public string 科別 { get; set; }
            [JsonPropertyName("MNO_DOCTOR_CODE")]
            public string 看診醫師 { get; set; }
            [JsonPropertyName("MDA_MED_CODE")]
            public string 藥品代碼 { get; set; }
            [JsonPropertyName("DRG_PURE_NAME")]
            public string 英文Pure商品名 { get; set; }
            [JsonPropertyName("DRG_SPEC")]
            public string 規格 { get; set; }
            [JsonPropertyName("DRG_UNIT")]
            public string 單位 { get; set; }
            [JsonPropertyName("DRG_BARCODE")]
            public string 藥局條碼 { get; set; }
            [JsonPropertyName("DRG_BARCODE_STOCK")]
            public string 藥庫條碼 { get; set; }
            [JsonPropertyName("MDA_DAY_INDICATE")]
            public string 頻次 { get; set; }
            [JsonPropertyName("MDA_TAKE_INDICATE")]
            public string 服藥指示 { get; set; }
            [JsonPropertyName("MDA_ONE_QTY")]
            public double 次量 { get; set; }
            [JsonPropertyName("MDA_PRESCRIPTION_DAYS")]
            public double 給藥天數 { get; set; }
            [JsonPropertyName("MDA_TOT_QTY")]
            public double 總量 { get; set; }
            [JsonPropertyName("MDA_UPDATE_FLAG")]
            public string 處方是否相同 { get; set; }
            [JsonPropertyName("MDA_INDEX")]
            public double 藥袋號 { get; set; }
            [JsonPropertyName("MDA_INDEX_SEQ")]
            public double 開藥順序 { get; set; }
            [JsonPropertyName("LAST_MED_NO")]
            public double 最後領藥序號 { get; set; }
            [JsonPropertyName("PAK_SPEC_QTY")]
            public string 片數換算 { get; set; }
            [JsonPropertyName("MNO_NOON_NO")]
            public string 午別 { get; set; }
        }

        public class Order_UD_Class
        {
            [JsonPropertyName("CHART_NO")]         
            public string 病歷號 { get; set; }
            [JsonPropertyName("NET_WARD")]
            public string 病房號 { get; set; }
            [JsonPropertyName("NET_BED")]
            public string 床號 { get; set; }
            [JsonPropertyName("NET_NAME")]
            public string 病人姓名 { get; set; }
            [JsonPropertyName("NET_FREQCODE")]
            public string 頻次 { get; set; }
            [JsonPropertyName("NET_PATHWAYCODE")]
            public string 途徑 { get; set; }
            [JsonPropertyName("NET_CHARGE_QTY")]
            public double 總量 { get; set; }
            [JsonPropertyName("DRG_UNIT")]
            public string 單位 { get; set; }
            [JsonPropertyName("NET_PRNJOBID")]
            public string 領藥號 { get; set; }
            [JsonPropertyName("DRG_NAME")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("NET_FEECODE")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("NET_BARCODE")]
            public string 條碼 { get; set; }
            [JsonPropertyName("NET_LOG_TIME")]
            public string 開方時間 { get; set; }
            [JsonPropertyName("NET_DATE")]
            public string 開方日期 { get; set; }

        }


        public class POST_Order_API
        {
            public string terminal { get; set; }
            public string barcode { get; set; }
            public string pharmacist { get; set; }
            public string type { get; set; }
        }
        public class Return_Order_API
        {
            public Return_Order_API(string _message ,bool isSuccess)
            {
                this.isSuccess = isSuccess ? "Y" : "N";
                this.message = _message;
            }
            public string isSuccess { get; set; }
            public string message { get; set; }
        }
        static string MySQL_server = $"{ConfigurationManager.AppSettings["MySQL_server"]}";
        static string MySQL_database = $"{ConfigurationManager.AppSettings["MySQL_database"]}";
        static string MySQL_userid = $"{ConfigurationManager.AppSettings["MySQL_user"]}";
        static string MySQL_password = $"{ConfigurationManager.AppSettings["MySQL_password"]}";
        static string MySQL_port = $"{ConfigurationManager.AppSettings["MySQL_port"]}";
        static string API_SERVER = $"{ConfigurationManager.AppSettings["API_SERVER"]}";
        private SQLControl sQLControl_醫囑資料 = new SQLControl(MySQL_server, MySQL_database, "order_list", MySQL_userid, MySQL_password, (uint)MySQL_port.StringToInt32(), MySql.Data.MySqlClient.MySqlSslMode.None);

        [HttpGet]
        async public Task<string> Get(string? BarCode)
        {
            returnData returnData = new returnData();
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic(50000);
                List<OrderClass> orderClasses = new List<OrderClass>();
                List<Order_DBVM_Class> order_DBVM_Classes = new List<Order_DBVM_Class>();
                List<object[]> list_value_Add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                string json_order = "";

                if (BarCode.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "barcode 空白";
                    return returnData.JsonSerializationt(true);
                }
                if(!((BarCode.Length == 14 || BarCode.Length == 15) || BarCode.Length == 19 || BarCode.Length == 12))
                {
                    returnData.Code = -200;
                    returnData.Result = $"barcode 長度異常! [{BarCode}]";
                    return returnData.JsonSerializationt(true);
                }
                ServiceReference.ADCMedicineCabinetWCFServiceClient client = new ADCMedicineCabinetWCFServiceClient();
                if(BarCode.Length == 14 || BarCode.Length == 15)
                {
                    json_order = await client.QueryRtxmda_DataAsync(BarCode);
                    order_DBVM_Classes = json_order.JsonDeserializet<List<Order_DBVM_Class>>();
                }
                if (BarCode.Length == 19)
                {
                    json_order = await client.QueryRtxmda_Bags_ContentAsync(BarCode);
                    order_DBVM_Classes = json_order.JsonDeserializet<List<Order_DBVM_Class>>();
                }
                if (BarCode.Length == 12)
                {
                    json_order = await client.MEDCABINET_DataAsync(BarCode);
                    List<Order_UD_Class> order_DBVM_UD_Classes = json_order.JsonDeserializet<List<Order_UD_Class>>();
                    for (int i = 0; i < order_DBVM_UD_Classes.Count; i++)
                    {
                        OrderClass orderClass = new OrderClass();
                        orderClass.藥品碼 = order_DBVM_UD_Classes[i].藥品碼.Trim();
                        orderClass.病歷號 = order_DBVM_UD_Classes[i].病歷號.Trim();
                        orderClass.藥品名稱 = order_DBVM_UD_Classes[i].藥品名稱.Trim();
                        if (order_DBVM_UD_Classes[i].病房號 == null) order_DBVM_UD_Classes[i].病房號 = "";
                        if (order_DBVM_UD_Classes[i].床號 == null) order_DBVM_UD_Classes[i].床號 = "";
                        orderClass.PRI_KEY = $"{BarCode}{orderClass.藥品碼}";
                        orderClass.病房 = order_DBVM_UD_Classes[i].病房號.Trim();
                        orderClass.床號 = order_DBVM_UD_Classes[i].床號.Trim();
                        orderClass.病人姓名 = order_DBVM_UD_Classes[i].病人姓名.Trim();
                        orderClass.頻次 = order_DBVM_UD_Classes[i].頻次.Trim();
                        orderClass.途徑 = order_DBVM_UD_Classes[i].途徑.Trim();
                        orderClass.交易量 = Math.Ceiling(order_DBVM_UD_Classes[i].總量 * -1).ToString();
                        orderClass.領藥號 = order_DBVM_UD_Classes[i].領藥號.Trim();
                        string 開方日期 = order_DBVM_UD_Classes[i].開方日期;
                        string hour = order_DBVM_UD_Classes[i].開方時間.Substring(0, 2);
                        string min = order_DBVM_UD_Classes[i].開方時間.Substring(2, 2);
                        string sec = order_DBVM_UD_Classes[i].開方時間.Substring(4, 2);
                        string 開方時間 = $"{hour}:{min}:{sec}";
                        orderClass.開方日期 = $"{開方日期} {開方時間}";
                        orderClasses.Add(orderClass);
                    }
                }
                for (int i = 0; i < order_DBVM_Classes.Count; i++)
                {
                    OrderClass orderClass = new OrderClass();
                  
                    orderClass.藥品碼 = order_DBVM_Classes[i].藥品代碼.Trim();
                    orderClass.PRI_KEY = $"{BarCode}{orderClass.藥品碼}";
                    orderClass.藥品名稱 = order_DBVM_Classes[i].英文Pure商品名.Trim();
                    orderClass.病歷號 = order_DBVM_Classes[i].病歷號碼.Trim();
                    orderClass.藥袋條碼 = BarCode;
                    orderClass.病人姓名 = order_DBVM_Classes[i].病人姓名.Trim();
                    orderClass.交易量 = Math.Ceiling(order_DBVM_Classes[i].總量 * -1).ToString();
                    string temp = order_DBVM_Classes[i].調劑日期.ToString();
                    if(temp.Length != 7)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"調劑日期資料異常![{temp}]";
                        return returnData.JsonSerializationt(true);
                    }
                    string year = (temp.Substring(0, 3).StringToInt32() + 1911).ToString();
                    string month = (temp.Substring(3, 2)).ToString();
                    string day = (temp.Substring(5, 2)).ToString();

                    orderClass.開方日期 = $"{year}/{month}/{day}";
                    orderClass.頻次 = order_DBVM_Classes[i].頻次.Trim();
                    orderClass.單次劑量 = order_DBVM_Classes[i].次量.ToString();
                    orderClass.劑量單位 = order_DBVM_Classes[i].單位.Trim();
                    orderClass.領藥號 = order_DBVM_Classes[i].領藥序號.ToString();
                    orderClasses.Add(orderClass);

                }
                string 病歷號 = orderClasses[0].病歷號;
                List<object[]> list_value = this.sQLControl_醫囑資料.GetRowsByDefult(null, enum_醫囑資料.病歷號.GetEnumName(), 病歷號);
                List<object[]> list_value_buf = new List<object[]>();
                for (int i = 0; i < orderClasses.Count; i++)
                {
                    list_value_buf = list_value.GetRows((int)enum_醫囑資料.PRI_KEY, orderClasses[i].PRI_KEY);
                    list_value_buf = list_value_buf.GetRows((int)enum_醫囑資料.藥品碼, orderClasses[i].藥品碼);
                    list_value_buf = list_value_buf.GetRows((int)enum_醫囑資料.頻次, orderClasses[i].頻次);
                    list_value_buf = list_value_buf.GetRows((int)enum_醫囑資料.交易量, orderClasses[i].交易量);
                    if (list_value_buf.Count == 0)
                    {
                        object[] value = new object[new enum_醫囑資料().GetLength()];
                        value[(int)enum_醫囑資料.GUID] = Guid.NewGuid().ToString();
                        orderClasses[i].GUID = value[(int)enum_醫囑資料.GUID].ObjectToString();
                        value[(int)enum_醫囑資料.PRI_KEY] = orderClasses[i].PRI_KEY;
                        value[(int)enum_醫囑資料.藥局代碼] = orderClasses[i].藥局代碼;
                        value[(int)enum_醫囑資料.藥品碼] = orderClasses[i].藥品碼;
                        value[(int)enum_醫囑資料.藥品名稱] = orderClasses[i].藥品名稱;
                        value[(int)enum_醫囑資料.病歷號] = orderClasses[i].病歷號;
                        value[(int)enum_醫囑資料.藥袋條碼] = orderClasses[i].藥袋條碼;
                        value[(int)enum_醫囑資料.病人姓名] = orderClasses[i].病人姓名;
                        value[(int)enum_醫囑資料.交易量] = orderClasses[i].交易量;
                        value[(int)enum_醫囑資料.頻次] = orderClasses[i].頻次;
                        value[(int)enum_醫囑資料.單次劑量] = orderClasses[i].單次劑量;
                        value[(int)enum_醫囑資料.劑量單位] = orderClasses[i].劑量單位;
                        value[(int)enum_醫囑資料.領藥號] = orderClasses[i].領藥號;

                        value[(int)enum_醫囑資料.開方日期] = orderClasses[i].開方日期;
                        value[(int)enum_醫囑資料.產出時間] = DateTime.Now.ToDateTimeString_6();
                        value[(int)enum_醫囑資料.結方日期] = DateTime.MinValue.ToDateTimeString_6();
                        value[(int)enum_醫囑資料.過帳時間] = DateTime.MinValue.ToDateTimeString_6();
                        value[(int)enum_醫囑資料.展藥時間] = DateTime.MinValue.ToDateTimeString_6();
                        value[(int)enum_醫囑資料.狀態] = "未過帳";
                        list_value_Add.Add(value);
                    }
                    else
                    {
                        object[] value_src = orderClasses[i].ClassToSQL<OrderClass, enum_醫囑資料>();
                        bool flag_replace = false;
                        if (value_src[(int)enum_醫囑資料.開方日期].StringToDateTime().StringToDateTime() != list_value_buf[0][(int)enum_醫囑資料.開方日期].StringToDateTime().StringToDateTime()) flag_replace = true;
                        if (flag_replace)
                        {
                            object[] value = list_value_buf[0];
                            orderClasses[i].GUID = value[(int)enum_醫囑資料.GUID].ObjectToString();
                            value[(int)enum_醫囑資料.PRI_KEY] = orderClasses[i].PRI_KEY;
                            value[(int)enum_醫囑資料.藥局代碼] = orderClasses[i].藥局代碼;
                            value[(int)enum_醫囑資料.藥品碼] = orderClasses[i].藥品碼;
                            value[(int)enum_醫囑資料.藥品名稱] = orderClasses[i].藥品名稱;
                            value[(int)enum_醫囑資料.病歷號] = orderClasses[i].病歷號;
                            value[(int)enum_醫囑資料.藥袋條碼] = orderClasses[i].藥袋條碼;
                            value[(int)enum_醫囑資料.病人姓名] = orderClasses[i].病人姓名;
                            value[(int)enum_醫囑資料.交易量] = orderClasses[i].交易量;
                            value[(int)enum_醫囑資料.頻次] = orderClasses[i].頻次;
                            value[(int)enum_醫囑資料.單次劑量] = orderClasses[i].單次劑量;
                            value[(int)enum_醫囑資料.劑量單位] = orderClasses[i].劑量單位;
                            value[(int)enum_醫囑資料.領藥號] = orderClasses[i].領藥號;
                            value[(int)enum_醫囑資料.開方日期] = orderClasses[i].開方日期;
                            value[(int)enum_醫囑資料.展藥時間] = DateTime.MinValue.ToDateTimeString_6();
                            value[(int)enum_醫囑資料.狀態] = "未過帳";

                            list_value_replace.Add(value);
                        }



                    }
                }
                Task task = Task.Run(() =>
                {
                    if (list_value_Add.Count > 0)
                    {
                        this.sQLControl_醫囑資料.AddRows(null, list_value_Add);
                    }
                    if (list_value_replace.Count > 0)
                    {
                        this.sQLControl_醫囑資料.UpdateByDefulteExtra(null, list_value_replace);
                    }
                });

                returnData.Code = 200;
                returnData.Data = orderClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"取得醫囑完成! 共<{orderClasses.Count}>筆 ,新增<{list_value_Add.Count}>筆,修改<{list_value_replace.Count}>筆";

                return returnData.JsonSerializationt(true);
            }
            catch
            {
                return "醫令串接異常";
            }

        }


        [Route("OutTakeMed")]
        [HttpPost]
        async public Task<string> OutTakeMed([FromBody] POST_Order_API pOST_Order_API)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic(50000);
                if (pOST_Order_API == null)
                {
                    Return_Order_API return_Order_API = new Return_Order_API($"傳入資料錯誤!\n{pOST_Order_API.JsonSerializationt()}", false);
                    return return_Order_API.JsonSerializationt();
                }
                string dps_api_name = "";
                string terminal = pOST_Order_API.terminal;
                string barcode = pOST_Order_API.barcode;
                string pharmacist = pOST_Order_API.pharmacist;
                bool flag_terminal_OK = true;
                bool flag_barcode_OK = true;
                bool flag_pharmacist_OK = true;
                if(terminal.StringIsEmpty() || terminal != "急診(1)")
                {
                    flag_terminal_OK = false;
                }
                if (barcode.StringIsEmpty())
                {
                    flag_barcode_OK = false;
                }
                if (pharmacist.StringIsEmpty())
                {
                    flag_pharmacist_OK = false;
                }

                if(!flag_terminal_OK|| !flag_barcode_OK || !flag_pharmacist_OK)
                {
                    string msg = "";
                    if (!flag_terminal_OK) msg += "[terminal]";
                    if (!flag_barcode_OK) msg += "[barcode]";
                    if (!flag_pharmacist_OK) msg += "[pharmacist]";
                    msg += "參數異常";
                    Return_Order_API return_Order_API = new Return_Order_API($"{msg}", false);
                    return return_Order_API.JsonSerializationt();
                }
                dps_api_name = terminal;

                string json_BBAR = await Get(barcode);
                returnData returnData_BBAR = json_BBAR.JsonDeserializet<returnData>();
                if(returnData_BBAR.Code != 200)
                {
                    Return_Order_API return_Order_API = new Return_Order_API($"{returnData_BBAR.Result}", false);
                    return return_Order_API.JsonSerializationt();
                }
                List<OrderClass> orderClasses = returnData_BBAR.Data.ObjToListClass<OrderClass>();
                DB2VM_API.BBSEController bBSEController = new DB2VM_API.BBSEController();
                returnData returnData_BBSE = new returnData();
                returnData_BBSE.Value = pharmacist;
                string json_BBSE = await bBSEController.api_login(returnData_BBSE);
                returnData_BBSE = json_BBSE.JsonDeserializet<returnData>();
                if (returnData_BBSE.Code != 200)
                {
                    Return_Order_API return_Order_API = new Return_Order_API($"{returnData_BBSE.Result}", false);
                    return return_Order_API.JsonSerializationt();
                }
                sessionClass sessionClass = returnData_BBSE.Data.ObjToClass<sessionClass>();
                List<class_OutTakeMed_data> class_OutTakeMed_Datas = new List<class_OutTakeMed_data>();
                for (int i = 0; i < orderClasses.Count; i++)
                {
                    class_OutTakeMed_data _class_OutTakeMed_Data = new class_OutTakeMed_data();
                    _class_OutTakeMed_Data.PRI_KEY = orderClasses[i].PRI_KEY;
                    _class_OutTakeMed_Data.藥品碼 = orderClasses[i].藥品碼;
                    _class_OutTakeMed_Data.交易量 = orderClasses[i].交易量;
                    _class_OutTakeMed_Data.病歷號 = orderClasses[i].病歷號;
                    _class_OutTakeMed_Data.病人姓名 = orderClasses[i].病人姓名;
                    _class_OutTakeMed_Data.開方時間 = orderClasses[i].開方日期;
                    _class_OutTakeMed_Data.領藥號 = orderClasses[i].領藥號;
                    _class_OutTakeMed_Data.操作人 = sessionClass.Name;
                    _class_OutTakeMed_Data.ID = sessionClass.ID;
                    _class_OutTakeMed_Data.電腦名稱 = "api server";
                    _class_OutTakeMed_Data.類別 = orderClasses[i].藥袋類型;
                    _class_OutTakeMed_Data.功能類型 = "A1";
                    class_OutTakeMed_Datas.Add(_class_OutTakeMed_Data);
                }
                string json_OutTakeMed = class_OutTakeMed_Datas.JsonSerializationt();
                string json_OutTakeMed_result = Basic.Net.WEBApiPostJson($"{API_SERVER}/api/OutTakeMed/{terminal}", json_OutTakeMed);
                Return_Order_API return_Order_API_result = new Return_Order_API($"{myTimerBasic.ToString()} {orderClasses.Count} {json_OutTakeMed_result}", true);
                return return_Order_API_result.JsonSerializationt();
                return $"{myTimerBasic.ToString()} {orderClasses.Count} {json_OutTakeMed_result}";
            }
            catch (Exception e)
            {
                Return_Order_API return_Order_API = new Return_Order_API($"例外,錯誤訊息:{e.Message}", false);
                return return_Order_API.JsonSerializationt();
            }
        }
    }
}
