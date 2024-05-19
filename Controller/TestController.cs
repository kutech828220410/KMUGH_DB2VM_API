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


            string json = "";
            json += $"[MEDCABINET_DataAsync]:{json0}\n";
            json += $"[AutoStorage_DataAsync]:{json1}\n";
            json += $"[QueryRtxmda_DataAsync]:{json2}\n";
            json += $"[QueryRtxmda_Bags_ContentAsync]:{json3}\n";
            return json;


        }


    }
}
