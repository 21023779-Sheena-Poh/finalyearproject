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
    public class PatientMedicationsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientMedicationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PatientMedications
        public async Task<IActionResult> Index()
        {
            var patientMedications = await _context.PatientMedication
                .Include(pm => pm.Patient)       // Include Patient navigation property
                .Include(pm => pm.Medication)    // Include Medication navigation property
                .ToListAsync();

            return View(patientMedications);
        }

        // GET: PatientMedications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PatientMedication == null)
            {
                return NotFound();
            }

            var patientMedication = await _context.PatientMedication
                .Include(p => p.Medication)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientMedicationID == id);
            if (patientMedication == null)
            {
                return NotFound();
            }

            return View(patientMedication);
        }

        // GET: PatientMedications/Create
        public IActionResult Create()
        {
            ViewData["MedicationId"] = new SelectList(_context.Medication, "MedicationId", "MedicationId");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
            return View();
        }

        // POST: PatientMedications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientMedicationID,PatientId,MedicationId,Dosage,Frequency,Duration,StartMedication,EndMedication")] PatientMedication patientMedication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientMedication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Only include properties from PatientMedication in ViewData
            ViewData["MedicationId"] = new SelectList(_context.Medication, "MedicationId", "MedicationId", patientMedication.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", patientMedication.PatientId);
            return View(patientMedication);
        }


        // GET: PatientMedications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PatientMedication == null)
            {
                return NotFound();
            }

            var patientMedication = await _context.PatientMedication.FindAsync(id);
            if (patientMedication == null)
            {
                return NotFound();
            }
            ViewData["MedicationId"] = new SelectList(_context.Medication, "MedicationId", "MedicationId", patientMedication.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", patientMedication.PatientId);
            return View(patientMedication);
        }

        // POST: PatientMedications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientMedicationID,PatientId,MedicationId,Dosage,Frequency,Duration,StartMedication,EndMedication")] PatientMedication patientMedication)
        {
            if (id != patientMedication.PatientMedicationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientMedication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientMedicationExists(patientMedication.PatientMedicationID))
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
            ViewData["MedicationId"] = new SelectList(_context.Medication, "MedicationId", "MedicationId", patientMedication.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", patientMedication.PatientId);
            return View(patientMedication);
        }

        // GET: PatientMedications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PatientMedication == null)
            {
                return NotFound();
            }

            var patientMedication = await _context.PatientMedication
                .Include(p => p.Medication)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientMedicationID == id);
            if (patientMedication == null)
            {
                return NotFound();
            }

            return View(patientMedication);
        }

        // POST: PatientMedications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PatientMedication == null)
            {
                return Problem("Entity set 'AppDbContext.PatientMedication' is null.");
            }
            var patientMedication = await _context.PatientMedication.FindAsync(id);
            if (patientMedication != null)
            {
                _context.PatientMedication.Remove(patientMedication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientMedicationExists(int id)
        {
            return (_context.PatientMedication?.Any(e => e.PatientMedicationID == id)).GetValueOrDefault();
        }
    }
}
