using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Word = Microsoft.Office.Interop.Word; 

using Microsoft.Office.Interop.Word;        

namespace AIS_exchangeOffice.classes
{
    public static class otchet_print          
    {

        public static void Start(string file_path, string[] data, string StartDate, string EndDate, string OrderCount, string AdminName)
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
                    t.Cell(1, 1).Range.Text = "номер заказа";
                    t.Cell(1, 2).Range.Text = "дата заказа";
                    t.Cell(1, 3).Range.Text = "дата доставки";
                    
                    for (int i = 0; i < data.Length; i++)
                    {
                        var data_edit = data[i].Split(' ');
                        t.Rows.Add(t.Rows[i + 2]);
                        t.Cell(i + 2, 1).Range.Text = data_edit[0];
                        t.Cell(i + 2, 2).Range.Text = data_edit[1];
                        t.Cell(i + 2, 3).Range.Text = data_edit[2];
                    }

                    var items = new Dictionary<string, string>
                    {
                        { "<start_date>", StartDate },
                        { "<end_date>", EndDate },
                        { "<order_count>", OrderCount },
                        { "<admin_name>", AdminName },
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
                    
                    doc.SaveAs(Environment.CurrentDirectory + "\\order_doc\\" + DateTime.Now.ToString("yyyMMdd_ssmm") + "otchet_print.docx");
                    app.Visible = true;
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error: \r\n{0}", ex.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: \r\n{0}", ex.ToString());
            }
        }
    }
}

