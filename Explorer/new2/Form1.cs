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
using System.Collections;

namespace new2
{
    enum Nums : byte { Zero = 0, One, Two, Three, Four, Five, Six, Seven, Eight };
    /// <summary>
    /// Основной класс выполнения программы
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Очередь в которой храниться путь
        /// </summary>
        Queue<string> queue = null;
        /// <summary>
        /// Очередь в которой храниться путь для второго окна
        /// </summary>
        Queue<string> queueForm2 = null;
        /// <summary>
        /// индекс что бы было удобно перемещаться по элементам очереди
        /// </summary>
        int index = 0;
        /// <summary>
        /// строка в которой храниться путь от куда следует копировать файлы или папки
        /// </summary>
        string from = null;
        /// <summary>
        /// строка в которой храниться путь куда следует копировать файлы или папки
        /// </summary>
        string to = null;
        /// <summary>
        /// для множественного выбота файлов
        /// </summary>
        Queue<string> files = null;
        /// <summary>
        /// для множественного выбота файлов
        /// </summary>
        Queue<string> direcroty = null;
        /// <summary>
        /// для сортировки
        /// </summary>
        bool sorting = false;
        /// <summary>
        /// инициализация формы
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Заполнение формы перед открытием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                TreeNode node = new TreeNode("Computer", (int)Nums.Zero, (int)Nums.Zero);
                node.Nodes.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "My Videos", (int)Nums.One, (int)Nums.One);
                node.Nodes.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Documents", (int)Nums.Two, (int)Nums.Two);
                node.Nodes.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "My Music", (int)Nums.Three, (int)Nums.Three);
                node.Nodes.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "My Pictures", (int)Nums.Four, (int)Nums.Four);
                node.Nodes.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Desktop", (int)Nums.Five, (int)Nums.Five);
                node.Nodes.Add((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads"), "Downloads", (int)Nums.Seven, (int)Nums.Seven);
                GetMyDriver();

                foreach (var i in DriveInfo.GetDrives())
                {
                    node.Nodes.Add(i.Name, i.Name, (int)Nums.Eight, (int)Nums.Eight);
                }
                this.treeView.Nodes.Add(node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// добавляет в listView логические диски
        /// </summary>
        private void GetMyDriver()
        {
            try
            {
                this.listView.Items.Clear();
                foreach (var i in DriveInfo.GetDrives())
                {
                    var k = this.listView.Items.Add(i.Name, (int)Nums.Eight);
                    k.SubItems.Add("");
                    k.SubItems.Add(i.DriveType.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// событие реагирует если выбран узел treeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Checked == false && e.Node.Parent != null)
                {
                    DirectoryInfo di = new DirectoryInfo(e.Node.Name);
                    foreach (var i in di.GetDirectories())
                    {
                        foreach (var j in this.treeView.Nodes)
                            if (i == j)
                                break;
                            else
                                this.treeView.SelectedNode.Nodes.Add(i.FullName, i.ToString(), (int)Nums.Six);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// если кликнуть мышкой по узлу то он отобразит папки которые в нем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Node.Checked == false && e.Node.Parent != null)
                {
                    this.queue = new Queue<string>();
                    var tmp = ParseSlash(e.Node.Name);
                    foreach (var i in tmp)
                        this.queue.Enqueue(string.Format(@"{0}\", i));
                    this.index = this.queue.Count;
                    FillingListView(this.queue);
                    this.toolStripStatusLabel1.Text = String.Format("Elements: {0}", this.listView.Items.Count.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// по клику переход или открытие папки или файла в listView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                foreach (ListViewItem i in this.listView.SelectedItems)
                {
                    CheckDiskLetter(i.Text);
                    this.btnBack.Enabled = true;
                    if (this.queue == null)
                        this.queue = new Queue<string>();

                    if (File.Exists(this.txtbPath.Text + i.Text))
                    {
                        var txt = ParseSlash(this.txtbPath.Text);

                        FillingQueue(txt);
                    }

                    var tmp = new Queue<string>();
                    for (var j = (int)Nums.Zero; j < index; j++)
                        tmp.Enqueue(queue.ElementAt(j));

                    queue = tmp;
                    i.Text = CheckSlash(i.Text);
                    this.queue.Enqueue(i.Text);
                    this.index = this.queue.Count;
                }
                FillingListView(this.queue);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Заполнение элементами listView
        /// </summary>
        /// <param name="q"></param>
        private void FillingListView(Queue<string> q)
        {
            try
            {
                this.txtbPath.Text = null;
                this.renameToolStripMenuItem.Enabled = true;
                for (var i = (int)Nums.Zero; i <= index - (int)Nums.One; i++)
                    this.txtbPath.Text += q.ElementAt(i);
                bool flag = false;

                DirectoryInfo di = new DirectoryInfo(this.txtbPath.Text);
                if (File.Exists(this.txtbPath.Text))
                {
                    System.Diagnostics.Process.Start(this.txtbPath.Text);
                    di = null;

                    this.queue = RemoveLastElemQueue(this.queue);
                    this.index = this.queue.Count();
                }
                if (di != null)
                {
                    this.listView.Items.Clear();
                    foreach (var j in di.GetDirectories())
                    {
                        flag = Attribut(j.FullName);
                        if (flag == false)
                            continue;
                        var k = this.listView.Items.Add(j.Name, (int)Nums.Six);
                        k.SubItems.Add(j.LastWriteTime.ToString());
                        k.SubItems.Add(j.GetType().Name);
                    }
                    foreach (var j in di.GetFiles())
                    {
                        flag = Attribut(j.FullName);
                        if (flag == false)
                            continue;
                        Icon icon = Icon.ExtractAssociatedIcon(j.FullName);
                        this.imageList.Images.Add(icon);
                        var k = this.listView.Items.Add(j.Name, this.imageList.Images.Count - (int)Nums.One);
                        k.SubItems.Add(j.LastAccessTime.ToString());
                        k.SubItems.Add(string.Format("File '{0}'", j.Extension));
                        k.SubItems.Add(string.Format("{0} byte", j.Length.ToString()));
                    }
                }
                this.toolStripStatusLabel1.Text = String.Format("Elements: {0}", this.listView.Items.Count.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Проверка если файлы или папки скрыты что бы их не отображать
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool Attribut(string str)
        {
            DirectoryInfo di = new DirectoryInfo(str);
            if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                return false;
            else
                return true;
        }
        /// <summary>
        /// кнопка которая возвращает к предедущей папке которая была
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                this.index--;
                this.btnForward.Enabled = true;
                if (this.index > (int)Nums.Zero)
                    FillingListView(this.queue);
                else
                {
                    this.btnBack.Enabled = false;
                    GetMyDriver();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// если был переход вперед затем назад кнопка может вернуть отображение папок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForward_Click(object sender, EventArgs e)
        {
            try
            {
                this.index++;
                this.btnBack.Enabled = true;
                if (this.index <= this.queue.Count)
                    FillingListView(this.queue);
                else
                    this.btnForward.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// проверяет есть ли в строке слэш
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string CheckSlash(string str)       // проверка
        {
            try
            {
                if (!str.Contains(@"\") && !str.Contains("."))
                    str += @"\";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return str;
        }
        /// <summary>
        /// проверка на букву диска
        /// </summary>
        /// <param name="str"></param>
        private void CheckDiskLetter(string str)            // проверка
        {
            try
            {
                foreach (var i in DriveInfo.GetDrives())
                {
                    if (str == i.Name)
                        this.queue = null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// разбивает строку по слэшу для заполнения очереди
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string[] ParseSlash(string str)
        {
            var tmp = str.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            return tmp;
        }
        /// <summary>
        /// удаление последнего элемента в очереди
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private Queue<string> RemoveLastElemQueue(Queue<string> q)
        {
            Queue<string> tmp = null;
            try
            {
                tmp = new Queue<string>();
                for (var i = (int)Nums.Zero; i < q.Count - (int)Nums.One; i++)
                    tmp.Enqueue(q.ElementAt(i));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return tmp;
        }
        /// <summary>
        /// открывает через контекстное меню файл или папку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (ListViewItem i in this.listView.SelectedItems)
                {
                    i.Text = CheckSlash(i.Text);
                    CheckDiskLetter(i.Text);
                    if (this.queue == null)
                        this.queue = new Queue<string>();
                    if (this.queue.Count == (int)Nums.Zero || i.Text != this.queue.ElementAt(this.queue.Count - (int)Nums.One))
                        this.queue.Enqueue(string.Format(@"{0}", i.Text));

                    this.index = this.queue.Count;
                    if (i.Index > (int)Nums.One || i.Text.Contains("."))
                    {
                        this.btnBack.Enabled = true;
                        FillingListView(this.queue);
                        if (!i.Text.Contains("."))
                            this.queue = RemoveLastElemQueue(this.queue);
                    }
                    else
                    {
                        this.queueForm2 = this.queue;
                        FillingListViewForm2(this.queueForm2);
                        this.queueForm2 = RemoveLastElemQueue(this.queueForm2);
                        this.queue = RemoveLastElemQueue(this.queue);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Заполнение ListView во втором окне
        /// </summary>
        /// <param name="q"></param>
        private void FillingListViewForm2(Queue<string> q)
        {
            try
            {
                var f = new Form2();
                this.renameToolStripMenuItem.Enabled = true;
                f.Show();
                f.txtbPath.Text = null;
                for (var i = (int)Nums.Zero; i <= index - (int)Nums.One; i++)
                    f.txtbPath.Text += q.ElementAt(i);

                DirectoryInfo di = new DirectoryInfo(f.txtbPath.Text);
                if (File.Exists(f.txtbPath.Text))
                {
                    System.Diagnostics.Process.Start(f.txtbPath.Text);
                    di = null;
                    this.queue = RemoveLastElemQueue(this.queue);
                }
                if (di != null)
                {
                    f.listView.Items.Clear();
                    foreach (var j in di.GetDirectories())
                    {
                        var k = f.listView.Items.Add(j.Name, (int)Nums.Six);
                        k.SubItems.Add(j.LastWriteTime.ToString());
                        k.SubItems.Add(j.GetType().Name);
                    }
                    foreach (var j in di.GetFiles())
                    {
                        Icon icon = Icon.ExtractAssociatedIcon(j.FullName);
                        f.imageList.Images.Add(icon);
                        var k = f.listView.Items.Add(j.Name, f.imageList.Images.Count - (int)Nums.One);
                        k.SubItems.Add(j.LastAccessTime.ToString());
                        k.SubItems.Add(string.Format("File '{0}'", j.Extension));
                        k.SubItems.Add(string.Format("{0} byte", j.Length.ToString()));
                    }
                }

                f.toolStripStatusLabel1.Text = String.Format("Elements: {0}", f.listView.Items.Count.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// открывает в новом окне через контекстное меню файл или папку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toOpenInANewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem i in this.listView.SelectedItems)
                {
                    i.Text = CheckSlash(i.Text);
                    CheckDiskLetter(i.Text);

                    var txt = ParseSlash(this.txtbPath.Text);
                    this.queueForm2 = new Queue<string>();
                    for (var j = (int)Nums.Zero; j < txt.Length; j++)
                    {
                        txt[j] = CheckSlash(txt[j]);
                        this.queueForm2.Enqueue(txt[j]);

                    }
                    this.queueForm2.Enqueue(i.Text);

                    this.index = this.queueForm2.Count;

                    FillingListViewForm2(this.queueForm2);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// копирует через контекстное меню файл или папку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.files = new Queue<string>();
                this.direcroty = new Queue<string>();
                this.insertToolStripMenuItem.Enabled = true;
                foreach (ListViewItem i in this.listView.SelectedItems)
                {
                    this.from = this.txtbPath.Text;
                    if (i.Text.Contains("."))
                        this.files.Enqueue(i.Text);
                    else
                        this.direcroty.Enqueue(i.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Заполнение очереди
        /// </summary>
        /// <param name="str"></param>
        private void FillingQueue(string[] str)
        {
            try
            {
                this.queue = new Queue<string>();
                for (var j = (int)Nums.Zero; j < str.Length; j++)
                {
                    str[j] = CheckSlash(str[j]);
                    this.queue.Enqueue(str[j]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// вставляет через контекстное меню файл или папку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tmp = ParseSlash(this.txtbPath.Text);
                FillingQueue(tmp);
                var f = this.from;
                foreach (var i in this.files)
                {
                    var t = to;
                    this.to = Path.Combine(this.txtbPath.Text, i);
                    this.from = Path.Combine(from, i);

                    File.Copy(from, to);
                    FillingListView(this.queue);
                    this.from = f;
                }
                foreach (var i in this.direcroty)
                {
                    this.from = Path.Combine(from, i);
                    this.to = String.Format("{0}", this.txtbPath.Text);
                    DirectoryInfo di = new DirectoryInfo(from);
                    if (to.Contains(from))
                        MessageBox.Show("The final folder in which it is necessary to place files is affiliated for the folder in which they are.", "Action is interrupted", MessageBoxButtons.OK);

                    GetDir(this.from, this.to);
                    FillingListView(this.queue);
                    this.from = f;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// рекурсивно обходит вложенные папки (нужно для копирования)
        /// </summary>
        /// <param name="f"></param>
        /// <param name="t"></param>
        private void GetDir(string f, string t)
        {
            try
            {
                if (Directory.Exists(f))
                {
                    DirectoryInfo di = new DirectoryInfo(f);
                    t += CheckSlash(di.Name);
                    Directory.CreateDirectory(t);
                    var d = di.EnumerateFileSystemInfos();
                    foreach (var i in d)
                    {
                        if (i.FullName.Contains("."))
                        {
                            t = CheckSlash(t);
                            File.Copy(i.FullName, string.Format("{0}{1}", t, i));
                        }
                        else
                            GetDir(i.FullName, t);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// удаляет через контекстное меню файл или папку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem i in this.listView.SelectedItems)
                {

                    var del = string.Format("{0}{1}", this.txtbPath.Text, i.Text);
                    var tmp = ParseSlash(del);
                    CheckDiskLetter(i.Text);

                    if (this.queue != null)
                    {
                        FillingQueue(tmp);
                        DirectoryInfo di = new DirectoryInfo(del);
                        if ((di.Attributes & FileAttributes.System) == FileAttributes.System)
                            MessageBox.Show("It is the system file, it is better not to delete it");
                        if (di.Extension != null)
                        {
                            var q = MessageBox.Show(string.Format("To remove this file {0}?", i), "Delete?", MessageBoxButtons.YesNoCancel);
                            if (q == DialogResult.Yes)
                                File.Delete(del);
                        }
                        else
                        {
                            var q = MessageBox.Show(string.Format("To remove this directory {0}?", i), "Delete?", MessageBoxButtons.YesNoCancel);
                            if (q == DialogResult.Yes)
                                Directory.Delete(del);
                        }
                    }
                    FillingListView(this.queue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// переименовывает через контекстное меню файл или папку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem i in this.listView.SelectedItems)
                {

                    var tmp = Path.Combine(this.txtbPath.Text, i.Text);
                    DirectoryInfo di = new DirectoryInfo(tmp);
                    if ((di.Attributes & FileAttributes.System) == FileAttributes.System || this.listView.SelectedItems.Count > 1)
                    {
                        this.renameToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        this.from = tmp;
                        var name = i.Text;
                        var tc = new TextChenge();
                        tc.textBox1.Text = i.SubItems[(int)Nums.Zero].Text;
                        tc.ShowDialog();
                        i.SubItems[0].Text = tc.textBox1.Text;
                        if (i.SubItems[(int)Nums.Zero].Text == "")
                            i.SubItems[(int)Nums.Zero].Text = name;
                        this.to = this.txtbPath.Text + i.SubItems[(int)Nums.Zero].Text;
                        if ((di.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            Directory.Move(from, to);
                        }
                        else
                            File.Move(from, to);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// срабатывает после переименования файла и проверяет на запрещенные знаки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                var t = @"\/:*?'<>|";
                bool flag = true;
                foreach (var i in t)
                {
                    if (e.Label.Contains(i))
                    {
                        MessageBox.Show(@"The name of the file shouldn't contain the following signs: \/:*?'<>|");
                        flag = false;
                    }
                }
                if (flag)
                {
                    this.to = this.txtbPath.Text + e.Label;
                    DirectoryInfo di = new DirectoryInfo(from);
                    if ((di.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        Directory.Move(from, to);
                    }
                    else
                        File.Move(from, to);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// срабатывает после переименования файла и сохраняет что переименовывать
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                foreach (ListViewItem i in this.listView.SelectedItems)
                {
                    this.from = this.txtbPath.Text + i.Text;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// сортировка столбцов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            try
            {
                if (sorting == false)
                {
                    this.listView.Sorting = SortOrder.Ascending;
                    this.sorting = true;
                }
                else
                {
                    this.listView.Sorting = SortOrder.Descending;
                    this.sorting = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// отображение элементов listView спаиском
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnList_Click(object sender, EventArgs e)
        {
            this.listView.View = View.Details;
        }
        /// <summary>
        /// отображение элементов listView эскизами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPictures_Click(object sender, EventArgs e)
        {
            this.listView.View = View.LargeIcon;
        }
        /// <summary>
        /// открытие через меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.openToolStripMenuItem_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// возможность копировать в проект при помощи перетаскивания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var t = ((string[])e.Data.GetData(DataFormats.FileDrop));
                var tmp = ParseSlash(t[(int)Nums.Zero]);
                var tmp1 = ParseSlash(this.txtbPath.Text);
                FillingQueue(tmp1);
                var file = tmp[tmp.Length - (int)Nums.One];

                File.Copy(t[(int)Nums.Zero], this.txtbPath.Text + file);
                FillingListView(this.queue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Начало перетаскивания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// выдает небольшую справку о программе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void опрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox abox = new AboutBox();
            abox.Show();
        }
    }
}

