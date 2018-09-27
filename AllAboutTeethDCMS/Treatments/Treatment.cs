using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Treatments
{
    public class Treatment : ModelBase
    {
        private int no = -1;
        private string name = "";
        private string description = "";

        private bool decayedCariesIndicatedForFilling = false;
        private bool missingDueToCaries = false;
        private bool filled = false;
        private bool cariesIndicatedForExtraction = false;
        private bool rootFragment = false;
        private bool missingDueToOtherCauses = false;
        private bool impactedTooth = false;
        private bool jacketCrown = false;
        private bool amalgamFilling = false;
        private bool abutment = false;
        private bool pontic = false;
        private bool inlay = false;
        private bool fixedCureComposite = false;
        private bool removableDenture = false;
        private bool extractionDueToCaries = false;
        private bool extractionDueToOtherCauses = false;
        private bool presentTeeth = false;
        private bool congenitallyMissing = false;
        private bool supernumerary = false;

        private string output = "None";

        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;
        private string status = "Active";

        public int No { get => no; set => no = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }

        public bool DecayedCariesIndicatedForFilling { get => decayedCariesIndicatedForFilling; set => decayedCariesIndicatedForFilling = value; }
        public bool MissingDueToCaries { get => missingDueToCaries; set => missingDueToCaries = value; }
        public bool Filled { get => filled; set => filled = value; }
        public bool CariesIndicatedForExtraction { get => cariesIndicatedForExtraction; set => cariesIndicatedForExtraction = value; }
        public bool RootFragment { get => rootFragment; set => rootFragment = value; }
        public bool MissingDueToOtherCauses { get => missingDueToOtherCauses; set => missingDueToOtherCauses = value; }
        public bool ImpactedTooth { get => impactedTooth; set => impactedTooth = value; }
        public bool JacketCrown { get => jacketCrown; set => jacketCrown = value; }
        public bool AmalgamFilling { get => amalgamFilling; set => amalgamFilling = value; }
        public bool Abutment { get => abutment; set => abutment = value; }
        public bool Pontic { get => pontic; set => pontic = value; }
        public bool Inlay { get => inlay; set => inlay = value; }
        public bool FixedCureComposite { get => fixedCureComposite; set => fixedCureComposite = value; }
        public bool RemovableDenture { get => removableDenture; set => removableDenture = value; }
        public bool ExtractionDueToCaries { get => extractionDueToCaries; set => extractionDueToCaries = value; }
        public bool ExtractionDueToOtherCauses { get => extractionDueToOtherCauses; set => extractionDueToOtherCauses = value; }
        public bool PresentTeeth { get => presentTeeth; set => presentTeeth = value; }
        public bool CongenitallyMissing { get => congenitallyMissing; set => congenitallyMissing = value; }
        public bool Supernumerary { get => supernumerary; set => supernumerary = value; }

        public string Output { get => output; set => output = value; }
        public int Duration { get; set; }

        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
        public string Status { get => status; set => status = value; }

        public override string ToString()
        {
            return Name;
        }
    }
}
