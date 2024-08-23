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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private const string DataFolderPath = @"C:\data\log\";

        public Form1()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // 폴더가 존재하지 않으면 생성
            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }

            // 현재 날짜로 달력 초기화
            monthCalendar1.SelectionStart = DateTime.Today;
            monthCalendar1.SelectionEnd = DateTime.Today;

            LoadDiaryAndTodo();
        }

        private void LoadDiaryAndTodo()
        {
            // 선택된 날짜 가져오기
            DateTime selectedDate = monthCalendar1.SelectionStart.Date;
            string diaryFilePath = GetDiaryFilePath(selectedDate);
            string todoFilePath = GetTodoFilePath(selectedDate);

            // 일기 내용 로드
            if (File.Exists(diaryFilePath))
            {
                textBox1.Text = File.ReadAllText(diaryFilePath);
            }
            else
            {
                textBox1.Text = string.Empty;
            }

            // TODO 목록 로드
            checkedListBox1.Items.Clear();
            if (File.Exists(todoFilePath))
            {
                string[] todoLines = File.ReadAllLines(todoFilePath);
                foreach (var line in todoLines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        string todo = parts[0];
                        bool isChecked = bool.Parse(parts[1]);
                        checkedListBox1.Items.Add(todo, isChecked);
                    }
                }
            }
        }

        private string GetDiaryFilePath(DateTime date)
        {
            return Path.Combine(DataFolderPath, $"log-{date:yyyyMMdd}.dat");
        }

        private string GetTodoFilePath(DateTime date)
        {
            return Path.Combine(DataFolderPath, $"todo-{date:yyyyMMdd}.dat");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 저장 버튼 클릭 시, 일기 및 TODO 목록 저장
            SaveDiaryAndTodo();
        }

        private void SaveDiaryAndTodo()
        {
            // 선택된 날짜 가져오기
            DateTime selectedDate = monthCalendar1.SelectionStart.Date;

            // 일기 저장
            string diaryFilePath = GetDiaryFilePath(selectedDate);
            File.WriteAllText(diaryFilePath, textBox1.Text);

            // TODO 목록 저장
            string todoFilePath = GetTodoFilePath(selectedDate);

            // TODO 목록을 텍스트와 체크 상태를 포함하여 저장
            List<string> todoLines = new List<string>();
            foreach (var item in checkedListBox1.Items)
            {
                bool isChecked = checkedListBox1.GetItemChecked(checkedListBox1.Items.IndexOf(item));
                todoLines.Add($"{item.ToString()}|{isChecked}");
            }
            File.WriteAllLines(todoFilePath, todoLines);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // TODO 추가 버튼 클릭 시, 입력 상자를 통해 내용 입력
            string todo = Prompt.ShowDialog("Enter TODO item", "Add TODO");
            if (!string.IsNullOrWhiteSpace(todo))
            {
                checkedListBox1.Items.Add(todo, false);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // TODO 체크 상태 저장 버튼 클릭 시, 체크 상태 저장
            SaveDiaryAndTodo();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            LoadDiaryAndTodo();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
// Form1 클래스가 정의된 파일 내에 Prompt 클래스를 추가합니다.
public static class Prompt
{
    public static string ShowDialog(string text, string caption)
    {
        Form prompt = new Form()
        {
            Width = 500,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = caption,
            StartPosition = FormStartPosition.CenterScreen
        };
        Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
        TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
        Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
        confirmation.Click += (sender, e) => { prompt.Close(); };
        prompt.Controls.Add(textBox);
        prompt.Controls.Add(confirmation);
        prompt.Controls.Add(textLabel);
        prompt.AcceptButton = confirmation;

        return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
    }
}
