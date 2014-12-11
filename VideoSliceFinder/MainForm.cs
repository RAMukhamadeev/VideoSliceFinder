using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Media;

namespace VideoSliceFinder
{
    public partial class MainForm : Form
    {
        String[] masOfPath;
        public static MainForm instance;
        int countOfResults = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private String[] FindAllSubs(String path)
        {
            Queue<String> queue = new Queue<string>();
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo fi in di.GetFiles())
            {
                if (Path.GetExtension(fi.Name) == ".srt")
                {
                    queue.Enqueue(fi.FullName);
                }
            }

            foreach (DirectoryInfo childDI in di.GetDirectories())
            {
                String[] res = FindAllSubs(childDI.FullName);
                foreach (String nextPath in res)
                {
                    queue.Enqueue(nextPath);
                }
            }

            return queue.ToArray();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                labelPath.Text = fbd.SelectedPath;
                masOfPath = FindAllSubs(fbd.SelectedPath);
            }
        }

        private void SearchText(String path)
        {
            FileInfo fi = new FileInfo(path);
            String subsName = Path.GetFileNameWithoutExtension(path);
            String filmName = "";
            String pathToFilm = "";
            DirectoryInfo di = new DirectoryInfo(fi.DirectoryName);
            foreach (FileInfo nextFile in di.GetFiles())
            {
                if (subsName == Path.GetFileNameWithoutExtension(nextFile.FullName) && nextFile.Extension != ".srt")
                {
                    filmName = nextFile.Name;
                    pathToFilm = nextFile.FullName;
                    break;
                }
            }
            
            String phrase = textBoxPhrase.Text.ToLower();
            StreamReader sr = new StreamReader(path);

            String line;
            String currPhrase = "";
            String currTime = "";
            int locPos = 1;
            while ( (line = sr.ReadLine()) != null )
            {
                if (line == "")
                {
                    locPos = 1;

                    if (currPhrase.Length > 0)
                    {
                        int pos = currPhrase.ToLower().IndexOf(phrase);
                        if (pos != -1)
                            PushOneResult(filmName, currTime, currPhrase, pathToFilm);
                    }
                    currPhrase = "";
                    continue;
                }
                else
                {
                    if (locPos == 2)
                    {
                        currTime = line.Substring(0, line.IndexOf(' ') - 1);
                    }
                    if (locPos > 2)
                    {
                        currPhrase += line + " ";
                    }

                    locPos++;
                }
            }

            sr.Close();
        }

        private void SearchTextInFiles(String[] masOfPath)
        {
            foreach (String path in masOfPath)
            {
                SearchText(path);
            }
        }

        private void InitializeControls()
        {
            dataGridViewResults.ColumnCount = 5;
            dataGridViewResults.Columns[0].Name = "№";
            dataGridViewResults.Columns[1].Name = "Название фильма";
            dataGridViewResults.Columns[2].Name = "Момент времени";
            dataGridViewResults.Columns[3].Name = "Фраза";
            dataGridViewResults.Columns[4].Name = "Путь к фильму";
            dataGridViewResults.AutoResizeColumns();
        }

        private void PushOneResult(String filmName, String currTime, String line, String path)
        {
            countOfResults++;
            dataGridViewResults.RowCount++;
            int n = dataGridViewResults.RowCount - 2;
            dataGridViewResults.Rows[n].Cells[0].Value = (n + 1).ToString();
            dataGridViewResults.Rows[n].Cells[1].Value = filmName;
            dataGridViewResults.Rows[n].Cells[2].Value = currTime;
            dataGridViewResults.Rows[n].Cells[3].Value = line;
            dataGridViewResults.Rows[n].Cells[4].Value = path;
            dataGridViewResults.AutoResizeColumns();
        }

        public int GetCountOfResults()
        {
            return countOfResults;
        }

        public int GetTimeMoment(int num)
        {
            int res = 0;
            String temp = dataGridViewResults.Rows[num].Cells[2].Value.ToString();
            res += Int32.Parse(temp.Substring(0, 2)) * 3600;
            res += Int32.Parse(temp.Substring(3, 2)) * 60;
            res += Int32.Parse(temp.Substring(6, 2));

            return res;
        }

        public String GetPathToFilm(int num)
        {
            return dataGridViewResults.Rows[num].Cells[4].Value.ToString();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            instance = this;
            InitializeControls();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (labelPath.Text != "не задан")
            {
                dataGridViewResults.RowCount = 1;
                SearchTextInFiles(masOfPath);
            }
            else
            {
                MessageBox.Show("Не указан источник!");
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            FormVideo formVideo = new FormVideo();
            formVideo.Show();
        }
    }
}
