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
using System.IO;

namespace LABA6
{
    public partial class Form1 : Form
    {
        Graphics gra;//объявляем графику
        Pen pen, pentemp;//контур
        public int choice;//выбор крга или линии
        
        public Form1()
        {
            InitializeComponent();
            gra = Drawing.CreateGraphics();//привязываем графику к панели
            pen = new Pen(Color.Black, 5);//изначальный цвет контура
            pentemp = new Pen(Color.White, 5);//текущий цвет контура
        }

        abstract class AbstractFactory//паттерн Abstract Factory
        {
            public string name = @"C:\StoreInformation";
            public abstract void inputTXT();
            public abstract void outputTXT();
            public abstract void AddToGroup(CCircle a);
            public abstract CCircle RemoveFromGroup();
        }
        class CGroup : AbstractFactory//паттерн Composite
        {
            public int tempsize = 0;
            public CCircle[] group;
            public CGroup ()
            {
                group = new CCircle[1000];
            }
           
            public override void AddToGroup(CCircle a)
            {
                this.group[this.tempsize] = a;
                this.tempsize++;
            }
            public override CCircle RemoveFromGroup()
            {
                return this.group[this.tempsize - 1];
            }
            // Необходимое переодпределение
            public override void inputTXT()
            {
                throw new NotImplementedException();
            }
            public override void outputTXT()
            {
                throw new NotImplementedException();
            }
        }

        class CCircle : AbstractFactory//класс кругов и линий
        {
            public GraphicsPath circlepath;
            public Pen circlepen;
            public int x, y, rad;
            private int xc, yc;

            public CCircle()//конструктор по умолчанию
            {
                this.x = Cursor.Position.X;
                this.y = Cursor.Position.Y;
                this.rad = 90;
                this.circlepen = new Pen(Color.White, 5);
                this.circlepath = new GraphicsPath();
            }

            public bool CircleIsOutside(int xt, int yt, Panel a)//контроль границ круга
            {
                if ((xt + this.rad <= a.Width) && (yt + this.rad <= a.Height) && (xt - this.rad >= 0) && (yt - this.rad) >= 0)
                {
                    return false;
                }
                else return true;
            }

            public bool LineIsOutside(int xt, int yt, Panel a)//контроль границ линии
            {
                if ((xt <= a.Width) && (xt + 300 <= a.Width) && (yt <= a.Height) && (xt >= 0) && (yt >= 0))
                    return false;
                else return true;
            }

            public void ChangeLine(int xt)//изменение размера линии
            {
                this.circlepath.Reset();
                this.circlepath.AddLine(this.x, this.y, this.x + xt, this.y);
            }

            public void DrawCircle(Graphics g, Color penc, int c)//отрисовка круга и линии
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
            public void ChangeColor(Color a, Graphics g)//изменение цвета
            {
                this.circlepen.Color = a;
                g.DrawPath(this.circlepen, this.circlepath);
            }
           
            
            //Необходимое переопределение
            public override void AddToGroup(CCircle a)
            {
                throw new NotImplementedException();
            }
            public override CCircle RemoveFromGroup()
            {
                throw new NotImplementedException();
            }
            public override void outputTXT()
            {

            }
            public override void inputTXT()
            {

            }


                ~CCircle()//деструктор
            {
                //this.circlepath.Reset();
            }
        }

        class KatesStorage : CCircle//мое хранилище
        {
            private int size = 0;
            private bool flag;
            private int CountCircle = 0, CountLine = 0;
            private string info;
            private const int maxsize = 1000;
            private CCircle[] arr; 
            private CCircle musor = new CCircle();

            public KatesStorage()//конструктор по умолчанию
            {
                arr = new CCircle[maxsize];
            }
            
