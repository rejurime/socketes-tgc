using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Geometry;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Text;
using Microsoft.DirectX.Direct3D;
using TGC.Group;
using TGC.Core.Direct3D;
using System.Windows.Forms;

namespace AlumnoEjemplos.Socketes.Menu
{
	/// <summary>
	/// Ejemplo del alumno
	/// </summary>
	public class MenuInicial
    {
        private TgcText2D titulo;
        private TgcSphere pelota;
        private TgcBox cancha;
        private List<MenuItem> menus;
        //private float tiempoDeAnimacion = 0.1f;
        //private float tiempoTranscurridoDeAnimacion = 0;
        private bool configuracion = false;
        private CustomSprite panelConfiguracion;
        private TgcThirdPersonCamera camara;
        private EjemploAlumno main;
		private Drawer2D drawer2D;

        public MenuInicial(string pathRecursos, TgcThirdPersonCamera camara, EjemploAlumno main)
        {
            this.main = main;
            this.camara = camara;
			this.drawer2D = new Drawer2D();

            //Titulo
            this.titulo = new TgcText2D();
            this.titulo.Text = "Socketes";
            this.titulo.Color = Color.White;
            this.titulo.Align = TgcText2D.TextAlign.CENTER;
            this.titulo.Position = new Point(280, 0);
            this.titulo.Size = new Size(400, 100);
            this.titulo.changeFont(new System.Drawing.Font("Arial", 35));

            //Brazuca
            this.pelota = new TgcSphere();
			//TODO cambiar por matrices
			this.pelota.AutoTransformEnable = true;
            this.pelota.setTexture(TgcTexture.createTexture(pathRecursos + Settings.Default.textureBall));
            this.pelota.Radius = 2.5f;
            this.pelota.LevelOfDetail = 4;
            this.pelota.Position = new Vector3(3, 0, -4);
            this.pelota.updateValues();

            //Cancha donde esta la pelota
            this.cancha = TgcBox.fromSize(new Vector3(20, 0, 20), TgcTexture.createTexture(pathRecursos + Settings.Default.textureMenuField));
            //TODO cambiar por matrices
			this.cancha.AutoTransformEnable = true;
			this.cancha.Position = new Vector3(0, -2.5f, 0);

            //Menu
            this.menus = new List<MenuItem>();
            this.menus.Add(new MenuItem("picadito", new Vector3(-5, 2, 0), new Vector3(8, 1, 0), pathRecursos + Settings.Default.texturePicadito1, pathRecursos + Settings.Default.texturePicadito2));
            this.menus.Add(new MenuItem("configuracion", new Vector3(-5, 0.8f, 0), new Vector3(8, 1, 0), pathRecursos + Settings.Default.textureControles1, pathRecursos + Settings.Default.textureControles2));
            this.menus.Add(new MenuItem("salir", new Vector3(-5, -0.4f, 0), new Vector3(8, 1, 0), pathRecursos + Settings.Default.textureSalir1, pathRecursos + Settings.Default.textureSalir2));

            this.menus[0].Select();

            //Menu de configuracion
            //Crear Sprite
            this.panelConfiguracion = new CustomSprite();
			this.panelConfiguracion.Bitmap = new CustomBitmap(pathRecursos + Settings.Default.texturePanelcontroles, D3DDevice.Instance.Device);
            this.panelConfiguracion.Scaling = new Vector2(0.75f, 0.75f);

			Size textureSize = this.panelConfiguracion.Bitmap.Size;
			this.panelConfiguracion.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height / 2, 0));
        }

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public void render(float elapsedTime)
        {
            //Pongo la camara en posicion
            camara.OffsetForward = -12;
            camara.OffsetHeight = 0;

            //Adelante
            if (this.main.Input.keyPressed(Key.UpArrow))
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
            if (this.main.Input.keyPressed(Key.DownArrow))
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
            if (this.main.Input.keyPressed(Key.Return))
            {
                //TODO esto es muy horrible
                foreach (MenuItem item in this.menus)
                {
                    if (item.isSelect())
                    {
                        if (item.Nombre.Equals("picadito"))
                        {
                            this.main.PantallaActual = 1;
                            return;
                        }
                        if (item.Nombre.Equals("configuracion"))
                        {
                            this.configuracion = true;
                        }
                        if (item.Nombre.Equals("salir"))
                        {
                            this.exit();
                        }
                    }
                }
            }

            if (this.main.Input.keyPressed(Key.BackSpace))
            {
                this.configuracion = false;
            }

            if (configuracion)
            {
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                drawer2D.BeginDrawSprite();

                //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
				drawer2D.DrawSprite(this.panelConfiguracion);

                //Finalizar el dibujado de Sprites
                drawer2D.EndDrawSprite();
            }
            else
            {
                this.pelota.rotateX(Geometry.DegreeToRadian(elapsedTime * 20));
                this.pelota.rotateY(Geometry.DegreeToRadian(elapsedTime * 15));
                this.pelota.rotateZ(Geometry.DegreeToRadian(elapsedTime * 10));

                this.pelota.updateValues();
                this.titulo.render();
                this.pelota.render();
                this.cancha.render();

                foreach (MenuItem item in this.menus)
                {
                    item.render();
                }
            }
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public void close()
        {
            this.titulo.Dispose();
            this.pelota.dispose();
            this.cancha.dispose();

            foreach (MenuItem item in this.menus)
            {
                item.dispose();
            }

            this.panelConfiguracion.Dispose();
        }

        public void exit()
        {
			//FIXME FEO FEO FEO sacar dependencia
			Application.Exit();
            //GuiController.Instance.MainForm.Close();
        }
    }
}