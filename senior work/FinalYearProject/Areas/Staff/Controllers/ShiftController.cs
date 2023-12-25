using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using QRCode = QRCoder.QRCode;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = SD.ShiftManage)]
    public class ShiftController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ShiftController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Shift.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            var payrate = await _db.Payrate.ToListAsync();
            List<string> parent_shift = new List<string>();
            parent_shift.Add(null);

            List<string> repeatable = new List<string>();
            repeatable.Add("WeekDay");
            repeatable.Add("WeekEnd");
            repeatable.Add("Whole Week");

            var parent_Shift = from s in _db.Shift where s.is_overtime == false select s.shift_id;
            string[] parentShift = parent_Shift.ToArray();
            for (int i = 0; i < parentShift.Length; i++)
            {
                parent_shift.Add(parentShift[i]);
            }
            ViewBag.parent_shift = new SelectList(parent_shift);
            ViewBag.payrate_id = new SelectList(payrate.AsEnumerable(), "payrate_id", "payrate_id");
            ViewBag.repeatable = new SelectList(repeatable);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shift shift, string repeat)
        {
            if (shift.is_overtime == false && shift.parent_shift != null)
            {
                ModelState.AddModelError("Custom Error", "Not Over Time shift cannot have parent shift");
            }
            else if (shift.is_overtime == true && shift.parent_shift == null)
            {
                ModelState.AddModelError("Custom Error", " Over Time shift must have have parent shift");
            }
            else
            {
                if (shift.shift_end < shift.shift_start)
                {
                    ModelState.AddModelError("Custom Error", " Shift End Time cannot early than shift Start Time");
                }
                else
                {
                    DateTime shiftEnd = (DateTime)shift.shift_end;
                    DateTime shiftStart = (DateTime)shift.shift_start;
                    if (shiftStart.DayOfWeek == shiftEnd.DayOfWeek)
                    {
                        if (repeat == "WeekDay")
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (shiftStart.DayOfWeek == DayOfWeek.Monday)
                                {
                                    shift.shift_id = "M" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Tuesday)
                                {
                                    shift.shift_id = "TU" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Wednesday)
                                {
                                    shift.shift_id = "W" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Thursday)
                                {
                                    shift.shift_id = "TH" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Friday)
                                {
                                    shift.shift_id = "F" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else
                                {
                                    --i;

                                }
                                if (shiftStart.DayOfWeek != DayOfWeek.Sunday && shiftStart.DayOfWeek != DayOfWeek.Saturday)
                                {
                                    if (ModelState.IsValid)
                                    {
                                        _db.Shift.Add(shift);
                                        await _db.SaveChangesAsync();
                                    }

                                }
                                shiftStart = shiftStart.AddDays(1);
                                shiftEnd = shiftEnd.AddDays(1);
                                shift.shift_start = shiftStart;
                                shift.shift_end = shiftEnd;
                            }
                        }
                        else if (repeat == "WeekEnd")
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (shiftStart.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    shift.shift_id = "SA" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    shift.shift_id = "SU" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else
                                {
                                    --i;

                                }
                                if (shiftStart.DayOfWeek == DayOfWeek.Sunday || shiftStart.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    if (ModelState.IsValid)
                                    {
                                        _db.Shift.Add(shift);
                                        await _db.SaveChangesAsync();
                                    }

                                }

                                shiftStart = shiftStart.AddDays(1);
                                shiftEnd = shiftEnd.AddDays(1);
                                shift.shift_start = shiftStart;
                                shift.shift_end = shiftEnd;
                            }
                        }
                        else if (repeat == "Whole Week")
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                if (shiftStart.DayOfWeek == DayOfWeek.Monday)
                                {
                                    shift.shift_id = "M" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Tuesday)
                                {
                                    shift.shift_id = "TU" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Wednesday)
                                {
                                    shift.shift_id = "W" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Thursday)
                                {
                                    shift.shift_id = "TH" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Friday)
                                {
                                    shift.shift_id = "F" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    shift.shift_id = "SA" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                else if (shiftStart.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    shift.shift_id = "SU" + shiftStart.Year + shiftStart.Month + shiftStart.Day + shiftStart.Hour + shiftStart.Minute + shiftEnd.Year + shiftEnd.Month + shiftEnd.Day + shiftEnd.Hour + shiftEnd.Minute;
                                }
                                if (ModelState.IsValid)
                                {
                                    _db.Shift.Add(shift);
                                    await _db.SaveChangesAsync();
                                }
                                shiftStart = shiftStart.AddDays(1);
                                shiftEnd = shiftEnd.AddDays(1);
                                shift.shift_start = shiftStart;
                                shift.shift_end = shiftEnd;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("Custom Error", "Shift Start Time and Shift End Time must at the same day");
                    }

                }
            }
            var payrate = await _db.Payrate.ToListAsync();
            List<string> parent_shift = new List<string>();
            parent_shift.Add(null);

            List<string> repeatable = new List<string>();
            repeatable.Add("WeekDay");
            repeatable.Add("WeekEnd");
            repeatable.Add("Whole Week");

            var parent_Shift = from s in _db.Shift where s.is_overtime == false select s.shift_id;
            string[] parentShift = parent_Shift.ToArray();
            for (int i = 0; i < parentShift.Length; i++)
            {
                parent_shift.Add(parentShift[i]);
            }
            ViewBag.parent_shift = new SelectList(parent_shift);
            ViewBag.payrate_id = new SelectList(payrate.AsEnumerable(), "payrate_id", "payrate_id");
            ViewBag.repeatable = new SelectList(repeatable);
            return View(shift);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var shiftFromDb = _db.Shift.Find(id);
            if (shiftFromDb == null)
            {
                return NotFound();
            }
            var payrate = await _db.Payrate.ToListAsync();
            ViewBag.payrate_id = new SelectList(payrate.AsEnumerable(), "payrate_id", "payrate_id");
            return View(shiftFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Shift shift)
        {
            if (shift.shift_end < shift.shift_start)
            {
                ModelState.AddModelError("Custom Error", " Shift End Time cannot early than shift Start Time");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _db.Shift.Update(shift);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(shift);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            var shiftFromDb = _db.Shift.Find(id);
            return View(shiftFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string? id)
        {
            var shift = await _db.Shift.FindAsync(id);
            var child_shift = from s in _db.Shift where s.parent_shift == id select s.shift_id;
            string[] childShift = child_shift.ToArray();
            if (shift == null)
            {
                return View();
            }
            for (int i = 0; i < childShift.Length; i++)
            {
                var shifts = await _db.Shift.FindAsync(childShift[i]);
                _db.Shift.Remove(shifts);
                await _db.SaveChangesAsync();
            }
            _db.Shift.Remove(shift);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CreateQRCode()
        {
            var shiftID = await _db.Shift.ToListAsync();
            ViewBag.shiftID = new SelectList(shiftID.AsEnumerable(), "shift_id", "shift_id");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateQRCode(string inputText)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrcode = new QRCodeGenerator();
                QRCodeData qrcodeData = qrcode.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrcodeData);
                using (Bitmap obitmap = qrCode.GetGraphic(20))
                {
                    obitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
                var shiftID = await _db.Shift.ToListAsync();
                ViewBag.shiftID = new SelectList(shiftID.AsEnumerable(), "shift_id", "shift_id");
                return View();
            }
        }


    }
    //Extension method to convert Bitmap to Byte Array


}






