using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class PatientMedication

{
    public int PatientMedicationID { get; set; }

    public int PatientId { get; set; }

    public int MedicationId { get; set; }

    public string? Dosage { get; set; }

    public string? Frequency { get; set; }

    public string? Duration { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartMedication { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndMedication { get; set; }

    [ValidateNever]
    public virtual Medication Medication { get; set; } = null!;

    public string MedicationName
    {
        get
        {
            if (Medication != null && Medication.MedicationName != null)
            {
                return Medication.MedicationName;
            }
            return "DefaultMedicationName"; // Or any default value you prefer
        }
    }

    [ValidateNever]
    public virtual Patients Patient { get; set; } = null!;

}
