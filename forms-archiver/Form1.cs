using MaterialSkin;
using System.IO.Compression;

namespace forms_archiver
{
    public partial class Form1 : MaterialSkin.Controls.MaterialForm
    {
        private List<string> items = new List<string>();
        private string destFolder = "";

        public Form1()
        {
            InitializeComponent();
            var skinManager = MaterialSkin.MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
            skinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.Blue600, MaterialSkin.Primary.Blue700,
                MaterialSkin.Primary.Blue200, MaterialSkin.Accent.LightBlue200,
                MaterialSkin.TextShade.WHITE);
        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Оберіть файли для архівації",
                Filter = "Усі файли (*.*)|*.*"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            foreach (var f in dlg.FileNames)
            {
                items.Add(f);
                materialListBox1.Items.Add(new MaterialListBoxItem(f));
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Оберіть директорії для архівації",
                ShowNewFolderButton = true
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            items.Add(dlg.SelectedPath);
            materialListBox1.Items.Add(new MaterialListBoxItem(dlg.SelectedPath));
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Директорія для збереження архіву",
                ShowNewFolderButton = true
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            destFolder = dlg.SelectedPath;
            materialLabel2.Text = $"Папка: {destFolder}";
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            materialLabel2.Text = "";
            if (!items.Any() || string.IsNullOrEmpty(destFolder) || string.IsNullOrWhiteSpace(materialTextBox1.Text))
            {
                materialLabel2.Text = "Заповніть назву, оберіть елементи та теку збереження.";
                return;
            }

            var archivePath = Path.Combine(destFolder, materialTextBox1.Text.Trim() + ".zip");
            try
            {
                // Стиснення за допомогою ZipFile
                using var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create);
                foreach (var path in items)
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                        {
                            var entryName = Path.GetRelativePath(Path.GetDirectoryName(path)!, file);
                            archive.CreateEntryFromFile(file, Path.Combine(Path.GetFileName(path), entryName));
                        }
                    }
                    else
                    {
                        archive.CreateEntryFromFile(path, Path.GetFileName(path)!);
                    }
                }
                materialLabel2.Text = $"Готово: {archivePath}";

                items.Clear();
                materialListBox1.Items.Clear();
                materialTextBox1.Text = "";
            }
            catch (Exception ex)
            {
                materialLabel2.Text = $"Помилка: {ex.Message}";
            }
        }
    }
}
