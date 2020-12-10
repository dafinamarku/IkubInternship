using IkubInternship.Extensions;
using IkubInternship.ServiceContracts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IkubInternship.Areas.Supervisor.Controllers
{
  [Authorize(Roles ="Supervisor")]
    public class SupervisorReportsController : Controller
    {
        IReportsService rService;
        public SupervisorReportsController(IReportsService serv)
        {
          rService = serv;
        }
        // GET: Supervisor/SupervisorReports
        public ActionResult Index()
        {
            return View();
        }

    public ActionResult EmployeesPermissions()
    {
      var result = rService.EmployeesPermissions(User.Identity.GetUserId());
      if (result.HasError)
      {
        this.AddNotification(result.MessageResult, NotificationType.ERROR);
      }
      return View(result.ReturnValue);
    }

    public void EmployeesPermissionsToPDF()
    {
      try
      {
        Document doc = new Document(iTextSharp.text.PageSize.A4, 25, 25, 42, 35);
        using (MemoryStream stream = new MemoryStream())
        {
          PdfWriter w = PdfWriter.GetInstance(doc, stream);
          doc.Open();
          Font font = FontFactory.GetFont(
          FontFactory.TIMES_ROMAN, 30, BaseColor.BLACK);
          Paragraph prg = new Paragraph();
          prg.Alignment = Element.ALIGN_CENTER;
          prg.SpacingAfter = 20;
          prg.Add(new Chunk("Employees Permissions", font));
          doc.Add(prg);

          var info = rService.EmployeesPermissions(User.Identity.GetUserId()).ReturnValue;
          int nrOfRows = info.Count();
          PdfPTable table = new PdfPTable(6);

          PdfPCell cell1 = new PdfPCell();
          cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell1.AddElement(new Chunk("Full Name"));
          table.AddCell(cell1);

          PdfPCell cell2 = new PdfPCell();
          cell2.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell2.AddElement(new Chunk("Remainig"));
          table.AddCell(cell2);

          PdfPCell cell3 = new PdfPCell();
          cell3.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell3.AddElement(new Chunk("Approved"));
          table.AddCell(cell3);

          PdfPCell cell4 = new PdfPCell();
          cell4.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell4.AddElement(new Chunk("Refused"));
          table.AddCell(cell4);

          PdfPCell cell5 = new PdfPCell();
          cell5.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell5.AddElement(new Chunk("Asked"));
          table.AddCell(cell5);

          PdfPCell cell6 = new PdfPCell();
          cell6.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell6.AddElement(new Chunk("Canceled"));
          table.AddCell(cell6);

          foreach (var i in info)
          {
            table.WidthPercentage = 100;
            table.AddCell(new Phrase(i.FullName));
            table.AddCell(new Phrase(i.Remaining.ToString()));
            table.AddCell(new Phrase(i.Approved.ToString()));
            table.AddCell(new Phrase(i.Refused.ToString()));
            table.AddCell(new Phrase(i.Asked.ToString()));
            table.AddCell(new Phrase(i.Canceled.ToString()));
          }
          doc.Add(table);
          doc.Close();

          Response.Clear();
          Response.ContentType = "\".pdf\", \"application/pdf\"";
          Response.AddHeader("content-disposition", "attachment; filename=EmployeesPermissions.pdf");
          Response.BinaryWrite(stream.ToArray());
          Response.Flush();
          Response.End();
        }
      }
      catch (Exception ex)
      {
        this.AddNotification(ex.Message, NotificationType.ERROR);
      }

    }

    public void EmployeesPermissionsToExcel()
    {
      var result = rService.EmployeesPermissions(User.Identity.GetUserId());
      if (result.HasError == false)
      {
        ExcelPackage excel = new ExcelPackage();
        var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
        workSheet.TabColor = System.Drawing.Color.Black;
        workSheet.DefaultRowHeight = 12;
        //Header of table  
        //  
        workSheet.Row(1).Height = 20;
        workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Row(1).Style.Font.Bold = true;
        workSheet.Cells[3, 1].Value = "Full Name";
        workSheet.Cells[3, 2].Value = "Remaining Permissions";
        workSheet.Cells[3, 3].Value = "Approved Permissions";
        workSheet.Cells[3, 4].Value = "Refused Permissions";
        workSheet.Cells[3, 5].Value = "Asked Permissions";
        workSheet.Cells[3, 6].Value = "Canceled Permissions";
        int recordIndex = 4;
        foreach (var record in result.ReturnValue)
        {
          workSheet.Cells[recordIndex, 1].Value = record.FullName;
          workSheet.Cells[recordIndex, 2].Value = record.Remaining;
          workSheet.Cells[recordIndex, 3].Value = record.Approved;
          workSheet.Cells[recordIndex, 4].Value = record.Refused;
          workSheet.Cells[recordIndex, 5].Value = record.Asked;
          workSheet.Cells[recordIndex, 6].Value = record.Canceled;
          recordIndex++;
        }
        workSheet.Column(1).AutoFit();
        workSheet.Column(2).AutoFit();
        workSheet.Column(3).AutoFit();
        workSheet.Column(4).AutoFit();
        workSheet.Column(5).AutoFit();
        workSheet.Column(6).AutoFit();
        string excelName = "SupervisorsPermissions";
        using (var memoryStream = new MemoryStream())
        {
          Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
          Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
          excel.SaveAs(memoryStream);
          memoryStream.WriteTo(Response.OutputStream);
          Response.Flush();
          Response.End();
        }
      }
    }
  }
}