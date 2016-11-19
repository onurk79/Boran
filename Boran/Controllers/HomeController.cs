using Boran.Models;
using Boran.ViewModels;
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boran.Controllers
{
    public class HomeController : Controller
    {
        private boranEntities context = new boranEntities();

        public ActionResult Index(bool clearDb = false)
        {
            if (clearDb)
            {
                context.db.RemoveRange(context.db.ToList());
                context.SaveChanges();
                return View();
            }
            else
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {

                    Stream stream = upload.InputStream;

                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    reader.IsFirstRowAsColumnNames = true;

                    DataSet result = reader.AsDataSet();
                    reader.Close();
                    foreach (DataRow Row in result.Tables[0].Rows)
                    {
                        Boran.Models.db db = new db();
                        db.OrderID = Convert.ToInt32(Row[result.Tables[0].Columns[0]]);
                        db.OrderDate = Convert.ToDateTime(Row[result.Tables[0].Columns[1]]);
                        db.OrderPriority = Row[result.Tables[0].Columns[2]].ToString();
                        db.OrderQuantity = Convert.ToInt32(Row[result.Tables[0].Columns[3]]);
                        db.Discount = Convert.ToDouble(Row[result.Tables[0].Columns[4]]);
                        db.ShipMode = Row[result.Tables[0].Columns[5]].ToString();
                        db.UnitPrice = Convert.ToDouble(Row[result.Tables[0].Columns[6]]);
                        db.UnitShippingCost = Convert.ToDouble(Row[result.Tables[0].Columns[7]]);
                        db.CustomerName = Row[result.Tables[0].Columns[8]].ToString();
                        db.Province = Row[result.Tables[0].Columns[9]].ToString();
                        db.Region = Row[result.Tables[0].Columns[10]].ToString();
                        db.CustomerSegment = Row[result.Tables[0].Columns[11]].ToString();
                        db.ProductCategory = Row[result.Tables[0].Columns[12]].ToString();
                        db.ProductSubCategory = Row[result.Tables[0].Columns[13]].ToString();
                        db.ProductName = Row[result.Tables[0].Columns[14]].ToString();
                        db.ProductContainer = Row[result.Tables[0].Columns[15]].ToString();
                        if (Row[result.Tables[0].Columns[16]].ToString() == "")

                            db.ProductBaseMargin = null;

                        else

                            db.ProductBaseMargin = Convert.ToDouble((Row[result.Tables[0].Columns[16]]));

                        db.ShipDate = Convert.ToDateTime(Row[result.Tables[0].Columns[17]]);

                        context.db.Add(db);

                    }
                    context.SaveChanges();

                    return RedirectToAction("Calculation");
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();

        }
        List<DateTime> DateList = new List<DateTime>();
        List<string> ProductCategoryList = new List<string>();

        public ActionResult Calculation()
        {
            if (context.db.Count() == 0)
            {
                ModelState.AddModelError("File", "Database Null");
                return View();
            }
            else
            {
                context.Database.CommandTimeout = 99000;

                var date = (from a in context.db.OrderBy(x => x.OrderDate)
                            group a by a.OrderDate.Value.Year into g
                            select new
                            {
                                date = g.Key,
                                count = g.Count(),

                            }).ToList();
                List<List<Sum>> sum = new List<List<Sum>>();
                foreach (var item in date)
                {
                    List<Sum> result = (from a in context.db.Where(x => x.OrderDate.Value.Year == item.date)
                                        group a by a.ProductCategory into g
                                        select new Sum
                                        {
                                            Date = item.date,
                                            ProductCategory = g.Key,
                                            _totalPriceToCategory = g.Sum(oi => oi.OrderQuantity * (oi.UnitPrice - (oi.UnitPrice * oi.Discount) + oi.UnitShippingCost)),
                                            _count = g.Count(),

                                        }).ToList();
                    foreach (var _string in result)
                    {
                        _string.TotalPriceToCategory = string.Format("{0:0,0.000}", _string._totalPriceToCategory);
                        _string.Count = string.Format("{0:0,0.}", _string._count);
                    }
                    sum.Add(result);
                }
                return View(sum);
            }
        }
    }
}