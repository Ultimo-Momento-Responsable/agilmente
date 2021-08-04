using Assets.Resources.Scripts.Classes.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.Classes.HayUnoRepetido
{
    class HayUnoRepetidoSession : IGameSession
    {
        private long a_id;
        private FigureQuantity a_figureQuantity;
        private MaximumTime a_maximumTime;
        private List<HayUnoRepetidoResult> a_results;
        private Game a_game;

        public HayUnoRepetidoSession(long id, FigureQuantity figureQuantity, MaximumTime maximumTime, List<HayUnoRepetidoResult> results, Game game)
        {
            this.id = id;
            this.figureQuantity = figureQuantity;
            this.maximumTime = maximumTime;
            this.results = results;
            this.game = game;
        }

        public long id { get => a_id; set => a_id = value; }
        public List<IParam> parameters { 
            get => new List<IParam>() { figureQuantity, maximumTime };
            set {
                value.ForEach(parameter => { });
            }
        }
        internal FigureQuantity figureQuantity { get => a_figureQuantity; set => a_figureQuantity = value; }
        internal MaximumTime maximumTime { get => a_maximumTime; set => a_maximumTime = value; }
        internal List<HayUnoRepetidoResult> results { get => a_results; set => a_results = value; }
        internal Game game { get => a_game; set => a_game = value; }

        public void initialize()
        {
            throw new NotImplementedException();
        }
    }
}
