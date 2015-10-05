using Microsoft.DirectX;
using System;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    public class MenuItem : IRenderObject
    {
        private TgcBox opcion;
        private TgcBox opcionSelect;
        private bool select;

        public bool AlphaBlendEnable
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public MenuItem(Vector3 vectorOrigen, Vector3 vectorFin, string texturaSelect, string texturaUnselect)
        {
            this.opcion = TgcBox.fromSize(vectorOrigen, vectorFin, TgcTexture.createTexture(texturaUnselect));
            this.opcionSelect = TgcBox.fromSize(new Vector3(vectorOrigen.X + 0.4f, vectorOrigen.Y, vectorOrigen.Z), vectorFin, TgcTexture.createTexture(texturaSelect));

            this.select = false;
            this.opcion.Enabled = false;

            //this.opcion.Effect = TgcShaders.loadEffect(GuiController.Instance.ExamplesDir + "Shaders\\WorkshopShaders\\Shaders\\BasicShader.fx");
            //this.opcion.Technique = "RenderScene";
        }

        public void render()
        {
            this.opcion.render();
            this.opcionSelect.render();
        }

        public void dispose()
        {
            this.opcion.dispose();
            this.opcionSelect.dispose();
        }

        public void Select()
        {
            this.select = true;
            this.opcionSelect.Enabled = true;
            this.opcion.Enabled = false;
        }

        public void Unselect()
        {
            this.select = false;
            this.opcionSelect.Enabled = false;
            this.opcion.Enabled = true;
        }

        public bool isSelect()
        {
            return select;
        }
    }
}