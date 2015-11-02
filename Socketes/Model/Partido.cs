using AlumnoEjemplos.Socketes.Model.ElementosCancha;
using AlumnoEjemplos.Socketes.Model.Jugadores;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Socketes.Model
{
    /// <summary> Clase que tiene todo lo que se esta haciendo render</summary>
    public class Partido
    {
        #region Miembros

        private bool mostrarBounding;
        private bool inteligenciaArtificial;
        private bool luz;
        private Marcador marcador;
        private Cancha cancha;
        private Pelota pelota;
        private Arco arcoLocal;
        private Arco arcoVisitante;
        private Equipo equipoLocal;
        private Equipo equipoVisitante;

        private Dictionary<string, TgcStaticSound> sonidos;

        //FIXME sacar esto en cuanto se pueda NO A LOS SINGLETONS
        private static readonly Partido instance = new Partido();

        #endregion

        #region  Constructores

        /// <summary> Constructor privado para poder hacer el singleton</summary>
        private Partido() { }

        #endregion

        #region Propiedades

        public Cancha Cancha
        {
            get { return cancha; }
            set { cancha = value; }
        }

        public Pelota Pelota
        {
            get { return pelota; }
            set { pelota = value; }
        }

        public Arco ArcoLocal
        {
            get { return arcoLocal; }
            set { arcoLocal = value; }
        }

        public Arco ArcoVisitante
        {
            get { return arcoVisitante; }
            set { arcoVisitante = value; }
        }

        public Equipo EquipoLocal
        {
            get { return equipoLocal; }
            set { equipoLocal = value; }
        }

        public Equipo EquipoVisitante
        {
            get { return equipoVisitante; }
            set { equipoVisitante = value; }
        }

        public Marcador Marcador
        {
            get { return marcador; }
            set { marcador = value; }
        }

        public bool MostrarBounding
        {
            get { return mostrarBounding; }
            set
            {
                mostrarBounding = value;
                this.cancha.MostrarBounding = value;
                this.pelota.MostrarBounding = value;
                this.arcoLocal.MostrarBounding = value;
                this.arcoVisitante.MostrarBounding = value;
                this.equipoLocal.MostrarBounding = value;
                this.equipoVisitante.MostrarBounding = value;
            }
        }

        public bool InteligenciaArtificial
        {
            get { return inteligenciaArtificial; }
            set
            {
                inteligenciaArtificial = value;
                this.equipoLocal.InteligenciaArtificial = value;
                this.equipoVisitante.InteligenciaArtificial = value;
            }
        }

        public static Partido Instance
        {
            get { return instance; }
        }

        public Dictionary<string, TgcStaticSound> Sonidos
        {
            get { return sonidos; }
            set { sonidos = value; }
        }

        public bool Luz
        {
            get { return luz; }
            set { luz = value; }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public void render(float elapsedTime)
        {
            this.marcador.render(this.equipoLocal.Goles, this.equipoVisitante.Goles);
            this.cancha.render();
            this.arcoLocal.render();
            this.arcoVisitante.render();

            //sombras
            this.equipoLocal.renderShadow(elapsedTime, this.cancha.Luces);
            this.equipoVisitante.renderShadow(elapsedTime, this.cancha.Luces);
            this.pelota.renderShadow(elapsedTime, this.cancha.Luces);

            //Luces
            Effect currentShader;
            Pelota mesh = this.pelota;

            if (this.luz)
            {
                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight para Skeletal Mesh
                currentShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcSkeletalMeshShader;
            }

            //Aplicar al mesh el shader actual
            mesh.Effect = currentShader;
            //El Technique depende del tipo RenderType del mesh
            //mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(mesh.RenderType);
            mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(0);

            //Renderizar mesh
            if (this.luz)
            {
                //Cargar variables shader de la luz
                mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(new Vector3(60, 35, 250)));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.FpsCamera.getPosition()));
                mesh.Effect.SetValue("lightIntensity", 20);
                mesh.Effect.SetValue("lightAttenuation", 0.3f);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularExp", 9f);
            }


            //objetos locos

            this.equipoLocal.render(elapsedTime);
            this.equipoVisitante.render(elapsedTime);
            this.pelota.updateValues(elapsedTime);
            this.pelota.render();
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public void dispose()
        {
            this.marcador.dispose();
            this.cancha.dispose();
            this.pelota.dispose();
            this.arcoLocal.dispose();
            this.arcoVisitante.dispose();
            this.equipoLocal.dispose();
            this.equipoVisitante.dispose();
        }

        //TODO De aca para abajo hay que ver como llegar aca por ahora se llega con el singleton PUAJIS
        public void NotificarPelotaDominada(Jugador jugador)
        {
            this.equipoLocal.NotificarPelotaDominada(jugador);
            this.equipoVisitante.NotificarPelotaDominada(jugador);
        }

        public void NotificarGol(Red red)
        {
            if (this.equipoLocal.ArcoPropio.Red.Equals(red))
            {
                this.equipoVisitante.Goles += 1;
            }
            else
            {
                this.equipoLocal.Goles += 1;
            }

            this.ReiniciarPosiciones();
        }

        public void ReiniciarPosiciones()
        {
            this.equipoLocal.ReiniciarPosiciones();
            this.equipoVisitante.ReiniciarPosiciones();
            this.pelota.ReiniciarPosicion();
        }

        #endregion
    }
}