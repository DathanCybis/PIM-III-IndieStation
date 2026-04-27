using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

public class Auth
{
    private static string GetConnectionString()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        return config.GetConnectionString("DefaultConnection")!;
    }


    // Retorna o ID da equipe se o login for bem-sucedido, ou null se falhar
    public static int? LoginEquipe(string nome, string senha)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        connection.Open();

        string sql = "SELECT id FROM equipes WHERE nome_equipe = @nome AND senha_equipe = @senha";
        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.Parameters.AddWithValue("@senha", senha);

        var result = cmd.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : null;
    }


    // Valida se o usuário tem permissões de Administrador
    public static int? LoginAdmin(string nome, string senhaAdmin)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        connection.Open();

        string sql = "SELECT id FROM equipes WHERE nome_equipe = @nome AND senha_admin = @senhaAdmin";
        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.Parameters.AddWithValue("@senhaAdmin", senhaAdmin);

        var result = cmd.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : null;
    }


    public static bool CadastrarEquipe(string nome, string senhaEquipe, string senhaAdmin)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        try
        {
            connection.Open();

            // SQL para inserir a nova equipe
            string sql = @"INSERT INTO equipes (nome_equipe, senha_equipe, senha_admin, saldo_disponivel, total_gasto) 
                        VALUES (@nome, @senhaE, @senhaA, 0.00, 0.00)";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@senhaE", senhaEquipe);
            cmd.Parameters.AddWithValue("@senhaA", senhaAdmin);

            int linhasAfetadas = cmd.ExecuteNonQuery();
            return linhasAfetadas > 0;
        }
        catch (MySqlException ex)
        {
            // Caso o nome da equipe já exista (devido ao UNIQUE no banco)
            if (ex.Number == 1062) 
            {
                Console.WriteLine("Erro: Este nome de equipe já está em uso.");
            }
            else
            {
                Console.WriteLine($"Erro ao cadastrar: {ex.Message}");
            }
            return false;
        }
    }

    public static bool EquipeExiste(string nome)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        connection.Open();
        string sql = "SELECT COUNT(*) FROM equipes WHERE nome_equipe = @nome";
        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@nome", nome);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

}
