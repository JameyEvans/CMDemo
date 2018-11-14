using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WPFUI.ViewModels
{
    public class FirstChildViewModel: Screen
    {
        private string _workOrder;
        private string _partNumber;
        private float _stdCycle;

        public string WorkOrder
        {
            get { return _workOrder; }
            set
            {
                _workOrder = value;
                NotifyOfPropertyChange(() => WorkOrder);
            }
        }


        public string PartNumber
        {
            get { return _partNumber; }
            set { _partNumber = value;
                NotifyOfPropertyChange(() => PartNumber);
                //NotifyOfPropertyChange(() => WorkOrder);
            }
        }
        public float StdCycle
        {
            get { return _stdCycle; }
            set
            {
                _stdCycle = value;
                NotifyOfPropertyChange(() => StdCycle);
                //NotifyOfPropertyChange(() => WorkOrder);
            }
        }

        private float _setupHours;

        public float SetupHours
        {
            get { return _setupHours; }
            set
            {
                _setupHours = value;
                NotifyOfPropertyChange(() => SetupHours);
                //NotifyOfPropertyChange(() => WorkOrder);
            }
        }

        private string _rawMaterial;

        public string RawMaterial
        {
            get { return _rawMaterial; }
            set
            {
                _rawMaterial = value;
                NotifyOfPropertyChange(() => RawMaterial);
                //NotifyOfPropertyChange(() => WorkOrder);
            }
        }

        private float _pieceWeight;

        public float PieceWeight
        {
            get { return _pieceWeight; }
            set
            {
                _pieceWeight = value;
                NotifyOfPropertyChange(() => PieceWeight);
                //NotifyOfPropertyChange(() => WorkOrder);
            }
        }

        private float _orderQty;

        public float OrderQty
        {
            get { return _orderQty; }
            set
            {
                _orderQty = value;
                NotifyOfPropertyChange(() => OrderQty);
                //NotifyOfPropertyChange(() => WorkOrder);
            }
        }

        public System.Windows.Forms.Integration.WindowsFormsHost WFHost { get; set; }

        public void AssignPropWorkOrderChange()
        {
            if (!string.IsNullOrEmpty(WorkOrder))
            {
                DataAccess DB = new DataAccess();
                SFPacket packetData = DB.GetPrimaryOpData(WorkOrder);
                PartNumber = packetData.item_no;
                OrderQty = packetData.qty;
                StdCycle = packetData.cyc_per;
                SetupHours = packetData.setup_std_lbr_hrs;
                RawMaterial = packetData.RawMaterial;
                PieceWeight = packetData.PieceWeight;
            }
            
        }

        public void InitializeProperties()
        {
            PartNumber = "tbd";
            OrderQty = 0;
            StdCycle = 0;
            SetupHours = 0;
            RawMaterial = "tbd";
            PieceWeight = 0;

        }

        public void CreateJobPacket()
        {
            DataAccess DB = new DataAccess();
            string prodSheetLoc = DB.GetHydroProdSheetLocation(PartNumber);
            //System.Windows.Forms.MessageBox.Show($"Prod Sheet Location = { prodSheetLoc }");
            bool isNewPath = true;
            if (string.IsNullOrEmpty(prodSheetLoc))
            {
                prodSheetLoc = @"C:\Dev\prodSheet18\masterAcmeHydroProdSheet.xlsx";
            }
            else
            {
                isNewPath = false;
            }
            SFPacket sfObj = new SFPacket()
            {
                item_no = PartNumber,
                ord_no = WorkOrder,
                cyc_per = StdCycle,
                setup_std_lbr_hrs = SetupHours,
                RawMaterial = RawMaterial,
                PieceWeight = PieceWeight,
                qty = (int)OrderQty
                };
            ExcelInterop.popHydroProdSheet(prodSheetLoc, sfObj, isNewPath);

        }

        public void CreateDrawingPDF()
        {
            WFHost = 
            PDM pdm = new PDM();            
            pdm.getDrawing(PartNumber, WFHost);

        }
        
    }

       

}
