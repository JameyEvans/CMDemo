using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EPDM.Interop.epdm;
using WPFUI.Views;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WPFUI
{
    public class PDM
    {
        eDwHost hostContainer = null;

        public void getDrawing(string itemNumber, System.Windows.Forms.Integration.WindowsFormsHost wfHost)
        {
                      
            if (null == hostContainer)
            {
                hostContainer = new eDwHost();
            }

            if (null == wfHost)
            {
                wfHost = new System.Windows.Forms.Integration.WindowsFormsHost();
            }

            wfHost.Child = hostContainer;
            //wfHost.Child.Controls.Add(hostContainer);

            object Val1 = "%" + itemNumber + "%";

            PdmLogin(Val1);

        }

        internal void OpenSaveSldrwPdf(string sldrwPath)
        {
            //eDwHost hostContainer = new eDwHost();
            if (hostContainer != null)
            {
                ((System.Windows.Forms.Control)hostContainer).Hide();
                dynamic emvControl = hostContainer.GetOcx();
                emvControl.OpenDoc(sldrwPath, false, false, true, "");

                emvControl.SetPageSetupOptions(EModelView.EMVPrintOrientation.eLandscape, 1, 0, 0, 1, 0, "pdfAutoSave", 0, 0, 0, 0);
                emvControl.Print5(false, @"drawing.pdf", true, false, false, 1, 0, 0, 0, true, 0, 0, "");
            }
        }

        private void PdmLogin(object Val1)
        {
            try
            {
                //Create a file vault interface and log into a vault
                IEdmVault5 vault = new EdmVault5();
                vault.LoginAuto("CDI Controlled Documents", 32);

                SearchFiles(vault, Val1);

                

            }

            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + "\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchFiles(IEdmVault5 vault, object Val1)
        {
            try

            {

                IEdmSearch5 search = null;
                IEdmFolder5 folder = null;

                folder = vault.GetFolderFromPath(@"C:\CDI Controlled Documents\drawings\part drawings- controlled");

                search = vault.CreateSearch();
                search.FileName = "%.%d%w%";
                search.StartFolderID = folder.ID;

                object Var1 = "Part Numbers";
                object State = "Approved for Production";


                search.AddVariable(Var1, Val1);

                search.set_State(State);
                search.FindHistoricStates = false;
                search.Recursive = true;

                String message = string.Empty;



                List<IEdmSearchResult5> results = new List<IEdmSearchResult5>();

                IEdmSearchResult5 result = search.GetFirstResult();

                while (result != null)

                {
                    results.Add(result);
                    result = search.GetNextResult();
                }
                results = results.Distinct(new SearchResultComparer()).ToList();
                if (results.Count < 1)
                {
                    search.Clear();
                    search.FileName = Val1 + ".%d%w%";
                    search.StartFolderID = folder.ID;
                    State = "Approved for Production";
                    search.set_State(State);
                    search.FindHistoricStates = false;
                    search.Recursive = true;

                    result = search.GetFirstResult();
                    while (result != null)

                    {
                        results.Add(result);
                        result = search.GetNextResult();
                    }
                    results = results.Distinct(new SearchResultComparer()).ToList();


                }
                foreach (IEdmSearchResult5 item in results)
                {
                    //get latest version
                    IEdmFile5 file = null;
                    IEdmFolder5 retFolder = default(IEdmFolder5);
                    file = vault.GetFileFromPath(item.Path, out retFolder);
                    file.GetFileCopy(0);
                    //System.Diagnostics.Process.Start(item.Path, @"C:\Program Files\SOLIDWORKS Corp\eDrawings\eDrawings.exe");
                    //OpenEdwg(item.Path);
                    OpenSaveSldrwPdf(item.Path);



                    message = message + "Filename: " + item.Name + ", Rev: " + file.CurrentRevision + "; \n";
                }



                MessageBox.Show(message);

            }





            catch (Exception ex)

            {

                MessageBox.Show(ex.Message);

            }

        }



        class SearchResultComparer : IEqualityComparer<IEdmSearchResult5>
        {
            public bool Equals(IEdmSearchResult5 x, IEdmSearchResult5 y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(IEdmSearchResult5 obj)
            {
                return obj.ID.GetHashCode();
            }
        }

        partial class eDwHost : System.Windows.Forms.AxHost
        {
            public eDwHost()
                : base("{22945A69-1191-4DCF-9E6F-409BDE94D101}")
            {
                //InitializeComponent();
            }
            private dynamic ocx;
            protected override void AttachInterfaces()
            {
                base.AttachInterfaces();
                try
                {
                    if (IntPtr.Size == 8) //64 bit
                    {
                        // "Forced compiler error! This code can never work in 32-bit processes since it depends on an ActiveX contronl which is only available in 64-bit."
                        this.ocx = (dynamic)base.GetOcx();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\r\n\r\n" + ex.StackTrace, "Exception loading eModelViewControl");
                }
            }

            public new dynamic GetOcx()
            {
                return (dynamic)base.GetOcx();
            }
        }
    }
}
