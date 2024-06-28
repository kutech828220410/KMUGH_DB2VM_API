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
    public class drugReceiptDetailClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        public string 訂單編號
        {
            get
            {
                return RMS_PURCHASE_NO.ToString("000000");
            }
        }
        /// <summary>
        /// 訂單編號
        /// </summary>
        [JsonPropertyName("RMS_PURCHASE_NO")]
        public double RMS_PURCHASE_NO { get; set; }

        /// <summary>
        /// 藥碼
        /// </summary>
        [JsonPropertyName("RMS_DRG_NO")]
        public string 藥碼 { get; set; }

        /// <summary>
        /// 贈品註記
        /// </summary>
        [JsonPropertyName("RMS_FREE_CHARGE_FLAG")]
        public string 贈品註記 { get; set; }

        /// <summary>
        /// 輸入日
        /// </summary>
        [JsonPropertyName("RMS_DATE")]
        public double 輸入日 { get; set; }

        public string 編號
        {
            get
            {
                return RMS_SEQ_NO.ToString();
            }
        }
        /// <summary>
        /// 編號
        /// </summary>
        [JsonPropertyName("RMS_SEQ_NO")]
        public double RMS_SEQ_NO { get; set; }

        /// <summary>
        /// 線上藥典商品名
        /// </summary>
        [JsonPropertyName("DRG_PURE_NAME")]
        public string 線上藥典商品名 { get; set; }

        /// <summary>
        /// 藥品中文名
        /// </summary>
        [JsonPropertyName("DRUG_CHINESE_NAME")]
        public string 藥品中文名 { get; set; }

        /// <summary>
        /// 藥品學名
        /// </summary>
        [JsonPropertyName("DRG_NOMENCLATURE")]
        public string 藥品學名 { get; set; }

        /// <summary>
        /// 條碼
        /// </summary>
        [JsonPropertyName("DRG_BARCODE")]
        public string 條碼 { get; set; }

        /// <summary>
        /// 藥名
        /// </summary>
        [JsonPropertyName("DRG_NAME")]
        public string 藥名 { get; set; }

        /// <summary>
        /// 規格
        /// </summary>
        [JsonPropertyName("DRG_SPEC")]
        public string 規格 { get; set; }

        /// <summary>
        /// 單位
        /// </summary>
        [JsonPropertyName("DRG_UNIT")]
        public string 單位 { get; set; }

        /// <summary>
        /// 包裝
        /// </summary>
        [JsonPropertyName("RMS_QTY")]
        public double 包裝數量 { get; set; }

        /// <summary>
        /// 批號
        /// </summary>
        [JsonPropertyName("EXP_GOOD_NO")]
        public string 批號 { get; set; }

        /// <summary>
        /// 效期
        /// </summary>
        [JsonPropertyName("EXP_DATE_NEW")]
        public string 效期 { get; set; }

        /// <summary>
        /// 儲備藥
        /// </summary>
        [JsonPropertyName("STOCKAGE")]
        public string 儲備藥 { get; set; }

        /// <summary>
        /// 簽呈藥
        /// </summary>
        [JsonPropertyName("PETITION")]
        public string 簽呈藥 { get; set; }

        /// <summary>
        /// 多種規格
        /// </summary>
        [JsonPropertyName("MULTISPEC")]
        public string 多種規格 { get; set; }

        /// <summary>
        /// 櫃位庫存量(盒數)
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_FLOOR")]
        public double 櫃位庫存量_盒數 { get; set; }

        /// <summary>
        /// 櫃位庫存量(粒數)
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_MOD")]
        public double 櫃位庫存量_粒數 { get; set; }

        /// <summary>
        /// 收到瓶數
        /// </summary>
        [JsonPropertyName("RMS_BOTTLE_QTY")]
        public double 收到瓶數 { get; set; }

        /// <summary>
        /// 收到粒數
        /// </summary>
        [JsonPropertyName("RMS_UNIT_QTY")]
        public double 收到粒數 { get; set; }

        /// <summary>
        /// 入庫後存量(盒數)
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_FLOOR_NET")]
        public double 入庫後存量_盒數 { get; set; }

        /// <summary>
        /// 入庫後存量(粒數)
        /// </summary>
        [JsonPropertyName("DRG_STOCK_QTY_MOD_NET")]
        public double 入庫後存量_粒數 { get; set; }

        /// <summary>
        /// 藥庫櫃位
        /// </summary>
        [JsonPropertyName("LOCATION_STK")]
        public string 藥庫櫃位 { get; set; }

        /// <summary>
        /// 顯示序號(僅顯示wok_seq_no= 1)
        /// </summary>
        [JsonPropertyName("WOK_SEQ_NO")]
        public double 顯示序號 { get; set; }

        /// <summary>
        /// 此訂單本次收到瓶數小計
        /// </summary>
        [JsonPropertyName("RMS_BOTTLE_QTY_SUM")]
        public double 此訂單本次收到瓶數小計 { get; set; }

        /// <summary>
        /// 此訂單本次收到粒數小計
        /// </summary>
        [JsonPropertyName("RMS_UNIT_QTY_SUM")]
        public double 此訂單本次收到粒數小計 { get; set; }

        /// <summary>
        /// 此訂單本次收到瓶數贈品小計
        /// </summary>
        [JsonPropertyName("RMS_BOTTLE_QTY_GIFT_SUM")]
        public double 此訂單本次收到瓶數贈品小計 { get; set; }

        /// <summary>
        /// 此訂單本次收到粒數贈品小計
        /// </summary>
        [JsonPropertyName("RMS_UNIT_QTY_GIFT_SUM")]
        public double 此訂單本次收到粒數贈品小計 { get; set; }

        /// <summary>
        /// 行狀態
        /// </summary>
        [JsonPropertyName("RowState")]
        public int RowState { get; set; }
    }

    static public class drugReceiptDetailMethod
    {
        static public System.Collections.Generic.Dictionary<string, List<drugReceiptDetailClass>> CoverToDictionaryBy_IC_SN(this List<drugReceiptDetailClass> drugReceiptDetailClasses)
        {
            Dictionary<string, List<drugReceiptDetailClass>> dictionary = new Dictionary<string, List<drugReceiptDetailClass>>();

            foreach (var item in drugReceiptDetailClasses)
            {
                string key = item.訂單編號;

                // 如果字典中已經存在該索引鍵，則將值添加到對應的列表中
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(item);
                }
                // 否則創建一個新的列表並添加值
                else
                {
                    List<drugReceiptDetailClass> values = new List<drugReceiptDetailClass> { item };
                    dictionary[key] = values;
                }
            }

            return dictionary;
        }
        static public List<drugReceiptDetailClass> SortDictionaryBy_IC_SN(this System.Collections.Generic.Dictionary<string, List<drugReceiptDetailClass>> dictionary, string IC_SN)
        {
            if (dictionary.ContainsKey(IC_SN))
            {
                return dictionary[IC_SN];
            }
            return new List<drugReceiptDetailClass>();
        }
    }

}
