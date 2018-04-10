using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// References, "Add Reference", ".NET", "Microsoft.Office.Interop.Excel":
//using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;

using System.Windows.Forms;

namespace Print
{
    public class ExcelExport
    {
        public static void saveOnExcel(String strPath /*, string strTestResultMsg, int StartFrequency, int EndFrequency, float FreqStep, double QualityFactor,
            double Bandwidth, double ResonantFreq, double ResonantImped, double ResonantPhase,
            double AntiFreq, double AntiImped, double AntiPhase*/)
        {
            /*
            System.Globalization.CultureInfo ciBack = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            String[] strMsgs = TransducerTest.getTransducerTesetObject().getTransducerTestResultsStrings();



            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Excel.Range range;
            range = xlWorkSheet.get_Range("A1", "K1");
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
            range.Font.Bold = true;


            xlWorkSheet.Cells[1, 1] = strMsgs[0];
            xlWorkSheet.Cells[1, 4] = strMsgs[1];
            xlWorkSheet.Cells[1, 6] = strMsgs[2];
            xlWorkSheet.Cells[1, 7] = strMsgs[3];

            for (int i = 1; i < 6; i++)
            {
                xlWorkSheet.Cells[2 + i, 1] = strMsgs[4 * i];
                xlWorkSheet.Cells[2 + i, 4] = strMsgs[4 * i + 1];
                xlWorkSheet.Cells[2 + i, 6] = strMsgs[4 * i + 2];
                xlWorkSheet.Cells[2 + i, 7] = strMsgs[4 * i + 3];
            }

            for (int i = 6; i < strMsgs.Length / 4; i++)
            {
                xlWorkSheet.Cells[2 + i, 1] = strMsgs[4 * i];
                xlWorkSheet.Cells[2 + i, 4] = strMsgs[4 * i + 1];
                xlWorkSheet.Cells[2 + i, 6] = strMsgs[4 * i + 2];
                xlWorkSheet.Cells[2 + i, 7] = strMsgs[4 * i + 3];
            }

            String ettId = strMsgs[64];
            String presetName = strMsgs[65];
            xlWorkSheet.Cells[2, 1] = "ETT985 ID: " + ettId;
            xlWorkSheet.Cells[2, 4] = presetName;

            Excel.Range rangeTestResult;
            rangeTestResult = xlWorkSheet.get_Range("A3", "H3");

            if (!strMsgs[5].StartsWith("OK"))
                rangeTestResult.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
            else
                rangeTestResult.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);


            Excel.Range rangeSelfTestResult;
            rangeSelfTestResult = xlWorkSheet.get_Range("A4", "H4");
            if (!strMsgs[9].StartsWith("OK"))
                rangeSelfTestResult.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
            else
                rangeSelfTestResult.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);



            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);


            System.Threading.Thread.CurrentThread.CurrentCulture = ciBack;
            */
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;

                MessageBox.Show("ERROR 2112: " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}
