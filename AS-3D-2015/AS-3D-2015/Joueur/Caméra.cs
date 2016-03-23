using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
    public class Caméra : GameComponent
    {
        #region Attributs
        const float RAYON_COLLISION = 0.5f;
        const float RAYON_ZONE_BRUIT = 5f;
        const float DEFAULT_JUMP_SPEED = 0.3f;
        const float ASCEND_JUMP_SPEED = 0.1f;
        const float GRAVITY = -1f;
        const float STADE_MARCHE_VITE = 0.73f;
        const float NEAR_PLANE_DISTANCE = 0.05f;
        const float FAR_PLANE_DISTANCE = 1000f;
        const float VOLUME_FAIBLE = 0.1f;
        const float VOLUME_MOYEN = 0.25f;
        const float VOLUME_FORT = 0.4f;
        const float ANGLE_ROTATION = 75f;


        string NomJumpSon { get; set; }
        string NomLandingSon { get; set; }
        string NomMarche { get; set; }
        string NomMarcheLente { get; set; }

        float VitesseCamera { get; set; }
        float VitesseSaut { get; set; }
        float PositionInitialeCamera { get; set; }

        bool PeutSauter { get; set; }
        bool Remonte { get; set; }
        bool SonDeMarcheActivé { get; set; }
        public Vector3 PreviousPosition {get;private set;}

        Vector3 PositionCamera;
        public Vector3 Position
        {
            get { return PositionCamera; }
            set
            {
                PositionCamera = value;
            }
        }

        Vector3 RotationCamera;
        public Vector3 Rotation
        {
            get { return RotationCamera; }
            private set
            {
                RotationCamera = value;
            }
        }

        public Vector3 CameraLookAt;
        Vector3 CameraLookAtInitial { get; set; }
        Vector3 RotationInitiale { get; set; }

        GamePadState CurrentGamePad { get; set; }
        GamePadState PrevGamePad { get; set; }

        GrilleCollision MaGrille { get; set; }
        SoundManager GestionSons { get; set; }
        InputManager GestionInput { get; set; }
        CollisionManager GestionCollisions { get; set; }
        MouseState CurrentMouseState { get; set; }
        MouseState PreviousMouseState { get; set; }
        Vector3 RotationSouris { get; set; }
        public BoundingSphere ZoneBruit { get; set; }
        public Matrix Projection { get; set; }
        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(PositionCamera, CameraLookAt, Vector3.Up);
            }
        }
        public BoundingSphere ZoneCollisionPowerUps { get; set; }
        public BoundingFrustum Frustum { get; private set; }
        #endregion


        #region Constructeur
        public Caméra(Game game)
            : base(game)
        {
        }
        #endregion

        public override void Initialize()
        {
            GestionCollisions = Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            MaGrille = Game.Services.GetService(typeof(GrilleCollision)) as GrilleCollision;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                         Game.GraphicsDevice.Viewport.AspectRatio, NEAR_PLANE_DISTANCE, FAR_PLANE_DISTANCE);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            ZoneBruit = new BoundingSphere(Position, RAYON_ZONE_BRUIT);
            ZoneCollisionPowerUps = new BoundingSphere(Position, RAYON_COLLISION);
            base.Initialize();
        }
        
        public void CameraUpdate(Matrix playerMatrix,Vector3 lookAt, Vector3 position)
        {
            Position = playerMatrix.Translation + (playerMatrix.Backward * 0.5f);
            CameraLookAt = position + lookAt;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ZoneCollisionPowerUps = new BoundingSphere(Position, RAYON_COLLISION);
            ZoneBruit = new BoundingSphere(Position, RAYON_ZONE_BRUIT);

            foreach (GameComponent c in Game.Components)
            {
                if (c is ObjetTournoyant)
                {
                    ObjetTournoyant o = c as ObjetTournoyant;
                    if (GestionCollisions.CollisionJoueurObjet(o.ZoneCollision, ZoneCollisionPowerUps))
                    {
                        Game.Components.Remove(c);
                        Console.WriteLine("Objet acquis!");
                        break;
                    }
                }
            }
            
            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            base.Update(gameTime);
        }
       
        public bool EstEnCollision(object autreObjet, BoundingSphere zoneCollision)
        {
            bool estEnCollision = false;
            

            if (autreObjet is CubeColoré)
            {
                CubeColoré mur = autreObjet as CubeColoré;
                estEnCollision = GestionCollisions.CollisionJoueurMur(zoneCollision,mur );

            }

            //if (autreObjet is Caméra)
            //{
            //    Caméra joueur = autreObjet as Caméra;
            //    GestionCollisions.CollisionJoueurs(this, joueur);
            //}

            //if (autreObjet is MONSTRE)
            //{

            //FuturZombie zombie = autreObjet as FuturZombie
            //GestionCollisions.CollisionJoueurMonstre(this, zombie);
            //}

            //if(autreObjet is ObjetTournoyant)
            //{
            //    ObjetTournoyant obj = autreObjet as ObjetTournoyant;
            //    if(GestionCollisions.CollisionMonstreMonstre(obj.ZoneCollision, ZoneCollisionPowerUps))
            //    {
            //        Console.WriteLine("Objet acquis!");
            //    }
            //}

            return estEnCollision;
        }
       
    }
}
