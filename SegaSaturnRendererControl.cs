using OpenGL;
using SegaSaturn.NET.Imaging;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SegaSaturn.NET.WinForms
{
    public partial class SegaSaturnRendererControl : UserControl
    {
        public SegaSaturnRendererControl()
        {
            this.InitializeComponent();
            this.openGlControl.Render += this.RenderScene;
            this.openGlControl.Animation = true;
            this.renderPanel = new PictureBox
            {
                BackColor = this.BackColor,
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackgroundImageLayout = ImageLayout.Center,
                WaitOnLoad = false
            };
            this.Controls.Add(this.renderPanel);
            this.Controls.SetChildIndex(this.renderPanel, 0);
            this.renderPanel.BringToFront();
        }

        public delegate void BeforeRenderInitializeDelegate();
        public event BeforeRenderInitializeDelegate BeforeRenderInitialize;

        public delegate void AfterRenderInitializeDelegate();
        public event AfterRenderInitializeDelegate AfterRenderInitialize;

        public delegate void RenderFrameDelegate();
        public event RenderFrameDelegate RenderFrame;

        private void RenderScene(object sender, GlControlEventArgs e)
        {
            if (!this.initialized)
            {
                if (this.BeforeRenderInitialize != null)
                    this.BeforeRenderInitialize();
                Gl.Initialize();
                Gl.ShadeModel(ShadingModel.Flat);
                Gl.Disable(EnableCap.Lighting);
                Gl.Enable(EnableCap.DepthTest);
                Gl.DepthMask(true);
                Gl.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Fastest);
                Gl.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);
                Gl.Hint(HintTarget.PointSmoothHint, HintMode.Fastest);
                Gl.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
                Gl.Hint(HintTarget.TextureCompressionHint, HintMode.Fastest);
                Gl.DepthFunc(DepthFunction.Lequal);
                Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                Gl.Enable(EnableCap.Blend);
                if (this.AfterRenderInitialize != null)
                    this.AfterRenderInitialize();
                this.initialized = true;
            }
            #region Begin Render

            Gl.Viewport(0, 0, this.openGlControl.Width, this.openGlControl.Height);
            Gl.ClearColor(this.ClearColor.Rf, this.ClearColor.Gf, this.ClearColor.Bf, this.ClearColor.Af);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.MatrixMode(MatrixMode.Projection);
            Gl.LoadIdentity();
            Gl.Ortho(-1.0, 1.0, 1.0, -1.0, -1.0, 1.0);
            Gl.MatrixMode(MatrixMode.Modelview);
            Gl.LoadIdentity();

            #endregion

            if (this.RenderFrame != null)
                this.RenderFrame();
            this.renderPanel.SuspendLayout();
            this.renderPanel.Image = this.CaptureScaledBitmap(this.renderPanel.Width, this.renderPanel.Height);
            this.renderPanel.ResumeLayout();
        }

        public uint LoadTexture(string path, SegaSaturnColor transparentColor = null)
        {
            using (Bitmap bmp = SegaSaturnImageConverter.LoadBitmapFromFile(path, transparentColor))
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                uint id = Gl.GenTexture();
                Gl.BindTexture(TextureTarget.Texture2d, id);
                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgb8, bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bitmapData.Scan0);
                bmp.UnlockBits(bitmapData);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMaxLevel, 0);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                return id;
            }
        }

        public void DrawAxis(bool x, bool y, bool z)
        {
            const float arrowLength = 1.0f;
            const float arrowTopLength = 0.1f;
            Gl.Begin(PrimitiveType.Lines);
            if (x)
            {
                Gl.Color3(1.0f, 0.0f, 0.0f);
                Gl.Vertex3(-arrowLength, 0.0f, 0.0f);
                Gl.Vertex3(arrowLength, 0.0f, 0.0f);
                Gl.Vertex3(arrowLength - arrowTopLength, arrowTopLength, 0.0f);
                Gl.Vertex3(arrowLength, 0.0f, 0.0f);
                Gl.Vertex3(arrowLength - arrowTopLength, -arrowTopLength, 0.0f);
                Gl.Vertex3(arrowLength, 0.0f, 0.0f);
            }
            if (y)
            {
                Gl.Color3(0.0f, 1.0f, 0.0f);
                Gl.Vertex3(0.0f, -arrowLength, 0.0f);
                Gl.Vertex3(0.0f, arrowLength, 0.0f);
                Gl.Vertex3(arrowTopLength, arrowLength - arrowTopLength, 0.0f);
                Gl.Vertex3(0.0f, arrowLength, 0.0f);
                Gl.Vertex3(-arrowTopLength, arrowLength - arrowTopLength, 0.0f);
                Gl.Vertex3(0.0f, arrowLength, 0.0f);
            }
            if (z)
            {
                Gl.Color3(1.0f, 0.0f, 1.0f);
                Gl.Vertex3(0.0f, 0.0f, -arrowLength);
                Gl.Vertex3(0.0f, 0.0f, arrowLength);
                Gl.Vertex3(0.0f, arrowTopLength, arrowLength - arrowTopLength);
                Gl.Vertex3(0.0f, 0.0f, arrowLength);
                Gl.Vertex3(0.0f, -arrowTopLength, arrowLength - arrowTopLength);
                Gl.Vertex3(0.0f, 0.0f, arrowLength);
            }
            Gl.End();
        }

        public void DrawDistortedSprite(SegaSaturnDistortedSprite sprite)
        {
            if (sprite.TextureId.HasValue)
            {
                Gl.Enable(EnableCap.Texture2d);
                Gl.BindTexture(TextureTarget.Texture2d, sprite.TextureId.Value);
            }
            Gl.Color4(1.0f, 1.0f, 1.0f, sprite.UseHalfTransparency ? 0.5f : 1.0f);
            Gl.Begin(PrimitiveType.Quads);
            if (sprite.TextureId.HasValue)
                Gl.TexCoord2(0.0, 0.0);
            else
                Gl.Color3(1.0f, 0.0f, 0.0f);
            Gl.Vertex3(sprite.A.FloatX, sprite.A.FloatY, sprite.A.FloatZ);
            if (sprite.TextureId.HasValue)
                Gl.TexCoord2(1.0, 0.0);
            else
                Gl.Color3(0.0f, 1.0f, 0.0f);
            Gl.Vertex3(sprite.B.FloatX, sprite.B.FloatY, sprite.B.FloatZ);
            if (sprite.TextureId.HasValue)
                Gl.TexCoord2(1.0, 1.0);
            else
                Gl.Color3(0.0f, 0.0f, 1.0f);
            Gl.Vertex3(sprite.C.FloatX, sprite.C.FloatY, sprite.C.FloatZ);
            if (sprite.TextureId.HasValue)
                Gl.TexCoord2(0.0, 1.0);
            else
                Gl.Color3(1.0f, 0.0f, 1.0f);
            Gl.Vertex3(sprite.D.FloatX, sprite.D.FloatY, sprite.D.FloatZ);
            Gl.End();
            if (sprite.TextureId.HasValue)
                Gl.Disable(EnableCap.Texture2d);
        }

        public Bitmap CaptureOriginalBitmap()
        {
            if (this.lastOriginalCapture == null)
                this.lastOriginalCapture = new Bitmap(this.openGlControl.Width, this.openGlControl.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            else if (this.lastOriginalCapture.Width != this.openGlControl.Width || this.lastOriginalCapture.Height != this.openGlControl.Height)
            {
                this.lastOriginalCapture.Dispose();
                this.lastOriginalCapture = new Bitmap(this.openGlControl.Width, this.openGlControl.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
            BitmapData bitmapData = this.lastOriginalCapture.LockBits(new Rectangle(0, 0, this.lastOriginalCapture.Width, this.lastOriginalCapture.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Gl.ReadPixels(0, 0, this.lastOriginalCapture.Width, this.lastOriginalCapture.Height, OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bitmapData.Scan0);
            this.lastOriginalCapture.UnlockBits(bitmapData);
            this.lastOriginalCapture.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return this.lastOriginalCapture;
        }

        public Bitmap CaptureScaledBitmap(int width, int height)
        {
            return this.lastScaledCapture = this.CaptureOriginalBitmap().Resize(width, height, true, this.lastScaledCapture);
        }

        private Bitmap lastOriginalCapture;
        private Bitmap lastScaledCapture;

        private const int CaptureDelayCompensation = 1;

        [EditorBrowsable(EditorBrowsableState.Always)]
        public SegaSaturnColor ClearColor { get; set; } = SegaSaturnColor.Black;

        [EditorBrowsable(EditorBrowsableState.Always)]
        public new Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                if (this.renderPanel != null)
                    this.renderPanel.BackColor = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public SegaSaturnRenderFramerate Framerate
        {
            get => this.framerate;
            set
            {
                switch (value)
                {
                    case SegaSaturnRenderFramerate.FullFramerate:
                        this.openGlControl.AnimationTime = 16 - SegaSaturnRendererControl.CaptureDelayCompensation; // 16.33 - (capture delay compensation) for 60 fps
                        break;
                    case SegaSaturnRenderFramerate.HalfFramerate:
                        this.openGlControl.AnimationTime = 33 - SegaSaturnRendererControl.CaptureDelayCompensation; // 32.66 - (capture delay compensation) for 30 fps
                        break;
                    default:
                        throw new NotSupportedException(value.ToString());
                }
                this.framerate = value;
            }
        }

        private SegaSaturnRenderFramerate framerate = SegaSaturnRenderFramerate.FullFramerate;

        [EditorBrowsable(EditorBrowsableState.Always)]
        public SegaSaturnRenderResolution Resolution
        {
            get => this.resolution;
            set
            {
                switch (value)
                {
                    case SegaSaturnRenderResolution.TV_320x224: this.openGlControl.Width = 320; this.openGlControl.Height = 224; break;
                    case SegaSaturnRenderResolution.TV_320x240: this.openGlControl.Width = 320; this.openGlControl.Height = 240; break;
                    case SegaSaturnRenderResolution.TV_320x256: this.openGlControl.Width = 320; this.openGlControl.Height = 256; break;
                    case SegaSaturnRenderResolution.TV_320x448: this.openGlControl.Width = 320; this.openGlControl.Height = 448; break;
                    case SegaSaturnRenderResolution.TV_320x480: this.openGlControl.Width = 320; this.openGlControl.Height = 480; break;
                    case SegaSaturnRenderResolution.TV_320x512: this.openGlControl.Width = 320; this.openGlControl.Height = 512; break;
                    case SegaSaturnRenderResolution.TV_352x224: this.openGlControl.Width = 352; this.openGlControl.Height = 224; break;
                    case SegaSaturnRenderResolution.TV_352x240: this.openGlControl.Width = 352; this.openGlControl.Height = 240; break;
                    case SegaSaturnRenderResolution.TV_352x256: this.openGlControl.Width = 352; this.openGlControl.Height = 256; break;
                    case SegaSaturnRenderResolution.TV_352x448: this.openGlControl.Width = 352; this.openGlControl.Height = 448; break;
                    case SegaSaturnRenderResolution.TV_352x480: this.openGlControl.Width = 352; this.openGlControl.Height = 480; break;
                    case SegaSaturnRenderResolution.TV_352x512: this.openGlControl.Width = 352; this.openGlControl.Height = 512; break;
                    case SegaSaturnRenderResolution.TV_640x224: this.openGlControl.Width = 640; this.openGlControl.Height = 224; break;
                    case SegaSaturnRenderResolution.TV_640x240: this.openGlControl.Width = 640; this.openGlControl.Height = 240; break;
                    case SegaSaturnRenderResolution.TV_640x256: this.openGlControl.Width = 640; this.openGlControl.Height = 256; break;
                    case SegaSaturnRenderResolution.TV_640x448: this.openGlControl.Width = 640; this.openGlControl.Height = 448; break;
                    case SegaSaturnRenderResolution.TV_640x480: this.openGlControl.Width = 640; this.openGlControl.Height = 480; break;
                    case SegaSaturnRenderResolution.TV_640x512: this.openGlControl.Width = 640; this.openGlControl.Height = 512; break;
                    case SegaSaturnRenderResolution.TV_704x224: this.openGlControl.Width = 704; this.openGlControl.Height = 224; break;
                    case SegaSaturnRenderResolution.TV_704x240: this.openGlControl.Width = 704; this.openGlControl.Height = 240; break;
                    case SegaSaturnRenderResolution.TV_704x256: this.openGlControl.Width = 704; this.openGlControl.Height = 256; break;
                    case SegaSaturnRenderResolution.TV_704x448: this.openGlControl.Width = 704; this.openGlControl.Height = 448; break;
                    case SegaSaturnRenderResolution.TV_704x480: this.openGlControl.Width = 704; this.openGlControl.Height = 480; break;
                    case SegaSaturnRenderResolution.TV_704x512: this.openGlControl.Width = 704; this.openGlControl.Height = 512; break;
                    default:
                        throw new NotSupportedException(value.ToString());
                }
                this.resolution = value;
            }
        }

        private SegaSaturnRenderResolution resolution = SegaSaturnRenderResolution.TV_320x240;
        private bool initialized;

        private PictureBox renderPanel;
    }
}
