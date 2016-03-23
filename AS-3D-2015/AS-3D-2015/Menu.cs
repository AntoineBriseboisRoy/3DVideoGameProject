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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Menu : Microsoft.Xna.Framework.DrawableGameComponent, IMenu
    {
        List<Bouton> buttonList = new List<Bouton>();
        SpriteBatch GestionSprite { get; set; }
        InputManager GestionInput { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeSpriteFont { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        SpriteFont Police { get; set; }
        Color couleur { get; set; }
        Texture2D FondEcran { get; set; }
        Rectangle DimensionEcran { get; set; }
        public Menu(Game game)
            : base(game)
        {

        }


        public override void Initialize()
        {
            DimensionEcran = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionnaireDeSpriteFont = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            Police = GestionnaireDeSpriteFont.Find("menu");
            buttonList.Add(new Bouton(Game, Color.Green, Color.White, 0, "Un Joueur", Police, TypeBouton.UN_JOUEUR));
            buttonList.Add(new Bouton(Game, Color.Green, Color.White, 1, "Deux Joueurs (Réseau)", Police, TypeBouton.DEUX_JOUEUR));
            buttonList.Add(new Bouton(Game, Color.Green, Color.White, 2, "Utiliser la manette Xbox 360", Police, TypeBouton.CONTROLE));
            buttonList.Add(new Bouton(Game, Color.Green, Color.White, 3, "Quitter", Police, TypeBouton.QUITTER));

            GestionSprite = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            FondEcran = GestionnaireDeTextures.Find("BgMenu");

            foreach (Bouton b in buttonList)
            {
                Game.Components.Add(b);
            }

        }
        public override void Update(GameTime gameTime)
        {
            if (GestionInput.EstSourisActive)
            {
                for(int i = 0; i < buttonList.Count;++i)
                {
                    if(GestionInput.EstNouveauClicGauche() && !GestionInput.EstAncienClicGauche() && buttonList[i].ContientSouris())
                    {
                        buttonList[i].Clic();
                    }
                }
            }

            // Code de merde plein de bugs :
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GestionSprite.Draw(FondEcran,DimensionEcran, Color.White);
        }
    }
}
