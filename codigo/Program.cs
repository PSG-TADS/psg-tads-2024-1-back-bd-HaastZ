using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace trabalho
{
    public class Veiculo
    {
        [Key]
        public int IdVeiculo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
        public string Cor { get; set; }
        public decimal PrecoDiaria { get; set; }
    }

    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
    }

    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }
        public int IdVeiculo { get; set; }
        [ForeignKey("IdVeiculo")]
        public int IdCliente { get; set; }
        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public decimal ValorTotal { get; set; }

        public enum StatusReserva
        {
            Pendente,
            Confirmada,
            Cancelada
        }

        public StatusReserva Status { get; set; }
    }

    public class AplicationContext : DbContext //classe de conexão com o banco de dados
    {
        public DbSet<Veiculo> Veiculo { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Reserva> Reserva { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=LocadoraDeVeiculos;Trusted_Connection=True;TrustServerCertificate=true");
        }
    }

    class Program
    {
        static void AdicionarReserva() //Criar Reserva
        {
            using (var context = new AplicationContext())
            {
                // Buscar um cliente existente para fazer a reserva
                var cliente = context.Cliente.FirstOrDefault();

                // Buscar um veículo existente para a reserva
                var veiculo = context.Veiculo.FirstOrDefault();

                    var novaReserva = new Reserva
                    {
                        IdVeiculo = veiculo.IdVeiculo,
                        IdCliente = cliente.IdCliente,
                        Cliente = cliente,
                        DataInicio = DateTime.Today.AddDays(2), // Data de início em 2 dias
                        DataFim = DateTime.Today.AddDays(5),    // Data de fim em 5 dias
                        ValorTotal = 1000,
                        Status = Reserva.StatusReserva.Confirmada
                    };

                    context.Reserva.Add(novaReserva);
                    context.SaveChanges();
                }
        }
        static void AdicionarCliente() //Criar Cliente
        {
            using (var context = new AplicationContext())
            {
                var novoCliente = new Cliente
                {
                    Nome = "Vinicius",
                    CPF = "123.456.789-10",
                    Endereco = "Rua dos Goitacazes, 182",
                    Telefone = "(31) 12345-6789",
                    Email = "vinicius@email.com"
                };
                context.Cliente.Add(novoCliente);
                context.SaveChanges();
            }
        }
        static void AdicionarVeiculo() //Criar Veiculo
        {
            using (var context = new AplicationContext())
            {
                var novoVeiculo = new Veiculo
                {
                    Marca = "Hyundai",
                    Modelo = "HB20",
                    Ano = 2024,
                    Placa = "ABC1234",
                    Cor = "Preto",
                    PrecoDiaria = 100
                };
                context.Veiculo.Add(novoVeiculo);
                context.SaveChanges();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:5000");
            });

        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            AdicionarVeiculo();
            AdicionarCliente();
            AdicionarReserva();
        }
    }

}
