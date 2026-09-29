using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChineseChess
{
    public partial class Form1 : Form
    {
        int[,] ai ={{5,4,3,2,1,2,3,4,5},
                    {0,0,0,0,0,0,0,0,0},
                    {0,6,0,0,0,0,0,6,0},
                    {7,0,7,0,7,0,7,0,7},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {17,0,17,0,17,0,17,0,17},
                    {0,16,0,0,0,0,0,16,0},
                    {0,0,0,0,0,0,0,0,0},
                    {15,14,13,12,11,12,13,14,15}};
        int[,] ai2;
        Control bi = null, ci = null, di = null;
        int ao, bo;
        public Form1()
        {
            ai2 = (int[,])ai.Clone();
            Size = new Size(650, 750);
            this.BackColor = Color.White;
            Text = "Chinese Chess";
            StartPosition = FormStartPosition.CenterScreen;
            MouseDown += new MouseEventHandler(md);//佈局
            Paint += new PaintEventHandler(huitu);
            qizi();
        }
        void qizi()
        {
            for (int i = 0; i < 10; i++)//上棋子
                for (int l = 0; l < 9; l++)
                    if (ai[i, l] != 0)
                    {
                        PictureBox a = new PictureBox();
                        a.Image = new Bitmap(ai[i, l] + ".png");
                        a.Size = new Size(60, 60);
                        a.Tag = ai[i, l];
                        if (ai[i, l] < 10)//決定誰先出棋
                            a.Enabled = false;
                        a.Click += new EventHandler(cl);
                        a.Location = zb(new Point((l + 1) * 64, (i + 1) * 64));
                        if (ai[i, l] == 1)
                            di = a;
                        if (ai[i, l] == 11)
                            ci = a;
                        Controls.Add(a);
                    }
        }
        void md(object sender, MouseEventArgs e)
        {
            int h1 = zb2(e.Location).Y / 64, h2 = zb2(e.Location).X / 64;
            if (zb2(e.Location).X != 0)
                if (bi is Control)
                {
                    bool s = panduan(new Point(bi.Location.Y / 64, bi.Location.X / 64), new Point(h1, h2), Convert.ToInt32(bi.Tag));
                    if (ai[h1, h2] != 0 && s)
                        chidiao(zb2(e.Location));
                    if (s)
                    {
                        ai[h1, h2] = Convert.ToInt32(bi.Tag);
                        ai[bo, ao] = 0;
                        (bi as Control).Location = zb2(e.Location);
                        foreach (Control i in Controls)
                        {
                            if (Convert.ToInt32(i.Tag) / 10 == Convert.ToInt32(bi.Tag) / 10)
                                i.Enabled = false;
                            else
                                i.Enabled = true;
                        }
                    }
                }
            panjue();
            bi = null;
        }
        void chidiao(Point p)//吃掉
        {
            for (int i = 0; i < Controls.Count; i++)
                if (Controls[i].Location == p)
                {
                    Controls.RemoveAt(i);
                    break;
                }
        }
        bool panduan(Point a, Point b, int i)//a初始位置 b目標位置 i棋子種類
        {
            if (i == 17)//兵
            {
                if (a.Y == b.Y && a.X - b.X == 1)
                    return true;
                else if (Math.Abs(a.Y - b.Y) == 1 && a.X == b.X && a.X < 6)
                    return true;
            }
            else if (i == 7)//兵2
            {
                if (a.Y == b.Y && a.X - b.X == -1)
                    return true;
                else if (Math.Abs(a.Y - b.Y) == 1 && a.X == b.X && a.X > 4)
                    return true;
            }
            else if (i == 16 || i == 6)//炮 走法相同
            {
                int s = panding(a, b);
                if ((s == 0 && ai[b.X, b.Y] == 0) || (s == 1 && ai[b.X, b.Y] != 0))
                    return true;
            }
            else if (i == 15 || i == 5)//車 走法相同
            {
                int s = panding(a, b);
                if (a.X == b.X || a.Y == b.Y)
                    if (s == 0)//比較簡單 判定出來沒有子就可以了
                        return true;
            }
            else if (i == 14 || i == 4)//馬 走法相同
            {
                if ((Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)) == 5)//判斷長度通過
                {
                    if (b.X - a.X == 2 && ai[a.X + 1, a.Y] != 0)//判斷四個馬腳
                        return false;
                    if (b.X - a.X == -2 && ai[a.X - 1, a.Y] != 0)
                        return false;
                    if (b.Y - a.Y == 2 && ai[a.X, a.Y + 1] != 0)
                        return false;
                    if (b.Y - a.Y == -2 && ai[a.X, a.Y - 1] != 0)
                        return false;
                    return true;
                }
            }
            else if (i == 13 || i == 3)//象 稍微改變
            {
                if ((Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)) == 8)//判斷長度通過
                    if ((ai[(a.X + b.X) / 2, (a.Y + b.Y) / 2] == 0) && i == 13 ? b.X > 4 : b.X < 5)//判斷象腳通過還有不能過河
                        return true;
            }
            else if (i == 12 || i == 2)//士 稍微改變
            {
                if ((Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)) == 2)//判斷長度通過
                    if (b.Y >= 3 && b.Y <= 5)
                    {
                        if (b.X <= 2 && i == 2)
                            return true;
                        if (b.X > 6 && i == 12)//判斷不出界
                            return true;
                    }
            }
            else if (i == 11 || i == 1)//將 稍微改變
            {
                if ((Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)) == 1)//判斷長度通過
                    if (b.Y >= 3 && b.Y <= 5)
                    {
                        if (b.X <= 2 && i == 1)
                            return true;
                        if (b.X > 6 && i == 11)//判斷不出界
                            return true;
                    }
            }
            return false;
        }
        int panding(Point a, Point b)//用來判斷兩點之間有多少個棋子
        {
            int s = 0;
            if (a.X == b.X)
            {
                int max = a.Y > b.Y ? a.Y : b.Y;
                int mix = a.Y + b.Y - max;
                for (int h = mix + 1; h < max; h++)
                    if (ai[a.X, h] != 0)
                        s++;
            }
            if (a.Y == b.Y)
            {
                int max = a.X > b.X ? a.X : b.X;
                int mix = a.X + b.X - max;
                for (int h = mix + 1; h < max; h++)
                    if (ai[h, a.Y] != 0)
                        s++;
            }
            return s;
        }
        int panjue()//判斷勝負
        {
            int jiang = 0;
            for (int i = 0; i < 10; i++)
                for (int p = 0; p < 9; p++)
                    if (ai[i, p] == 1 || ai[i, p] == 11)
                        jiang += ai[i, p];
            if (jiang < 12)
            {
                MessageBox.Show(jiang == 1 ? "紅方贏了" : "黑方贏了");
                chongxia();
                return 0;
            }
            if (ci.Location.X == di.Location.X)//判斷直線相遇
                if (panding(z(ci.Location), z(di.Location)) == 0)
                {
                    MessageBox.Show(Convert.ToInt32(bi.Tag) > 10 ? "紅方贏了" : "黑方贏了");
                    chongxia();
                }
            return 0;
        }
        Point z(Point z)
        {
            return new Point(z.Y / 64, z.X / 64);
        }
        void chongxia()
        {
            ai = (int[,])ai2.Clone();
            Controls.Clear();
            qizi();
        }
        void cl(object sender, EventArgs e)
        {
            bi = sender as Control;
            ao = bi.Location.X / 64;
            bo = bi.Location.Y / 64;
        }
        Point zb(Point p)//坐標
        {
            return new Point(p.X - 30, p.Y - 30);
        }
        Point zb2(Point p)
        {
            for (int i = 1; i < 11; i++)
                for (int l = 1; l < 10; l++)
                    if (Math.Abs(p.X - l * 64) < 25 && Math.Abs(p.Y - i * 64) < 25)
                        return zb(new Point(l * 64, i * 64));
            return new Point(0, 0);
        }
        private void huitu(object sender, PaintEventArgs e)
        {
            for (int l = 0; l < 3; l += 2)
            {
                for (int i = 1; i < 10; i++)//畫棋盤
                    e.Graphics.DrawLine(Pens.Black, new Point(i * 64, (352 - 64) * l + 64), new Point(i * 64, (352 - 320 - ((i == 1 || i == 9) ? 64 : 0)) * l + 320 + ((i == 1 || i == 9) ? 64 : 0)));
                for (int i = 1; i < 6; i++)
                    e.Graphics.DrawLine(Pens.Black, new Point(64, (352 - 64 * i) * l + 64 * i), new Point(576, (352 - 64 * i) * l + 64 * i));
                e.Graphics.DrawLine(Pens.Black, new Point(256, (352 - 64) * l + 64), new Point(383, (352 - 192) * l + 192));
                e.Graphics.DrawLine(Pens.Black, new Point(383, (352 - 64) * l + 64), new Point(256, (352 - 192) * l + 192));
            }
        }
    }
}
