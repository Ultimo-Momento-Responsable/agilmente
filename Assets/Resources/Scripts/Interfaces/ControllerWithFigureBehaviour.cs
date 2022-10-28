using UnityEngine;

namespace Assets.Resources.Scripts
{
    public interface ControllerWithFigureBehaviour
    {
        public GameObject Particles { get; }
        public bool IsTouching { get; set; }
        public bool IsMakingMistake { get; set; }
        public FlexibleGameGrid Grid { get; }
        public GameWithFigureBehaviour Game { get; }
        public Camera MainCamera { get; }
        public bool VariableSizes { get; }
    }
}