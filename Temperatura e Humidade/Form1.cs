using System;
using System.Drawing;
using System.IO.Ports;
using System.Globalization;
using System.Windows.Forms;

namespace EstaçãoMeteorológica
{
    public partial class Form1 : Form
    {
        SerialPort porta;

        Label titulo;
        Label temperatura;
        Label humidade;
        Label estado;
        ComboBox portas;
        Button ligar;

        public Form1()
        {
            InitializeComponent();

            CriarInterface();
        }

        private void CriarInterface()
        {
            this.Text = "Estação Meteorológica";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(25, 35, 55);

            titulo = new Label();
            titulo.Text = "ESTAÇÃO METEOROLÓGICA";
            titulo.Font = new Font("Arial", 18, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.AutoSize = true;
            titulo.Location = new Point(65, 30);
            this.Controls.Add(titulo);

            temperatura = new Label();
            temperatura.Text = "Temperatura: -- °C";
            temperatura.Font = new Font("Arial", 20, FontStyle.Bold);
            temperatura.ForeColor = Color.Orange;
            temperatura.AutoSize = true;
            temperatura.Location = new Point(80, 110);
            this.Controls.Add(temperatura);

            humidade = new Label();
            humidade.Text = "Humidade: -- %";
            humidade.Font = new Font("Arial", 20, FontStyle.Bold);
            humidade.ForeColor = Color.LightSkyBlue;
            humidade.AutoSize = true;
            humidade.Location = new Point(80, 180);
            this.Controls.Add(humidade);

            portas = new ComboBox();
            portas.Location = new Point(80, 250);
            portas.Width = 150;
            portas.DropDownStyle = ComboBoxStyle.DropDownList;
            portas.Items.AddRange(SerialPort.GetPortNames());
            this.Controls.Add(portas);

            ligar = new Button();
            ligar.Text = "LIGAR ARDUINO";
            ligar.Location = new Point(250, 250);
            ligar.Size = new Size(150, 35);
            ligar.Click += Ligar_Click;
            this.Controls.Add(ligar);

            estado = new Label();
            estado.Text = "Desligado";
            estado.Font = new Font("Arial", 12);
            estado.ForeColor = Color.White;
            estado.AutoSize = true;
            estado.Location = new Point(180, 310);
            this.Controls.Add(estado);
        }

        private void Ligar_Click(object sender, EventArgs e)
        {
            try
            {
                if (portas.SelectedItem == null)
                {
                    MessageBox.Show("Seleciona uma porta COM!");
                    return;
                }

                porta = new SerialPort(
                    portas.SelectedItem.ToString(),
                    9600
                );

                porta.NewLine = "\n";
                porta.DataReceived += ReceberDados;
                porta.Open();

                estado.Text = "Arduino ligado!";
                estado.ForeColor = Color.LightGreen;

                ligar.Enabled = false;
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message);
            }
        }

        private void ReceberDados(
            object sender,
            SerialDataReceivedEventArgs e)
        {
            try
            {
                string dados = porta.ReadLine().Trim();

                string[] valores = dados.Split(',');

                if (valores.Length == 2)
                {
                    double temp;
                    double hum;

                    bool tempOk = double.TryParse(
                        valores[0],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out temp
                    );

                    bool humOk = double.TryParse(
                        valores[1],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out hum
                    );

                    if (tempOk && humOk)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            temperatura.Text =
                                "Temperatura: " +
                                temp.ToString("0.0") + " °C";

                            humidade.Text =
                                "Humidade: " +
                                hum.ToString("0.0") + " %";
                        }));
                    }
                }
            }
            catch
            {
                // Ignorar dados incompletos
            }
        }

        protected override void OnFormClosing(
            FormClosingEventArgs e)
        {
            if (porta != null && porta.IsOpen)
            {
                porta.Close();
            }

            base.OnFormClosing(e);
        }
    }
}
