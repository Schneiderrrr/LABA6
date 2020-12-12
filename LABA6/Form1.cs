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
        public int choice = 1;//выбор крга или линии

        public Form1()
        {
            InitializeComponent();
            gra = Drawing.CreateGraphics();//привязываем графику к панели
            pen = new Pen(Color.Black, 5);//изначальный цвет контура
            pentemp = new Pen(Color.White, 5);//текущий цвет контура
        }

        public abstract class AbstractFactory//паттерн Abstract Factory
        {
            protected string name = @"C:\StoreInformation", info;
            protected string[] infos;
            public int CountCircle, CountLine;
            public abstract void inputTXT(int CountC, int CountL);
            public abstract void outputTXT(int CountC, int CountL);
        }
        public class InPutFile: AbstractFactory
        {
            public InPutFile()
            {

            }
            public override void inputTXT(int CountC, int CountL)//добавление в файл данных
            {
                if (File.ReadAllLines(name) != null)
                {
                    File.Delete(name);
                    this.info = "Кругов - ";
                    this.info += CountC.ToString();
                    this.info += "\n";
                    File.AppendAllText(name, this.info);
                    this.info = "\0";
                    this.info = "Линий - ";
                    this.info += CountL.ToString();
                    this.info += "\n";
                    File.AppendAllText(name, this.info);
                    
                }
            }
            public override void outputTXT(int CountC, int CountL)
            {
                throw new NotImplementedException();
            }
        }
        class OutPutFile : AbstractFactory
        {
            public OutPutFile()
            {
                
            }
            public override void inputTXT(int CountC, int CountL)
            {
                throw new NotImplementedException();
            }
            public override void outputTXT(int CountC, int CountL)//вывод из текстового файла
            {
                this.infos = File.ReadAllLines(name);
                this.info = this.infos[0].Remove(0, 9);
                CountC = Int32.Parse(this.info);
                this.info = this.infos[1].Remove(0, 8);
                CountL = Int32.Parse(this.info);
                this.CountCircle = CountC;
                this.CountLine = CountL;
            }
        }
        class CShape//класс кругов и линий
        {
            public GraphicsPath circlepath;
            public Pen circlepen;
            public int x, y, rad, iS;
            private int xc, yc;

            public CShape()//конструктор по умолчанию
            {
                this.x = Cursor.Position.X;
                this.y = Cursor.Position.Y;
                this.rad = 90;
                this.circlepen = new Pen(Color.White, 5);
                this.circlepath = new GraphicsPath();
            }
            public CShape(int xt, int yt)
            {
                this.x = xt;
                this.y = yt;
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

            public void DrawShape(Graphics g, Color penc, int c)//отрисовка круга и линии
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
                    this.circlepath.AddLine(this.x, this.y, this.x + 300, this.y);
                }
                g.DrawPath(this.circlepen, this.circlepath);
            }
            public void ChangeColor(Color a, Graphics g)//изменение цвета
            {
                this.circlepen.Color = a;
                g.DrawPath(this.circlepen, this.circlepath);
            }
            ~CShape()//деструктор
            {
                
            }
        }
        class CGroup : CShape //паттерн Composite
        {
            private int tempsize;
            public CShape[] group;
            public CGroup()//конструктор по умолчанию
            {
                this.group = new CShape[1000];
                this.tempsize = 0;
            }
            public void AddToGroup(CShape a)//добавление в группу
            {
                this.group[this.tempsize] = a;
                this.tempsize++;
            }
            public void DelFromGroup()//удаление из группы
            {
                this.tempsize--;
            }
            public CShape GetFromGroup()//возврат элемента из группы
            {
                return this.group[this.tempsize-1];
            }
        }
        class KatesStorage : CShape//мое хранилище
        {
            public int size = 0;
            private bool flag;
            public int CountCircle = 0, CountLine = 0;
            private const int maxsize = 1000;
            private CShape[] arr;
            private CShape musor = new CShape();

            public KatesStorage()//конструктор по умолчанию
            {
                arr = new CShape[maxsize];
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
            public void add(CShape a, int c)//добавление элемента
            {
                this.arr[this.size] = a;
                this.size++;
                if (c == 1)
                    this.CountCircle++;
                else this.CountLine++;
            }
            public void del(CShape a, int c)//удаление элемента
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
            public CShape top()//возвращает верхний элемент
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
                    this.arr[i].DrawShape(g, Color.White, c);
                    this.arr[i] = null;
                    i++;
                    this.size--;
                }
            }
            public CShape search(int xt, int yt, int choice)//поиск необходимого элемента
            {
                this.flag = false;
                this.iS = 0;
                if (choice == 1)
                {
                    for (int i = 0; i < this.size; i++)
                    {
                        if ((this.arr[i].circlepath.IsVisible(xt, yt) == true) && (this.arr[i] != this.musor))
                        {
                            this.iS = i;
                            flag = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.size; i++)
                    {
                        if ((this.arr[i].circlepath.IsOutlineVisible(xt, yt, this.circlepen) == true) && (this.arr[i] != this.musor))
                        {
                            this.iS = i;
                            flag = true;
                        }
                    }
                }
                if (flag == false)
                    return null;
                else return this.arr[this.iS];
            }
            public int RetInd (int xt, int yt, int choice)
            {
                this.flag = false;
                this.iS = 0;
                if (choice == 1)
                {
                    for (int i = 0; i < this.size; i++)
                    {
                        if ((this.arr[i].circlepath.IsVisible(xt, yt) == true) && (this.arr[i] != this.musor))
                        {
                            this.iS = i;
                            flag = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.size; i++)
                    {
                        if ((this.arr[i].circlepath.IsOutlineVisible(xt, yt, this.circlepen) == true) && (this.arr[i] != this.musor))
                        {
                            this.iS = i;
                            flag = true;
                        }
                    }
                }
                if (flag == false)
                    return -1;
                else return this.iS;
            }
            public CShape RetCSByInd(int i)
            {
                return this.arr[i-1];
            }
            public KatesStorage Lipk(KatesStorage KS1, KatesStorage KS2, int c)//KS1 - selected, KS2 - store
            {
                for (int i = 0; i < KS2.size; i++)
                {
                    double rast = Math.Sqrt(Math.Pow((KS2.arr[i].x - KS1.arr[0].x), 2) + Math.Pow((KS2.arr[i].y - KS1.arr[0].y), 2));
                    int maxR = Math.Max(KS1.arr[0].rad, KS2.arr[i].rad);
                    /*double temp1 = Math.Pow((KS1.arr[0].x - KS2.arr[i].x), 2) + Math.Pow((KS1.arr[0].y - KS2.arr[i].y), 2);
                    double temp2 = Math.Abs(KS1.arr[0].rad - KS2.arr[i].rad);
                    double temp3 = Math.Pow((KS1.arr[0].rad + KS2.arr[i].rad), 2);*/
                    if (rast <= (2 * maxR))
                    {
                        KS1.add(KS2.arr[i], c);
                    }
                }
                return KS1;
            }
            ~KatesStorage()//деструктор
            {
                this.arr = null;
            }
        }//класс хранилище
        class Observer : KatesStorage //паттерн Observer
        {
            private string name;
            public void AddToTree(TreeView tree, int choice, KatesStorage KS)
            {
                if (choice == 1)
                {
                    this.name = "Круг";
                    this.name += KS.CountCircle.ToString();
                }
                else
                {
                    this.name = "Линия";
                    this.name += KS.CountLine.ToString();
                }
                tree.Nodes[0].Nodes.Add(name);
            }
            public void SelectInTree(TreeView tree, int index, Color c)
            {
                tree.Nodes[0].Nodes[index].BackColor = c;
            }
            public int SelectOutTree(string Name, int choice)
            {
                int index;
                if (choice == 1)
                {
                    index = Int32.Parse(Name.Substring(4));
                }
                else
                {
                    index = Int32.Parse(Name.Substring(5));
                }
                return index;
            }
            public KatesStorage Lipkii(KatesStorage a, KatesStorage b, int c)
            {
                return a.Lipk(a, b, c);
            }
        }
        /*Объявление необходимых объектов*/
        InPutFile inp = new InPutFile();
        OutPutFile outp = new OutPutFile();
        CGroup group = new CGroup();
        KatesStorage store = new KatesStorage();
        KatesStorage selected = new KatesStorage();
        Observer observer = new Observer();
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
                    selected.top().DrawShape(gra, Color.White, choice);
                    if (choice == 1)
                    {
                        selected.top().rad /= 2;
                        selected.top().circlepath.Reset();
                        selected.top().DrawShape(gra, Color.Red, choice);
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
                            selected.top().DrawShape(gra, Color.White, choice);
                            selected.top().circlepath.Reset();
                            selected.top().DrawShape(gra, Color.Red, choice);
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
                        selected.top().DrawShape(gra, Color.White, choice);
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
                        selected.top().DrawShape(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawShape(gra, Color.Red, choice);
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
            else if (e.KeyCode == Keys.S)//переместить вниз
            {
                while (selected.empty() == false)
                {
                    selected.top().y += 50;
                    if (selected.top().CircleIsOutside(selected.top().x, selected.top().y, Drawing) == false && selected.top().LineIsOutside(selected.top().x, selected.top().y, Drawing) == false)
                    {
                        selected.top().DrawShape(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawShape(gra, Color.Red, choice);
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
                        selected.top().DrawShape(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawShape(gra, Color.Red, choice);
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
                        selected.top().DrawShape(gra, Color.White, choice);
                        selected.top().circlepath.Reset();
                        selected.top().DrawShape(gra, Color.Red, choice);
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
            inp.inputTXT(store.CountCircle, store.CountLine);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            outp.outputTXT(store.CountCircle, store.CountLine);
            int x = 250, y = 250;
            for (int i = 0; i < outp.CountCircle; i++)
            {
                CShape a = new CShape();
                a.x = x;
                a.y = y;
                a.DrawShape(gra, pen.Color, 1);
                x += 50;
                store.add(a, 1);
            }
            x = 200;
            y = 200;
            for (int i = 0; i < outp.CountLine; i++)
            {
                CShape a = new CShape();
                a.x = x;
                a.y = y;
                a.DrawShape(gra, pen.Color, 2);
                y += 50;
                store.add(a, 2);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)//Выделение через дерево
        {
            while (selected.empty() == false)
            {
                group.GetFromGroup().ChangeColor(Color.Black, gra);
                selected.del(group.GetFromGroup(), choice);
                group.DelFromGroup();
            }
            for (int i = 0; i < store.size; i++)
            {
                treeView1.Nodes[0].Nodes[i].BackColor = Color.Gray;
            }
            treeView1.SelectedNode.BackColor = Color.Red;
            store.RetCSByInd(observer.SelectOutTree(treeView1.SelectedNode.Text, choice)).ChangeColor(Color.Red, gra);
            group.AddToGroup(store.RetCSByInd(observer.SelectOutTree(treeView1.SelectedNode.Text, choice)));
            selected.add(store.RetCSByInd(observer.SelectOutTree(treeView1.SelectedNode.Text, choice)), choice);
        }

        private void button5_Click(object sender, EventArgs e)//Липкость
        {
            if (selected.empty() == false)
            {
                selected = observer.Lipkii(selected, store, choice);
            }
        }

        private void Drawing_MouseDown(object sender, MouseEventArgs e)//здесь создаем круги
        {
            if (store.search(e.X, e.Y, choice) != null)//проверка попадания в какой-либо круг
            {
                if (Control.ModifierKeys == Keys.Control)//выделение кругов
                {
                    pentemp.Color = store.search(e.X, e.Y, choice).circlepen.Color;
                    store.search(e.X, e.Y, choice).ChangeColor(Color.Red, gra);
                    group.AddToGroup(store.search(e.X, e.Y, choice));
                    selected.add(group.GetFromGroup(), choice);
                    observer.SelectInTree(treeView1, store.RetInd(e.X, e.Y, choice), Color.Red);
                }
                else
                {
                    while (selected.empty() == false)
                    {
                        group.GetFromGroup().ChangeColor(pentemp.Color, gra);
                        selected.del(group.GetFromGroup(), choice);
                        group.DelFromGroup();
                    }
                    for (int i = 0; i<store.size; i++)
                    {
                        treeView1.Nodes[0].Nodes[i].BackColor = Color.Gray;
                    }
                    pentemp.Color = store.search(e.X, e.Y, choice).circlepen.Color;
                    store.search(e.X, e.Y, choice).ChangeColor(Color.Red, gra);
                    group.AddToGroup(store.search(e.X, e.Y, choice));
                    selected.add(group.GetFromGroup(), choice);
                    observer.SelectInTree(treeView1, store.RetInd(e.X, e.Y, choice), Color.Red);
                }
            }
            else
            {
                CShape circle = new CShape(e.X, e.Y);
                if (circle.CircleIsOutside(e.X, e.Y, Drawing) == false && circle.LineIsOutside(e.X, e.Y, Drawing) == false)
                {
                    circle.DrawShape(gra, pen.Color, choice);
                    store.add(circle, choice);
                    observer.AddToTree(treeView1, choice, store);
                }
                else
                {
                    MessageBox.Show("Выход за границы");
                }
            }
        }
    }
}
