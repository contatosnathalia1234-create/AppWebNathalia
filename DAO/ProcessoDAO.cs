using AppWebNathalia.Configs;
using AppWebNathalia.Model;
using MySql.Data.MySqlClient;

namespace AppWebNathalia.DAO
{
    public class ProcessoDAO
    {
        private readonly Conexao _conexao;

        public ProcessoDAO(Conexao conexao)
        {
            _conexao = conexao;
        }

        public List<Processo> Listar()
        {
            var lista = new List<Processo>();

            using var con = _conexao.GetConnection();

            string sql = @"SELECT
                id_pro,
                numero_pro,
                data_pro,
                interessado_pro,
                assunto_pro,
                descricao_pro,
                situacao_pro
                FROM processos
                ORDER BY id_pro DESC";

            using var comando = new MySqlCommand(sql, con);
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Processo
                {
                    Id = reader.GetInt32("id_pro"),
                    Numero = reader.GetString("numero_pro"),
                    Data = reader.IsDBNull(reader.GetOrdinal("data_pro"))
                        ? null
                        : DateOnly.FromDateTime(reader.GetDateTime("data_pro")),
                    Interessado = reader.GetString("interessado_pro"),
                    Assunto = reader.GetString("assunto_pro"),
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao_pro"))
                        ? ""
                        : reader.GetString("descricao_pro"),
                    Situacao = reader.GetString("situacao_pro")
                });
            }

            return lista;
        }

        public void Inserir(Processo processo)
        {
            using var con = _conexao.GetConnection();

            string sql = @"INSERT INTO processos
                (numero_pro, data_pro, interessado_pro, assunto_pro, descricao_pro, situacao_pro)
                VALUES
                (@numero, @data, @interessado, @assunto, @descricao, @situacao)";

            using var comando = new MySqlCommand(sql, con);

            comando.Parameters.AddWithValue("@numero", processo.Numero);
            comando.Parameters.AddWithValue("@data", processo.Data!.Value.ToDateTime(TimeOnly.MinValue));
            comando.Parameters.AddWithValue("@interessado", processo.Interessado);
            comando.Parameters.AddWithValue("@assunto", processo.Assunto);
            comando.Parameters.AddWithValue("@descricao", processo.Descricao);
            comando.Parameters.AddWithValue("@situacao", processo.Situacao);

            comando.ExecuteNonQuery();
        }
    }
}