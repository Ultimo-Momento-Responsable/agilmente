using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.Classes
{
    class Planning
    {
        private long a_id;
        private Professional a_professional;
        private Patient a_patient;
        private DateTime a_creationDatetime;
        private DateTime a_startDate;
        private DateTime a_dueTime;
        private List<PlanningDetail> a_detail;

        public Planning(long id, DateTime creationDatetime, DateTime startDate, DateTime dueTime, Professional professional, Patient patient, List<PlanningDetail> detail)
        {
            this.id = id;
            this.creationDatetime = creationDatetime;
            this.startDate = startDate;
            this.dueTime = dueTime;
            this.professional = professional;
            this.patient = patient;
            this.detail = detail;
        }

        public long id { get => a_id; set => a_id = value; }
        public DateTime creationDatetime { get => a_creationDatetime; set => a_creationDatetime = value; }
        public DateTime startDate { get => a_startDate; set => a_startDate = value; }
        public DateTime dueTime { get => a_dueTime; set => a_dueTime = value; }
        internal Professional professional { get => a_professional; set => a_professional = value; }
        internal Patient patient { get => a_patient; set => a_patient = value; }
        internal List<PlanningDetail> detail { get => a_detail; set => a_detail = value; }
    }
}
