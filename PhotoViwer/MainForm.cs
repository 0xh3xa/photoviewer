using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Mail;
using System.Net;
namespace PhotoViwer
{
    public partial class MainForm : Form
    {
        private OpenFileDialog _openDialog;
        private GraphicsPath _graphicsPath;
        private FileInfo[] _fileInfo;
        private DirectoryInfo _directoryInfo;
        private int _imageIndex;


        public MainForm()
        {
            InitializeComponent();

            _openDialog = new OpenFileDialog();
            _graphicsPath = new GraphicsPath();
            this.play_button.BackColor = Color.Transparent;
            _graphicsPath.AddEllipse(4, 4, this.play_button.Width - 10, this.play_button.Height - 10);
            
           // this.play_button.Region = new Region(_graphicsPath);

            this.UpdateInterFace();

        }

        private void UpdateInterFace()
        {
            
            this.KeyPreview = true;
            this.AllowDrop = true;
            
            this.WindowState = FormWindowState.Normal;
            this.Width = 800;
            this.Height = 600;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.menuStrip.Visible = true;

            this.pictureBox.SizeMode = PictureBoxSizeMode.CenterImage; 
            

            this.pictureBox.BackColor = Color.White;
         
            this.timer.Enabled = false;
        }

        Image _image;
        private void OpenImage()
        {
            try
            {
                this.pictureBox.Cursor = Cursors.Hand;
                _openDialog.Filter = "Images Files|*.jpg;*.jpeg;*.bmp;*.gif";
                if (_openDialog.ShowDialog(this) != DialogResult.OK) return;
                _openDialog.OpenFile();
                _image = Image.FromFile(_openDialog.FileName);
                //
                if (_image.Width > 800 && _image.Height > 600)
                {
                    Bitmap map = new Bitmap(_image, 800, 600);
                    this.pictureBox.Image = map;
                }
                else this.pictureBox.Image = _image;

                _directoryInfo = new DirectoryInfo(_openDialog.FileName.Substring(0, _openDialog.FileName.LastIndexOf('\\')));
                _fileInfo = _directoryInfo.GetFiles();


                _width = _image.Width;
                _height = _image.Height;
            }
            catch { }
        }
        private void NextImage()
        {

            try
            {
                if (this.pictureBox.Image == null) return;

                if (_imageIndex >= _fileInfo.Length - 2)
                    _imageIndex = 0;
                else _imageIndex++;
                _image = Image.FromFile(_fileInfo[_imageIndex].FullName);

                if (_image.Width > 800 && _image.Height > 600)
                {
                    Bitmap map = new Bitmap(_image, 800, 600);
                    this.pictureBox.Image = map;
                }
                else this.pictureBox.Image = _image;

            }
            catch { }
            finally { }
        }
        private void ResumeImage()
        {
            try
            {
                if (this.pictureBox.Image == null) return;
                if (_imageIndex <= 0)
                    _imageIndex = _fileInfo.Length - 2;
                else _imageIndex--;
                _image = Image.FromFile(_fileInfo[_imageIndex].FullName);

                if (_image.Width > 800 && _image.Height > 600)
                {
                    Bitmap map = new Bitmap(_image, 800, 600);
                    this.pictureBox.Image = map;
                }
                else this.pictureBox.Image = _image;

            }
            catch { }
            finally { }
       
        }
        private void play_button_Click(object sender, EventArgs e)
        {
            if (this.pictureBox.Image == null) return;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.pictureBox.BackColor = Color.Black;
            this.menuStrip.Visible = false;
            this.timer.Enabled = true;
        }
        private void next_button_Click(object sender, EventArgs e)
        {
            
                this.NextImage();
            
            
        }
        private void return_button_Click(object sender, EventArgs e)
        {
           
                this.ResumeImage();
            
            
        }
        private void rotate_button_Click(object sender, EventArgs e)
        {
            if (_openDialog.FileName == string.Empty) return;
            this.pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
            if (_image.Width > 800 && _image.Height > 600)
            {
                Bitmap map = new Bitmap(this.pictureBox.Image, 800, 600);
                this.pictureBox.Image = map;
            }
            else this.pictureBox.Image = _image;
            

        }
        float _width, _height;
        Size _sizeImage = new Size(2, 2);

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left)
                this.ResumeImage();
            if (e.KeyData == Keys.Right)
                this.NextImage();
            if (this.timer.Enabled)
                if (e.KeyCode == Keys.Escape)
                    this.UpdateInterFace();
        }
        private void SaveImage()
        {

            if (this.pictureBox.Image == null) return;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Images Files|*.jpg;*.jpeg;*.bmp;*.gif";
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            this.pictureBox.Image.Save(dialog.FileName);

        }
        private void ZoomIn()
        {
            if (this.pictureBox.Image == null) return;
            Bitmap map = new Bitmap(_image, new Size((int)(this.pictureBox.Image.Width * _sizeImage.Width), (int)(this.pictureBox.Image.Height * _sizeImage.Height)));

            this.pictureBox.Image = map;

        }
        private void ZoomOut()
        {
            if (this.pictureBox.Image == null) return;
            Bitmap map = new Bitmap(_image, new Size((int)(this.pictureBox.Image.Width / _sizeImage.Width), (int)(this.pictureBox.Image.Height / _sizeImage.Height)));
            this.pictureBox.Image = map;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            this.NextImage();


        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.pictureBox.Image == null) return;
            if (!this.timer.Enabled) return;
            ContextMenu context = new System.Windows.Forms.ContextMenu();
            MenuItem item = new MenuItem("Slow");
            MenuItem item2 = new MenuItem("Fast");
            MenuItem item3 = new MenuItem("Stop");
            context.MenuItems.Add(item);
            context.MenuItems.Add(item2);
            context.MenuItems.Add(item3);
            item.Click += item_Click;
            item2.Click += item_Click;
            item3.Click += item_Click;
            if (e.Button == MouseButtons.Right)
                context.Show(this.pictureBox, new Point(pictureBox.Width / 2, pictureBox.Height / 2));


        }

        void item_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item.Text == "Slow")
                this.timer.Interval = 5000;

            if (item.Text == "Fast")
                this.timer.Interval = 500;

            if (item.Text == "Stop")
                //this.timer.Enabled = false;
                this.UpdateInterFace();
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveImage();
        }

        private void zoomIn_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.ZoomIn();
            }
            catch { }
        }

        private void zoomOut_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.ZoomOut();
            }
            catch { }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            
            e.Effect = DragDropEffects.Move;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
           
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                _directoryInfo = new DirectoryInfo(files[0].Substring(0, files[0].LastIndexOf('\\')));

                _fileInfo = _directoryInfo.GetFiles("*.jpg");
              
                
                
                Bitmap bitmap = new Bitmap(Image.FromFile(files[0]), this.pictureBox.Width - 10, this.pictureBox.Height - 10);
                this.pictureBox.Image = bitmap;
            }
         
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenImage();
        }
        bool _mouseMove;
        Point _mouseLocation;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
          
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            _mouseMove = true;
            _mouseLocation = e.Location;
            
        }
        Graphics g;
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
        }

        
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            
            if (!_mouseMove) return;
            //p.Top = p.Top + e.Y - _mouseLocation.Y;
            //  p.Left = p.Left + e.X - _mouseLocation.X;
            //this.pictureBox.Location = new Point(e.X, e.Y);

            Bitmap bitmap = new Bitmap(_image);
            // this.pictureBox.Image = null;
            Graphics g = this.pictureBox.CreateGraphics();
            //this.pictureBox.Image = null;
            this.pictureBox.BackColor = Color.White;
            g.DrawImage(bitmap, new Rectangle(this.pictureBox.Location.X + _mouseLocation.X-e.X, this.pictureBox.Location.Y + _mouseLocation.Y-e.Y,
               bitmap.Width, bitmap.Height));
            this.DoubleBuffered = true;
            //this.pictureBox.Refresh();
            _mouseMove = false;   
    
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.pictureBox.Image == null) return;
            this.pictureBox.Image.Dispose();
            this.Dispose();
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
        }

    
        

     





    }
}
