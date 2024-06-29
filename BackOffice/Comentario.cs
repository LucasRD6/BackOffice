using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice
{
    class Comentario
    {
        public int idComentario { get; set; }
        public string comentario { get; set; }
        public int idDePost { get; set; }
        public int idAutor { get; set; } // referencia a usuario (idUsuario)

        
        
    }
}
