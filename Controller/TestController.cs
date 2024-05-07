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
using Oracle.ManagedDataAccess.Client;
using ServiceReference;
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

            //string json = await client.MEDCABINET_DataAsync(barcode);
            string json = await client.AutoStorage_DataAsync(barcode);
            //string json =await client.QueryRtxmda_DataAsync(barcode);
            //string json = await client.QueryRtxmda_Bags_ContentAsync(barcode);
            return json;


        }


    }
}
