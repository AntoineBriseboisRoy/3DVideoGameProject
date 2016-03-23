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
    enum GameState { MENU, GAME }
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        const int PORT = 5001;
        const string IP = "127.0.0.1";
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float HAUTEUR_ZOMBIE = -2.5f;
        const float INTENSITÉ_LUMINEUSE = 5f;
        const float RAYON_LUMIÈRE = 6f;
        const int DIMENSION_TERRAIN = 256;

        public bool EstManetteUtilisée { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        Carte CarteJeu { get; set; }
        GameState ÉtatJeu { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        RessourcesManager<Effect> GestionnaireDeShaders { get; set; }
        RessourcesManager<SoundEffect> GestionnaireDeSoundEffect { get; set; }
        RessourcesManager<Song> GestionnaireMusique { get; set; }
        CollisionManager GestionnaireDeCollisions { get; set; }
        public InputManager GestionInput { get; private set; }
        public SoundManager GestionSounds { get; private set; }
        Caméra CaméraJeu { get; set; }
        Server Serveur { get; set; }
        Lumière LumièreObjet { get; set; }
        GrilleCollision GrilleDeJeu { get; set; }

        public Atelier()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            PériphériqueGraphique.IsFullScreen = false;
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            ÉtatJeu = GameState.MENU;
            EstManetteUtilisée = false;

            Vector3 positionCaméra = new Vector3(0, 20, 125);
            Vector3 cibleCaméra = new Vector3(0, 0, 0);
            LumièreObjet = new Lumière(this, positionCaméra, Vector3.One, RAYON_LUMIÈRE, INTENSITÉ_LUMINEUSE, Vector3.One, Vector4.One / 10);
            GrilleDeJeu = new GrilleCollision(this, new Vector2(DIMENSION_TERRAIN, DIMENSION_TERRAIN), 5.22449f / 2f, "Labyrinthe", 0);
            CarteJeu = new Carte(this, new Vector3(DIMENSION_TERRAIN, 25, DIMENSION_TERRAIN), "Labyrinthe", LumièreObjet);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionnaireDeShaders = new RessourcesManager<Effect>(this, "Effects");
            GestionnaireDeSoundEffect = new RessourcesManager<SoundEffect>(this, "Sounds");
            GestionnaireMusique = new RessourcesManager<Song>(this, "Songs");
            GestionSprites = new SpriteBatch(GraphicsDevice);
            GestionInput = new InputManager(this);
            GestionSounds = new SoundManager(this);
            GestionnaireDeCollisions = new CollisionManager(this);
            CaméraJeu = new Caméra(this);
            Serveur = new Server(this, PORT);
            
            Components.Add(GestionSounds);
            Components.Add(GestionInput);
            Components.Add(CaméraJeu);
            Components.Add(new Afficheur3D(this));
            Components.Add(CarteJeu);
            Components.Add(GrilleDeJeu);
            Components.Add(new Zombie(this, "fml4", 1, new Vector3(-5.224495f / 2f, 0, -5.224495f / 2f), "Lambent_Femal", new Vector3(-MathHelper.PiOver2, 0, 0), INTERVALLE_MAJ_STANDARD, "zombies01", "Marche Zombie", 100f, 0));
            Components.Add(new Zombie(this, "fmj1", 1, new Vector3(5.224495f / 2f, 0, -5.224495f / 2f), "Lambent_Femal", new Vector3(-MathHelper.PiOver2, 0, 0), INTERVALLE_MAJ_STANDARD, "zombies01", "Default Take", 75f, 1));
            Components.Add(new Player(this, "Superboy", "Default Take", "Lambent_Femal", 1, new Vector3(-MathHelper.PiOver2, 0, 0), new Vector3(-5, 0.5f, -5), 15f, "jump", "landing", "walk", "walk_slow", PlayerIndex.One, true, PORT, IP));
            Components.Add(new ObjetTournoyant(this, "key", 0.01f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-2.612247f, -3.25f, 10f), 1 / 60f));
            Components.Add(new AfficheurFPS(this, INTERVALLE_CALCUL_FPS, "Arial20"));
            Components.Add(new Menu(this));
            Components.Add(Serveur);

            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(RessourcesManager<Effect>), GestionnaireDeShaders);
            Services.AddService(typeof(RessourcesManager<SoundEffect>), GestionnaireDeSoundEffect);
            Services.AddService(typeof(RessourcesManager<Song>), GestionnaireMusique);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(SoundManager), GestionSounds);
            Services.AddService(typeof(Caméra), CaméraJeu);
            Services.AddService(typeof(CollisionManager), GestionnaireDeCollisions);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            Services.AddService(typeof(GrilleCollision), GrilleDeJeu);
            Services.AddService(typeof(Pathfinder), new Pathfinder(this));
            Services.AddService(typeof(Carte), CarteJeu);
            Services.AddService(typeof(Server), Serveur);

            base.Initialize();

            //GestionSounds.Play("backgroundMusic");
            //GestionSounds.Play("First_Sentence", false, 0.5f);
            
            //wat?
            foreach (GameComponent gc in Components.Where(x => x is IGame))
            {
                gc.Enabled = false;
                if (gc is DrawableGameComponent)
                {
                    (gc as DrawableGameComponent).Visible = false;
                }
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GestionInput.EstEnfoncée(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GestionSprites.Begin();

            base.Draw(gameTime);
            GestionSprites.End();
        }
    }
}





