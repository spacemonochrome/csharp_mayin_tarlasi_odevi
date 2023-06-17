using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;


namespace bayram_ali_dursun
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Application.StartupPath + "\\Puanlar.mdb");
        OleDbCommand command = new OleDbCommand();

        public static int MayinSayisi;
        public static int MatrisKenarSayisi = 10;
        public static int ToplamKareSayisi = MatrisKenarSayisi * MatrisKenarSayisi;
        public static int Puan;

        int[] MayinKonumlari;
        int PuanCarpani;

        private void Form1_Load(object sender, EventArgs e)
        {
            VeriTabaniniGoster();
        }

        public void VeriTabaniniGoster()
        {
            string SQLkomutu = "select *from puan_tablosu";
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            command.CommandText = SQLkomutu;
            command.Connection = connection;

            OleDbDataAdapter adaptor = new OleDbDataAdapter(command);
            DataTable datatablosu = new DataTable();
            adaptor.Fill(datatablosu);
            dataGridView1.DataSource = datatablosu;
            connection.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int SiraIndexi = -1;
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(textBox1.Text))
                {
                    SiraIndexi = i;
                    break;
                }
            }

            if (textBox1.Text == "")
                MessageBox.Show("Lütfen Oyuncu Adı Ekleyiniz!");
            else if (SiraIndexi != -1)
                MessageBox.Show("Daha önceden kayıt edilmiş bir isim girdiniz. Lütfen farklı bir isim girin!");
            else
            {
                if (radioButton1.Checked) { MayinSayisi = 10; PuanCarpani = 1; }
                else if (radioButton2.Checked) { MayinSayisi = 25; PuanCarpani = 3; }
                else if (radioButton3.Checked) { MayinSayisi = 40; PuanCarpani = 5; }

                label3.Text = "Mayın Sayısı = " + MayinSayisi.ToString();
                toolStripProgressBar1.Maximum = ToplamKareSayisi - MayinSayisi; // processbar'ın maksimum değeri güncelleniyor
                toolStripProgressBar1.Value = 0;

                flowLayoutPanel1.Controls.Clear();
                MayinKonumlari = new int[MayinSayisi];
                Puan = 0;
                label3.Text = "0";
                Random rnd = new Random();

                for (int sayace = 0; sayace < MayinSayisi; sayace++)
                {
                    int sayi = rnd.Next(1, ToplamKareSayisi);
                    if (MayinKonumlari.Contains(sayi))
                    {
                        continue;
                    }
                        
                    MayinKonumlari[sayace] = sayi;
                }

                int sayac = 1;
                for (int i = 1; i <= MatrisKenarSayisi; i++)
                {
                    for (int j = 1; j <= MatrisKenarSayisi; j++)
                    {
                        Button mayinbuton = new Button() { Name = "buton" + string.Format(sayac.ToString()), Tag = sayac.ToString() };
                        mayinbuton.Size = new Size(40, 40);
                        mayinbuton.Margin = new Padding(0, 0, 0, 0);
                        mayinbuton.Click += new EventHandler(butona_bas);
                        mayinbuton.BackColor = Color.Blue;
                        sayac++;
                        flowLayoutPanel1.Controls.Add(mayinbuton);
                    }
                }
            }
        }

        private void butona_bas(object sender, EventArgs e)
        {
            Button mayinbutonu = sender as Button;

            if (MayinKonumlari.Contains(Convert.ToInt32(mayinbutonu.Tag)))
            {
                mayinbutonu.BackColor = Color.Red;
                MayinlariGoster();
                MessageBox.Show("Yandınız\nToplam Puan:" + Puan);
                VeritabaninaKaydet();
            }
            else
            {
                mayinbutonu.BackColor = Color.Green;
                Puan = Puan + PuanCarpani;
                mayinbutonu.Text = Mayintarama(Convert.ToInt32(mayinbutonu.Tag)).ToString();
                toolStripProgressBar1.Value++;
                label4.Text = Puan.ToString();
                mayinbutonu.Enabled = false;
            }
        }

        private int Mayintarama(int v)
        {
            int myn = 0;
            if (v <= MatrisKenarSayisi) //tıklanan buton ilk satırda mı
            {
                if (MayinKonumlari.Contains(v - 1)) myn++;
                if (MayinKonumlari.Contains(v + 1)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi - 1)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi + 1)) myn++;
            }
            else if (v % 10 == 1)  //tıklanan buton ilk sütunda mı
            {
                if (MayinKonumlari.Contains(v - MatrisKenarSayisi)) myn++;
                if (MayinKonumlari.Contains(v - MatrisKenarSayisi + 1)) myn++;
                if (MayinKonumlari.Contains(v + 1)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi + 1)) myn++;
            }
            else if (v % 10 == 0) //tıklanan buton son sütunda  mı
            {
                if (MayinKonumlari.Contains(v - MatrisKenarSayisi)) myn++;
                if (MayinKonumlari.Contains(v - MatrisKenarSayisi - 1)) myn++;
                if (MayinKonumlari.Contains(v - 1)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi - 1)) myn++;
            }
            else  //diğer durumlar
            {
                if (MayinKonumlari.Contains(v - MatrisKenarSayisi - 1)) myn++;
                if (MayinKonumlari.Contains(v - MatrisKenarSayisi)) myn++;
                if (MayinKonumlari.Contains(v - MatrisKenarSayisi + 1)) myn++;
                if (MayinKonumlari.Contains(v - 1)) myn++;
                if (MayinKonumlari.Contains(v + 1)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi - 1)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi)) myn++;
                if (MayinKonumlari.Contains(v + MatrisKenarSayisi + 1)) myn++;

            }
            return myn;
        }

        private void VeritabaninaKaydet()
        {
            string SQLkomutu = "INSERT INTO puan_tablosu(Ad,Puan,Tarih) VALUES('" + textBox1.Text + "'," + Puan + ",'" + DateTime.Now.Date + "');";
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.CommandText = SQLkomutu;
            command.Connection = connection;
            command.ExecuteNonQuery();
            connection.Close();
            VeriTabaniniGoster();
        }

        private void MayinlariGoster()
        {
            for (int i = 1; i <= 100; i++)
            {
                if (MayinKonumlari.Contains(i))
                {
                    flowLayoutPanel1.Controls[i - 1].BackColor = Color.Red;
                    flowLayoutPanel1.Controls[i - 1].BackgroundImage = Image.FromFile("mayin.png");
                }
                else
                {
                    flowLayoutPanel1.Controls[i - 1].BackColor = Color.Green;
                    flowLayoutPanel1.Controls[i - 1].Text = Mayintarama(i).ToString();
                }
                flowLayoutPanel1.Controls[i - 1].Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = DateTime.Now.ToString();
        }
    }
}
