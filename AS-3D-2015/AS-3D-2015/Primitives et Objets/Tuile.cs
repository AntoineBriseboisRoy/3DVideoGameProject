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
    public class Tuile : PrimitiveDeBase
    {
        const int NB_SOMMETS = 6;
        protected const int NB_TRIANGLES = 2;
        const int RAYON_AFFICHAGE = 7;

        string NomTexture { get; set; }
        protected VertexPositionNormalTexture[] Sommets { get; set; }
        Vector3 Origine { get; set; }
        Vector3 Delta { get; set; }
        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        protected Texture2D TextureTuile { get; set; }

        bool EstPlancher { get; set; }
        protected bool ShouldDraw { get; set; }

        public Tuile(Game game, Vector3 positionInitiale, Vector3 dimension, string nomTexture, bool estPlancher)
            : base(game, 1f, Vector3.Zero, positionInitiale)
        {
            Delta = dimension;
            Origine = new Vector3(-Delta.X / 2, -Delta.Y / 2, -Delta.Z / 2);
            NomTexture = nomTexture;
            EstPlancher = estPlancher;
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
            TextureTuile = GestionnaireDeTextures.Find(NomTexture);

            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureTuile;
            base.LoadContent();
        }
        
        protected override void InitialiserSommets()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(1, 0);
            Vector2 c = new Vector2(0, 1);
            Vector2 d = new Vector2(1, 1);

            Vector3 A = Origine;
            Vector3 B = new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z);
            Vector3 C = new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z + Delta.Z);
            Vector3 D = new Vector3(Origine.X, Origine.Y, Origine.Z + Delta.Z);

            if(EstPlancher)
            {
                Vector3 norm = CalculerNormale(A, B, C);
                Sommets[0] = new VertexPositionNormalTexture(A,norm,c);
                Sommets[1] = new VertexPositionNormalTexture(B,norm,a);
                Sommets[2] = new VertexPositionNormalTexture(C, norm, b);

                Sommets[3] = new VertexPositionNormalTexture(C, norm, b);
                Sommets[4] = new VertexPositionNormalTexture(D, norm, d);
                Sommets[5] = new VertexPositionNormalTexture(A, norm, c);
            }

            else
            {
                Vector3 norm = CalculerNormale(A, D, C);
                Sommets[0] = new VertexPositionNormalTexture(A, norm, c);
                Sommets[1] = new VertexPositionNormalTexture(D, norm, a);
                Sommets[2] = new VertexPositionNormalTexture(C, norm, b);

                Sommets[3] = new VertexPositionNormalTexture(C, norm, b);
                Sommets[4] = new VertexPositionNormalTexture(B, norm, d);
                Sommets[5] = new VertexPositionNormalTexture(A, norm, c);
            }
        }

        Vector3 CalculerNormale(Vector3 ptA, Vector3 ptB, Vector3 ptC)
        {
            Vector3 dir = Vector3.Cross(ptB - ptA, ptC - ptA);
            Vector3 norm = Vector3.Normalize(dir);
            return norm;
        }

        public override void Update(GameTime gameTime)
        {
            ShouldDraw = Vector3.Distance(PositionInitiale, CaméraJeu.Position) <= RAYON_AFFICHAGE * Delta.X;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (ShouldDraw)
            {
                EffetDeBase.World = GetMonde();
                EffetDeBase.View = CaméraJeu.View;
                EffetDeBase.Projection = CaméraJeu.Projection;

                foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
                {
                    passeEffet.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES);
                }
            }
            base.Draw(gameTime);
        }
    }
}