            public bool empty()//проверка пустоту
            {
                flag = false;
                for (int i = 0; i < size; i++)
                    if (this.arr[i] != this.musor)
                        flag = true;

                if (flag == false)
                    return true;
                else return false;
            }
            public void add(CCircle a, int c)//добавление элемента
            {
                this.arr[this.size] = a;
                this.size++;
                if (c == 1)
                    this.CountCircle++;
                else this.CountLine++;

            }
            public void del(CCircle a, int c)//удаление элемента
            {
                for (int i = 0; i < this.size; i++)
                    if (this.arr[i] == a)
                    {
                        if (c == 1)
                            this.CountCircle--;
                        else this.CountLine--;
                        this.arr[i] = this.musor;
                    }
                this.size--;

            }
            public CCircle top()//возвращает верхний элемент
            {
                if (empty() == false)
                    return this.arr[this.size - 1];
                else return null;
            }

            public void DelAll(Graphics g, int c)//удаление всех элементов
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
            public CCircle search(int xt, int yt, int choice)//поиск необходимого элемента
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
            public override void outputTXT()//Чтение из файла
            {

            }
            public override void inputTXT()//Вывод в файл
            {
                File.Delete(name);
                this.info = "Кругов - ";
                this.info += this.CountCircle.ToString();
                this.info += "\n";
                File.AppendAllText(name, this.info);
                this.info = "\0";
                this.info = "Линий - ";
                this.info += this.CountLine.ToString();
                this.info += "\n";
                File.AppendAllText(name, this.info);
            }

            ~KatesStorage()//деструктор
            {
                this.arr = null;
            }
        }

        KatesStorage store = new KatesStorage();
        KatesStorage selected = new KatesStorage();
        private void pictureBox1_Click(object sender, EventArgs e)//здесь задаю текущий цвет
        {
            PictureBox p = (PictureBox)sender;
            pen.Color = p.BackColor;
            while (selected.empty() == false)
            {
                selected.top().ChangeColor(pen.Color, gra);
                selected.del(selected.top(), choice);
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)//проверка нажатия клавиш клавиатуры
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
                    selected.del(selected.top(), choice);
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
                            selected.del(selected.top(), choice);
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
                        selected.del(selected.top(), choice);
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
                        selected.del(selected.top(), choice);
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
                        selected.del(selected.top(), choice);
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
                        selected.del(selected.top(), choice);
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
                        selected.del(selected.top(), choice);
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

        private void button1_Click(object sender, EventArgs e)//выбрали круг
        {
            choice = 1;
        }

        private void button2_Click(object sender, EventArgs e)//выбрали линию
        {
            choice = 2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            store.inputTXT();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            store.outputTXT();
        }

        private void Drawing_MouseDown(object sender, MouseEventArgs e)//здесь создаем круги
        {
            if (store.search(e.X, e.Y, choice) != null)//проверка попадания в какой-либо круг
            { 
                if (Control.ModifierKeys == Keys.Control)//выделение кругов
                {
                    pentemp.Color = store.search(e.X, e.Y, choice).circlepen.Color;
                    store.search(e.X, e.Y, choice).ChangeColor(Color.Red, gra);
                    selected.add(store.search(e.X, e.Y, choice), choice);
                    
                }
                else
                {
                    while (selected.empty() == false)
                    {
                        selected.top().ChangeColor(pentemp.Color, gra);
                        selected.del(selected.top(), choice);
                    }
                    pentemp.Color = store.search(e.X, e.Y, choice).circlepen.Color;
                    store.search(e.X, e.Y, choice).ChangeColor(Color.Red, gra);
                    selected.add(store.search(e.X, e.Y, choice), choice);
                }
            }
            else
            {
                CCircle circle = new CCircle();
                if (circle.CircleIsOutside(e.X, e.Y, Drawing) == false && circle.LineIsOutside(e.X, e.Y, Drawing) == false)
                {
                    circle.DrawCircle(gra, pen.Color, choice);
                    store.add(circle, choice);
                } else
                {
                    MessageBox.Show("Выход за границы");
                }
            }
        }
    }
}
