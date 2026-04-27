using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

public class EquipeManager
{
    private static string GetConnectionString()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        return config.GetConnectionString("DefaultConnection")!;
    }

    // --- GESTÃO DE MEMBROS ---

    public static void AdicionarMembro(int equipeId, string nome)
    {
        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        string sql = "INSERT INTO membros (equipe_id, nome_membro) VALUES (@id, @nome)";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", equipeId);
        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.ExecuteNonQuery();
        Console.WriteLine("Membro adicionado com sucesso!");
    }

    public static void ListarMembros(int equipeId)
    {
        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        string sql = "SELECT id, nome_membro FROM membros WHERE equipe_id = @id";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", equipeId);
        using var reader = cmd.ExecuteReader();
        
        Console.WriteLine("\n--- MEMBROS DA EQUIPE ---");
        while (reader.Read())
        {
            Console.WriteLine($"ID: {reader["id"]} | Nome: {reader["nome_membro"]}");
        }
    }

    public static void ExcluirMembro(string membroId)
    {
        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        string sql = "DELETE FROM membros WHERE id = @id";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", membroId);
        cmd.ExecuteNonQuery();
        Console.WriteLine("Membro removido.");
    }

    // --- GESTÃO DA EQUIPE ---

    public static void DetalhesEquipe(int equipeId)
    {
        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        // SQL que conta quantos membros a equipe tem usando COUNT
        string sql = @"SELECT e.nome_equipe, COUNT(m.id) as total_membros 
                       FROM equipes e 
                       LEFT JOIN membros m ON e.id = m.equipe_id 
                       WHERE e.id = @id GROUP BY e.id";
        
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", equipeId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Console.WriteLine($"\nEquipe: {reader["nome_equipe"]}");
            Console.WriteLine($"Total de Membros: {reader["total_membros"]}");
        }
    }

    public static void ExcluirEquipe(int equipeId)
    {
        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        string sql = "DELETE FROM equipes WHERE id = @id";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", equipeId);
        cmd.ExecuteNonQuery();
        Console.WriteLine("Equipe e todos os seus dados foram excluídos.");
    }
}