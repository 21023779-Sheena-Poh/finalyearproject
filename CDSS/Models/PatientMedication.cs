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

    public DateTime? StartMedication { get; set; }

    public DateTime? EndMedication { get; set; }

    public virtual Medication Medication { get; set; } = null!;

    public string MedicationName => $"{MedicationName}";

    public virtual Patients Patient { get; set; } = null!;

}
