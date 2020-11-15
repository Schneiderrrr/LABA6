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
        Pen pen, pentemp;
        public Form1()
        {
            InitializeComponent();
            gra = Drawing.CreateGraphics();
            pen = new Pen(Color.Black, 5);
            pentemp = new Pen(Color.White, 5);
        }


        public class CCircle
        {
            public GraphicsPath circlepath;
            public Pen circlepen;
            public int x, y, rad;
            public double hit;
            private int xc, yc;

            public CCircle()
            {
                this.x = Cursor.Position.X;
                this.y = Cursor.Position.Y;
                this.rad = 200;
                circlepen = new Pen(Color.White, 5);
                circlepath = new GraphicsPath();
            }

            public void DrawCircle(Graphics g, Color penc)
            {
                this.circlepen.Color = penc;
                this.xc = this.x - rad;
                this.yc = this.y - rad;
                this.circlepath.AddEllipse(this.xc, this.yc, this.rad, this.rad);
                g.DrawPath(this.circlepen, this.circlepath);
            }
            public void ChangeColor(Color a, Graphics g)
            {
                this.circlepen.Color = a;
                g.DrawPath(this.circlepen, circlepath);
            }
            ~CCircle()
            {

            }
        }

        class Storage : CCircle
        {
            private int size = 0;
            private bool flag;
            private const int maxsize = 1000;
            private CCircle[] arr; 
            private CCircle musor = new CCircle();

            public Storage()
            {
                arr = new CCircle[maxsize];
            }
            public bool empty()
            {
                flag = false;
                for (int i = 0; i < size; i++)
                    if (this.arr[i] != this.musor)
                        flag = true;

                if (flag == false)
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
                        this.arr[i] = this.musor;
                    }
                this.size--;
            }
            public CCircle top()
            {
                if (empty() == false)
                    return this.arr[this.size - 1];
                else return null;
            }


            public CCircle GetCC (CCircle a)
            {
                for (int i = 0; i < this.size; i++)
                    if (this.arr[i] == a)
                        return this.arr[i];
                return null;
            }

            public void DelAll(Graphics g)
            {
                int i = 0;
                while (this.size != 0)
                {
                    this.arr[i].DrawCircle(g, Color.White);
                    arr[i] = null;
                    i++;
                    this.size--;
                }
            }
            public CCircle search(int xt, int yt)
            {
                this.flag = false;
                int iS = 0;
                for (int i = 0; i < this.size; i++)
                {
                    if ((this.arr[i].circlepath.IsVisible(xt, yt) == true) && (this.arr[i] != this.musor))
                    {
                        iS = i;
                        flag = true;
                    } 
                }
                if (flag == false)
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
            while (selected.empty() == false)
            {
                selected.top().ChangeColor(pen.Color, gra);
                selected.del(selected.top());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selected.DelAll(gra);
        }

        private void Drawing_MouseDown(object sender, MouseEventArgs e)
        {
            if (store.search(e.X, e.Y) != null)
            { 
                if (Control.ModifierKeys == Keys.Control)
                {
                    pentemp.Color = store.search(e.X, e.Y).circlepen.Color;
                    store.search(e.X, e.Y).ChangeColor(Color.Red, gra);
                    selected.add(store.search(e.X, e.Y));
                    
                }
                else
                {
                    while (selected.empty() == false)
                    {
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                    pentemp.Color = store.search(e.X, e.Y).circlepen.Color;
                    store.search(e.X, e.Y).ChangeColor(Color.Red, gra);
                    selected.add(store.search(e.X, e.Y));
                }
            }
            else
            {
                CCircle circle = new CCircle();
                circle.DrawCircle(gra, pen.Color);
                store.add(circle);
            }
        }
    }
}
