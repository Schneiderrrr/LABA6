using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LABA6
{
    public partial class Form1 : Form
    {
        Graphics gra;
        Pen pen;
        public Form1()
        {
            InitializeComponent();
            gra = Drawing.CreateGraphics();
            pen = new Pen(Color.Black, 5);
        }


        public class CCircle
        {
            public GraphicsPath circlepath = new GraphicsPath();
            public Pen circlepen;
            public int x, y, rad;
            public double hit;
            private int xc, yc;

            public CCircle()
            {
                this.x = Cursor.Position.X;
                this.y = Cursor.Position.Y;
                this.rad = 200;
            }

            public void DrawCircle(Graphics g, Pen penc)
            {
                this.circlepen = penc;
                this.xc = this.x - rad;
                this.yc = this.y - rad;
                circlepath.AddEllipse(this.xc, this.yc, this.rad, this.rad);
                g.DrawPath(penc, circlepath);
            }
            public void ChangeColor(Graphics g)
            {
                this.circlepen.Color = Color.Red;
                g.DrawPath(circlepen, circlepath);
            }
            ~CCircle()
            {

            }
        }

        class Storage : CCircle
        {
            private int size = 0, flag;
            private const int maxsize = 1000;
            private CCircle[]arr;

            public Storage()
            {
                arr = new CCircle[maxsize];
            }
            public bool empty()
            {
                if (size == 0)
                    return true;
                else return false;
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
            public CCircle top()
            {
                if (empty() == false)
                    return this.arr[this.size - 1];
                else return null;
            }

            public CCircle search(int xt, int yt)
            {
                this.flag = 0;
                int iS = 0;
                for (int i = 0; i < this.size; i++)
                {
                    if (this.arr[i].circlepath.IsVisible(xt, yt) == true)
                    {
                        iS = i;
                        flag = 1;
                    }
                }
                if (flag == 0)
                    return null;
                else return this.arr[iS];
            }


            ~Storage()
            {
                this.arr = null;
            }
        }
        Storage store = new Storage();
        Storage selected = new Storage();
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            pen.Color = p.BackColor;
        }

        private void Drawing_MouseDown(object sender, MouseEventArgs e)
        {
            if (store.search(e.X, e.Y) != null)
            {
                if (selected.empty() == false)
                {
                    selected.top().DrawCircle(gra, pen);
                    selected.del(selected.search(e.X, e.Y));
                }
                store.search(e.X, e.Y).ChangeColor(gra);
                selected.add(store.search(e.X, e.Y));
                pen.Color = Color.Black;
            }
            else
            {
                CCircle circle = new CCircle();
                circle.DrawCircle(gra, pen);
                store.add(circle);
            }
        }
    }
}
