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
        const float NEAR_PLANE_DISTANCE = 0.05f;
        const float FAR_PLANE_DISTANCE = 1000f;



        Vector3 PositionCamera;
        public Vector3 Position
        {
            get { return PositionCamera; }
            set
            {
                PositionCamera = value;
            }
        }
      

        public Vector3 CameraLookAt;
        public Matrix Projection { get; set; }
        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(PositionCamera, CameraLookAt, Vector3.Up);
            }
        }
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
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                         Game.GraphicsDevice.Viewport.AspectRatio, NEAR_PLANE_DISTANCE, FAR_PLANE_DISTANCE);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
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
            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            base.Update(gameTime);
        }
       
     
        
       
    }
}
