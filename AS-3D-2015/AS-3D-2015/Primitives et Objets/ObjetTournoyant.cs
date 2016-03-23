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
        float TempsÉcouléDepuisMAJ { get; set; }
        Vector3 RotationInitiale { get; set; }
        Vector3 PositionInitiale { get; set; }
        Quaternion QuatRotation { get; set; }
        float TempsTotal { get; set; }
        Vector3 RotationModel { get; set; }
        public string NomModèle { get; set; }

        public BoundingSphere ZoneCollision { get; private set; }

        public ObjetTournoyant(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
            RotationInitiale = rotationInitiale;
            PositionInitiale = positionInitiale;
            NomModèle = nomModèle;
        }

        public override void Initialize()
        {
            base.Initialize();
            float sinMoitiéAngle = (float)Math.Sin(ANGLE / 2);
            float cosMoitiéAngle = (float)Math.Cos(ANGLE / 2);

            RotationInitiale = Vector3.Normalize(RotationInitiale); 
            Vector3 imaginaireQuat = sinMoitiéAngle * RotationInitiale;

            QuatRotation = new Quaternion(imaginaireQuat, cosMoitiéAngle);
            QuatRotation = Quaternion.Normalize(QuatRotation);

            ZoneCollision = new BoundingSphere(PositionInitiale, 5f);
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            TempsTotal += tempsÉcoulé;
            if(TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                RotationObjet();
                TranslationObjet(TempsTotal);
                TempsÉcouléDepuisMAJ = 0;
            }
            ZoneCollision = new BoundingSphere(Position, 5f);

            ObjetTournoyantInfo o = new ObjetTournoyantInfo(Position, Rotation, NomModèle);
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
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromQuaternion(QuatRotation);
            Monde *= Matrix.CreateTranslation(Position);
            return Monde;
        }
    }
}
