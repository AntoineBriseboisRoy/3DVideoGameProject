using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;



namespace AtelierXNA
{
    public class Zombie : ObjetDeBase
    {
        #region temp
        const int NB_SOMMETS = 24;
        const int NB_TRIANGLES = 8;
        const float RAYON_COLLISION = 1f;
        const float RAYON_OUIE = 20f;
        const float NB_PAS = 100f;
        const float VOLUME_FAIBLE = 0.7f;

        Vector3 Delta { get; set; }
        BasicEffect EffetDeBase { get; set; }
        List<Vector3> DéplacementsPossibles { get; set; }
        CollisionManager GestionCollisions { get; set; }
        SoundManager GestionSons { get; set; }
        Vector3 PositionCible { get; set; }
        Vector3 Pas { get; set; }
        Caméra CaméraJeu { get; set; }
        string Grognement { get; set; }
        string NomAnim { get; set; }
        public BoundingSphere ZoneCollisionCible { get; set; }
        public BoundingSphere ZoneOuïe { get; set; }

        Random Gen = new Random();
        #endregion

        Vector3 DéplacementHaut;
        Vector3 DéplacementBas;
        Vector3 DéplacementDroite;
        Vector3 DéplacementGauche;
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D Habit { get; set; }
        Pathfinder Watney { get; set; }
        public BoundingSphere ZoneCollision { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        float NbPas { get; set; }
        Vector3 PositionJoueurAttaqué { get; set; }
        bool EstEnMouvement { get; set; }
        bool EstEnMarche { get; set; }
        float Volume { get; set; }
        string NomTexture { get; set; }
        Carte CarteJeu { get; set; }
        int cpt = 0;
        bool ShouldDraw { get; set; }
        AnimationPlayer AnimationPlayer { get; set; }
        public int NuméroIdentification { get; set; }


        public Zombie(Game game, string nomModèle, float échelleInitiale, Vector3 positionInitiale, string nomTexture, Vector3 rotationInitiale, float intervalleMAJ, string grognement, string nomAnim, float vitesse, int num)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            ZoneCollision = new BoundingSphere(positionInitiale, RAYON_COLLISION);
            ZoneOuïe = new BoundingSphere(positionInitiale, RAYON_OUIE);
            NomTexture = nomTexture;
            IntervalleMAJ = intervalleMAJ;
            Grognement = grognement;
            NomAnim = nomAnim;
            NbPas = vitesse;
            NuméroIdentification = num;
        }

