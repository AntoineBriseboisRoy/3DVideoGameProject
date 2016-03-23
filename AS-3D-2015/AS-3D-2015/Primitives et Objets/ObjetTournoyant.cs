using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class ObjetTournoyant : ObjetDeBase
    {
        const float ANGLE = MathHelper.PiOver2 / 10;
        const float VITESSE_ROTATION = 2f;
        const float MAX_VARIATION = 0.5f;
        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        Vector3 RotationInitiale { get; set; }
        Vector3 PositionInitiale { get; set; }
        Quaternion QuatRotation { get; set; }
        float TempsTotal { get; set; }
        Vector3 RotationModel { get; set; }
        public string NomMod�le { get; set; }

        public BoundingSphere ZoneCollision { get; private set; }

        public ObjetTournoyant(Game jeu, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
            RotationInitiale = rotationInitiale;
            PositionInitiale = positionInitiale;
            NomMod�le = nomMod�le;
        }

        public override void Initialize()
        {
            base.Initialize();
            float sinMoiti�Angle = (float)Math.Sin(ANGLE / 2);
            float cosMoiti�Angle = (float)Math.Cos(ANGLE / 2);

            RotationInitiale = Vector3.Normalize(RotationInitiale); 
            Vector3 imaginaireQuat = sinMoiti�Angle * RotationInitiale;

            QuatRotation = new Quaternion(imaginaireQuat, cosMoiti�Angle);
            QuatRotation = Quaternion.Normalize(QuatRotation);

            ZoneCollision = new BoundingSphere(PositionInitiale, 5f);
        }

        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            TempsTotal += temps�coul�;
            if(Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                RotationObjet();
                TranslationObjet(TempsTotal);
                Temps�coul�DepuisMAJ = 0;
            }
            ZoneCollision = new BoundingSphere(Position, 5f);

            ObjetTournoyantInfo o = new ObjetTournoyantInfo(Position, Rotation, NomMod�le);
            base.Update(gameTime);
        }

        void RotationObjet()
        {
            RotationModel = (Vector3.Up * VITESSE_ROTATION);

            QuatRotation *= Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(RotationModel.X))
                         * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(RotationModel.Y))
                         * Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(RotationModel.Z));
        }

        void TranslationObjet(float temps)
        {
            Position = new Vector3(Position.X, (float)(MAX_VARIATION * Math.Sin(2 * temps)) + PositionInitiale.Y, Position.Z);
        }

        public override Matrix GetMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromQuaternion(QuatRotation);
            Monde *= Matrix.CreateTranslation(Position);
            return Monde;
        }
    }
}
