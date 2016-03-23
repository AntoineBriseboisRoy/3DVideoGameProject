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
using SkinnedModel;


namespace AtelierXNA
{
    public class Player : DrawableGameComponent, IGame
    {
        #region Attributs et Propri�t�s
        const float RAYON_COLLISION = 0.75f;
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
        string IP { get; set; }

        int Port { get; set; }

        float VitesseCamera { get; set; }
        float VitesseSaut { get; set; }
        float PositionInitialeCamera { get; set; }

        bool PeutSauter { get; set; }
        bool Remonte { get; set; }
        bool SonDeMarcheActiv� { get; set; }
        bool IsCurrentPlayer { get; set; }
        public Vector3 PreviousPosition { get; set; }

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
            set
            {
                RotationCamera = value;
            }
        }

        Vector3 CameraLookAt;
        Vector3 GamePadRotationBuffer;
        Vector3 RotationInitiale { get; set; }

        GamePadState CurrentGamePad { get; set; }
        GamePadState PrevGamePad { get; set; }

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
        RessourcesManager<Model> GestionnaireDeMod�les { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        float Scale { get; set; }
        string NomAnim { get; set; }
        string NomModel { get; set; }
        string NomTexture { get; set; }
        Model Modele { get; set; }
        Matrix[] TransformationsMod�le { get; set; }
        Matrix Monde { get; set; }
        Texture2D Habit { get; set; }
        AnimationPlayer AnimationPlayer { get; set; }
        Cam�ra Cam�raJeu { get; set; }
        PlayerIndex PlayerIndex { get; set; }
        Matrix MondeCamera { get; set; }
        #endregion

        public Player(Game game, string nomModel, string nomAnim, string nomTexture, float scale,
            Vector3 rotation, Vector3 position, float vitesse, string nomSonSaut, string nomSonAtterissage,
            string nomSonMarche, string nomMarcheLente, PlayerIndex playerIndex, bool isCurrentPlayer, int port, string ip)
            : base(game)
        {
            NomAnim = nomAnim;
            NomModel = nomModel;
            NomTexture = nomTexture;
            Scale = scale;
            Rotation = rotation;
            NomJumpSon = nomSonSaut;
            NomLandingSon = nomSonAtterissage;
            NomMarche = nomSonMarche;
            NomMarcheLente = nomMarcheLente;
            PositionInitialeCamera = position.Y;
            VitesseCamera = vitesse;
            RotationInitiale = rotation;
            PreviousPosition = position;
            Position = position;
            PlayerIndex = playerIndex;
            IsCurrentPlayer = isCurrentPlayer;
            Port = port;
            IP = ip;
        }


        public override void Initialize()
        {
            PeutSauter = true;
            SonDeMarcheActiv� = true;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                         Game.GraphicsDevice.Viewport.AspectRatio, NEAR_PLANE_DISTANCE, FAR_PLANE_DISTANCE);
            PrevGamePad = GamePad.GetState(PlayerIndex);
            PreviousMouseState = Mouse.GetState();
            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            ZoneBruit = new BoundingSphere(Position, RAYON_ZONE_BRUIT);
            ZoneCollisionPowerUps = new BoundingSphere(Position, RAYON_COLLISION);


            base.Initialize();
        }
        protected override void LoadContent()
        {
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionSons = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
            GestionCollisions = Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            Cam�raJeu = Game.Services.GetService(typeof(Cam�ra)) as Cam�ra;
            GestionnaireDeMod�les = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;

            CalculerMonde();

            Modele = GestionnaireDeMod�les.Find(NomModel);
            TransformationsMod�le = new Matrix[Modele.Bones.Count];
            Habit = GestionnaireDeTextures.Find(NomTexture);

            SkinningData skinningData = Modele.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            AnimationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips[NomAnim];

            AnimationPlayer.StartClip(clip);
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            if (PreviousPosition != Position)
                AnimationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            else
                AnimationPlayer.Update(new TimeSpan(0, 0, 0), true, Matrix.Identity);

            PreviousPosition = Position;
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentGamePad = GamePad.GetState(PlayerIndex);
            CurrentMouseState = Mouse.GetState();

            if (GestionInput.EstManetteActiv�e)
            {
                G�rerManette(Temps�coul�);

            }
            else if (GestionInput.EstSourisActive || GestionInput.EstClavierActiv�)
            {
                G�rerControlePC(Temps�coul�);
            }
            else
            {
                GestionSons.PauseSoundEffect(NomMarcheLente);
                GestionSons.PauseSoundEffect(NomMarche);
            }

            if (!PeutSauter)
            {
                VitesseSaut += Temps�coul� * GRAVITY;
                PositionCamera.Y += VitesseSaut;
                Cam�raJeu.CameraLookAt.Y += VitesseSaut;
            }

            if (PositionCamera.Y < PositionInitialeCamera - 1)
            {
                VitesseSaut = ASCEND_JUMP_SPEED;
                Remonte = true;
            }
            // je peux surement mettre les deux methode ensemble
            if (Remonte)
            {
                GestionSons.Play(NomLandingSon, false, VOLUME_FAIBLE);
                VitesseSaut -= (Temps�coul� * GRAVITY);
                PositionCamera.Y += VitesseSaut;
                Cam�raJeu.CameraLookAt.Y += VitesseSaut;

                if (PositionCamera.Y > PositionInitialeCamera)
                {
                    float ajusterVue = Cam�raJeu.CameraLookAt.Y - PositionCamera.Y;
                    PositionCamera.Y = PositionInitialeCamera;
                    Cam�raJeu.CameraLookAt.Y = ajusterVue + PositionCamera.Y;
                    PeutSauter = true;
                    Remonte = false;
                }
            }
            // a voir
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
            if (IsCurrentPlayer)
            {
                PlayerInfo p = new PlayerInfo(Position, Rotation, PreviousPosition, Port, IP);
            }
            CalculerMonde();
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Matrix[] bones = AnimationPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in Modele.Meshes)
            {
                Matrix mondeLocal = TransformationsMod�le[mesh.ParentBone.Index] * Monde;
                Modele.CopyAbsoluteBoneTransformsTo(TransformationsMod�le);
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = Cam�raJeu.View;
                    effect.Projection = Cam�raJeu.Projection;
                    effect.World = mondeLocal;
                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                    effect.Texture = Habit;

                }
                mesh.Draw();
            }
        }

        void PreviewMove(Vector3 prochainMouvement)
        {
            Matrix rotate = Matrix.CreateRotationY(Rotation.Y);
            Vector3 mouvement = new Vector3(prochainMouvement.X, prochainMouvement.Y, prochainMouvement.Z);
            mouvement = Vector3.Transform(mouvement, rotate);
            BoundingSphere zoneCollisionX = new BoundingSphere(new Vector3(Position.X + mouvement.X, Position.Y, Position.Z), RAYON_COLLISION);
            BoundingSphere zoneCollisionZ = new BoundingSphere(new Vector3(Position.X, Position.Y, Position.Z + mouvement.Z), RAYON_COLLISION);

            foreach (GameComponent c in Game.Components)
            {
                if (EstEnCollision(c, zoneCollisionX))
                {
                    Position = new Vector3(Position.X - mouvement.X, Position.Y, Position.Z);
                }
                if (EstEnCollision(c, zoneCollisionZ))
                {
                    Position = new Vector3(Position.X, Position.Y, Position.Z - mouvement.Z);
                }
            }
            SonDeMarcheActiv� = true;
            Position += mouvement;
        }
        void Bouger(Vector3 scale)
        {
            PreviewMove(scale);
        }
        void CalculerMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Scale) *
                    Matrix.CreateFromYawPitchRoll(Rotation.Y, -MathHelper.PiOver2, Rotation.Z) *
                    Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, Position.Z));
            MondeCamera = Matrix.Identity *
                   Matrix.CreateScale(Scale) *
                   Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                   Matrix.CreateTranslation(new Vector3(Position.X, Position.Y + 0.3f, Position.Z));
            Matrix rotationMatrix = Matrix.CreateRotationX(RotationCamera.X) * Matrix.CreateRotationY(RotationCamera.Y);
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            Cam�raJeu.CameraUpdate(MondeCamera, lookAtOffset, Position);
        }
        void G�rerControlePC(float temps�coul�)
        {
            float deltaX, deltaY;

            Vector3 moveVector = Vector3.Zero;
            if ((GestionInput.EstEnfonc�e(Keys.Space)) && PeutSauter)
            {
                GestionSons.PauseSoundEffect(NomMarche);
                GestionSons.Play(NomJumpSon, false, VOLUME_MOYEN);
                VitesseSaut = DEFAULT_JUMP_SPEED;
                PeutSauter = false;
            }
            moveVector.Z = G�rerTouche(Keys.W) - G�rerTouche(Keys.S);
            moveVector.X = G�rerTouche(Keys.A) - G�rerTouche(Keys.D);
            if (moveVector != Vector3.Zero)
            {

                if (GestionInput.EstEnfonc�e(Keys.LeftShift))
                    moveVector = moveVector * 2;

                if (!SonDeMarcheActiv�)
                    GestionSons.PauseSoundEffect(NomMarche);
                if (PeutSauter && SonDeMarcheActiv� && (moveVector.X > STADE_MARCHE_VITE || moveVector.X < -STADE_MARCHE_VITE
                                            || moveVector.Z > STADE_MARCHE_VITE || moveVector.Z < -STADE_MARCHE_VITE))
                {
                    GestionSons.PauseSoundEffect(NomMarcheLente);
                    GestionSons.Play(NomMarche, true, VOLUME_MOYEN);
                }
                else if (PeutSauter && SonDeMarcheActiv�)
                {
                    GestionSons.PauseSoundEffect(NomMarche);
                    GestionSons.Play(NomMarcheLente, true, VOLUME_MOYEN);
                }

                moveVector *= temps�coul� * VitesseCamera;
                Bouger(moveVector);
            }

            if (CurrentMouseState != PreviousMouseState)
            {
                deltaX = CurrentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = CurrentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                RotationSouris = new Vector3(RotationSouris.X - 0.1f * deltaX * temps�coul�, RotationSouris.Y - 0.1f * deltaY * temps�coul�, RotationSouris.Z);

                deltaX = 0;
                deltaY = 0;
                if (RotationSouris.Y < MathHelper.ToRadians(-75.0f))
                    RotationSouris = new Vector3(RotationSouris.X, RotationSouris.Y - (RotationSouris.Y - MathHelper.ToRadians(-75.0f)), RotationSouris.Z);

                if (RotationSouris.Y > MathHelper.ToRadians(75.0f))
                    RotationSouris = new Vector3(RotationSouris.X, RotationSouris.Y - (RotationSouris.Y - MathHelper.ToRadians(75.0f)), RotationSouris.Z);
                Rotation = new Vector3(-MathHelper.Clamp(RotationSouris.Y, MathHelper.ToRadians(-40), MathHelper.ToRadians(75.0f)),
                     MathHelper.WrapAngle(RotationSouris.X), 0);


            }
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            PreviousMouseState = CurrentMouseState;
        }
        int G�rerBouton(Buttons b)
        {
            return GestionInput.EstBoutonEnfonc�e(b) ? 1 : 0;
        }
        int G�rerTouche(Keys k)
        {
            return GestionInput.EstEnfonc�e(k) ? 1 : 0;
        }
        void G�rerManette(float temps�coul�)
        {
            float deltaX, deltaY;
            Vector3 moveVector = Vector3.Zero;
            if (GestionInput.EstBoutonEnfonc�e(Buttons.LeftStick))
                moveVector = moveVector * 2;
            //G�rer les sauts
            if ((GestionInput.EstBoutonEnfonc�e(Buttons.A)) && PeutSauter)
            {
                GestionSons.PauseSoundEffect(NomMarche);
                GestionSons.Play(NomJumpSon, false, VOLUME_MOYEN);
                VitesseSaut = DEFAULT_JUMP_SPEED;
                PeutSauter = false;
            }
            //G�rer les d�placements du joystick droit
            CameraLookAt.Y += G�rerBouton(Buttons.RightThumbstickUp) - G�rerBouton(Buttons.RightThumbstickDown);
            CameraLookAt.X += G�rerBouton(Buttons.RightThumbstickRight) - G�rerBouton(Buttons.RightThumbstickLeft);
            deltaY = CameraLookAt.Y - (Game.GraphicsDevice.Viewport.Height / 2f);
            deltaX = CameraLookAt.X - (Game.GraphicsDevice.Viewport.Width / 2f);
            GamePadRotationBuffer.Y += GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y * 0.01f * deltaY * temps�coul�;
            GamePadRotationBuffer.X += GamePad.GetState(PlayerIndex).ThumbSticks.Right.X * 0.01f * deltaX * temps�coul�;


            //D�placement de la cam�ra avec le joystick gauche
            moveVector.Z = GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y;
            moveVector.X = -GamePad.GetState(PlayerIndex).ThumbSticks.Left.X;

            //Fait d�placer le Joueur
            if (moveVector != Vector3.Zero)
            {

                if (!SonDeMarcheActiv�)
                    GestionSons.PauseSoundEffect(NomMarche);
                if (PeutSauter && SonDeMarcheActiv� && (moveVector.X > STADE_MARCHE_VITE || moveVector.X < -STADE_MARCHE_VITE
                                            || moveVector.Z > STADE_MARCHE_VITE || moveVector.Z < -STADE_MARCHE_VITE))
                {
                    GestionSons.PauseSoundEffect(NomMarcheLente);
                    GestionSons.Play(NomMarche, true, VOLUME_MOYEN);
                }
                else if (PeutSauter && SonDeMarcheActiv�)
                {
                    GestionSons.PauseSoundEffect(NomMarche);
                    GestionSons.Play(NomMarcheLente, true, VOLUME_MOYEN);
                }

                moveVector *= temps�coul� * VitesseCamera;
                Bouger(moveVector);
            }

            if (GamePadRotationBuffer.Y < MathHelper.ToRadians(-ANGLE_ROTATION))
                GamePadRotationBuffer.Y = GamePadRotationBuffer.Y - (GamePadRotationBuffer.Y - MathHelper.ToRadians(-ANGLE_ROTATION));

            if (GamePadRotationBuffer.Y > MathHelper.ToRadians(ANGLE_ROTATION))
                GamePadRotationBuffer.Y = GamePadRotationBuffer.Y - (GamePadRotationBuffer.Y - MathHelper.ToRadians(ANGLE_ROTATION));

            Rotation = new Vector3(MathHelper.Clamp(GamePadRotationBuffer.Y + RotationInitiale.X, MathHelper.ToRadians(-ANGLE_ROTATION), MathHelper.ToRadians(ANGLE_ROTATION)),
                MathHelper.WrapAngle(GamePadRotationBuffer.X + RotationInitiale.Y), 0);


            deltaX = 0;
            deltaY = 0;
        }
        bool EstEnCollision(object autreObjet, BoundingSphere zoneCollision)
        {
            bool estEnCollision = false;


            if (autreObjet is CubeColor�)
            {
                CubeColor� mur = autreObjet as CubeColor�;
                estEnCollision = GestionCollisions.CollisionJoueurMur(zoneCollision, mur);

            }

            //if (autreObjet is Cam�ra)
            //{
            //    Cam�ra joueur = autreObjet as Cam�ra;
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