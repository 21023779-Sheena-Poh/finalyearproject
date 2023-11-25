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

        /*// GET: PatientMedications/PrescriptionHistory/5
        public async Task<IActionResult> PrescriptionHistory(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patientMedications = await _context.PatientMedication
                .Include(pm => pm.Medication) // Include Medication information
                .Where(pm => pm.PatientId == id)
                .ToListAsync();

            if (patientMedications == null)
            {
                return NotFound();
            }

            // Display the Prescription History view with the patient's medications
            return View("PrescriptionHistory", patientMedications);
        }*/

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
                .Include(pm => pm.Patient)       // Include Patient navigation property
                .Include(pm => pm.Medication)    // Include Medication navigation property
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
            var medications = _context.Medication.ToList();

            if (medications != null)
            {
                ViewBag.MedicationName = new SelectList(medications, "MedicationId", "MedicationName");
            }
            else
            {
                ViewBag.MedicationName = new SelectList(new List<Medication>(), "MedicationId", "MedicationName");
            }

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
            var medications = _context.Medication.ToList();
            if (medications != null)
            {
                ViewBag.MedicationName = new SelectList(_context.Medication.ToList(), "MedicationId", "MedicationName");
            }
            else
            {
                ViewBag.MedicationName = new SelectList(new List<Medication>(), "MedicationId", "MedicationName");
            }
            return View(medications);
        }


        // GET: PatientMedications/Edit/5
        public IActionResult Edit (int id)
        {
            // Retrieve the PatientMedication entity from the database based on the provided id
            var patientMedication = _context.PatientMedication.FirstOrDefault(p => p.PatientMedicationID == id);

            if (patientMedication == null)
            {
                ViewBag.MedicationName = new SelectList(_context.Medication.ToList(), "MedicationId", "MedicationName");
            }

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
                return RedirectToAction("Details", "Patients", new { id = patientMedication.PatientId });
            }
            var medications = _context.Medication.ToList();
            if (medications != null)
            {
                ViewBag.MedicationName = new SelectList(_context.Medication.ToList(), "MedicationId", "MedicationName");
            }
            else
            {
                ViewBag.MedicationName = new SelectList(new List<Medication>(), "MedicationId", "MedicationName");
            }
            return View(medications);
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