        public override void Initialize()
        {
            ShouldDraw = false;
            CalculerMatriceMonde();
            DéplacementsPossibles = new List<Vector3>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //IMPORTANT POUR ZOMBIE
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;
            GestionCollisions = Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            GestionSons = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
            Watney = Game.Services.GetService(typeof(Pathfinder)) as Pathfinder;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            CarteJeu = Game.Services.GetService(typeof(Carte)) as Carte;

            Delta = CarteJeu.Delta;

            DéplacementHaut = new Vector3(0, 0, -Delta.Z);
            DéplacementBas = new Vector3(0, 0, Delta.Z);
            DéplacementDroite = new Vector3(Delta.X, 0, 0);
            DéplacementGauche = new Vector3(-Delta.X, 0, 0);
            base.LoadContent();

            Habit = GestionnaireDeTextures.Find(NomTexture);
            
            SkinningData skinningData = Modèle.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            AnimationPlayer = new AnimationPlayer(skinningData);
            
            AnimationClip clip = skinningData.AnimationClips[NomAnim];

            AnimationPlayer.StartClip(clip);
           
        }
        void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Échelle) *
                    Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                    Matrix.CreateTranslation(Position);
        }
        public override void Draw(GameTime gameTime)
        {
            Matrix[] bones = AnimationPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Modèle.Meshes)
            {
                Matrix mondeLocal = TransformationsModèle[mesh.ParentBone.Index] * GetMonde();
                Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = CaméraJeu.View;
                    effect.Projection = CaméraJeu.Projection;
                    effect.World = mondeLocal;
                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                    effect.Texture = Habit;

                }
                mesh.Draw();
            }
        }
        public override Matrix GetMonde()
        {
            return Monde;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (EstEnMarche)
                AnimationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            else
                AnimationPlayer.Update(new TimeSpan(0, 0, 0), true, Matrix.Identity);

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMiseÀJour();
                TempsÉcouléDepuisMAJ = 0;
            }

            CalculerMatriceMonde();

            #region Sons
            if (Entend())
            {
                if (Vector3.Distance(Position, PositionJoueurAttaqué) < 1)
                {
                    Volume = VOLUME_FAIBLE;
                }

                else
                {
                    Volume = VOLUME_FAIBLE / Vector3.Distance(Position, PositionJoueurAttaqué);
                }

                GestionSons.Play(Grognement, true, Volume);
            }

            else
            {
                GestionSons.PauseSoundEffect(Grognement);
            }

            #endregion

            //ShouldDraw = CaméraJeu.Frustum.Intersects(ZoneCollision) && (Vector3.Distance(Position, CaméraJeu.Position) <= 15 * Delta.X);

            ZombieInfo z = new ZombieInfo(Position, Rotation, NuméroIdentification);
            base.Update(gameTime);
        }
        void EffectuerMiseÀJour()
        {
            if (!EstEnMouvement)
            {
                if (Entend())
                {
                    ModeChasse();
                }

                else
                {
                    CalculerDéplacementsModeAléatoire();
                }


            }

            ModifierPosition();
            CalculerMatriceMonde();

        }
        void CalculerDéplacementsModeAléatoire()
        {

            DéplacementsPossibles.Clear();
            EstEnMarche = true;


            if (!EstEnCollision(new BoundingSphere(Position + DéplacementBas, RAYON_COLLISION)))
            {
                DéplacementsPossibles.Add(DéplacementBas);
            }

            if (!EstEnCollision(new BoundingSphere(Position + DéplacementHaut, RAYON_COLLISION)))
            {
                DéplacementsPossibles.Add(DéplacementHaut);
            }

            if (!EstEnCollision(new BoundingSphere(Position + DéplacementDroite, RAYON_COLLISION)))
            {
                DéplacementsPossibles.Add(DéplacementDroite);
            }

            if (!EstEnCollision(new BoundingSphere(Position + DéplacementGauche, RAYON_COLLISION)))
            {
                DéplacementsPossibles.Add(DéplacementGauche);
            }

            if (DéplacementsPossibles.Count == 0)
            {
                DéplacementsPossibles.Add(Vector3.Zero);
                EstEnMouvement = false;
            }


            int i = Gen.Next(0, DéplacementsPossibles.Count);
            PositionCible = Position + DéplacementsPossibles[i];
            ZoneCollisionCible = new BoundingSphere(PositionCible, RAYON_COLLISION);
            Pas = DéplacementsPossibles[i] / NbPas;
        }
        void ModeChasse()
        {
            Watney.CalculerChemin(Position, PositionJoueurAttaqué);

            if (Watney.Chemin == null || Watney.Chemin.Count == 0)
            {
                if(Watney.Chemin==null)
                {
                    CalculerDéplacementsModeAléatoire();
                }

                else
                {
                    PositionCible = Position;
                    Pas = Vector3.Zero;
                   
                }
                EstEnMarche = false;
            }

            else
            {
                EstEnMarche = true; ;
                if (!EstEnCollision(new BoundingSphere(Watney.Chemin[0].PositionDansLeMonde, RAYON_COLLISION)))
                {
                    PositionCible = Watney.Chemin[0].PositionDansLeMonde;
                    ZoneCollisionCible = new BoundingSphere(PositionCible, RAYON_COLLISION);
                    Pas = (PositionCible - Position) / NbPas;
                }

                else
                {
                    PositionCible = Position;
                    Pas = Vector3.Zero;
                    EstEnMouvement = false;
                    EstEnMarche = false;
                }
            }
        }
        void ModifierPosition()
        {
            
            if (cpt < NbPas)
            {
                Position += Pas;
                EstEnMouvement = true;
                ++cpt;
            }

            else
            {
                Position = PositionCible;
                cpt = 0;
                EstEnMouvement = false;
            }
            if (EstEnMarche)
            {
                float rotationY = (float)Math.Atan2(Pas.X, Pas.Z);
                Rotation = new Vector3(Rotation.X, rotationY, Rotation.Z);
            }
            ZoneCollision = new BoundingSphere(Position, RAYON_COLLISION);
            ZoneOuïe = new BoundingSphere(Position, RAYON_OUIE);


        }
        bool EstEnCollision(BoundingSphere zoneCollTest)
        {
            bool estEnCollision = false;

            foreach (GameComponent c in Game.Components)
            {
                if (c is Zombie)
                {
                    Zombie zombie = c as Zombie;
                    estEnCollision = GestionCollisions.CollisionMonstreMonstre(zoneCollTest, zombie.ZoneCollisionCible) || GestionCollisions.CollisionMonstreMonstre(zoneCollTest, zombie.ZoneCollision);
                }

                if (c is CubeColoré)
                {
                    CubeColoré mur = c as CubeColoré;
                    estEnCollision = GestionCollisions.CollisionMonstreMur(zoneCollTest, mur);
                }

                if (estEnCollision)
                {
                    break;
                }

            }

            return estEnCollision;
        }
        bool Entend()
        {
            bool entend = false;
            foreach (GameComponent c in Game.Components)
            {
                if (c is Player)
                {
                    Player joueur = c as Player;
                    entend = GestionCollisions.DétectionJoueur(ZoneOuïe, joueur.ZoneBruit);

                    if (entend)
                    {
                        PositionJoueurAttaqué = joueur.Position;
                        break;
                    }

                    break;
                }
            }

            return entend;
        }
    }
}
