using IkubInternship.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using IkubInternship.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace IkubInternship.Areas.HR.Controllers
{
    [Authorize(Roles ="HR")]
    public class HrReportsController : Controller
    {
        IReportsService rService;
        public HrReportsController(IReportsService ser)
        {
          rService = ser;
        }
        // GET: HR/HrReports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CountPermissionsPerStatus()
        {
          var model = rService.NrOfPermissionsForeachStatus();
          return View(model);
        }

        public void CountPermissionsPerStatusPDF()
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
              prg.Add(new Chunk("Nr of permissions for every status", font));
              doc.Add(prg);

              var info = rService.NrOfPermissionsForeachStatus();
              int nrOfRows = info.Count();
              PdfPTable table = new PdfPTable(2);

              PdfPCell cell1 = new PdfPCell();
              cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
              cell1.AddElement(new Chunk("Permission Status"));
              table.AddCell(cell1);

              PdfPCell cell2 = new PdfPCell();
              cell2.BackgroundColor = BaseColor.LIGHT_GRAY;
              cell2.AddElement(new Chunk("Quantity"));
              table.AddCell(cell2);

              foreach (var i in info)
              {
                table.WidthPercentage = 100;
                table.AddCell(new Phrase(i.Key));
                table.AddCell(new Phrase(i.Value.ToString()));
              }
              doc.Add(table);
              doc.Close();

              Response.Clear();
              Response.ContentType = "\".pdf\", \"application/pdf\""; 
              Response.AddHeader("content-disposition", "attachment; filename=PermissionsPerStatus.pdf");
              Response.BinaryWrite(stream.ToArray());
              Response.Flush();
              Response.End();
            }
          }
          catch(Exception ex)
          {
            this.AddNotification(ex.Message, NotificationType.ERROR);
          }
         

        }

        public ActionResult PermissionsForeachSupervisor()
        {
          var res = rService.SupervisorsPermissions();
          if (res.HasError)
          {
            this.AddNotification(res.MessageResult, NotificationType.ERROR);
          }
          return View(res.ReturnValue);
        }

        public void PermissionsForeachSupervisorToExcel()
        {
          var result = rService.SupervisorsPermissions();
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

        public ActionResult HrEmployeesPermissions()
        {
          return View();
        }

        public void HrEmployeesPermissionsToExcel(string depName, DateTime? fromDate, DateTime? toDate, string employeeName)
        {
          var result = rService.HrEmployeesPermissions(depName, fromDate, toDate, employeeName);
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
            workSheet.Cells[3, 2].Value = "Departament";
            workSheet.Cells[3, 3].Value = "Permission Date";
            workSheet.Cells[3, 4].Value = "Permission Status";
            int recordIndex = 4;
            foreach (var record in result.ReturnValue)
            {
              workSheet.Cells[recordIndex, 1].Value = record.FullName;
              workSheet.Cells[recordIndex, 2].Value = record.Departament;
              workSheet.Cells[recordIndex, 3].Value = record.PermissionDate.ToString();
              workSheet.Cells[recordIndex, 4].Value = record.PermissionStatus;
              recordIndex++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            string excelName = "HrEmployeePermissions";
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

    public void HrEmployeesPermissionsToPDF(string depName, DateTime? fromDate, DateTime? toDate, string employeeName)
    {
      var request = rService.HrEmployeesPermissions(depName, fromDate, toDate, employeeName);
      if (request.HasError == false)
      {
        Document doc = new Document(iTextSharp.text.PageSize.A4, 25, 25, 42, 35);
        var info = request.ReturnValue;
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


          int nrOfRows = info.Count();
          PdfPTable table = new PdfPTable(4);

          PdfPCell cell1 = new PdfPCell();
          cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell1.AddElement(new Chunk("Full Name"));
          table.AddCell(cell1);

          PdfPCell cell2 = new PdfPCell();
          cell2.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell2.AddElement(new Chunk("Departament"));
          table.AddCell(cell2);

          PdfPCell cell3 = new PdfPCell();
          cell3.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell3.AddElement(new Chunk("Permission Date"));
          table.AddCell(cell3);

          PdfPCell cell4 = new PdfPCell();
          cell4.BackgroundColor = BaseColor.LIGHT_GRAY;
          cell4.AddElement(new Chunk("Permission Status"));
          table.AddCell(cell4);

          foreach (var i in info)
          {
            table.WidthPercentage = 100;
            table.AddCell(new Phrase(i.FullName));
            table.AddCell(new Phrase(i.Departament));
            table.AddCell(new Phrase(i.PermissionDate.ToString()));
            table.AddCell(new Phrase(i.PermissionStatus));
          }
          doc.Add(table);
          doc.Close();

          Response.Clear();
          Response.ContentType = "\".pdf\", \"application/pdf\"";
          Response.AddHeader("content-disposition", "attachment; filename=HrEmployeesPermissions.pdf");
          Response.BinaryWrite(stream.ToArray());
          Response.Flush();
          Response.End();
        }
      }
      
    }
  }
}