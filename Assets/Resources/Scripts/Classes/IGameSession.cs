using Assets.Resources.Scripts.Classes.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.Classes
{
    interface IGameSession
    {
        /// <summary>
        /// Lista de parámetros de la sesión de juego.
        /// </summary>
        public List<IParam> parameters { get; set; }
        
        /// <summary>
        /// Inicializa el juego con sus respectivos parámetros.
        /// </summary>
        public void initialize();
    }
}
