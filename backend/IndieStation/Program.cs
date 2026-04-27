using System;

// Inicializa o banco de dados e as tabelas (conforme sua classe Database)
Database.Inicializar();

bool rodando = true;

while (rodando)
{
    Console.WriteLine("======================================");
    Console.WriteLine("      INDIESTATION - GESTÃO           ");
    Console.WriteLine("======================================");
    Console.WriteLine("1. Entrar (Login)");
    Console.WriteLine("2. Cadastrar Nova Equipe");
    Console.WriteLine("0. Sair");
    Console.Write("\nEscolha uma opção: ");

    string opcaoInicial = Console.ReadLine()!;

    switch (opcaoInicial)
    {
        case "1":
            RealizarLogin();
            break;

        case "2":
            RealizarCadastro();
            break;

        case "0":
            rodando = false;
            Console.WriteLine("Encerrando o sistema...");
            break;

        default:
            Console.WriteLine("Opção inválida! Pressione qualquer tecla para tentar novamente.");
            Console.ReadKey();
            break;
    }
}

// --- MÉTODOS AUXILIARES PARA ORGANIZAR O CÓDIGO ---

void RealizarCadastro()
{
    Console.Clear();
    Console.WriteLine("--- NOVO CADASTRO DE EQUIPE ---");
    
    Console.Write("Nome da Equipe: ");
    string nome = Console.ReadLine()!;

    Console.Write("Defina a Senha da Equipe (Membros): ");
    string sEquipe = Console.ReadLine()!;

    Console.Write("Defina a Senha do Administrador (Mestre): ");
    string sAdmin = Console.ReadLine()!;

    bool sucesso = Auth.CadastrarEquipe(nome, sEquipe, sAdmin);

    if (sucesso)
    {
        Console.WriteLine("\n[SUCESSO] Equipe registrada! Pressione qualquer tecla para voltar.");
    }
    else
    {
        Console.WriteLine("\n[ERRO] Não foi possível realizar o cadastro.");
    }
    Console.ReadKey();
}

void RealizarLogin()
{
    Console.Clear();
    Console.WriteLine("--- LOGIN INDIESTATION ---");
    
    Console.Write("Nome da Equipe: ");
    string nomeInput = Console.ReadLine()!;

    if (!Auth.EquipeExiste(nomeInput))
    {
        Console.WriteLine("\n[ERRO] Esta equipe não existe no IndieStation!");
        Console.ReadKey();
        return; // Volta para o menu
    }

    Console.WriteLine("\nTipo de Acesso:");
    Console.WriteLine("1. Membro da Equipe (Kanban/Tarefas)");
    Console.WriteLine("2. Administrador (Financeiro/Gestão)");
    Console.Write("Opção: ");
    string tipoLogin = Console.ReadLine()!;

    Console.Write("Senha: ");
    string senhaInput = Console.ReadLine()!;

    if (tipoLogin == "1")
    {
        int? equipeId = Auth.LoginEquipe(nomeInput, senhaInput);
        if (equipeId != null)
        {
            MenuEquipe(equipeId.Value, nomeInput);
        }
        else
        {
            Console.WriteLine("\n[!] Credenciais de Equipe incorretas.");
            Console.ReadKey();
        }
    }
    else if (tipoLogin == "2")
    {
        int? adminId = Auth.LoginAdmin(nomeInput, senhaInput);
        if (adminId != null)
        {
            MenuAdmin(adminId.Value, nomeInput);
        }
        else
        {
            Console.WriteLine("\n[!] Senha de Administrador incorreta.");
            Console.ReadKey();
        }
    }
}

void MenuEquipe(int id, string nome)
{
    Console.Clear();
    Console.WriteLine($"=== PAINEL DA EQUIPE: {nome.ToUpper()} ===");
    Console.WriteLine("1. Ver Kanban (Tarefas)");
    Console.WriteLine("2. Notas de Atualização");
    Console.WriteLine("0. Sair");
    Console.ReadKey(); // Placeholder para futuras implementações
}

void MenuAdmin(int id, string nome)
{
    bool logado = true;
    while (logado) {
        Console.Clear();
        Console.WriteLine($"=== ADMIN: {nome} ===");
        Console.WriteLine("1. Adicionar Membro | 2. Listar Membros | 3. Detalhes Equipe");
        Console.WriteLine("4. Excluir Membro   | 5. EXCLUIR EQUIPE | 0. Voltar");
        
        string op = Console.ReadLine()!;
        if (op == "1") {
            Console.Write("Nome do Membro: ");
            EquipeManager.AdicionarMembro(id, Console.ReadLine()!);
        }
        else if (op == "2") {
            EquipeManager.ListarMembros(id);
            Console.ReadKey();
        }
        else if (op == "3") {
            EquipeManager.DetalhesEquipe(id);
            Console.ReadKey();
        }
        else if (op == "4") {
            Console.Write("ID do Membro: ");
            string ID = Console.ReadLine()!;
            EquipeManager.ExcluirMembro(ID);
            Console.ReadKey();
        }
        else if (op == "5") {
            Console.Write("TEM CERTEZA? (s/n): ");
            if (Console.ReadLine() == "s") {
                EquipeManager.ExcluirEquipe(id);
                logado = false;
            }
        }
        else if (op == "0") logado = false;
    }
}
