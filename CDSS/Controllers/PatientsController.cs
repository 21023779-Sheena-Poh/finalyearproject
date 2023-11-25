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
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            return _context.Patients != null ?
                        View(await _context.Patients.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Patients'  is null.");
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            var prescriptions = await _context.PatientMedication
                .Where(pm => pm.PatientId == id)
                .Include(pm => pm.Medication)
                .ToListAsync();

            ViewData["prescriptions"] = prescriptions;

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId, FirstName, LastName, Birthdate, Ward, Bed, Weight, BloodType, MedicalCondition")] Patients patients)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patients);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patients);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patients = await _context.Patients.FindAsync(id);
            if (patients == null)
            {
                return NotFound();
            }
            return View(patients);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,Birthdate,Ward,Bed,Weight,BloodType,MedicalCondition")] Patients patients)
        {
            if (id != patients.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patients);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientsExists(patients.PatientId))
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
            return View(patients);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patients = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patients == null)
            {
                return NotFound();
            }

            return View(patients);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'AppDbContext.Patients'  is null.");
            }
            var patients = await _context.Patients.FindAsync(id);
            if (patients != null)
            {
                _context.Patients.Remove(patients);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientsExists(int id)
        {
            return (_context.Patients?.Any(e => e.PatientId == id)).GetValueOrDefault();
        }
    }
}
