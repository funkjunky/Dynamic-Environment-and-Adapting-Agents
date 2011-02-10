using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Project.Classes;

namespace Project
{
    public partial class BayesForm : Form
    {
        Timer timer;
        int year = 0;

        public BayesForm()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            timer = new Timer(this);

            Application.Run(this);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            double TIME_ELAPSED = 1;
            int counter = 0;
            while (counter < 50)
            {
                ++counter;
                ++year;
                timer.bnet.updateFull(TIME_ELAPSED);
                timer.clearCounter(timer.counter);

                EnvironmentMap.update(timer.counter);
            }

            this.Refresh();

            base.OnMouseClick(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int x = 10;
            int y = 10;
            Rectangle bounds = new Rectangle(new Point(x, y), new Size(775, 700));
            Rectangle square = new Rectangle(new Point(x, y), new Size(14, 14));
            Graphics dc = e.Graphics;
            Brush whiteBrush = Brushes.White;
            Brush blackBrush = Brushes.DarkGreen;
            Brush blueBrush = Brushes.Aqua;
            Brush lightgreenBrush = Brushes.LightGreen;
            Brush greenBrush = Brushes.Green;

            Font consolasFont = new Font("Consolas", 10);

            /*
             * Loop through EnvironmentMap, use toString of top level object
             * to obtain character to print and well, print it.
             * Possible to print color depening on land/water
             */
            dc.FillRectangle(whiteBrush, bounds);
            String Tile;
            for (int column = 0; column < EnvironmentMap.sizeY; ++column)
            {
                for (int row = 0; row < EnvironmentMap.sizeX; ++row)
                {
                    Tile = EnvironmentMap.outputAgentSquare(row, column);
                    square.X = x;
                    square.Y = y;
                    if (Tile.Equals("~"))
                        dc.FillRectangle(blueBrush, square);
                    else if (Tile.Equals("."))
                        dc.FillRectangle(lightgreenBrush, square);
                    else if (Tile.Equals("T"))
                        dc.FillRectangle(greenBrush, square);
                    dc.DrawString(Tile, consolasFont, blackBrush, new Point(x, y));

                    x += 11;
                }
                y += 14;
                x = 10;
            }

            // this part needs to go on the group box
            x = 10;
            y = 10;
            dc.DrawString("Year : " + year + ", jay-time : " + timer.bnet.TotalTimeElapsed, consolasFont, blackBrush, new Point(x, y));
            y += 28;
            dc.DrawString("Counter :", consolasFont, blackBrush, new Point(x, y));
            y += 14;
            foreach (KeyValuePair<string, int> C in timer.counter)
            {
                if (C.Key.Equals("Cave"))
                {
                    y += 14;
                    dc.DrawString("Agent Related Information", consolasFont, blackBrush, new Point(x, y));
                }
                y += 14;
                dc.DrawString(C.Key + " = " + C.Value, consolasFont, blackBrush, new Point(x, y));
            }

            base.OnPaint(e);
            
        }
    }
}
