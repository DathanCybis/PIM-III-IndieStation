using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

public class Database
{
    private static string GetConnectionString()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        return config.GetConnectionString("DefaultConnection")!;
    }

    public static void Inicializar()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        connection.Open();

        // Tabela de Equipes (com senha da equipe e do admin)
        string sqlEquipes = @"
            CREATE TABLE IF NOT EXISTS equipes (
                id INT AUTO_INCREMENT PRIMARY KEY,
                nome_equipe VARCHAR(100) NOT NULL UNIQUE,
                senha_equipe VARCHAR(255) NOT NULL,
                senha_admin VARCHAR(255) NOT NULL,
                saldo_disponivel DECIMAL(10, 2) DEFAULT 0.00,
                total_gasto DECIMAL(10, 2) DEFAULT 0.00
            );";

        // Tabela de Tarefas (Kanban)
        string sqlTarefas = @"
            CREATE TABLE IF NOT EXISTS tarefas (
                id INT AUTO_INCREMENT PRIMARY KEY,
                equipe_id INT,
                titulo VARCHAR(100),
                descricao TEXT,
                status ENUM('Backlog', 'Fazendo', 'Concluído') DEFAULT 'Backlog',
                FOREIGN KEY (equipe_id) REFERENCES equipes(id)
            );";

        // Tabela de Transações (Extrato)
        string sqlExtrato = @"
            CREATE TABLE IF NOT EXISTS extrato (
                id INT AUTO_INCREMENT PRIMARY KEY,
                equipe_id INT,
                descricao VARCHAR(255),
                valor DECIMAL(10,2),
                tipo ENUM('Ganho', 'Gasto'),
                data_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (equipe_id) REFERENCES equipes(id)
            );";

        // Tabela de Membros (Que fazem parte da equipe)
        string sqlMembros = @"
            CREATE TABLE IF NOT EXISTS membros (
                id INT AUTO_INCREMENT PRIMARY KEY,
                equipe_id INT,
                nome_membro VARCHAR(100) NOT NULL,
                cargo VARCHAR(50),
                FOREIGN KEY (equipe_id) REFERENCES equipes(id) ON DELETE CASCADE
            );";
        // O 'ON DELETE CASCADE' faz com que, se a equipe for excluída, os membros sumam automaticamente.

        ExecuteCommand(sqlEquipes, connection);
        ExecuteCommand(sqlTarefas, connection);
        ExecuteCommand(sqlExtrato, connection);
        ExecuteCommand(sqlMembros, connection);
        
        Console.WriteLine("--- IndieStation: Tabelas Verificadas ---");
    }

    private static void ExecuteCommand(string sql, MySqlConnection conn)
    {
        using var cmd = new MySqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
}
