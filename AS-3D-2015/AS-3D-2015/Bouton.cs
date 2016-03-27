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
    public enum TypeBouton {UN_JOUEUR, DEUX_JOUEUR, CONTROLE, QUITTER}
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Bouton : Microsoft.Xna.Framework.DrawableGameComponent, IMenu
    {
        const int NB_BOUTON = 4;
        Color CouleurClic { get; set; }
        Color CouleurNonClic { get; set; }
        Color Couleur { get; set; }
        int Indice { get; set; }
        String Texte { get; set; }
        SpriteFont Font { get; set; }
        Vector2 Position { get; set; }
        Rectangle RectangleBouton { get; set; }
        InputManager GestionInput { get; set; }
        SpriteBatch GestionSprite { get; set; }
        TypeBouton TypeBouton { get; set; }
        
        public Bouton(Game game, Color couleurClic, Color couleurNonClic, int indice, string texte, SpriteFont font, TypeBouton typeBouton)
            : base(game)
        {
            GestionSprite = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CouleurClic = couleurClic;
            CouleurNonClic = couleurNonClic;
            Indice = indice;
            Texte = texte;
            Font = font;
            TypeBouton = typeBouton;
        }

        public override void Initialize()
        {
            Position = new Vector2((Game.Window.ClientBounds.Width / 2) - Font.MeasureString(Texte).X / 2,
                    (Game.Window.ClientBounds.Height / 2) - Font.LineSpacing * NB_BOUTON + ((Font.LineSpacing + 5) * Indice));

            RectangleBouton = new Rectangle((int)Position.X, (int)Position.Y, (int)Font.MeasureString(Texte).X, (int)Font.MeasureString(Texte).Y);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Couleur = ContientSouris()? CouleurClic:CouleurNonClic;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            GestionSprite.DrawString(Font, Texte, Position, Couleur);

            base.Draw(gameTime);
        }

        public bool ContientSouris()
        {
            return RectangleBouton.Contains(GestionInput.GetPositionSouris());
        }

        public void Clic()
        {
            switch (TypeBouton)
            {
                case TypeBouton.UN_JOUEUR:
                    Game.IsMouseVisible = false;
                    foreach (GameComponent gc in Game.Components.Where(x => x is IGame))
                    {
                        gc.Enabled = true;
                        if (gc is DrawableGameComponent)
                        {
                            (gc as DrawableGameComponent).Visible = true;
                        }
                    }
                    foreach (GameComponent gc in Game.Components.Where(x => x is IMenu))
                    {
                        gc.Enabled = false;
                        if (gc is DrawableGameComponent)
                        {
                            (gc as DrawableGameComponent).Visible = false;
                        }
                    }

                    foreach (GameComponent gc in Game.Components.Where(x => x is Server))
                    {
                        Game.Components.Remove(gc);
                    }
                    break;

                case TypeBouton.DEUX_JOUEUR:
                    Game.IsMouseVisible = false;
                    foreach (GameComponent gc in Game.Components.Where(x => x is IGame))
                    {
                        gc.Enabled = true;
                        if (gc is DrawableGameComponent)
                        {
                            (gc as DrawableGameComponent).Visible = true;
                        }
                    }
                    foreach (GameComponent gc in Game.Components.Where(x => x is IMenu))
                    {
                        gc.Enabled = false;
                        if (gc is DrawableGameComponent)
                        {
                            (gc as DrawableGameComponent).Visible = false;
                        }
                    }
                    break;

                case TypeBouton.CONTROLE:
                    Texte = (Texte == "Utiliser la manette Xbox 360") ? "Utiliser la souris et le clavier" : "Utiliser la manette Xbox 360";
                    Position = new Vector2((Game.Window.ClientBounds.Width / 2) - Font.MeasureString(Texte).X / 2,
                    (Game.Window.ClientBounds.Height / 2) - Font.LineSpacing * NB_BOUTON + ((Font.LineSpacing + 5) * Indice));
                    RectangleBouton = new Rectangle((int)Position.X, (int)Position.Y, (int)Font.MeasureString(Texte).X, (int)Font.MeasureString(Texte).Y);
                    break;

                case TypeBouton.QUITTER:
                    Game.Exit();
                    break;
            }
        }
    }
}
