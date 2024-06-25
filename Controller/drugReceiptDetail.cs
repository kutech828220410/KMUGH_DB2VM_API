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

namespace DB2VM_API
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class drugReceiptDetail : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433";

        [Route("test")]
        [HttpPost]
        public string POST_test(List<drugReceiptDetailClass> drugReceiptDetailClass)
        {
           


            return drugReceiptDetailClass.JsonSerializationt(true);
        }


        [Route("add")]
        [HttpPost]
        public string POST_add(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic(500000);
            myTimerBasic.StartTickTime();
            returnData returnData_out = new returnData();
            try
            {
                ServerSettingClass serverSettingClass = ServerSettingClass.get_VM_Server(API_Server);
                if (serverSettingClass == null)
                {                  
                    returnData_out.Code = -200;
                    returnData_out.Result = $"找無伺服器資訊";
                    return returnData_out.JsonSerializationt(true);
                }
                List<medClass> medClasses = medClass.get_med_cloud(API_Server);
                List<medClass> medClasses_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClass = medClasses.CoverToDictionaryByCode();

                List<drugReceiptDetailClass> drugReceiptDetailClasses = returnData.Data.ObjToClass<List<drugReceiptDetailClass>>();
                List<drugReceiptDetailClass> drugReceiptDetailClasses_buf = new List<drugReceiptDetailClass>();
                List<drugReceiptDetailClass> drugReceiptDetailClasses_temp = new List<drugReceiptDetailClass>();
                if (drugReceiptDetailClasses == null)
                {
                    returnData_out.Code = -200;
                    returnData_out.Result = $"傳入資料異常";
                    return returnData_out.JsonSerializationt(true);
                }
                List<Table> tables = inspectionClass.Init(API_Server);
                List<inspectionClass.creat> creats_add = new List<inspectionClass.creat>();
                List<inspectionClass.creat> creats_buf = new List<inspectionClass.creat>();
                System.Collections.Generic.Dictionary<string, List<drugReceiptDetailClass>> keyValuePairs_drugReceiptDetail =  drugReceiptDetailClasses.CoverToDictionaryBy_IC_SN();
                foreach (string keys in keyValuePairs_drugReceiptDetail.Keys)
                {
             
                    drugReceiptDetailClasses_buf = keyValuePairs_drugReceiptDetail.SortDictionaryBy_IC_SN(keys);
                    inspectionClass.creat creat = inspectionClass.creat_get_by_IC_SN(API_Server, keys);
                    List<string> list_訂單內藥碼 = (from temp in drugReceiptDetailClasses_buf
                                               select temp.藥碼.Trim()).Distinct().ToList();
                    
                    if (drugReceiptDetailClasses_buf.Count > 0)
                    {
                        if (creat == null)
                        {
                            medClasses_buf = keyValuePairs_medClass.SortDictionaryByCode(drugReceiptDetailClasses_buf[0].藥碼);
                            creat = new inspectionClass.creat();
                            creat.驗收單號 = drugReceiptDetailClasses_buf[0].訂單編號;
                            creat.驗收名稱 = creat.驗收單號;
                            creat.建表時間 = DateTime.Now.ToDateTimeString_6();
                            creat.建表人 = "系統";
                            creat.驗收開始時間 = DateTime.MinValue.ToDateTimeString_6();
                            creat.驗收結束時間 = DateTime.MinValue.ToDateTimeString_6();

                            for (int i = 0; i < list_訂單內藥碼.Count; i++)
                            {
                                inspectionClass.content content = new inspectionClass.content();
                                string 藥碼 = list_訂單內藥碼[i];
                                drugReceiptDetailClasses_temp = (from temp in drugReceiptDetailClasses_buf
                                                                 where temp.藥碼 == 藥碼
                                                                 select temp).ToList();
                                if(drugReceiptDetailClasses_temp.Count > 0)
                                {
                               
                                    content.藥品碼 = drugReceiptDetailClasses_buf[0].藥碼;                      
                                    content.應收數量 = (drugReceiptDetailClasses_buf[0].此訂單本次收到瓶數小計 * drugReceiptDetailClasses_buf[i].包裝數量).ToString();

                                    if (medClasses_buf.Count > 0)
                                    {
                                        content.藥品名稱 = medClasses_buf[0].藥品名稱;
                                        content.中文名稱 = medClasses_buf[0].中文名稱;
                                        content.包裝單位 = medClasses_buf[0].包裝單位;
                                    }
                                    for (int k = 0; k < drugReceiptDetailClasses_temp.Count; k++)
                                    {
                                        inspectionClass.sub_content sub_Content = new inspectionClass.sub_content();
                                        sub_Content.藥品碼 = content.藥品碼;
                                        sub_Content.藥品名稱 = content.藥品名稱;
                                        sub_Content.中文名稱 = content.中文名稱;
                                        sub_Content.批號 = drugReceiptDetailClasses_buf[0].批號.Trim();
                                        sub_Content.效期 = $"{drugReceiptDetailClasses_buf[0].效期.Substring(0,4)}-{drugReceiptDetailClasses_buf[0].效期.Substring(4, 2)}-{drugReceiptDetailClasses_buf[0].效期.Substring(6, 2)}";
                                        sub_Content.實收數量 = (drugReceiptDetailClasses_temp[k].收到瓶數 * drugReceiptDetailClasses_buf[i].包裝數量).ToString();
                                        content.Sub_content.Add(sub_Content);
                                    }
                                }

                                creat.Contents.Add(content);
                            }
                           

                            creat = inspectionClass.creat_add(API_Server, creat);
                        }
                    }
                   
                }
     


            }
            catch(Exception ex)
            {
                returnData_out.Code = -200;
                returnData_out.Result = $"Exception : {ex.Message}";
                return returnData_out.JsonSerializationt(true);
            }
            finally
            {

            }
            return returnData.JsonSerializationt();
        }
    }
}
