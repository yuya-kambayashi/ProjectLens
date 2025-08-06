using System;
using System.Data;
using System.IO;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace WinFormsApp3
{
    public partial class MainForm : Form
    {
        private DataGridView dataGridView1;
        private TextBox textBoxName;
        private TextBox textBoxCity;
        private CheckBox checkBoxChild;
        private BindingSource bindingSource1 = new BindingSource();
        private List<Project> projects = new List<Project>();
        private DataTable dataTable;
        private Button buttonSelectFolder;
        private TextBox textBoxSelectedFolder;


        public MainForm()
        {
            InitializeComponent();
            LoadData();
            SetupEvents();
        }
        private void LoadData()
        {
            // データを追加
            //people.Add(new Person { Name = "田中太郎", City = "東京", Age = 30, IsChild = true });
            //people.Add(new Person { Name = "佐藤花子", City = "大阪", Age = 25, IsChild = false });
            //people.Add(new Person { Name = "鈴木一郎", City = "東京", Age = 40, IsChild = true });
            //people.Add(new Person { Name = "山田健", City = "福岡", Age = 22, IsChild = false });

            //dataTable = ConvertToDataTable(people);
            //bindingSource1.DataSource = dataTable;
            //dataGridView1.DataSource = bindingSource1;
        }
        private DataTable ConvertToDataTable(List<Project> projects)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ProjectName", typeof(string));
            table.Columns.Add("ProjectID", typeof(string));


            foreach (var project in projects)
            {

                table.Rows.Add(project.Name, project.ID);
            }

            return table;
        }
        private void InitializeComponent()
        {
            this.Text = "DataGridView フィルターサンプル";
            this.Width = 600;
            this.Height = 400;

            textBoxName = new TextBox { Left = 60, Top = 40, Width = 120 };
            textBoxName.PlaceholderText = "名前を入力";

            textBoxCity = new TextBox { Left = 180, Top = 40, Width = 120 };
            textBoxCity.PlaceholderText = "都市名を入力";

            checkBoxChild = new CheckBox { Left = 510, Top = 40, Width = 20 };

            dataGridView1 = new DataGridView
            {
                Left = 10,
                Top = 80,
                Width = 560,
                Height = 300,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                DataSource = bindingSource1
            };

            buttonSelectFolder = new Button
            {
                Text = "フォルダ選択",
                Left = 10,
                Top = 8,
                Width = 100,
                Height = 30
            };
            buttonSelectFolder.Click += ButtonSelectFolder_Click;

            textBoxSelectedFolder = new TextBox
            {
                Left = 150,
                Top = 8,
                Width = 500,
                ReadOnly = true
            };
            this.Controls.Add(textBoxSelectedFolder);

            this.Controls.Add(textBoxName);
            this.Controls.Add(textBoxCity);
            this.Controls.Add(checkBoxChild);
            this.Controls.Add(dataGridView1);
            this.Controls.Add(buttonSelectFolder);
        }
        private void SetupEvents()
        {
            textBoxName.TextChanged += (s, e) => ApplyFilter();
            textBoxCity.TextChanged += (s, e) => ApplyFilter();
            checkBoxChild.CheckedChanged += (s, e) => ApplyFilter();
        }
        private void ApplyFilter()
        {
            string nameFilter = textBoxName.Text.Trim().Replace("'", "''");
            string cityFilter = textBoxCity.Text.Trim().Replace("'", "''");

            List<string> filters = new List<string>();

            if (!string.IsNullOrEmpty(nameFilter))
                filters.Add($"Name LIKE '%{nameFilter}%'");

            if (!string.IsNullOrEmpty(cityFilter))
                filters.Add($"City LIKE '%{cityFilter}%'");

            if (checkBoxChild.Checked)
                filters.Add("IsChild = TRUE");

            bindingSource1.Filter = string.Join(" AND ", filters);
        }
        private void ButtonSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "フォルダを選択してください";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    textBoxSelectedFolder.Text = selectedPath;

                    // インポート
                    Import();
                }
            }
        }
        private void Import()
        {
            string rootPath = textBoxSelectedFolder.Text;

            string[] subfolders = Directory.GetDirectories(rootPath);

            List<Project> importedProjects = new List<Project>();

            foreach (string subfolder in subfolders)
            {

                var project = new Project();
                var projectID = Path.GetFileName(subfolder);

                var adx = ImportFromADX(subfolder + "\\ProjectDetails_" + projectID + ".adx");

                project.ID = projectID;
                project.Name = adx.ProjectName;

                importedProjects.Add(project);
            }

            projects.AddRange(importedProjects);
            dataTable = ConvertToDataTable(projects);
            bindingSource1.DataSource = dataTable;
            dataGridView1.DataSource = bindingSource1;

        }
        private Adx ImportFromADX(string filePath)
        {

            var parser = new AdxParser();
            parser.Load(filePath);

            //var lines = new List<string>();
            var adx = new Adx();

            if ( parser.Sections.TryGetValue("PROJECT", out var lines) && lines.Count > 0)
            {
                adx.ProjectName = lines[0];
            }

            return adx;
        }



    }
}
