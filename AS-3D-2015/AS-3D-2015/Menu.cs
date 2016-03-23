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
        SpriteFont spriteFont { get; set; }
        Color couleur { get; set; }
        Texture2D FondEcran { get; set; }
        Rectangle DimensionEcran { get; set; }
        public Menu(Game game)
            : base(game)
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionnaireDeSpriteFont = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            spriteFont = GestionnaireDeSpriteFont.Find("menu");
            buttonList.Add(new Bouton(Game, Color.Blue, Color.White,0, "Un Joueur", spriteFont, TypeBouton.UN_JOUEUR));
            buttonList.Add(new Bouton(Game, Color.Blue, Color.White, 1, "Deux Joueurs (Réseau)", spriteFont, TypeBouton.DEUX_JOUEUR));
            buttonList.Add(new Bouton(Game, Color.Blue, Color.White, 2, "Utiliser la manette Xbox 360", spriteFont, TypeBouton.CONTROLE));
            buttonList.Add(new Bouton(Game, Color.Blue, Color.White, 3, "Quitter", spriteFont, TypeBouton.QUITTER));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            foreach (Bouton b in buttonList)
            {
                Game.Components.Add(b);
            }
            GestionSprite = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            FondEcran = GestionnaireDeTextures.Find("BackgroundTexture");
            DimensionEcran = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
