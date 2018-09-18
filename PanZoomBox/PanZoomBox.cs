using System;
using System.Drawing;
using System.Windows.Forms;

namespace PanZoomBox
{
    public partial class PanZoomBox: UserControl
    {

        private Image baseImage;
        private Rectangle viewPort;
        private Point panStartPoint = new Point();
        private double zoomFactor = 1;
        private int viewPortMaxX;
        private int viewPortMaxY;
        private int viewPortMinX;
        private int viewPortMinY;

        public PanZoomBox()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public Rectangle ViewPort
        {
            get
            {
                return this.viewPort;
            }
        }

        public Image Image
        {
            get
            {
                return this.baseImage;
            }
            
            set
            {
                this.baseImage = value;
                this.Invalidate();
            }
        }

        public double ZoomFactor
        {
            get
            {
                return this.zoomFactor;
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            if (baseImage != null)
            {
                this.CalculateViewport();
                this.DrawImage(e.Graphics);
            }

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                panStartPoint.X = e.X;
                panStartPoint.Y = e.Y;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {

            int MouseX = (int)Math.Round((e.X / zoomFactor) + viewPort.X);
            int MouseY = (int)Math.Round((e.Y / zoomFactor) + viewPort.Y);

            double zoomChange; //= (zoomFactor >= 10) ? 1 : (zoomFactor >= 1) ? .25 : .1;
            if (zoomFactor < 1)
            {
                zoomChange = .1;
            }
            else
            {
                zoomChange = zoomFactor / 10;
            }

            if (e.Delta > 0)
            {
                zoomFactor = Math.Round(zoomFactor + zoomChange, 1);
                
            }
            else
            {
                zoomFactor = Math.Round(zoomFactor - zoomChange, 1);
            }
            if (zoomFactor < .1)
            {
                zoomFactor = .1;
                return;
            }

            if (zoomFactor > 100)
            {
                zoomFactor = 100;
                return;
            }

            // Calculate displacement after zooming

            viewPort.X  += (int)Math.Round(MouseX - ((e.X / zoomFactor) + viewPort.X));
            viewPort.Y  += (int)Math.Round(MouseY - ((e.Y / zoomFactor) + viewPort.Y));

            this.Invalidate();
            
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {   
            if (e.Button == MouseButtons.Right)
            {
                int deltaX = (int)Math.Round((panStartPoint.X - e.X) / zoomFactor);
                int deltaY = (int)Math.Round((panStartPoint.Y - e.Y) / zoomFactor);

                // Update viewport location
                
                if (viewPort.X != (viewPort.X += deltaX))
                {
                    panStartPoint.X = e.X;
                }
                if (viewPort.Y != (viewPort.Y += deltaY))
                {
                    panStartPoint.Y = e.Y;
                }
                this.Invalidate();
            }

            base.OnMouseMove(e);
        }

        private void CalculateViewport()
        {
            // Calculate resulting viewport dimensions
            viewPort.Width = (int)Math.Round(ClientSize.Width / zoomFactor);
            viewPort.Height = (int)Math.Round(ClientSize.Height / zoomFactor);

            viewPortMaxX = baseImage.Width + viewPort.Width;
            viewPortMaxY = baseImage.Height + viewPort.Height;
            viewPortMinX = viewPort.Width * -1;
            viewPortMinY = viewPort.Height * -1;

            viewPort.X = (viewPort.X > viewPortMaxX) ? viewPortMaxX : (viewPort.X < viewPortMinX) ? viewPortMinX : viewPort.X;
            viewPort.Y = (viewPort.Y > viewPortMaxY) ? viewPortMaxY : (viewPort.Y < viewPortMinY) ? viewPortMinY : viewPort.Y;
            
        }

        private void DrawImage(Graphics g)
        {
            if (baseImage != null)
            {
                g.Clear(this.BackColor);
                Rectangle sourceRectangle = new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height);
                g.DrawImage(baseImage, ClientRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }
        }
    }
}
