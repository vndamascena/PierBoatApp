using PierBoatApp.Data.Entities;
using PierBoatApp.Data.Settings;
using System;
using Dapper;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PierBoatApp.Data.Repositories
{
    public class LanchaRepository
    {
        public void Add(Lancha lancha)
        {
            //escrevendo o comando SQL
            var query = @"
                INSERT INTO LANCHA(ID, NOME,TELEFONE, DATA, PERIODO, OBSERVACAO)
                VALUES(@Id, @Nome,@Telefone, @Data, @Periodo, @Observacao)
            ";

            //conectando no banco de dados do sqlserver
            using (var connection = new SqlConnection(SqlServerSettings.GetConnectionString()))
            {
                connection.Execute(query, lancha);
            }
        }

        public List<Lancha> GetAgendamento(DateTime dataInicio, DateTime dataFim)
        {
            //escrevendo o comando SQL
            var query = @"SELECT * FROM LANCHA WHERE  DATA BETWEEN @DataInicio AND @DataFim ORDER BY DATA ASC";
            using (var connection = new SqlConnection(SqlServerSettings.GetConnectionString()))
            {
                return connection.Query<Lancha>(query,
                   new { @DataInicio = dataInicio, @DataFim = dataFim })
                   .ToList();
            }
        }

        public Lancha GetById(Guid id)
        {
            var query = @"
                SELECT * FROM LANCHA
                WHERE ID = @Id
            ";

            using (var connection = new SqlConnection(SqlServerSettings.GetConnectionString()))
            {
                return connection.QueryFirstOrDefault<Lancha>(query, new { Id = id });
            }
        }


        public void Update(Lancha lancha)
        {
            var query = @"
                UPDATE LANCHA
                SET NOME = @Nome,
                    TELEFONE = @Telefone,
                    DATA = @Data,
                    PERIODO = @Periodo,
                    OBSERVACAO = @Observacao
                WHERE ID = @Id
            ";

            using (var connection = new SqlConnection(SqlServerSettings.GetConnectionString()))
            {
                connection.Execute(query, lancha);
            }
        }

        public void Delete(Lancha lancha)
        {
            var query = @"
                DELETE FROM LANCHA
                WHERE ID = @Id
            ";

            using (var connection = new SqlConnection(SqlServerSettings.GetConnectionString()))
            {
                connection.Execute(query, lancha);
            }
        }

    }
}
