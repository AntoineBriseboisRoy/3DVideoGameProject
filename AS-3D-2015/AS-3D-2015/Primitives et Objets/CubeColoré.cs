using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace AtelierXNA
{
    public class CubeColoré : PrimitiveDeBase
    {
        int NB_SOMMETS = 24;
        public int NB_TRIANGLES = 8;
        const int RAYON_AFFICHAGE = 7;

        string NomTexture { get; set; }
        protected VertexPositionNormalTexture[] Sommets { get; set; }
        Vector3 Origine { get; set; }
        Vector3 Delta { get; set; }
        public BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        protected Texture2D TextureDuCube { get; set; }
        public BoundingBox ZoneCollision { get; set; }
        public BoundingSphere ZoneVerifCollision { get; set; }
        public BoundingSphere ZoneCollisionZombie { get; set; }

        public bool ShouldDraw { get; set; }

        public CubeColoré(Game game, Vector3 positionInitiale, Vector3 dimension, string nomTexture)
            : base(game, 1f, Vector3.Zero, positionInitiale)
        {
            ZoneCollision = new BoundingBox(new Vector3(positionInitiale.X - dimension.X / 2, positionInitiale.Y - dimension.Y / 2, positionInitiale.Z - dimension.Z / 2),
                            new Vector3(positionInitiale.X + dimension.X / 2, positionInitiale.Y + dimension.Y, positionInitiale.Z + dimension.Z / 2));
            Delta = dimension;
            ZoneVerifCollision = new BoundingSphere(positionInitiale, dimension.X);
            Origine = new Vector3(-Delta.X / 2, -Delta.Y / 2, -Delta.Z / 2);
            NomTexture = nomTexture;
            ZoneCollisionZombie = new BoundingSphere(positionInitiale, dimension.X / 2f);

        }

        public override void Initialize()
        {
            Sommets = new VertexPositionNormalTexture[NB_SOMMETS];
            ShouldDraw = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TextureDuCube = GestionnaireDeTextures.Find(NomTexture);

            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureDuCube;
            base.LoadContent();
        }

        protected override void InitialiserSommets()
        {


            // Calculate the position of the vertices on the top face.
            Vector3 topLeftFront = Origine + new Vector3(-1.0f, 1.0f, -1.0f) * Delta;
            Vector3 topLeftBack = Origine + new Vector3(-1.0f, 1.0f, 1.0f) * Delta;
            Vector3 topRightFront = Origine + new Vector3(1.0f, 1.0f, -1.0f) * Delta;
            Vector3 topRightBack = Origine + new Vector3(1.0f, 1.0f, 1.0f) * Delta;

            // Calculate the position of the vertices on the bottom face.
            Vector3 btmLeftFront = Origine + new Vector3(-1.0f, -1.0f, -1.0f) * Delta;
            Vector3 btmLeftBack = Origine + new Vector3(-1.0f, -1.0f, 1.0f) * Delta;
            Vector3 btmRightFront = Origine + new Vector3(1.0f, -1.0f, -1.0f) * Delta;
            Vector3 btmRightBack = Origine + new Vector3(1.0f, -1.0f, 1.0f) * Delta;

            // Normal vectors for each face (needed for lighting / display)
            Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f);

            // UV texture coordinates
            Vector2 textureTopLeft = new Vector2(1f, 0f);
            Vector2 textureTopRight = new Vector2(0f, 0f);
            Vector2 textureBottomLeft = new Vector2(1f, 1f);
            Vector2 textureBottomRight = new Vector2(0, 1f);

            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(1, 0);
            Vector2 c = new Vector2(0, 1);
            Vector2 d = new Vector2(1, 1);

            //POSITIONS
             Vector3 A = Origine;
             Vector3 B = new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z);
             Vector3 C = new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z + Delta.Z);
             Vector3 D = new Vector3(Origine.X, Origine.Y, Origine.Z + Delta.Z);

             Vector3 E = new Vector3(Origine.X, Origine.Y + Delta.Y, Origine.Z);
             Vector3 F = new Vector3(Origine.X + Delta.X, Origine.Y + Delta.Y, Origine.Z);
             Vector3 G = new Vector3(Origine.X + Delta.X, Origine.Y + Delta.Y, Origine.Z + Delta.Z);
             Vector3 H = new Vector3(Origine.X, Origine.Y + Delta.Y, Origine.Z + Delta.Z);
            


            // Add the vertices for the RIGHT face. 
             Vector3 norm = CalculerNormale(F, G, B);
             Sommets[0] = new VertexPositionNormalTexture(F, norm, b);
             Sommets[1] = new VertexPositionNormalTexture(G, norm, a);
             Sommets[2] = new VertexPositionNormalTexture(B, norm, d);

             Sommets[3] = new VertexPositionNormalTexture(B, norm, d);
             Sommets[4] = new VertexPositionNormalTexture(G, norm, a);
             Sommets[5] = new VertexPositionNormalTexture(C, norm, c);

             Sommets[6] = new VertexPositionNormalTexture(C, norm, c);
             Sommets[7] = new VertexPositionNormalTexture(G, norm, a);
             Sommets[8] = new VertexPositionNormalTexture(D, norm, d);

             Sommets[9] = new VertexPositionNormalTexture(D, norm, d);
             Sommets[10] = new VertexPositionNormalTexture(G, norm, a);
             Sommets[11] = new VertexPositionNormalTexture(H, norm, b);

             Sommets[12] = new VertexPositionNormalTexture(H, norm, b);
             Sommets[13] = new VertexPositionNormalTexture(E, norm, a);
             Sommets[14] = new VertexPositionNormalTexture(D, norm, d);

             Sommets[15] = new VertexPositionNormalTexture(D, norm, d);
             Sommets[16] = new VertexPositionNormalTexture(E, norm, a);
             Sommets[17] = new VertexPositionNormalTexture(A, norm, c);

             Sommets[18] = new VertexPositionNormalTexture(A, norm, c);
             Sommets[19] = new VertexPositionNormalTexture(E, norm, a);
             Sommets[20] = new VertexPositionNormalTexture(B, norm, d);

             Sommets[21] = new VertexPositionNormalTexture(B, norm, d);
             Sommets[22] = new VertexPositionNormalTexture(E, norm, a);
             Sommets[23] = new VertexPositionNormalTexture(F, norm, b);
 
        }

        Vector3 CalculerNormale(Vector3 ptA, Vector3 ptB, Vector3 ptC)
        {
            Vector3 dir = Vector3.Cross(ptB - ptA, ptC - ptA);
            Vector3 norm = Vector3.Normalize(dir);
            return norm;
        }

        public override void Update(GameTime gameTime)
        {
            ShouldDraw = CaméraJeu.Frustum.Intersects(ZoneCollision) && (Vector3.Distance(PositionInitiale, CaméraJeu.Position) <= RAYON_AFFICHAGE * Delta.X);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (ShouldDraw)
            {
                EffetDeBase.World = GetMonde();
                EffetDeBase.View = CaméraJeu.View;
                EffetDeBase.Projection = CaméraJeu.Projection;

                RasterizerState old = GraphicsDevice.RasterizerState;
                RasterizerState ras = new RasterizerState();
                ras.CullMode = CullMode.CullClockwiseFace;
                GraphicsDevice.RasterizerState = ras;

                foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
                {
                    passeEffet.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES);
                }

                GraphicsDevice.RasterizerState = old;
            }
            base.Draw(gameTime);
        }
    }
}