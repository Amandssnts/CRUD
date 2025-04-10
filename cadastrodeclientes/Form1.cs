using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;


namespace cadastrodeclientes
{
    public partial class frmCadastrodeClientes : Form
    {
        //Conexão com o banco de dados MySQL
        MySqlConnection Conexao;
        string data_source = "datasource=localhost; username=root; password=; database=cadastrodeclientes";

        public frmCadastrodeClientes()
        {
            InitializeComponent();
            //Condiguração inicial do ListView para exibição dos dados dos clientes.
            lstCliente.View = View.Details; //Define a visualização como "Detalhes".
            lstCliente.LabelEdit = true; //Permite ediatr os títulosdas colunas.
            lstCliente.AllowColumnReorder = true; //Permite reordenar as colunas.
            lstCliente.FullRowSelect = true; //Seleciona a linha inteira ao clicar.
            lstCliente.GridLines = true; //Exibe as linhas de grade no ListView.

            //Definindo as coluna no ListView.
            lstCliente.Columns.Add("Codigo", 100, HorizontalAlignment.Left); //Coluna de código.
            lstCliente.Columns.Add("Nome Completo", 200, HorizontalAlignment.Left); //Coluna de NomeCompleto.
            lstCliente.Columns.Add("Nome Social", 200, HorizontalAlignment.Left); //Coluna de Nome Social.
            lstCliente.Columns.Add("Email", 200, HorizontalAlignment.Left); //Coluna de E-mail.
            lstCliente.Columns.Add("CPF", 200, HorizontalAlignment.Left); //Coluna de CPF.
       

            //Carrega os dados dos clientes na interface.
            carregar_clientes();
        }

        private void carregar_clientes_com_query(string query)
        {
            try
            {
                //Criando a conexão com o banco de dados.
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                //Executa a consulta SQL fornecida.
                MySqlCommand cmd = new MySqlCommand(query, Conexao);

                //Se a consulta contém o parâmetro @q, adiciona o valor da caixa de pesquisa.
                if (query.Contains("@q"))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                }

                //Executa o comando e obtém os resultados.
                MySqlDataReader reader = cmd.ExecuteReader();

                //Limpa os itens existentes no ListView antes de adicionar novos.
                lstCliente.Items.Clear();

                //Preenche o ListView com os dados dos clientes.
                while (reader.Read())
                {
                    //Cria uma linha para cada cliente com os dados retornados da consulta.
                    string[] row =
                    {
                    Convert.ToString(reader.GetInt32(0)), //Codigo
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)
                    };

                    //Adiciona uma linha ao ListView
                    lstCliente.Items.Add(new ListViewItem(row));
                }
            }

            catch (MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro " + ex.Number + " ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                //Trata outros tipos de erro * erros no backEnd do formulário
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                }
            }

        }

        //Método para carregar todos os clientes no ListView (usando uma consulta sem parâmetros).
        private void carregar_clientes()
        {
            string query = "SELECT * FROM cadastro ORDER BY codigo DESC";
            carregar_clientes_com_query(query);
        }

                    // Validação Regex - para avisar o usuário caso esqueça de preencher algum elemento doseu e-mail
                    private bool isValidEmail(string email)
                    {
                        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-z]{2,}$";
                        Regex regex = new Regex(pattern);
                        return regex.IsMatch(email);
                    }
                    // Função para validar se o cpf tem exatamente 11 digítos númericos.
                    private bool isValidCPFLegth(string cpf)
                    {
                        // Remover quaisquer caracteres não númericos (como pontos e traços)
                        cpf = cpf.Replace(".", "").Replace("-", "");
                        // Verificar se o CPF tem exatamente 11 caracteres númericos
                        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                        {
                            return false;
                        }

                        return true;
                    }

                    private void btnSalvar_Click(object sender, EventArgs e)
                    {
                        {
                            try
                            {
                                // valiadeção de campos obrigátorios
                                if (string.IsNullOrEmpty(txtNomeCompleto.Text.Trim()) ||
                                   string.IsNullOrEmpty(txtEmail.Text.Trim()) || // o Trim ( .Trim) elimina o espaço vazio de um campo (Ex: gui fran) o espaço não entraria na contagem de caracteres. 
                                    string.IsNullOrEmpty(txtCPF.Text.Trim()))

                                {
                                    MessageBox.Show("Todos os campos devem ser preenchidos.",
                                        "Validação",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);

                                    return; //Impede o prosseguimento se algum campo estiver vazio
                                }

                                //validação do e-mail
                                string email = txtEmail.Text.Trim();
                                if (!isValidEmail(email))
                                {
                                    MessageBox.Show("Certifique-se de que o e-mail está no formato correto.",
                                        "Validação",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                    return; // impede o prosseguimento se o e-mail for inválido.
                                }
                                //Validadção do CPF
                                string cpf = txtCPF.Text.Trim();
                                if (!isValidCPFLegth(cpf))
                                {
                                    MessageBox.Show("CPF inválido. Certifique-se de que o CPF tenha 11 digítos númericos",
                                         "Validação",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);

                                    return; //Impede o prosseguimento se algum campo estiver vazio
                                }

                                //Cria a conexão com o banco de dados
                                Conexao = new MySqlConnection(data_source);
                                Conexao.Open();

                                //Teste de abertura de banco de dados
                                // MessageBox.Show("Conexão aberta com sucesso");

                                //Comando SQL para inserir um cliente no banco de dados

                                MySqlCommand cmd = new MySqlCommand
                                {
                                    Connection = Conexao
                                };

                                cmd.Prepare();

                                cmd.CommandText = "INSERT INTO cadastro(nomecompleto, nomesocial, email, cpf)" +
                                    "VALUES (@nomecompleto, @nomesocial, @email, @cpf)";

                            //Adiciona os parâmetros com os dados do formulário
                                cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                                cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@cpf", cpf);

                                // Executa o comando de Inserção no banco
                                cmd.ExecuteNonQuery();

                                // mensagem de sucesso
                                MessageBox.Show("Contato Inserido com sucesso: ",
                                    "Sucesso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                    //Limpa os campos após o sucesso
                    txtNomeCompleto.Text = String.Empty;
                    txtNomeSocial.Text = "";
                    txtEmail.Text = "";
                    txtCPF.Text = "";

                    //Recarregar os clientes na ListView.
                    carregar_clientes();

                    //Muda para a aba de pesquisa.
                    tabControl1.SelectedIndex = 1;

                            }
                            catch (MySqlException ex)
                            {
                                //Trata erros relacionados ao MySQL
                                MessageBox.Show("Erro " + ex.Number + " ocorreu: " + ex.Message,
                                    "Erro",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                            catch (Exception ex)
                            {
                                //Trata outros tipos de erro * erros no backEnd do formulário
                                MessageBox.Show("Ocorreu: " + ex.Message,
                                    "Erro",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                            }
                            finally
                            {
                                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                                if (Conexao != null && Conexao.State == ConnectionState.Open)
                                {
                                    Conexao.Close();

                                    //Teste de fechamento do banco
                                    //MessageBox.Show("Conexão fechada com sucesso");
                                }
                            }
                        }

                    }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM  cadastro WHERE nomecompleto LIKE @q OR nomesocial LIKE @q ORDER BY codigo DESC";
            carregar_clientes_com_query(query);
        }
    }
           
}
     



