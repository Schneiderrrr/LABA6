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
            private int xc, yc;

            public CCircle()
            {
                this.x = Cursor.Position.X;
                this.y = Cursor.Position.Y;
                this.rad = 90;
                this.circlepen = new Pen(Color.White, 5);
                this.circlepath = new GraphicsPath();
            }
            public bool CircleIsOutside(int xt, int yt, Panel a)
            {
                if ((xt + this.rad <= a.Width) && (yt + this.rad <= a.Height) && (xt - this.rad >= 0) && (yt - this.rad) >= 0)
                {
                    return false;
                }
                else return true;
            }

            public void DrawCircle(Graphics g, Color penc)
            {
                    this.circlepen.Color = penc;
                    this.xc = this.x - this.rad;
                    this.yc = this.y - this.rad;
                    this.circlepath.AddEllipse(this.xc, this.yc, this.rad, this.rad);
                    g.DrawPath(this.circlepen, this.circlepath);
            }
            public void ChangeColor(Color a, Graphics g)
            {
                this.circlepen.Color = a;
                g.DrawPath(this.circlepen, this.circlepath);
            }
            ~CCircle()
            {

            }
        }

        class KatesStorage : CCircle
        {
            private int size = 0;
            private bool flag;
            private const int maxsize = 1000;
            private CCircle[] arr; 
            private CCircle musor = new CCircle();

            public KatesStorage()
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

            public void DelAll(Graphics g)
            {
                int i = 0;
                while (this.size != 0)
                {
                    this.arr[i].DrawCircle(g, Color.White);
                    this.arr[i] = null;
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


            ~KatesStorage()
            {
                this.arr = null;
            }
        }

        KatesStorage store = new KatesStorage();
        KatesStorage selected = new KatesStorage();
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
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                selected.DelAll(gra);
            else if (e.KeyCode == Keys.Subtract)
            {
                while (selected.empty() == false)
                {
                    selected.top().DrawCircle(gra, Color.White);
                    selected.top().rad /= 2;
                    selected.top().circlepath.Reset();
                    selected.top().DrawCircle(gra, Color.Red);
                    selected.top().ChangeColor(pentemp.Color, gra);
                    selected.del(selected.top());
                }
            } 
            else if (e.KeyCode == Keys.Add)
            {
                while (selected.empty() == false)
                {
                    selected.top().rad *= 2;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red);
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                    else 
                    { 
                        MessageBox.Show("Не влезет"); 
                        selected.top().rad /= 2; 
                        break; 
                    }
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                while (selected.empty() == false)
                {
                    selected.top().y -= 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red);
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                    else
                    {
                        MessageBox.Show("Не влезет");
                        selected.top().y += 50;
                        break;
                    }
                }
            }
            else if(e.KeyCode == Keys.Down)
            {
                while (selected.empty() == false)
                {
                    selected.top().y += 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red);
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                    else
                    {
                        MessageBox.Show("Не влезет");
                        selected.top().y -= 50;
                        break;
                    }
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                while (selected.empty() == false)
                {
                    selected.top().x -= 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red);
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                    else
                    {
                        MessageBox.Show("Не влезет");
                        selected.top().x += 50;
                        break;
                    }
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                while (selected.empty() == false)
                {
                    
                    selected.top().x += 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red);
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                    else
                    {
                        MessageBox.Show("Не влезет");
                        selected.top().x -= 50;
                        break;
                    }
                }
            }
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
                if (circle.CircleIsOutside(e.X, e.Y, Drawing) == false)
                {
                    circle.DrawCircle(gra, pen.Color);
                    store.add(circle);
                } else
                {
                    MessageBox.Show("Выход за границы");
                }
            }
        }
    }
}
