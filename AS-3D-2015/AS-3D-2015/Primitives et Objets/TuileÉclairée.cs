using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace AtelierXNA
{
    class TuileÉclairée : Tuile
    {

        public const float PUISSANCE_SPÉCULAIRE = 8f;
        protected string NomTextureBumpMap { get; set; }
        Texture2D TextureBumpMap { get; set; }
        protected string NomEffetAffichage { get; set; }
        protected Effect EffetAffichage { get; set; }
        Lumière LumièreJeu { get; set; }
        protected RessourcesManager<Effect> GestionnaireDeShaders { get; private set; }
        MatériauÉclairé MatériauAffichage { get; set; }
        Vector3 CouleurLumièreAmbiante { get; set; }
        Vector4 CouleurLumièreDiffuse { get; set; }
        Vector3 CouleurLumièreSpéculaire { get; set; }
        Vector3 CouleurLumièreEmissive { get; set; }
        float CarréDistanceLumière { get; set; }
        InfoModèle InfoSphère { get; set; }
        BoundingSphere SphèreDeCollision { get; set; }
        LampeTorche LampeDePoche { get; set; }

        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }


        public TuileÉclairée(Game jeu, Vector3 positionInitiale, Vector3 dimension, string nomTexture, bool estPlancher, string nomEffetAffichage, Lumière lumièreJeu)
            : base(jeu, positionInitiale, dimension, nomTexture,estPlancher)
        {
            NomTextureBumpMap = null;
            NomEffetAffichage = nomEffetAffichage.ToUpper();
            LumièreJeu = lumièreJeu;
        }

        public override void Initialize()
        {
            base.Initialize();
            CouleurLumièreAmbiante = new Vector3(0.4f, 0.4f, 0.4f);
            CouleurLumièreDiffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            CouleurLumièreEmissive = new Vector3(0.1f, 0.1f, 0.1f);
            CouleurLumièreSpéculaire = new Vector3(0.6f, 0.6f, 0.6f);

            InfoSphère = new InfoModèle(EffetAffichage, TextureTuile, true, CouleurLumièreAmbiante, CouleurLumièreDiffuse, CouleurLumièreEmissive, CouleurLumièreSpéculaire, PUISSANCE_SPÉCULAIRE);
            SphèreDeCollision = SphèreDeCollision.Transform(GetMonde());
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeShaders = Game.Services.GetService(typeof(RessourcesManager<Effect>)) as RessourcesManager<Effect>;
            EffetAffichage = GestionnaireDeShaders.Find(NomEffetAffichage);
            LampeDePoche = Game.Services.GetService(typeof(LampeTorche)) as LampeTorche;
            TextureBumpMap = NomTextureBumpMap != null ? GestionnaireDeTextures.Find(NomTextureBumpMap) : null;
            MatériauAffichage = new MatériauÉclairé(CaméraJeu, LumièreJeu, TextureBumpMap, CouleurLumièreAmbiante, CouleurLumièreDiffuse,
                                                    CouleurLumièreEmissive, CouleurLumièreSpéculaire, LumièreJeu.Intensité, LampeDePoche);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            LumièreJeu.Position = CaméraJeu.Position;
        }

        public override void Draw(GameTime gameTime)
        {
            if (ShouldDraw)
            {
                MatériauAffichage.UpdateMatériau(PositionInitiale, GetMonde());
                ParamètresShaders.InitialiserParamètresShader(NomEffetAffichage, EffetAffichage, InfoSphère, MatériauAffichage);
                foreach (EffectPass passeEffet in EffetAffichage.CurrentTechnique.Passes)
                {
                    passeEffet.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES);
                }

            }
        }
    }
}
