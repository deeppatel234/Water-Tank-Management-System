using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Configuration;

namespace TankSystem
{
    /// <summary>
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Window
    {
        public ReportsPage()
        {
            InitializeComponent();

            DAO dao = new DAO();

            Usage usageM1 = new Usage();
            usageM1.startdate = DateTime.Today.ToString("dd-MM-yyyy");
            usageM1.starttime = DateTime.Now.ToString("hh:mm:ss");

            usageM1.enddate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
            usageM1.endtime = DateTime.Now.ToString("hh:mm:ss");
            usageM1.motorid = "M1";
            usageM1.amountleter = 0;
            dao.addData(usageM1);
        }
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);

        private void usagebtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create);

                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    PdfPTable table = new PdfPTable(7);
                    PdfPCell cell = new PdfPCell(new Phrase("Usages of Water"));
                    cell.Colspan = 7;
                    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);

                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);


                    table.AddCell(new iTextSharp.text.Paragraph("ID", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Date", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Date", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Motor ID", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Usage Leter", boldFont));


                    SqlCommand com1 = new SqlCommand("select * from usage;", conn);

                    conn.Open();
                    SqlDataReader dr = com1.ExecuteReader();
                    int i = 1;
                    int totalusage = 0;
                    while (dr.Read())
                    {
                        table.AddCell(i + "");
                        table.AddCell(dr[1].ToString());
                        table.AddCell(dr[2].ToString());
                        table.AddCell(dr[3].ToString());
                        table.AddCell(dr[4].ToString());
                        table.AddCell(dr[5].ToString());
                        totalusage += Convert.ToInt32(dr[6].ToString());
                        table.AddCell(dr[6].ToString());
                        i++;
                    }

                    PdfPCell cell1 = new PdfPCell(new Phrase("Total Usage : " + totalusage + " Liter   "));
                    cell1.Colspan = 7;
                    cell1.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right

                    table.AddCell(cell1);

                    document.Add(table);
                    document.Close();
                    writer.Close();
                    fs.Close();
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("Something Went Wrong");
            }
        }

        private void dateusagebtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create);

                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    PdfPTable table = new PdfPTable(7);
                    PdfPCell cell = new PdfPCell(new Phrase("Usages of Water"));
                    cell.Colspan = 7;
                    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);

                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);


                    table.AddCell(new iTextSharp.text.Paragraph("ID", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Date", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Date", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Motor ID", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Usage Leter", boldFont));


                    SqlCommand com1 = new SqlCommand("select * from usage;", conn);

                    conn.Open();
                    SqlDataReader dr = com1.ExecuteReader();

                    string startdateValue = startdate.SelectedDate.ToString().Substring(0, 10);
                    string enddatevalue = enddate.SelectedDate.ToString().Substring(0, 10);

                    int i = 1;
                    int totalusage = 0;

                    while (dr.Read())
                    {
                        if (DateTime.Parse(dr[3].ToString()) >= DateTime.Parse(startdateValue) && DateTime.Parse(enddatevalue) >= DateTime.Parse(dr[4].ToString()))
                        {
                            table.AddCell(i + "");
                            table.AddCell(dr[1].ToString());
                            table.AddCell(dr[2].ToString());
                            table.AddCell(dr[3].ToString());
                            table.AddCell(dr[4].ToString());
                            table.AddCell(dr[5].ToString());
                            totalusage += Convert.ToInt32(dr[6].ToString());
                            table.AddCell(dr[6].ToString());
                            i++;
                        }
                    }

                    PdfPCell cell1 = new PdfPCell(new Phrase("Total Usage between " + startdateValue + " to " + enddatevalue + " : " + totalusage + " Liter   "));
                    cell1.Colspan = 7;
                    cell1.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right

                    table.AddCell(cell1);
                    document.Add(table);
                    document.Close();
                    writer.Close();
                    fs.Close();
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("Something Went Wrong");
            }
        }
    }
}
