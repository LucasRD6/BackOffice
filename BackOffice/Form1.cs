using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace BackOffice
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargarLista();

        }

        private void listaUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnGetPost_Click(object sender, EventArgs e)
        {

        }

        private void cargarLista()
        {
            // Configura la conexión a la base de datos
            string connectionString = "server=localhost;port=3306;database=base_prueba;user=root;password=;";

            // Crea una lista para almacenar los usuarios
            List<Usuario> usuarios = new List<Usuario>();

            // Establece la conexión y realiza la consulta
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT idUsuario, nombreUsuario FROM usuarios;";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                idUsuario = Convert.ToInt32(reader["idUsuario"]),
                                nombreUsuario = reader["nombreUsuario"].ToString(),

                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Mostrar usuarios en la ListBox
            foreach (var usuario in usuarios)
            {
                listaUsuarios.Items.Add($"ID: {usuario.idUsuario}, Nombre: {usuario.nombreUsuario}");
            }
        }

        private void btnBuscarUser_Click(object sender, EventArgs e)
        {
            string searchText = txtBuscarUser.Text.Trim().ToLower();

            // Buscar el ítem que coincida con el texto buscado
            for (int i = 0; i < listaUsuarios.Items.Count; i++)
            {
                string itemText = listaUsuarios.Items[i].ToString().ToLower();
                if (itemText.Contains(searchText))
                {
                    listaUsuarios.SelectedIndex = i; // Seleccionar el ítem encontrado
                    return;
                }
            }

            // Si no se encuentra, podrías manejar este caso según tu lógica (por ejemplo, mostrar un mensaje).
            MessageBox.Show("No existe un usuario que contenga esos caracteres.");
        }

        private void listaUsuarios_DoubleClick(object sender, EventArgs e)
        {
            if (listaUsuarios.SelectedItem != null)
            {
                string selectedItem = listaUsuarios.SelectedItem.ToString();
                int idUsuario = Convert.ToInt32(selectedItem.Split(',')[0].Split(':')[1].Trim());

                string connectionString = "server=localhost;port=3306;database=base_prueba;user=root;password=;";
                List<Publicacion> publicaciones = new List<Publicacion>();

                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT usuarios.idUsuario AS id_usuarios, publicaciones.idPost AS id_publicaciones, " +
                                       "publicaciones.nombrePost AS nombre_publicaciones FROM publicaciones " +
                                       "INNER JOIN usuarios ON publicaciones.idDeUsuario = usuarios.idUsuario " +
                                       "WHERE usuarios.idUsuario = @idUsuario";

                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                publicaciones.Add(new Publicacion
                                {
                                    idPost = Convert.ToInt32(reader["id_publicaciones"]),
                                    nombrePost = reader["nombre_publicaciones"].ToString(),
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

                listaPosts.Items.Clear();
                listaComentarios.Items.Clear();
                fotoPost.Image = null;
                foreach (var publicacion in publicaciones)
                {
                    listaPosts.Items.Add($"ID: {publicacion.idPost}, Nombre: {publicacion.nombrePost}");
                }
            }


        }



        private void listaPosts_DoubleClick(object sender, EventArgs e)
        {
            if (listaPosts.SelectedItem != null)
            {
                string selectedItem = listaPosts.SelectedItem.ToString();
                int idPost = Convert.ToInt32(selectedItem.Split(',')[0].Split(':')[1].Trim());

                string connectionString = "server=localhost;port=3306;database=base_prueba;user=root;password=;";

                List<Comentario> comentarios = new List<Comentario>();
                List<int> idsPosts = new List<int>(); // Lista para almacenar los IDs de los posts

                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT usuarios.idUsuario AS id_usuarios, publicaciones.idPost AS id_publicaciones, " +
                                       "comentario.comentario AS comentario_comentario, comentario.idComentario AS id_comentario " +
                                       "FROM comentario " +
                                       "INNER JOIN usuarios ON comentario.idAutor = usuarios.idUsuario " +
                                       "INNER JOIN publicaciones ON comentario.idDePost = publicaciones.idPost " +
                                       "WHERE publicaciones.idPost = @idPost";

                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@idPost", idPost);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comentarios.Add(new Comentario
                                {
                                    idComentario = Convert.ToInt32(reader["id_comentario"]),
                                    comentario = reader["comentario_comentario"].ToString(),
                                });
                            }
                        }

                        // Agregar el ID del post actual a la lista
                        idsPosts.Add(idPost);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

                listaComentarios.Items.Clear();
                foreach (var comentario in comentarios)
                {
                    listaComentarios.Items.Add($"ID: {comentario.idComentario}, Cuerpo: {comentario.comentario}");
                }

                foreach (int idImagen in idsPosts)
                {
                    try
                    {
                        using (var connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "SELECT imagen FROM Imagenes WHERE idImagen = @id";
                            MySqlCommand command = new MySqlCommand(query, connection);

                            command.Parameters.AddWithValue("@id", idImagen);

                            byte[] imageBytes = (byte[])command.ExecuteScalar();

                            if (imageBytes != null)
                            {
                                fotoPost.Image = ByteArrayToImage(imageBytes); // Esto debería ajustarse si hay múltiples fotos
                            }
                            else
                            {
                                MessageBox.Show($"No se encontró la imagen para el post con ID: {idImagen}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar la imagen para el post con ID: {idImagen}. Detalles: {ex.Message}");
                    }
                }
            }
        




    }
    private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private void btnEliminarPost_Click(object sender, EventArgs e)
        {
            listaPosts.Items.RemoveAt(listaPosts.SelectedIndex);
        }

        private void btnEliminarComentarios_Click(object sender, EventArgs e)
        {
            listaComentarios.Items.RemoveAt(listaComentarios.SelectedIndex);
        }

        private void btnBuscaPost_Click(object sender, EventArgs e)
        {
            string searchText = txtBuscarPost.Text.Trim().ToLower();

            
            for (int i = 0; i < listaPosts.Items.Count; i++)
            {
                string itemText = listaPosts.Items[i].ToString().ToLower();
                if (itemText.Contains(searchText))
                {
                    listaPosts.SelectedIndex = i; // Seleccionar el ítem encontrado
                    return;
                }
            }

            
            MessageBox.Show("No existe un post que contenga esos caracteres.");
        }

        private void btnBuscaComentarios_Click(object sender, EventArgs e)
        {
            string searchText = txtBuscarComentario.Text.Trim().ToLower();

           
            for (int i = 0; i < listaComentarios.Items.Count; i++)
            {
                string itemText = listaComentarios.Items[i].ToString().ToLower();
                if (itemText.Contains(searchText))
                {
                    listaComentarios.SelectedIndex = i; // Seleccionar el ítem encontrado
                    return;
                }
            }

           
            MessageBox.Show("No existe un comentario que contenga esos caracteres");
        }

        private void fotoPost_Click(object sender, EventArgs e)
        {

        }

        private void btnSeleccionarImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fotoPost.Image = Image.FromFile(openFileDialog.FileName);
                }
            }
        }

        private void btnSubirImagen_Click(object sender, EventArgs e)
        {
            if (fotoPost.Image != null)
            {
                // Convierte la imagen a un arreglo de bytes
                byte[] imageBytes = ImageToByteArray(fotoPost.Image);

       
                int idPostAsociado = ObtenerIdPostAsociado(); 
                int idAutorPost = ObtenerIdAutorPost(); 

                string connectionString = "server=localhost;port=3306;database=base_prueba;user=root;password=;";

                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "INSERT INTO Imagenes (nombre, imagen, idPostAsociado, idAutorPost) VALUES (@nombre, @imagen, @idPostAsociado, @idAutorPost)";
                        MySqlCommand command = new MySqlCommand(query, connection);

                        command.Parameters.AddWithValue("@nombre", "Nombre de la imagen");
                        command.Parameters.AddWithValue("@imagen", imageBytes);
                        command.Parameters.AddWithValue("@idPostAsociado", idPostAsociado);
                        command.Parameters.AddWithValue("@idAutorPost", idAutorPost);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Imagen subida correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Primero selecciona una imagen.");
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        private int ObtenerIdPostAsociado()
        {
            if (listaPosts.SelectedItem != null)
            {
                string selectedItem = listaPosts.SelectedItem.ToString();
                int idPost = Convert.ToInt32(selectedItem.Split(',')[0].Split(':')[1].Trim());
                return idPost;
            }
            else
            {
                throw new Exception("No hay ningún post seleccionado.");
            }
        }


        private int ObtenerIdAutorPost()
        {
            if (listaUsuarios.SelectedItem != null)
            {
                string selectedItem = listaUsuarios.SelectedItem.ToString();
                int idUsuario = Convert.ToInt32(selectedItem.Split(',')[0].Split(':')[1].Trim());
                return idUsuario;
            }
            else
            {
                throw new Exception("No hay ningún usuario seleccionado.");
            }
        }



    }

}
    

