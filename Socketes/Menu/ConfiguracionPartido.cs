﻿using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.Socketes.Menu
{
    public class ConfiguracionPartido
    {
        private TgcText2d titulo;
        private List<MenuItem> menus;
        private List<TgcSphere> pelotas;
        private List<TgcSkeletalMesh> jugadores;
        private List<TgcText2d> camaras;
        private int jugadorLocal;
        private int jugadorVisitante;
        private int pelota;
        private int camaraSeleccionada;
        private Vector3 positionJugadorLocal;
        private Vector3 positionJugadorVisitante;
        TgcThirdPersonCamera camara;
        private EjemploAlumno main;

        public ConfiguracionPartido(string pathRecursos, Size screenSize, TgcThirdPersonCamera camara, EjemploAlumno main)
        {
            //Titulo
            this.titulo = new TgcText2d();
            this.titulo.Text = "Configuración del partido";
            this.titulo.Color = Color.White;
            this.titulo.Align = TgcText2d.TextAlign.CENTER;
            this.titulo.Size = new Size(400, 100);
            this.titulo.changeFont(new System.Drawing.Font("Arial", 24));
            this.titulo.Position = new Point((screenSize.Width - this.titulo.Size.Width) / 2, 20);

            //Menu
            this.menus = new List<MenuItem>();
            this.menus.Add(new MenuItem("listo", new Vector3(-7, 4, 0), new Vector3(14, 2, 0), pathRecursos + "Menu\\listo.png", pathRecursos + "Menu\\listo-seleccionado.png"));
            this.menus.Add(new MenuItem("uniformeLocal", new Vector3(-7, 1, 0), new Vector3(14, 4, 0), pathRecursos + "Menu\\local.png", pathRecursos + "Menu\\local-seleccionado.png"));
            this.menus.Add(new MenuItem("uniformeVisitante", new Vector3(-7, -3, 0), new Vector3(14, 4, 0), pathRecursos + "Menu\\visitante.png", pathRecursos + "Menu\\visitante-seleccionado.png"));
            this.menus.Add(new MenuItem("pelota", new Vector3(7, 3.2f, 0), new Vector3(14, 4, 0), pathRecursos + "Menu\\pelota.png", pathRecursos + "Menu\\pelota-seleccionado.png"));
            this.menus.Add(new MenuItem("camara", new Vector3(7, 0.2f, 0), new Vector3(14, 2, 0), pathRecursos + "Menu\\camara.png", pathRecursos + "Menu\\camara-seleccionado.png"));
            this.menus.Add(new MenuItem("volver", new Vector3(7, -2.2f, 0), new Vector3(14, 2, 0), pathRecursos + "Menu\\volver.png", pathRecursos + "Menu\\volver-seleccionado.png"));

            //Pelotas
            this.pelotas = new List<TgcSphere>();
            this.pelotas.Add(this.CrearPelota(pathRecursos, new Vector3(10, 3.2f, 0), TgcTexture.createTexture(pathRecursos + Settings.Default.textureBall)));
            this.pelotas.Add(this.CrearPelota(pathRecursos, new Vector3(10, 3.2f, 0), TgcTexture.createTexture(pathRecursos + "Texturas\\pelota2.jpg")));
            this.pelotas.Add(this.CrearPelota(pathRecursos, new Vector3(10, 3.2f, 0), TgcTexture.createTexture(pathRecursos + "Texturas\\pelota3.jpg")));

            //Jugadores
            this.jugadores = new List<TgcSkeletalMesh>();
            this.jugadores.Add(this.CrearJugador(pathRecursos, TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.textureTeam1)));
            this.jugadores.Add(this.CrearJugador(pathRecursos, TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.textureTeam2)));
            this.jugadores.Add(this.CrearJugador(pathRecursos, TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + "Textures\\uvw.jpg")));
            this.jugadores.Add(this.CrearJugador(pathRecursos, TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + "Textures\\uvwBlack.png")));
            this.jugadores.Add(this.CrearJugador(pathRecursos, TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + "Textures\\uvwOrange.png")));
            this.jugadores.Add(this.CrearJugador(pathRecursos, TgcTexture.createTexture(pathRecursos + Settings.Default.meshFolderPlayer + "Textures\\uvwViolet.png")));

            //Camaras
            this.camaras = new List<TgcText2d>();
            this.camaras.Add(this.CrearCamara("Pelota", screenSize));
            //this.camaras.Add(this.CrearCamara("Jugador", screenSize));
            this.camaras.Add(this.CrearCamara("Aérea", screenSize));

            this.menus[0].Select();
            this.pelota = 0;
            this.jugadorLocal = 0;
            this.jugadorVisitante = 1;
            this.camaraSeleccionada = 0;

            this.positionJugadorLocal = new Vector3(-4, -0.1f, 0);
            this.positionJugadorVisitante = new Vector3(-4, -4, 0);

            this.camara = camara;
            this.main = main;
        }

        public void render(float elapsedTime)
        {
            //Pongo la camara en posicion
            this.camara.OffsetForward = -20;
            this.camara.OffsetHeight = 0;

            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            if (d3dInput.keyPressed(Key.UpArrow))
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

            if (d3dInput.keyPressed(Key.DownArrow))
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
            if (d3dInput.keyPressed(Key.Return) && !this.main.CambiandoPantalla)
            {
                //TODO esto es muy horrible
                foreach (MenuItem item in this.menus)
                {
                    if (item.isSelect())
                    {
                        if (item.Nombre.Equals("listo"))
                        {
                            this.main.PantallaActual = 2;
                            this.main.CambiandoPantalla = true;
                            return;
                        }
                        if (item.Nombre.Equals("volver"))
                        {
                            this.main.PantallaActual = 0;
                            this.main.CambiandoPantalla = true;
                            return;
                        }
                    }
                }
            }

            if (d3dInput.keyPressed(Key.LeftArrow))
            {
                //TODO esto es muy horrible
                foreach (MenuItem item in this.menus)
                {
                    if (item.isSelect())
                    {
                        if (item.Nombre.Equals("uniformeLocal"))
                        {
                            if (this.jugadorLocal > 0)
                            {
                                this.jugadorLocal -= 1;
                            }
                            else
                            {
                                this.jugadorLocal = this.jugadores.Count - 1;
                            }
                        }
                        if (item.Nombre.Equals("uniformeVisitante"))
                        {
                            if (this.jugadorVisitante > 0)
                            {
                                this.jugadorVisitante -= 1;
                            }
                            else
                            {
                                this.jugadorVisitante = this.jugadores.Count - 1;
                            }
                        }
                        if (item.Nombre.Equals("pelota"))
                        {
                            if (this.pelota > 0)
                            {
                                this.pelota -= 1;
                            }
                            else
                            {
                                this.pelota = this.pelotas.Count - 1;
                            }
                        }
                        if (item.Nombre.Equals("camara"))
                        {
                            if (this.camaraSeleccionada > 0)
                            {
                                this.camaraSeleccionada -= 1;
                            }
                            else
                            {
                                this.camaraSeleccionada = this.camaras.Count - 1;
                            }
                        }
                    }
                }
            }

            if (d3dInput.keyPressed(Key.RightArrow))
            {
                //TODO esto es muy horrible
                foreach (MenuItem item in this.menus)
                {
                    if (item.isSelect())
                    {
                        if (item.Nombre.Equals("uniformeLocal"))
                        {
                            if (this.jugadorLocal < this.jugadores.Count - 1)
                            {
                                this.jugadorLocal += 1;
                            }
                            else
                            {
                                this.jugadorLocal = 0;
                            }
                        }
                        if (item.Nombre.Equals("uniformeVisitante"))
                        {
                            if (this.jugadorVisitante < this.jugadores.Count - 1)
                            {
                                this.jugadorVisitante += 1;
                            }
                            else
                            {
                                this.jugadorVisitante = 0;
                            }
                        }
                        if (item.Nombre.Equals("pelota"))
                        {
                            if (this.pelota < this.pelotas.Count - 1)
                            {
                                this.pelota += 1;
                            }
                            else
                            {
                                this.pelota = 0;
                            }
                        }
                        if (item.Nombre.Equals("camara"))
                        {
                            if (this.camaraSeleccionada < this.camaras.Count - 1)
                            {
                                this.camaraSeleccionada += 1;
                            }
                            else
                            {
                                this.camaraSeleccionada = 0;
                            }
                        }
                    }
                }
            }

            this.titulo.render();

            GuiController.Instance.D3dDevice.RenderState.ZBufferEnable = false;

            foreach (MenuItem item in this.menus)
            {
                item.render();
            }

            GuiController.Instance.D3dDevice.RenderState.ZBufferEnable = true;

            for (int i = 0; i < this.jugadores.Count; i++)
            {
                if (this.jugadorLocal == i)
                {
                    this.jugadores[i].Position = this.positionJugadorLocal;
                    this.jugadores[i].animateAndRender();
                }
            }

            for (int i = 0; i < this.jugadores.Count; i++)
            {
                if (this.jugadorVisitante == i)
                {
                    this.jugadores[i].Position = this.positionJugadorVisitante;
                    this.jugadores[i].animateAndRender();
                }
            }

            for (int i = 0; i < this.pelotas.Count; i++)
            {
                if (this.pelota == i)
                {
                    this.pelotas[i].rotateY(Geometry.DegreeToRadian(elapsedTime * 40));
                    this.pelotas[i].updateValues();
                    this.pelotas[i].render();
                }
            }

            for (int i = 0; i < this.camaras.Count; i++)
            {
                if (this.camaraSeleccionada == i)
                {
                    this.camaras[i].render();
                }
            }
        }

        public void close()
        {
            this.titulo.dispose();

            foreach (MenuItem menu in this.menus)
            {
                menu.dispose();
            }

            foreach (TgcSphere pelota in this.pelotas)
            {
                pelota.dispose();
            }

            foreach (TgcSkeletalMesh jugador in this.jugadores)
            {
                jugador.dispose();
            }

            foreach (TgcText2d camara in this.camaras)
            {
                camara.dispose();
            }
        }

        public TgcSphere CrearPelota(string pathRecursos, Vector3 position, TgcTexture texturaPelota)
        {
            float radio = 1;

            //Crear esfera
            TgcSphere sphere = new TgcSphere();
            sphere.setTexture(texturaPelota);
            sphere.Radius = radio;
            sphere.Position = position;
            sphere.updateValues();

            return sphere;
        }

        private TgcSkeletalMesh CrearJugador(string pathRecursos, TgcTexture texturaJugador)
        {
            //Cargar personaje con animaciones
            TgcSkeletalMesh personaje = new TgcSkeletalLoader().loadMeshAndAnimationsFromFile(
                pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.meshFilePlayer,
                pathRecursos + Settings.Default.meshFolderPlayer,
                new string[] { pathRecursos + Settings.Default.meshFolderPlayer + Settings.Default.animationStopFilePlayer });

            //Le cambiamos la textura
            personaje.changeDiffuseMaps(new TgcTexture[] { texturaJugador });

            //Configurar animacion inicial
            personaje.playAnimation(Settings.Default.animationStopPlayer, true);

            //Lo Escalo porque es muy grande
            personaje.Scale = new Vector3(0.018f, 0.018f, 0.018f);

            return personaje;
        }

        private TgcText2d CrearCamara(string texto, Size screenSize)
        {
            TgcText2d camara = new TgcText2d();
            camara.Text = texto;
            camara.Color = Color.White;
            camara.Align = TgcText2d.TextAlign.CENTER;
            camara.Size = new Size(400, 100);
            camara.changeFont(new System.Drawing.Font("Arial", 14));
            camara.Position = new Point(screenSize.Width - camara.Size.Width + 35, 242);

            return camara;
        }

        public TgcSphere GetPelotaActual()
        {
            return this.pelotas[this.pelota];
        }

        public TgcSkeletalMesh GetJugadorLocalActual()
        {
            return this.jugadores[this.jugadorLocal];
        }

        public TgcSkeletalMesh GetJugadorVisitanteActual()
        {
            return this.jugadores[this.jugadorVisitante];
        }

        public string GetCamara()
        {
            return this.camaras[this.camaraSeleccionada].Text;
        }
    }
}