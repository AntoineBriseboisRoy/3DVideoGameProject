using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace AtelierXNA
{
    public class LampeTorche : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const float BATTERIE_MAX = 1;
        const float SEUIL_BATTERIE_PLEINE = 0.75f;
        const float SEUIL_BATTERIE_MOYEN = 0.30f;
        const float SEUIL_BATTERIE_CRITIQUE = 0.10f;
        public float Intensit�Initiale { get; private set; }
        float TempsBatterie { get; set; }
        public float Intensit�Batterie { get; private set; }
        Texture2D Ic�ne { get; set; }
        Texture2D BatteriePleine { get; set; }
        Texture2D BatterieMoyenne { get; set; }
        Texture2D BatterieCritique { get; set; }
        Texture2D BatterieVide { get; set; }
        SpriteBatch GestionSprite { get; set; }
        Rectangle Dimensions { get; set; }
        float Dur�eDeVieBatterie { get; set; }
        public float Temps�coul�eDepuisBatterieTrouv�e { get; set; } //V�RIF SI SET >=0
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        public LampeTorche(Game game, float intensit�, float dur�eDeVieBatterie)
            : base(game)
        {
            Intensit�Initiale = intensit�;
            Dur�eDeVieBatterie = dur�eDeVieBatterie;
            Temps�coul�eDepuisBatterieTrouv�e = 0;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionSprite = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            BatteriePleine = GestionnaireDeTextures.Find("BatteriePleine");
            BatterieMoyenne = GestionnaireDeTextures.Find("BatterieMoyen");
            BatterieCritique = GestionnaireDeTextures.Find("BatterieCritique");
            BatterieVide = GestionnaireDeTextures.Find("BatterieVide");
            Ic�ne = BatteriePleine;
            Dimensions = new Rectangle(Game.Window.ClientBounds.Width - Ic�ne.Width, 0, Ic�ne.Width, Ic�ne.Height);
            
        }
        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�eDepuisBatterieTrouv�e += temps�coul�;

            if (Dur�eDeVieBatterie >= Temps�coul�eDepuisBatterieTrouv�e)
            {
                TempsBatterie = Dur�eDeVieBatterie - Temps�coul�eDepuisBatterieTrouv�e;
            }

            if (Dur�eDeVieBatterie < Temps�coul�eDepuisBatterieTrouv�e)
            {
                TempsBatterie = 0;
                Ic�ne = BatterieVide;
            }

            else
            {
                if (TempsBatterie >= Dur�eDeVieBatterie * SEUIL_BATTERIE_PLEINE)
                {
                    Ic�ne = BatteriePleine;
                }
                else if (TempsBatterie >= Dur�eDeVieBatterie * SEUIL_BATTERIE_MOYEN)
                {
                    Ic�ne = BatterieMoyenne;
                }

                else if (TempsBatterie >= Dur�eDeVieBatterie * SEUIL_BATTERIE_CRITIQUE)
                {
                    Ic�ne = BatterieCritique;
                }
                else
                {
                    Ic�ne = BatterieVide;
                }

            }

            Intensit�Batterie = Intensit�Initiale * TempsBatterie / Dur�eDeVieBatterie;

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GestionSprite.Draw(Ic�ne, Dimensions, Color.White);
        }
    }
}
