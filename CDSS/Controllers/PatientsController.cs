using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CDSS.Models;
using CDSS.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace CDSS.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Patients/Index
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            if (_context != null && _context.Patients != null)
            {
                var patients = _context.Patients.AsQueryable();

                // Filter patients based on the search term in FirstName, LastName, or MedicalCondition
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    patients = patients.Where(p =>
                        p.FirstName.Contains(searchTerm) ||
                        p.LastName.Contains(searchTerm) ||
                        (p.MedicalCondition != null && p.MedicalCondition.Contains(searchTerm)) ||
                        (p.Ward != null && p.Ward.Contains(searchTerm))
                    );
                }

                return View(await patients.ToListAsync());
            }
            else
            {
                return Problem("AppDbContext or its entity set 'Patients' is null.");
            }
        }





        //// GET: Patients
        //public async Task<IActionResult> Index()
        //{
        //    if (_context != null && _context.Patients != null)
        //    {
        //        return View(await _context.Patients.ToListAsync());
        //    }
        //    else
        //    {
        //        return Problem("AppDbContext or its entity set 'Patients' is null.");
        //    }
        //}

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
            // Log or display validation errors
            foreach (var modelState in ModelState.Values)
            {
                if (modelState.ValidationState == ModelValidationState.Invalid)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }



            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            // Get the distinct medical conditions from the existing data
            var allMedicalConditions = _context.Patients
                 .Where(p => !string.IsNullOrEmpty(p.MedicalCondition))
                 .Select(p => (p.MedicalCondition ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
                 .AsEnumerable() // Materialize the query on the server side
                 .SelectMany(splitConditions => splitConditions)
                 .Select(trimmedCondition => trimmedCondition.Trim())
                 .Where(trimmedCondition => !string.IsNullOrEmpty(trimmedCondition))
                 .Distinct()
                 .ToArray();

            ViewBag.AllMedicalConditions = allMedicalConditions;


            return View();
        }



        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId, FirstName, LastName, Birthdate, Ward, Bed, Weight, BloodType, MedicalCondition")] Patients patients, string[] MedicalCondition)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set the MedicalCondition property using the array
                    patients.MedicalCondition = string.Join(",", MedicalCondition);

                    _context.Add(patients);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    // Handle the exception appropriately
                    return View("Error");
                }
            }

            // Repopulate ViewBag.MedicalConditions in case of validation errors
            ViewBag.MedicalConditions = _context.Patients
                .Select(p => p.MedicalCondition)
                .ToList();

            return View(patients);
        }




        // GET: Patients/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = _context.Patients.Find(id);

            if (patient == null)
            {
                return NotFound();
            }

            // Get all distinct medical conditions from the existing data
            var allMedicalConditions = _context.Patients
                .Where(p => !string.IsNullOrEmpty(p.MedicalCondition))
                .Select(p => (p.MedicalCondition ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
                .AsEnumerable() // Materialize the query on the server side
                .SelectMany(splitConditions => splitConditions)
                .Select(trimmedCondition => trimmedCondition.Trim())
                .Where(trimmedCondition => !string.IsNullOrEmpty(trimmedCondition))
                .Distinct()
                .ToArray();

            ViewBag.AllMedicalConditions = allMedicalConditions;

            // Split the MedicalCondition of the patient into an array for pre-selection
            var selectedConditions = (patient.MedicalCondition ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)?.Select(c => c.Trim()) ?? Array.Empty<string>();

            // Pass the selected conditions as an array to the view
            ViewBag.SelectedMedicalConditions = selectedConditions;


            return View(patient);


        }



        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId, FirstName, LastName, Birthdate, Ward, Bed, Weight, BloodType, MedicalCondition")] Patients patients, string[] MedicalCondition)
        {
            if (id != patients.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure unique medical conditions in the array
                    var uniqueMedicalConditions = MedicalCondition?.Distinct().ToArray() ?? Array.Empty<string>();

                    // Set the MedicalCondition property using the array
                    patients.MedicalCondition = string.Join(",", uniqueMedicalConditions);

                    _context.Update(patients);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    // Handle the exception appropriately
                    return View("Error");
                }
            }

            // Repopulate ViewBag.MedicalConditions in case of validation errors
            ViewBag.MedicalConditions = _context.Patients
                .Select(p => p.MedicalCondition)
                .ToList();

            ViewBag.SelectedMedicalConditions = MedicalCondition; // Pass selected conditions back to the view

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