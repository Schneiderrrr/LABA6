using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LABA6
{
    public partial class Form1 : Form
    {
        public Graphics gra;
        public Pen pen;
        public SolidBrush fig;
        public Form1()
        {
            InitializeComponent();
            gra = Drawing.CreateGraphics();
        }


        public class CCircle
        {
            public int x, y, rad;
            private int xc, yc;
            public Pen pen;
            public SolidBrush fig;
            public CCircle()
            {
                this.x = Cursor.Position.X;
                this.y = Cursor.Position.Y;
                this.rad = 200;
                pen = new Pen(Color.Black, 5);
                fig = new SolidBrush(Color.White);
            }

            public void DrawCircle(Graphics g)
            {
                this.xc = this.x - rad;
                this.yc = this.y - rad;
                g.FillEllipse(this.fig, this.xc, this.yc, this.rad, this.rad);
                g.DrawEllipse(this.pen, this.xc, this.yc, this.rad, this.rad);
            }

            ~CCircle()
            {

            }
        }

        class Storage : CCircle
        {
            private int size = 0, flag = 0;
            private const int maxsize = 1000;
            private CCircle[]arr;

            public Storage()
            {
                arr = new CCircle[maxsize];
            }
            public void add(CCircle a)
            {
                this.arr[this.size] = a;
                this.size++;
            }
            public void del(CCircle a)
            {
                for (int i = 0; i < this.size; i++)
                    if (this.arr[i] == a)
                    {
                        this.arr[i] = null;
                        this.size--;
                    }
            }
            public CCircle get (CCircle a)
            {
                for (int i = 0; i<this.size; i++)
                    if (this.arr[i] == a)
                        this.flag = 1;

                if (this.flag == 0)
                    return null;
                else return a;
            }

            ~Storage()
            {
                this.arr = null;
            }
        }
        Storage store = new Storage();
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //PictureBox p = (PictureBox)sender;
        }

        private void Drawing_MouseDown(object sender, MouseEventArgs e)
        {
            CCircle circle = new CCircle();
            circle.DrawCircle(gra);
            store.add(circle);
        }
    }
}
