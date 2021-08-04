using Assets.Resources.Scripts.Classes.HayUnoRepetido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.Classes
{
    class PlanningDetail
    {
        private long a_id;
        private HayUnoRepetidoSession a_hayUnoRepetidoSession;
        private int a_maxNumberOfSessions;

        public PlanningDetail(long id, int maxNumberOfSessions, HayUnoRepetidoSession hayUnoRepetidoSession)
        {
            this.id = id;
            this.maxNumberOfSessions = maxNumberOfSessions;
            this.hayUnoRepetidoSession = hayUnoRepetidoSession;
        }

        public long id { get => a_id; set => a_id = value; }
        public int maxNumberOfSessions { get => a_maxNumberOfSessions; set => a_maxNumberOfSessions = value; }
        internal HayUnoRepetidoSession hayUnoRepetidoSession { get => a_hayUnoRepetidoSession; set => a_hayUnoRepetidoSession = value; }
    }
}
