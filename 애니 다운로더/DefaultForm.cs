using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Media;

namespace 애니_다운로더
{
    public partial class DefaultForm : Form
    {
        string textboxtext;

        string currentPath;
        SQLiteConnection conn;

        string folderPath;

        string animeSelectedURL = null;

        

        public DefaultForm()
        {
            InitializeComponent();

            textboxtext = "";

            currentPath = Environment.CurrentDirectory + "\\dosl.db";
            conn = new SQLiteConnection("Data Source=" + currentPath);
            conn.Open();
        }

        private void DefaultForm_Load(object sender, EventArgs e)
        {
            label1.Text = "애니 다운로더 by 하루 / version 1.0" + Environment.NewLine + "불법 사용을 자제합시다";

            string sql = "SELECT localpath FROM UserInfo WHERE id=1";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                folderPath = (string)rdr["localpath"]; //데이터베이스에서 애니 저장 경로 가져오기
            }
            rdr.Close();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (textboxtext != "")
            {
                searchAnime(textboxtext);
                animeSelectedURL = null;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textboxtext = textBox1.Text;
            Console.WriteLine(textboxtext);
        }

        private void searchAnime(String text)
        {
            string sql = "SELECT name FROM AnimeTable WHERE name LIKE '%" + text + "%'";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (listBox1.Items.Count > 0)
                listBox1.Items.Clear();

            while (rdr.Read())
            {
                listBox1.Items.Add(rdr["name"]);
            }
            rdr.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sql = "SELECT name FROM AnimeTable";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            if (listBox1.Items.Count > 0)
                listBox1.Items.Clear();
            while (rdr.Read())
            {
                listBox1.Items.Add(rdr["name"]);
            }
            rdr.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.Description = "애니메이션이 다운로드될 경로를 지정해 주세요" + Environment.NewLine + "현재 경로: " + folderPath;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sql = "UPDATE UserInfo SET localpath=" + "'" + fbd.SelectedPath + "'";
                Console.WriteLine(sql);
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
                folderPath = fbd.SelectedPath;

                MessageBox.Show("성공적으로 경로를 변경하였습니다.", "알림");
            }
        }

        private void DefaultForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Close();
        }

        string animeName;

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string sql = "SELECT url FROM AnimeTable WHERE name=" + "'" + listBox1.SelectedItem.ToString() + "'";

                animeName = listBox1.SelectedItem.ToString();

                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    animeSelectedURL = (string)rdr["url"];
                }
                rdr.Close();
            }
        }

        Thread t1;

        private void button1_Click(object sender, EventArgs e)
        {
            if (animeSelectedURL != null)
            {
                
                t1 = new Thread(makeDownloadForm);
                t1.Start();
            }
            else SystemSounds.Beep.Play();
        }

        private void makeDownloadForm()
        {
            Application.Run(new DownloadForm(animeSelectedURL, animeName));
            
        }
    }
}
