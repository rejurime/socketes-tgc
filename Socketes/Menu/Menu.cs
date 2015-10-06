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

namespace AlumnoEjemplos.Socketes.Menu
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
        private float tiempoDeAnimacion = 0.1f;
        private float tiempoTranscurridoDeAnimacion = 0;

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
            this.menus.Add(new MenuItem("picadito", new Vector3(-5, 2, 0), new Vector3(8, 1, 0), pathRecursos + Settings.Default.picadito1, pathRecursos + Settings.Default.picadito2));
            this.menus.Add(new MenuItem("configuracion", new Vector3(-5, 0.8f, 0), new Vector3(8, 1, 0), pathRecursos + Settings.Default.opciones1, pathRecursos + Settings.Default.opciones2));
            this.menus.Add(new MenuItem("salir", new Vector3(-5, -0.4f, 0), new Vector3(8, 1, 0), pathRecursos + Settings.Default.salir1, pathRecursos + Settings.Default.salir2));

            this.menus[0].Select();

            //Pongo la camara en posicion
            camara.OffsetForward = -12;
            camara.OffsetHeight = 0;
        }

        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public void render(float elapsedTime)
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            //TODO hay que mejorar esto porque no anda bien pero por lo menos ahora es usable, el problema de la dependencia de frames.
            //Tiene que haber alguna forma mejor y sino por lo menos hay que mejorarlo capturando el Keyup no solo press
            if (this.tiempoTranscurridoDeAnimacion >= this.tiempoDeAnimacion)
            {
                this.tiempoTranscurridoDeAnimacion = 0;

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
            }
            else
            {
                this.tiempoTranscurridoDeAnimacion += elapsedTime;
            }

            //Enter
            if (d3dInput.keyDown(Key.Return))
            {
                //TODO esto es muy horrible
                foreach (MenuItem item in this.menus)
                {
                    if (item.isSelect())
                    {
                        if (item.Nombre.Equals("picadito"))
                        {
                            this.close();
                            return;
                        }
                        if (item.Nombre.Equals("conifguracion"))
                        {
                            return;
                        }
                        if (item.Nombre.Equals("salir"))
                        {
                            return;
                        }
                    }
                }
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
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
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