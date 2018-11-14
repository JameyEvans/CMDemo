using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI
{
    public class SFPacket
    {
        // Properties for sqlserver02\500  table = sfdtlfil_sql
        private string _item_no;
        private char[] _charsToTrim = { ' ' };
        private string _comp_item_no;
        private string _sfd_desc_1;
        private string _sfd_desc_2;

        public string ord_no { get; set; }

        public int oper_no { get; set; }

        public int oper_seq_no { get; set; }        

        public string item_no
        {
            get { return _item_no; }
            set { _item_no = value.Trim(_charsToTrim); }
        }

        public string comp_item_no
        {
            get { return _comp_item_no; }
            set { _comp_item_no = value.Trim(_charsToTrim); }
        }

        public string sfd_desc_1
        {
            get { return _sfd_desc_1; }
            set { _sfd_desc_1 = value.Trim(_charsToTrim); }
        }

        public string sfd_desc_2
        {
            get { return _sfd_desc_2; }
            set { _sfd_desc_2 = value.Trim(_charsToTrim); }
        }

        public int qty { get; set; }

        public float mat_qty_per_par { get; set; }

        public float setup_std_lbr_hrs { get; set; }

        public float cyc_per { get; set; }

        public string due_dt { get; set; }

        public string wc { get; set; }

        // not included in db
        public string RawMaterial { get; set; }
        public float PieceWeight { get; set; }


    }
}
