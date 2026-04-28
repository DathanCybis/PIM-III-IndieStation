using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

public class NotasManager
{
    private static string GetConnectionString()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        return config.GetConnectionString("DefaultConnection")!;
    }


    public static void AdicionarNota(int equipeId, string titulo, string descricao)
    {
        // Validação solicitada: Não podem estar vazios
        if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descricao))
        {
            Console.WriteLine("\n[ERRO] O título e a descrição são obrigatórios!");
            return;
        }

        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        string sql = "INSERT INTO notas_atualizacao (equipe_id, titulo, descricao) VALUES (@id, @titulo, @desc)";
        
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", equipeId);
        cmd.Parameters.AddWithValue("@titulo", titulo);
        cmd.Parameters.AddWithValue("@desc", descricao);
        
        cmd.ExecuteNonQuery();
        Console.WriteLine("\n[SUCESSO] Nota de atualização publicada!");
    }


    public static void EditarNota(int notaId, int equipeId, string novoTitulo, string novaDescricao)
    {
        if (string.IsNullOrWhiteSpace(novoTitulo) || string.IsNullOrWhiteSpace(novaDescricao))
        {
            Console.WriteLine("\n[ERRO] O título e a descrição não podem ficar vazios!");
            return;
        }

        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        
        string sql = "UPDATE notas_atualizacao SET titulo = @titulo, descricao = @desc WHERE id = @notaId AND equipe_id = @equipeId";
        
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@titulo", novoTitulo);
        cmd.Parameters.AddWithValue("@desc", novaDescricao);
        cmd.Parameters.AddWithValue("@notaId", notaId);
        cmd.Parameters.AddWithValue("@equipeId", equipeId);

        int linhas = cmd.ExecuteNonQuery();
        if (linhas > 0) Console.WriteLine("\n[SUCESSO] Nota atualizada!");
        else Console.WriteLine("\n[ERRO] Nota não encontrada ou sem permissão.");
    }


    public static void ExcluirNota(int notaId, int equipeId)
    {
        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        
        string sql = "DELETE FROM notas_atualizacao WHERE id = @notaId AND equipe_id = @equipeId";
        
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@notaId", notaId);
        cmd.Parameters.AddWithValue("@equipeId", equipeId);

        int linhas = cmd.ExecuteNonQuery();
        if (linhas > 0) Console.WriteLine("\n[SUCESSO] Nota excluída!");
        else Console.WriteLine("\n[ERRO] Falha ao excluir nota.");
    }

    public static void ListarNotas(int equipeId)
    {
        using var conn = new MySqlConnection(GetConnectionString());
        conn.Open();
        string sql = "SELECT id, titulo, descricao, data_criacao FROM notas_atualizacao WHERE equipe_id = @id ORDER BY data_criacao DESC";
        
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", equipeId);
        
        using var reader = cmd.ExecuteReader();
        
        Console.WriteLine("\n--- NOTAS DE ATUALIZAÇÃO (ATT) ---");
        bool temNotas = false;
        while (reader.Read())
        {
            temNotas = true;
            DateTime data = Convert.ToDateTime(reader["data_criacao"]);
            Console.WriteLine($"ID: {reader["id"]} | [{data:dd/MM/yyyy HH:mm}] {reader["titulo"]}");
            Console.WriteLine($"> {reader["descricao"]}");
            Console.WriteLine("----------------------------------");
        }

        if (!temNotas) Console.WriteLine("Nenhuma nota de atualização encontrada.");
    }
}
