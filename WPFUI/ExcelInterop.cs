using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace WPFUI
{
    public class ExcelInterop
    {
        public static void popHydroProdSheet(string wbPath, SFPacket sfObj, bool isNewPath)
        {

            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(wbPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.ActiveSheet;
            Excel.Range xlClrRng;
            xlApp.Visible = true;
            xlApp.DisplayAlerts = true;
            xlWorkbook.CheckCompatibility = false;
            xlWorkbook.DoNotPromptForConvert = true;
            
            xlClrRng = xlWorksheet.Range["C6, C8, C10, J8, J10, K10, M4, M8, N10, X2"];

            foreach (Excel.Range c in xlClrRng.Cells)
            {
                if (c.MergeCells)
                {
                    c.MergeArea.ClearContents();
                }
                else
                {
                    c.ClearContents();
                }
            }

            //xlClrRng.ClearContents();
            if (sfObj.RawMaterial != null || sfObj.RawMaterial != "empty")
            {
                float barWeight = DataAccess.GetBarWeight(sfObj.RawMaterial);
                xlWorksheet.Range["N10"].Value = sfObj.RawMaterial;
                if (barWeight > 0)
                {
                    xlWorksheet.Range["M4"].Value = barWeight;
                }

            }



            xlWorksheet.Range["C6"].Value = sfObj.item_no;
            xlWorksheet.Range["C8"].Value = sfObj.ord_no;
            xlWorksheet.Range["C10"].Value = sfObj.qty;
            xlWorksheet.Range["J8"].Value = (1 / sfObj.cyc_per) * 3600;
            //xlWorksheet.Range["M4"].Value = "TBD";
            xlWorksheet.Range["M8"].Value = sfObj.PieceWeight;
            //xlWorksheet.Range["N10"].Value = sfObj.RawMaterial;
            xlWorksheet.Range["X2"].Value = sfObj.setup_std_lbr_hrs;

            if (isNewPath)
            {
                wbPath = Helper.genNewProdSheetPath(sfObj.item_no);
                // create directory if it doesn't already exist
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(wbPath));

            }

            if (!System.IO.File.Exists(wbPath))
            {
                xlWorkbook.SaveAs(wbPath);
                DataAccess.InsertProdShtLoc(wbPath, sfObj.item_no);
            }
            else
            {
                xlWorkbook.Save();

            }

            xlWorkbook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, @"C:\Dev\prodSheet18\temp\temp1.pdf", OpenAfterPublish: true);
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlClrRng);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
