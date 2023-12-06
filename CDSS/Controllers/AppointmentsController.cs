using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CDSS.Models;
using CDSS.Services;

namespace CDSS.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchString, string sortOrder, string sortBy)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .ToListAsync();

            // Apply search filter if searchString is not null or empty
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                appointments = appointments
                    .Where(a =>
                        a.Patient.FullName.ToLower().Contains(searchString) ||
                        a.AppointmentDate.ToString().ToLower().Contains(searchString) ||
                        a.PurposeOfVisit.ToLower().Contains(searchString) ||
                        (a.AdditionalNotes != null && a.AdditionalNotes.ToLower().Contains(searchString))
                    )
                    .ToList();
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "asc":
                    appointments = appointments.OrderBy(GetSortExpression(sortBy)).ToList();
                    break;
                case "desc":
                    appointments = appointments.OrderByDescending(GetSortExpression(sortBy)).ToList();
                    break;
                default:
                    appointments = appointments.OrderBy(a => a.Patient.FullName).ToList();
                    break;
            }

            ViewData["NameSortParam"] = sortOrder == "desc" ? "asc" : "desc";

            return View(appointments);
        }

        private Func<Appointments, string> GetSortExpression(string sortBy)
        {
            switch (sortBy)
            {
                case "FullName":
                    return a => a.Patient.FullName;
                // Add other cases for different properties to sort by
                default:
                    return a => a.Patient.FullName;
            }
        }



        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointments == null)
            {
                return NotFound();
            }

            return View(appointments);
        }

        //GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
            return View();
        }

        //POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,PatientId,AppointmentDate,PurposeOfVisit,AdditionalNotes")] Appointments appointments)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", appointments.PatientId);
            return View(appointments);
        }




        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments.FindAsync(id);
            if (appointments == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", appointments.PatientId);
            return View(appointments);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,PatientId,AppointmentDate,PurposeOfVisit,AdditionalNotes")] Appointments appointments)
        {
            if (id != appointments.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentsExists(appointments.AppointmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", appointments.PatientId);
            return View(appointments);
        }

        //GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointments == null)
            {
                return NotFound();
            }

            // Check if there are associated reviews
            var hasReviews = _context.Review.Any(r => r.AppointmentId == id);

            if (hasReviews)
            {
                // Display warning message
                TempData["WarningMessage"] = "Cannot delete appointment with associated reviews. Delete reviews first.";
                return RedirectToAction(nameof(Index));
            }

            return View(appointments);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Appointments == null)
            {
                return Problem("Entity set 'AppDbContext.Appointments' is null.");
            }

            // Check if there are associated reviews
            var hasReviews = _context.Review.Any(r => r.AppointmentId == id);

            if (hasReviews)
            {
                // Display warning message
                TempData["WarningMessage"] = "Cannot delete appointment with associated reviews. Delete reviews first.";
                return RedirectToAction(nameof(Index));
            }

            var appointments = await _context.Appointments.FindAsync(id);
            if (appointments != null)
            {
                _context.Appointments.Remove(appointments);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool AppointmentsExists(int id)
        {
            return (_context.Appointments?.Any(e => e.AppointmentId == id)).GetValueOrDefault();
        }
    }
}
