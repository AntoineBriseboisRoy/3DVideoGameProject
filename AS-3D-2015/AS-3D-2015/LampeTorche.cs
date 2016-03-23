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
        public float IntensitéInitiale { get; private set; }
        float TempsBatterie { get; set; }
        public float IntensitéBatterie { get; private set; }
        Texture2D Icône { get; set; }
        Texture2D BatteriePleine { get; set; }
        Texture2D BatterieMoyenne { get; set; }
        Texture2D BatterieCritique { get; set; }
        Texture2D BatterieVide { get; set; }
        SpriteBatch GestionSprite { get; set; }
        Rectangle Dimensions { get; set; }
        float DuréeDeVieBatterie { get; set; }
        public float TempsÉcouléeDepuisBatterieTrouvée { get; set; } //VÉRIF SI SET >=0
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        public LampeTorche(Game game, float intensité, float duréeDeVieBatterie)
            : base(game)
        {
            IntensitéInitiale = intensité;
            DuréeDeVieBatterie = duréeDeVieBatterie;
            TempsÉcouléeDepuisBatterieTrouvée = 0;
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
            Icône = BatteriePleine;
            Dimensions = new Rectangle(Game.Window.ClientBounds.Width - Icône.Width, 0, Icône.Width, Icône.Height);
            
        }
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléeDepuisBatterieTrouvée += tempsÉcoulé;

            if (DuréeDeVieBatterie >= TempsÉcouléeDepuisBatterieTrouvée)
            {
                TempsBatterie = DuréeDeVieBatterie - TempsÉcouléeDepuisBatterieTrouvée;
            }

            if (DuréeDeVieBatterie < TempsÉcouléeDepuisBatterieTrouvée)
            {
                TempsBatterie = 0;
                Icône = BatterieVide;
            }

            else
            {
                if (TempsBatterie >= DuréeDeVieBatterie * SEUIL_BATTERIE_PLEINE)
                {
                    Icône = BatteriePleine;
                }
                else if (TempsBatterie >= DuréeDeVieBatterie * SEUIL_BATTERIE_MOYEN)
                {
                    Icône = BatterieMoyenne;
                }

                else if (TempsBatterie >= DuréeDeVieBatterie * SEUIL_BATTERIE_CRITIQUE)
                {
                    Icône = BatterieCritique;
                }
                else
                {
                    Icône = BatterieVide;
                }

            }

            IntensitéBatterie = IntensitéInitiale * TempsBatterie / DuréeDeVieBatterie;

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GestionSprite.Draw(Icône, Dimensions, Color.White);
        }
    }
}
