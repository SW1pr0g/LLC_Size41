using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word; 

using Microsoft.Office.Interop.Word;        

namespace AIS_exchangeOffice.classes
{
    public static class order_print          
    {

        public static void Start(string file_path, string[] data, string OrderNum, string FIO, string OrderDate, string DeliveryDate, string TotalSumm, string PickupAddr)
        {
            try
            {
                int j = data.Length;
                try
                {
                    Word.Application app = new Word.Application();

                    Object wdMiss = System.Reflection.Missing.Value;
                    Document doc = app.Documents.Open(file_path);
                    Range r = doc.Range();

                    Table t = doc.Tables[2];
                    t.Rows.Add(t.Rows[1]);
                    t.Cell(1, 1).Range.Text = "название";
                    t.Cell(1, 2).Range.Text = "кол-во";
                    t.Cell(1, 3).Range.Text = "цена(руб)";
                    t.Cell(1, 4).Range.Text = "скидка(руб/%)";
                    t.Cell(1, 5).Range.Text = "всего(руб)";
                    
                    for (int i = 0; i < data.Length; i++)
                    {
                        var data_edit = data[i].Split(' ');
                        t.Rows.Add(t.Rows[i + 2]);
                        t.Cell(i + 2, 1).Range.Text = data_edit[0];
                        t.Cell(i + 2, 2).Range.Text = data_edit[1];
                        t.Cell(i + 2, 3).Range.Text = data_edit[2];
                        t.Cell(i + 2, 4).Range.Text = data_edit[3];
                        t.Cell(i + 2, 5).Range.Text = data_edit[4];
                    }

                    var items = new Dictionary<string, string>
                    {
                        { "<number>", OrderNum },
                        { "<order_date>", OrderDate },
                        { "<order_deliverydate>", DeliveryDate },
                        { "<client_name>", FIO },
                        { "<totalSumm>", TotalSumm },
                        { "<order_pickupaddr>", PickupAddr },
                        { "<print_date>", DateTime.Now.ToString("hh:mm:ss dd.MM.yyyy") }
                    };
                    Object missing = Type.Missing;
                    foreach (var item in items)
                    {
                        Word.Find find = app.Selection.Find;
                        find.Text = item.Key;
                        find.Replacement.Text = item.Value;

                        Object wrap = Word.WdFindWrap.wdFindContinue;
                        Object replace = Word.WdReplace.wdReplaceAll;

                        find.Execute(FindText: Type.Missing,
                            MatchCase: false,
                            MatchWholeWord: false,
                            MatchWildcards: false,
                            MatchSoundsLike: missing,
                            MatchAllWordForms: false,
                            Forward: true,
                            Wrap: wrap,
                            Format: false,
                            ReplaceWith: missing, Replace: replace);
                    }
                    
                    doc.SaveAs(Environment.CurrentDirectory + "\\order_doc\\" + DateTime.Now.ToString("yyyMMdd_") + OrderNum + "order_print.docx");
                    app.Visible = true;
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error: \r\n{0}", ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: \r\n{0}", ex.ToString());
            }
        }
    }
}

