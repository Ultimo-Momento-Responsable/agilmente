using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.Classes.HayUnoRepetido
{
    class HayUnoRepetidoResult
    {
        private long a_id;
        private DateTime a_completeDatetime;
        private bool a_canceled;
        private int a_mistakes;
        private int a_successes;
        private float[] a_timeBetweenSuccesses;
        private float a_totalTime;

        public HayUnoRepetidoResult(long id, DateTime completeDatetime, bool canceled, int mistakes, int successes, float[] timeBetweenSuccesses, float totalTime)
        {
            this.id = id;
            this.completeDatetime = completeDatetime;
            this.canceled = canceled;
            this.mistakes = mistakes;
            this.successes = successes;
            this.timeBetweenSuccesses = timeBetweenSuccesses;
            this.totalTime = totalTime;
        }

        public long id { get => a_id; set => a_id = value; }
        public DateTime completeDatetime { get => a_completeDatetime; set => a_completeDatetime = value; }
        public bool canceled { get => a_canceled; set => a_canceled = value; }
        public int mistakes { get => a_mistakes; set => a_mistakes = value; }
        public int successes { get => a_successes; set => a_successes = value; }
        public float[] timeBetweenSuccesses { get => a_timeBetweenSuccesses; set => a_timeBetweenSuccesses = value; }
        public float totalTime { get => a_totalTime; set => a_totalTime = value; }
    }
}
