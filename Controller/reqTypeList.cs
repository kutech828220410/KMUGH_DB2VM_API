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
    public class reqTypeList : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433";

        [Route("test")]
        [HttpPost]
        public string POST_test(List<reqTypeListClass> reqTypeListClasses)
        {



            return reqTypeListClasses.JsonSerializationt(true);
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

                List<reqTypeListClass> reqTypeListClasses = returnData.Data.ObjToClass<List<reqTypeListClass>>();
                List<reqTypeListClass> reqTypeListClasses_buf = new List<reqTypeListClass>();
                List<reqTypeListClass> reqTypeListClasses_temp = new List<reqTypeListClass>();
                if (reqTypeListClasses == null)
                {
                    returnData_out.Code = -200;
                    returnData_out.Result = $"傳入資料異常";
                    return returnData_out.JsonSerializationt(true);
                }
              
                
              

            }
            catch (Exception ex)
            {
                returnData_out.Code = -200;
                returnData_out.Result = $"Exception : {ex.Message}";
                Logger.LogAddLine("reqTypeList");
                Logger.Log("reqTypeList", $"Exception : {ex.Message}");
                Logger.LogAddLine("reqTypeList");
                return returnData_out.JsonSerializationt(true);
            }
            finally
            {


            }
            Logger.LogAddLine("reqTypeList");
            Logger.Log("reqTypeList", $"{returnData_out.JsonSerializationt(true)}");
            Logger.LogAddLine("reqTypeList");
            returnData_out.Code = 200;
            returnData_out.TimeTaken = myTimerBasic.ToString();
            returnData_out.Method = "add";
            return returnData_out.JsonSerializationt();
        }
    }
}
