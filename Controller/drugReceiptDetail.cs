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

        [Route("confirm_in")]
        [HttpPost]
        async public Task<string> POST_confirm_in(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic(50000);
            returnData returnData_out = new returnData();
            returnData.Method = "confirm_in";
            string json_out = "";
            try
            {
                inspectionClass.content content = returnData.Data.ObjToClass<inspectionClass.content>();
                
                if(content == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料錯誤";
                    Logger.LogAddLine("drugReceiptDetail_confirm_in");
                    Logger.Log("drugReceiptDetail_confirm_in", $"{ returnData.JsonSerializationt(true)}");
                    Logger.LogAddLine("drugReceiptDetail_confirm_in");
                    return returnData.JsonSerializationt(true);
                }
                string IN_RMS_PURCHASE_NO = content.驗收單號;
                string IN_RMS_DRG_NO = content.藥品碼;
                string IN_RMS_FREE_CHARGE_FLAG = content.贈品註記;
                string IN_RMS_DATE = content.新增時間.StringToDateTime().ToDateString(TypeConvert.Enum_Year_Type.Republic_of_China ,"");
                //IN_RMS_DATE = "1130626";
                string IN_RMS_SEQ_NO = content.編號.StringToInt32().ToString("00");
                string IN_USER_ID = "1000558";
                string IN_ESL_FLAG = "Y";
                string IN_ESL_DATE = content.新增時間.StringToDateTime().ToDateTimeTiny(TypeConvert.Enum_Year_Type.Anno_Domini);

                ServiceReference.ADCMedicineCabinetWCFServiceClient client = new ADCMedicineCabinetWCFServiceClient();
                string json_ESL_feedback = await client.GET_STORAGE_ESL_ResultAsync(IN_RMS_PURCHASE_NO, IN_RMS_DRG_NO, IN_RMS_FREE_CHARGE_FLAG, "1130626", IN_RMS_SEQ_NO, IN_USER_ID, IN_ESL_FLAG, IN_ESL_DATE);
                if (json_ESL_feedback.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"WCF 回傳資料異常";
                    return returnData.JsonSerializationt(true);
                }

                List<drugReceiptDetailClass> drugReceiptDetailClasses = json_ESL_feedback.JsonDeserializet<List<drugReceiptDetailClass>>();


                returnData.Data = drugReceiptDetailClasses;
                returnData_out = json_out.JsonDeserializet<returnData>();
                return returnData_out.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.LogAddLine("drugReceiptDetail_confirm_in");
                Logger.Log("drugReceiptDetail_confirm_in", $"Exception : {ex.Message}");
                Logger.LogAddLine("drugReceiptDetail_confirm_in");
                return returnData.JsonSerializationt(true);

            }
            finally
            {
                Logger.LogAddLine("drugReceiptDetail_confirm_in");
                Logger.Log("drugReceiptDetail_confirm_in", $"{ returnData_out.JsonSerializationt(true)}");
                Logger.LogAddLine("drugReceiptDetail_confirm_in");
            }
        }
        [Route("refresh")]
        [HttpPost]
        async public Task<string> POST_refresh(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic(50000);
            returnData returnData_out = new returnData();
            returnData.Method = "refresh";
            string json_out = "";
            try
            {
                ServiceReference.ADCMedicineCabinetWCFServiceClient client = new ADCMedicineCabinetWCFServiceClient();
                string json_在途藥品 = await client.DrugReceiptDetailAsync();
                if(json_在途藥品.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"WCF 回傳資料異常";
                    return returnData.JsonSerializationt(true);
                }

                List<drugReceiptDetailClass> drugReceiptDetailClasses = json_在途藥品.JsonDeserializet<List<drugReceiptDetailClass>>();

              
                returnData.Data = drugReceiptDetailClasses;
                json_out = POST_add(returnData);
                returnData_out = json_out.JsonDeserializet<returnData>();
                return returnData_out.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.LogAddLine("drugReceiptDetail_refresh");
                Logger.Log("drugReceiptDetail_refresh", $"Exception : {ex.Message}");
                Logger.LogAddLine("drugReceiptDetail_refresh");
                return returnData.JsonSerializationt(true);
          
            }
            finally
            {
                Logger.LogAddLine("drugReceiptDetail_refresh");
                Logger.Log("drugReceiptDetail_refresh", $"{ returnData_out.JsonSerializationt(true)}");
                Logger.LogAddLine("drugReceiptDetail_refresh");
            }
        }
        [Route("add")]
        [HttpPost]
        public string POST_add(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic(500000);
            myTimerBasic.StartTickTime();
            returnData returnData_out = new returnData();
            returnData_out.Method = "add";
            returnData.Method = "add";
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
                    inspectionClass.creat creat_src = inspectionClass.creat_get_by_IC_SN(API_Server, keys);
                    List<string> list_訂單內藥碼 = (from temp in drugReceiptDetailClasses_buf
                                               select temp.藥碼.Trim()).Distinct().ToList();
                    
                    if (drugReceiptDetailClasses_buf.Count > 0)
                    {
                        drugReceiptDetailClasses_buf[0].藥碼 = drugReceiptDetailClasses_buf[0].藥碼.Trim();
                        medClasses_buf = keyValuePairs_medClass.SortDictionaryByCode(drugReceiptDetailClasses_buf[0].藥碼.Trim()) ;
                        inspectionClass.creat creat = new inspectionClass.creat();
                        creat.驗收單號 = drugReceiptDetailClasses_buf[0].訂單編號;
                        creat.驗收名稱 = creat.驗收單號;
                        creat.請購單號 = creat.驗收單號;
                        creat.建表時間 = DateTime.Now.ToDateTimeString_6();
                        creat.建表人 = "系統";
                        creat.驗收開始時間 = DateTime.MinValue.ToDateTimeString_6();
                        creat.驗收結束時間 = DateTime.MinValue.ToDateTimeString_6();

                        for (int i = 0; i < list_訂單內藥碼.Count; i++)
                        {
                            inspectionClass.content content = new inspectionClass.content();
                            string 藥碼 = list_訂單內藥碼[i];
                            drugReceiptDetailClasses_temp = (from temp in drugReceiptDetailClasses_buf
                                                             where temp.藥碼.Trim() == 藥碼.Trim()
                                                             select temp).ToList();
                            if (drugReceiptDetailClasses_temp.Count > 0)
                            {

                                content.藥品碼 = drugReceiptDetailClasses_temp[0].藥碼.Trim();
                                content.應收數量 = (drugReceiptDetailClasses_temp[0].此訂單本次收到瓶數小計 * drugReceiptDetailClasses_temp[0].包裝數量).ToString();
                                content.贈品註記 = drugReceiptDetailClasses_temp[0].贈品註記.Trim();
                                if (content.贈品註記.StringIsEmpty()) content.贈品註記 = "N";
                                content.編號 = drugReceiptDetailClasses_temp[0].編號.ToString();

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
                                    sub_Content.批號 = drugReceiptDetailClasses_temp[k].批號.Trim();
                                    sub_Content.效期 = $"{drugReceiptDetailClasses_temp[k].效期.Substring(0, 4)}-{drugReceiptDetailClasses_temp[k].效期.Substring(4, 2)}-{drugReceiptDetailClasses_temp[k].效期.Substring(6, 2)}";
                                    sub_Content.實收數量 = (drugReceiptDetailClasses_temp[k].收到瓶數 * drugReceiptDetailClasses_temp[k].包裝數量).ToString();
                                    sub_Content.操作人 = "系統";
                                    sub_Content.備註 = $"{drugReceiptDetailClasses_temp[k].訂單編號}-{drugReceiptDetailClasses_temp[k].編號}";
                                    content.Sub_content.Add(sub_Content);
                                }
                            }

                            creat.Contents.Add(content);
                        }

                        string json_src = creat_src.JsonSerializationt();
                        string json = creat.JsonSerializationt();



                        if (creat_src == null)
                        {                          
                            creat = inspectionClass.creat_add(API_Server, creat);
                            if(creat == null)
                            {
                                returnData_out.Code = -200;
                                returnData_out.Result = $"新增驗收資料失敗";
                                return returnData_out.JsonSerializationt(true);
                            }
                        }
                        else
                        {
                            inspectionClass.MergeData(creat_src, creat);
                            creat = inspectionClass.creat_update(API_Server, creat_src);
                            if (creat == null)
                            {
                                returnData_out.Code = -200;
                                returnData_out.Result = $"更新驗收資料失敗";
                                return returnData_out.JsonSerializationt(true);
                            }
                        }
                        creats_buf.Add(creat);
                 
                    }
                   
                }

                returnData_out.Data = creats_buf;

            }
            catch(Exception ex)
            {
                returnData_out.Code = -200;
                returnData_out.Result = $"Exception : {ex.Message}";
                Logger.LogAddLine("drugReceiptDetail");
                Logger.Log("drugReceiptDetail", $"Exception : {ex.Message}");
                Logger.LogAddLine("drugReceiptDetail");
                return returnData_out.JsonSerializationt(true);
            }
            finally
            {
                

            }
            Logger.LogAddLine("drugReceiptDetail");
            Logger.Log("drugReceiptDetail", $"{returnData_out.JsonSerializationt(true)}");
            Logger.LogAddLine("drugReceiptDetail");
            returnData_out.Code = 200;
            returnData_out.TimeTaken = myTimerBasic.ToString();
            returnData_out.Method = "add";
            return returnData_out.JsonSerializationt();
        }
    }
}
