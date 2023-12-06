using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
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

            //ViewBag.
            return View(patientMedication);
        }

        // GET: PatientMedications/Create
        public IActionResult Create(int patientId)
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

            ViewData["patientId"] = patientId;

            return View();
        }

        // POST: PatientMedications/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("PatientMedicationID,PatientId,MedicationId,Dosage,Frequency,Duration,StartMedication,EndMedication")] PatientMedication patientMedication)
        {
            if (ModelState.IsValid)
            {
                if (patientMedication.EndMedication < patientMedication.StartMedication)
                {
                    ModelState.AddModelError("EndMedication", "End Medication cannot be earlier than Start Medication");
                    var medications = _context.Medication.ToList();
                    ViewBag.MedicationName = new SelectList(medications, "MedicationId", "MedicationName");
                    return View(patientMedication);
                }

                _context.Add(patientMedication);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Patients", new { id = patientMedication.PatientId });
            }

            var meds = _context.Medication.ToList();
            ViewBag.MedicationName = new SelectList(meds, "MedicationId", "MedicationName");

            return View(patientMedication);
        }





        // GET: PatientMedications1/Edit/5
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
            ViewData["MedicationId"] = new SelectList(_context.Medication, "MedicationId", "MedicationName", patientMedication.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", patientMedication.PatientId);
            return View(patientMedication);
        }

        // POST: PatientMedications1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: PatientMedications1/Edit/5
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
                if (patientMedication.EndMedication < patientMedication.StartMedication)
                {
                    ModelState.AddModelError("EndMedication", "End Medication cannot be earlier than Start Medication");
                    ViewData["MedicationId"] = new SelectList(_context.Medication, "MedicationId", "MedicationId", patientMedication.MedicationId);
                    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", patientMedication.PatientId);
                    return View(patientMedication);
                }

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

            ViewData["MedicationId"] = new SelectList(_context.Medication, "MedicationId", "MedicationId", patientMedication.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", patientMedication.PatientId);
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
                await _context.SaveChangesAsync();

                // Assuming patientMedication.PatientId holds the patient's ID
                return RedirectToAction("Details", "Patients", new { id = patientMedication.PatientId });
            }

            return RedirectToAction(nameof(Index)); // Or handle appropriately if patientMedication is null
        }

        private bool PatientMedicationExists(int id)
        {
            return (_context.PatientMedication?.Any(e => e.PatientMedicationID == id)).GetValueOrDefault();
        }
    }
}
