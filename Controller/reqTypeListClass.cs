using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.Text.Json;

namespace DB2VM_API
{
    public class reqTypeListClass
    {
        /// <summary>
        /// 請領類別碼(KEY)。
        /// </summary>
        [JsonPropertyName("DAY_TX_CODE")]
        public string 請領類別碼 { get; set; }

        /// <summary>
        /// 異動日期(KEY)。
        /// </summary>
        [JsonPropertyName("DAY_DATE")]
        public double 異動日期 { get; set; }

        /// <summary>
        /// 自動編號(KEY)。
        /// </summary>
        [JsonPropertyName("DAY_SHEET_NO")]
        public double 自動編號 { get; set; }

        /// <summary>
        /// 條碼。
        /// </summary>
        [JsonPropertyName("BARCODE")]
        public string 條碼 { get; set; }

        /// <summary>
        /// 庫房碼。
        /// </summary>
        [JsonPropertyName("DAY_DEPT_NO")]
        public string 庫房碼 { get; set; }

        /// <summary>
        /// 藥碼。
        /// </summary>
        [JsonPropertyName("DAY_DRG_NO")]
        public string 藥碼 { get; set; }

        /// <summary>
        /// 最小批號。
        /// </summary>
        [JsonPropertyName("WOK_GOOD_NO_MIN")]
        public string 最小批號 { get; set; }

        /// <summary>
        /// 最小效期。
        /// </summary>
        [JsonPropertyName("WOK_EXP_DATE_MIN")]
        public double 最小效期 { get; set; }

        /// <summary>
        /// 藥名。
        /// </summary>
        [JsonPropertyName("DRG_NAME")]
        public string 藥名 { get; set; }

        /// <summary>
        /// 規格。
        /// </summary>
        [JsonPropertyName("DRG_SPEC")]
        public string 規格 { get; set; }

        /// <summary>
        /// 單位。
        /// </summary>
        [JsonPropertyName("DRG_UNIT")]
        public string 單位 { get; set; }

        /// <summary>
        /// 包裝。
        /// </summary>
        [JsonPropertyName("DAY_PACKAGE")]
        public double 包裝 { get; set; }

        /// <summary>
        /// 數量。
        /// </summary>
        [JsonPropertyName("WOK_QTY_FLOOR")]
        public double 數量 { get; set; }

        /// <summary>
        /// 粒數。
        /// </summary>
        [JsonPropertyName("WOK_QTY_MOD")]
        public double 粒數 { get; set; }

        /// <summary>
        /// 列印時間。
        /// </summary>
        [JsonPropertyName("DAY_PRINT_DATE")]
        public DateTime 列印時間 { get; set; }

        /// <summary>
        /// 檔案筆數。
        /// </summary>
        [JsonPropertyName("DAY_COUNT_NO")]
        public int 檔案筆數 { get; set; }

        /// <summary>
        /// 線上藥典商品名。
        /// </summary>
        [JsonPropertyName("DRG_PURE_NAME")]
        public string 線上藥典商品名 { get; set; }

        /// <summary>
        /// 藥品中文名。
        /// </summary>
        [JsonPropertyName("DRUG_CHINESE_NAME")]
        public string 藥品中文名 { get; set; }

        /// <summary>
        /// 藥品學名。
        /// </summary>
        [JsonPropertyName("DRG_NOMENCLATURE")]
        public string 藥品學名 { get; set; }

        /// <summary>
        /// 批號。
        /// </summary>
        [JsonPropertyName("EXP_GOOD_NO")]
        public string 批號 { get; set; }

        /// <summary>
        /// 效期。
        /// </summary>
        [JsonPropertyName("EXP_DATE_NEW")]
        public string 效期 { get; set; }

        /// <summary>
        /// 儲備藥。
        /// </summary>
        [JsonPropertyName("STOCKAGE")]
        public string 儲備藥 { get; set; }

        /// <summary>
        /// 簽呈藥。
        /// </summary>
        [JsonPropertyName("PETITION")]
        public string 簽呈藥 { get; set; }

        /// <summary>
        /// 多種規格。
        /// </summary>
        [JsonPropertyName("MULTISPEC")]
        public string 多種規格 { get; set; }

        /// <summary>
        /// 櫃位庫存量(盒數)。
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_FLOOR")]
        public double 櫃位庫存量盒數 { get; set; }

        /// <summary>
        /// 櫃位庫存量(粒數)。
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_MOD")]
        public double 櫃位庫存量粒數 { get; set; }

        /// <summary>
        /// 入庫後存量(盒數)。
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_FLOOR_NET")]
        public double 入庫後存量盒數 { get; set; }

        /// <summary>
        /// 入庫後存量(粒數)。
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_MOD_NET")]
        public double 入庫後存量粒數 { get; set; }

        /// <summary>
        /// 藥庫櫃位。
        /// </summary>
        [JsonPropertyName("LOCATION_STK")]
        public string 藥庫櫃位 { get; set; }
    }
}
