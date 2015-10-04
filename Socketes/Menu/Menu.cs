using AlumnoEjemplos.Properties;
using AlumnoEjemplos.Socketes.Model;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class Menu
    {
        private bool enable;
        private TgcText2d titulo;
        private TgcSphere pelota;
        private TgcBox cancha;
        private List<MenuItem> menus;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        public Menu(string pathRecursos, TgcThirdPersonCamera camara)
        {
            this.enable = true;

            //Titulo
            this.titulo = new TgcText2d();
            this.titulo.Text = "Socketes";
            this.titulo.Color = Color.White;
            this.titulo.Align = TgcText2d.TextAlign.CENTER;
            this.titulo.Position = new Point(280, 0);
            this.titulo.Size = new Size(400, 100);
            this.titulo.changeFont(new System.Drawing.Font("Arial", 35));

            //Brazuca
            this.pelota = new TgcSphere();
            this.pelota.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + Settings.Default.textureBall));
            this.pelota.Radius = 2.5f;
            this.pelota.LevelOfDetail = 4;
            this.pelota.Position = new Vector3(3, 0, -4);
            this.pelota.updateValues();

            //Cancha donde esta la pelota
            this.cancha = TgcBox.fromSize(new Vector3(20, 0, 20), TgcTexture.createTexture(pathRecursos + Settings.Default.textureFolder + "canchaMenu.jpg"));
            this.cancha.Position = new Vector3(0, -2.5f, 0);

            //Menu
            this.menus = new List<MenuItem>();
            this.menus.Add(new MenuItem(new Vector3(-5, 2, 0), new Vector3(8, 1, 0), pathRecursos + "Menu\\picadito.png", pathRecursos + "Menu\\picadito-seleccionado.png"));
            this.menus.Add(new MenuItem(new Vector3(-5, 0.8f, 0), new Vector3(8, 1, 0), pathRecursos + "Menu\\opciones.png", pathRecursos + "Menu\\opciones-seleccionado.png"));
            this.menus.Add(new MenuItem(new Vector3(-5, -0.4f, 0), new Vector3(8, 1, 0), pathRecursos + "Menu\\salir.png", pathRecursos + "Menu\\salir-seleccionado.png"));
            this.menus[0].Select();

            //Pongo la camara en posicion
            camara.OffsetForward = -12;
            camara.OffsetHeight = 0;
        }

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public void render(float elapsedTime)
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            //Adelante
            if (d3dInput.keyDown(Key.UpArrow))
            {
                for (int i = 0; i < this.menus.Count; i++)
                {
                    if (this.menus[i].isSelect())
                    {
                        this.menus[i].Unselect();

                        if (i == 0)
                        {
                            this.menus[this.menus.Count - 1].Select();
                        }
                        else
                        {
                            this.menus[i - 1].Select();
                        }
                        break;
                    }
                }
            }

            //Atras
            if (d3dInput.keyDown(Key.DownArrow))
            {
                for (int i = 0; i < this.menus.Count; i++)
                {
                    if (this.menus[i].isSelect())
                    {
                        this.menus[i].Unselect();

                        if (i == this.menus.Count - 1)
                        {
                            this.menus[0].Select();
                        }
                        else
                        {
                            this.menus[i + 1].Select();
                        }
                        break;
                    }
                }
            }

            //Enter
            if (d3dInput.keyDown(Key.Return))
            {
                this.close();
                return;
            }

            this.titulo.render();
            this.pelota.render();
            this.cancha.render();

            foreach (MenuItem item in this.menus)
            {
                item.render();
            }
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public void close()
        {
            this.titulo.dispose();
            this.pelota.dispose();
            this.cancha.dispose();

            foreach (MenuItem item in this.menus)
            {
                item.dispose();
            }

            this.enable = false;
        }
    }
}