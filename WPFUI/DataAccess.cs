using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Windows.Forms;


namespace WPFUI
{
    public class DataAccess
    {
        public List<SFPacket> GetSFDataByOrdNo(string ord_num)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("MFDB")))
            {

                return connection.Query<SFPacket>("dbo.SFData_GetByOrder @ord_num", new { ord_num = ord_num }).ToList();

            }
        }

        public SFPacket GetPrimaryOpData(string workOrder)
        {
            List<SFPacket> sfDataList = GetSFDataByOrdNo(workOrder);
            SFPacket packetData = new SFPacket();
            foreach (SFPacket item in sfDataList)
            {
                if (!string.IsNullOrEmpty(item.item_no))
                {
                    packetData.item_no = item.item_no;
                }

                // get raw material
                if (item.oper_no == 0 && Helper.IsLike(item.comp_item_no, "A?????-????"))
                {
                    packetData.RawMaterial = item.comp_item_no;
                    packetData.PieceWeight = item.mat_qty_per_par;
                }

                if (Helper.IsLike(item.sfd_desc_1, "*PRIMARY*"))
                {
                    packetData.qty = item.qty;
                    packetData.cyc_per = item.cyc_per;
                    packetData.setup_std_lbr_hrs = item.setup_std_lbr_hrs;
                }
            }
            return packetData;
        }

        public string GetHydroProdSheetLocation(string itemNo)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("MFDB")))
            {

                List<string> prodSheetList = connection.Query<string>("dbo.GetProdSheetLocByItem @itemNum", new { itemNum = itemNo }).ToList();
                string prodSheetPath = null;
                if (prodSheetList.Count > 0) { prodSheetPath = prodSheetList[0]; }
                if (prodSheetList.Count > 1)
                {
                    MessageBox.Show($"Multiple entries for prod sheet were found.  Using first result: {prodSheetPath}");
                    
                }
                
                return prodSheetPath;
            }
        }

        public static float GetBarWeight(string rawMatNumber)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("MFDB")))
            {

                List<float> barWeightList = connection.Query<float>("dbo.GetBarWeightByPartNumber @partNumber", new { partNumber = rawMatNumber }).ToList();
                float barWeight = 0;
                if (barWeightList.Count > 0) { barWeight = barWeightList[0]; }
                if (barWeightList.Count > 1)
                {
                    MessageBox.Show($"Multiple entries for bar weight were found.  Using first result: {barWeight}");

                }

                return barWeight;
            }
        }

        public static void InsertProdShtLoc(string prodShtLoc, string itemNo)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("MFDB")))
            {

                connection.Execute("dbo.InsertProdShtLoc @prodShtLoc, @itemNo", new { prodShtLoc = prodShtLoc, itemNo = itemNo });

            }
        }

    }
}
