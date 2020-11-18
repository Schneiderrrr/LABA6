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
        public int choice;
        
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

            public bool LineIsOutside(int xt, int yt, Panel a)
            {
                if ((xt <= a.Width) && (xt + 300 <= a.Width) && (yt <= a.Height) && (xt >= 0) && (yt >= 0))
                    return false;
                else return true;
            }

            public void ChangeLine(int xt)
            {
                this.circlepath.Reset();
                this.circlepath.AddLine(this.x, this.y, this.x + xt, this.y);
            }

            public void DrawCircle(Graphics g, Color penc, int c)
            {
                    this.circlepen.Color = penc;
                if (c == 1)
                {
                    this.xc = this.x - this.rad;
                    this.yc = this.y - this.rad;
                    this.circlepath.AddEllipse(this.xc, this.yc, this.rad, this.rad);
                }
                else
                {
                    this.circlepath.AddLine(this.x, this.y , this.x + 300, this.y);
                }
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

            public void DelAll(Graphics g, int c)
            {
                int i = 0;
                while (this.size != 0)
                {
                    this.arr[i].DrawCircle(g, Color.White, c);
                    this.arr[i] = null;
                    i++;
                    this.size--;
                }
            }
            public CCircle search(int xt, int yt, int choice)
            {
                this.flag = false;
                int iS = 0;
                if (choice == 1)
                {
                    for (int i = 0; i < this.size; i++)
                    {
                        if ((this.arr[i].circlepath.IsVisible(xt, yt) == true) && (this.arr[i] != this.musor))
                        {
                            iS = i;
                            flag = true;
                        }
                    }
                } else
                {
                    for (int i = 0; i < this.size; i++)
                    {
                        if ((this.arr[i].circlepath.IsOutlineVisible(xt, yt, this.circlepen) == true) && (this.arr[i] != this.musor))
                        {
                            iS = i;
                            flag = true;
                        }
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
            if (e.KeyCode == Keys.Delete)//удаление
                selected.DelAll(gra, choice);
            else if (e.KeyCode == Keys.Subtract)//уменьшение
            {
                while (selected.empty() == false)
                {
                    selected.top().DrawCircle(gra, Color.White, choice);
                    if (choice == 1)
                    {
                        selected.top().rad /= 2;
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red, choice);
                    }
                    else
                    {
                        selected.top().circlepath.Reset();
                        selected.top().ChangeLine(100);
                    }
                    selected.top().ChangeColor(pentemp.Color, gra);
                    selected.del(selected.top());
                }
            } 
            else if (e.KeyCode == Keys.Add)//увеличение
            {
                while (selected.empty() == false)
                {
                    if (choice == 1)
                    {
                        selected.top().rad *= 2;
                        if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false && selected.top().LineIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                        {
                            selected.top().DrawCircle(gra, Color.White, choice);
                            selected.top().circlepath.Reset();
                            selected.top().DrawCircle(gra, Color.Red, choice);
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
                    else
                    {
                        selected.top().DrawCircle(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().ChangeLine(500);
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                }
            }
            else if (e.KeyCode == Keys.W)//переместить вверх
            {
                while (selected.empty() == false)
                {
                    selected.top().y -= 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false && selected.top().LineIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red, choice);
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
            else if(e.KeyCode == Keys.S)//переместить вниз
            {
                while (selected.empty() == false)
                {
                    selected.top().y += 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false && selected.top().LineIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red, choice);
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
            else if (e.KeyCode == Keys.A)//переместить влево
            {
                while (selected.empty() == false)
                {
                    selected.top().x -= 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false && selected.top().LineIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red, choice);
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
            else if (e.KeyCode == Keys.D)//переместить вправо
            {
                while (selected.empty() == false)
                {
                    
                    selected.top().x += 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false && selected.top().LineIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawCircle(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawCircle(gra, Color.Red, choice);
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

        private void button1_Click(object sender, EventArgs e)
        {
            choice = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            choice = 2;
        }

        private void Drawing_MouseDown(object sender, MouseEventArgs e)
        {
            if (store.search(e.X, e.Y, choice) != null)
            { 
                if (Control.ModifierKeys == Keys.Control)
                {
                    pentemp.Color = store.search(e.X, e.Y, choice).circlepen.Color;
                    store.search(e.X, e.Y, choice).ChangeColor(Color.Red, gra);
                    selected.add(store.search(e.X, e.Y, choice));
                    
                }
                else
                {
                    while (selected.empty() == false)
                    {
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top());
                    }
                    pentemp.Color = store.search(e.X, e.Y, choice).circlepen.Color;
                    store.search(e.X, e.Y, choice).ChangeColor(Color.Red, gra);
                    selected.add(store.search(e.X, e.Y, choice));
                }
            }
            else
            {
                CCircle circle = new CCircle();
                if (circle.CircleIsOutside(e.X, e.Y, Drawing) == false && circle.LineIsOutside(e.X, e.Y, Drawing) == false)
                {
                    circle.DrawCircle(gra, pen.Color, choice);
                    store.add(circle);
                } else
                {
                    MessageBox.Show("Выход за границы");
                }
            }
        }
    }
}
