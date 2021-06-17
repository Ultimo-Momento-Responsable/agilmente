using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace Tests
{
    public class HayUnoRepetidoUnitTests
    {
        HayUnoRepetido juego;
        public Sprite[] sprites;

        [Test]
        public void HayUnoRepetidoUnitTestsChooseSprites()
        {
            juego = ScriptableObject.CreateInstance<HayUnoRepetido>();
            sprites = Resources.LoadAll<Sprite>("Sprites/Figures/");
            List<int> index = juego.chooseSprites(sprites, 6);
            Assert.AreEqual(index.Count,6);
        }

        [Test]
        public void HayUnoRepetidoUnitTestsCenterFigures()
        {
            juego = ScriptableObject.CreateInstance<HayUnoRepetido>();
            Vector2 v2 = juego.centerFigures(new Vector2(-1,2));
            Vector2 comparacion = new Vector2(-0.5f, 1.5f);
            Assert.AreEqual(v2, comparacion);
        }

        [Test]
        public void HayUnoRepetidoUnitTestsThereIsSomethingIn()
        {
            juego = ScriptableObject.CreateInstance<HayUnoRepetido>();
            Assert.IsFalse(juego.thereIsSomethingIn(new Vector2(0, 0)));
        }
    }
}
