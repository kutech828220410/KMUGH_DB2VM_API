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
namespace DB2VM
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {


        // GET api/values
        [HttpGet]
        async public Task<string> Get(string barcode)
        {

            ServiceReference.ADCMedicineCabinetWCFServiceClient client = new ADCMedicineCabinetWCFServiceClient();
            string json0 = await client.MEDCABINET_DataAsync(barcode);
            string json1 = await client.AutoStorage_DataAsync(barcode);
            string json2 =await client.QueryRtxmda_DataAsync(barcode);
            string json3 = await client.QueryRtxmda_Bags_ContentAsync(barcode);
            string DrugSpec = await client.DrugSpecInfoAsync();
            string json_在途藥品 = await client.DrugReceiptDetailAsync();
            string json_藥局庫房清單 = await client.GetSTKDetpListAsync();
            string json_請領庫房 = await client.GetReqDetpListAsync();
            string json_請領類別 = await client.GetReqTypeListAsync();
            string json_庫存資料 = await client.GetDrugInStockDetailAsync();
            string 庫別 = "4630";
            string json_請領資料_E= await client.DrugIssueDetailAsync("E", 庫別);
            string json_請領資料_B = await client.DrugIssueDetailAsync("B", 庫別);
            string json_請領資料_C = await client.DrugIssueDetailAsync("C", 庫別);
            string json_請領資料_F = await client.DrugIssueDetailAsync("F", 庫別);
            string json_倉儲機 = await client.AutoStorage_Data_ByDateTimesAsync("2024062000000000", "2024062700000000");
            string json = "";

            string json_StockDetail = await client.GetDrugInStockDetailAsync();
            json += $"[MEDCABINET_DataAsync]:{json0}\n";
            json += $"[AutoStorage_DataAsync]:{json1}\n";
            json += $"[QueryRtxmda_DataAsync]:{json2}\n";
            json += $"[QueryRtxmda_Bags_ContentAsync]:{json3}\n";
            json += $"[DrugReceiptDetailAsync(在途藥品)]:{json_在途藥品}\n";
            json += $"[GetSTKDetpListAsync(庫房清單)]:{json_藥局庫房清單}\n";
            json += $"[GetReqDetpListAsync(請領庫房)]:{json_請領庫房}\n";
            json += $"[GetReqTypeListAsync(請領類別)]:{json_請領類別}\n";
            json += $"[GetDrugInfoAsync(庫存資料)]:{json_庫存資料}\n";
            return json;


        }


    }
}
