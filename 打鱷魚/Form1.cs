using System.Media;
using System.Drawing;

namespace 打鱷魚
{
    public partial class Form1 : Form
    {
        private Button button1;
        private Button button2;
        private Button button3;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;

        public Form1()
        {
            InitializeComponent();
            CreateDynamicControls();
            projectPath = AppDomain.CurrentDomain.BaseDirectory; // Default, will try to find images
        }

        private void CreateDynamicControls()
        {
            // label1: Game Date/Time
            label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new Point(800, 25);
            label1.Name = "label1";
            label1.Size = new Size(0, 15);
            this.Controls.Add(label1);

            // label2: Countdown
            label2 = new Label();
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft JhengHei UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(840, 124);
            label2.Name = "label2";
            label2.Size = new Size(56, 41);
            label2.Text = "30";
            this.Controls.Add(label2);

            // label3: Lucky words
            label3 = new Label();
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            label3.ForeColor = Color.Blue;
            label3.Location = new Point(840, 206);
            label3.Name = "label3";
            label3.Size = new Size(57, 20);
            label3.Text = "吉祥話";
            this.Controls.Add(label3);

            // label4: Score
            label4 = new Label();
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft JhengHei UI", 18F, FontStyle.Bold);
            label4.Location = new Point(840, 275);
            label4.Name = "label4";
            label4.Size = new Size(87, 30);
            label4.Text = "分數: 0";
            this.Controls.Add(label4);

            // button1: Start
            button1 = new Button();
            button1.Location = new Point(840, 400);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.Text = "開始遊戲";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            this.Controls.Add(button1);

            // button2: Restart
            button2 = new Button();
            button2.Location = new Point(840, 443);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.Text = "重新遊戲";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            this.Controls.Add(button2);

            // button3: Exit
            button3 = new Button();
            button3.Location = new Point(840, 487);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.Text = "結束遊戲";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            this.Controls.Add(button3);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        int count = 0;
        int column = 0;
        int row = 0;
        int times = 100;
        DateTime startTime;
        DateTime endTime;
        int countdown = 30;
        List<Button> buttons = new List<Button>();
        Random rand = new Random();
        int score = 0;
        string projectPath;

        string[] zodiacSayings = {
            "鼠兆豐年", "牛轉乾坤", "虎虎生風", "鴻兔大展",
            "龍騰虎躍", "靈蛇昂首", "馬到成功", "三羊開泰",
            "靈猴獻瑞", "金雞報喜", "瑞犬納福", "豬事大吉"
        };

        // Helper to get image path safely
        private string GetImagePath(string fileName)
        {
            // Try current directory, then try project folder if running from IDE
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(path))
            {
                // Assuming standard bin/Debug/net10.0-windows structure
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", fileName);
            }
            if (!File.Exists(path))
            {
                // Fallback to the absolute path from original code if still not found
                path = Path.Combine(@"D:\Users\user\source\repos\打鱷魚\打鱷魚", fileName);
            }
            return path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 初始化
            startTime = DateTime.Now;
            countdown = 30;
            score = 0;
            label4.Text = "分數: 0";
            label3.Text = "";
            label2.Text = countdown.ToString();
            timer1.Enabled = true; timer1.Interval = 1000;
            timer2.Enabled = true; timer2.Interval = 2000;

            foreach (var btn in buttons)
            {
                this.Controls.Remove(btn);
            }
            buttons.Clear();
            count = 0; column = 0; row = 0;

            for (int i = 0; i < times; i++)
            {
                Button btn = new Button();  //建立1個btn 空間
                btn.Location = new Point(12 + (column * 75) + (column * 0), 12 + (row * 75) + (row * 0));
                count++;

                // 載入並縮放圖片至 75x75
                string imgPath = GetImagePath("A.png");
                if (File.Exists(imgPath))
                {
                    using (Image img = Image.FromFile(imgPath))
                    {
                        btn.Image = new Bitmap(img, new Size(75, 75));
                    }
                }
                btn.Tag = "A";

                btn.Size = new Size(75, 75);
                btn.TabIndex = count;
                btn.Click += new EventHandler(btn_Click);
                this.Controls.Add(btn);
                buttons.Add(btn);

                column += 1;
                if (count % 10 == 0)
                {
                    column = 0;
                    row += 1;
                }
            }
        }
        public void btn_Click(object sender, EventArgs e)
        {
            Button clickedBtn = sender as Button;
            if (clickedBtn.Tag != null && clickedBtn.Tag.ToString() == "B")
            {
                int r = rand.Next(1, 101);
                
                if (r <= 50) // 50% 機率
                {
                    score += 1;
                }
                else if (r <= 75) // 25% 機率
                {
                    score += 3;
                    string imgPath = GetImagePath("C.png");
                    if (File.Exists(imgPath))
                    {
                        using (Image img = Image.FromFile(imgPath))
                        {
                            clickedBtn.Image = new Bitmap(img, new Size(75, 75));
                        }
                    }
                    clickedBtn.Tag = "C";
                }
                else if (r <= 90) // 15% 機率
                {
                    score -= 2;
                    string imgPath = GetImagePath("D.png");
                    if (File.Exists(imgPath))
                    {
                        using (Image img = Image.FromFile(imgPath))
                        {
                            clickedBtn.Image = new Bitmap(img, new Size(75, 75));
                        }
                    }
                    clickedBtn.Tag = "D";
                }
                else // 10% 機率
                {
                    score += 5;
                    label3.Text = zodiacSayings[rand.Next(zodiacSayings.Length)];
                }
                label4.Text = $"分數: {score}";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (countdown > 0)
            {
                countdown--;
                label2.Text = countdown.ToString();
                
                DateTime now = DateTime.Now;
                label1.Text = $"遊戲日期: {startTime:yyyy/MM/dd}\n" +
                              $"開始時間: {startTime:HH:mm:ss}\n"; 
            }
            else
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                endTime = DateTime.Now;
                label1.Text += $"\n結束時間: {endTime:HH:mm:ss}";
                MessageBox.Show($"時間到！遊戲結束。\n最終分數: {score}");
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string imgPathA = GetImagePath("A.png");
            if (File.Exists(imgPathA))
            {
                using (Image imgA = Image.FromFile(imgPathA))
                {
                    Bitmap bmpA = new Bitmap(imgA, new Size(75, 75));
                    foreach (var btn in buttons)
                    {
                        btn.Image = bmpA;
                        btn.Tag = "A";
                    }
                }
            }

            int num = rand.Next(2, 6);
            string imgPathB = GetImagePath("B.png");
            if (File.Exists(imgPathB))
            {
                using (Image imgB = Image.FromFile(imgPathB))
                {
                    Bitmap bmpB = new Bitmap(imgB, new Size(75, 75));
                    for (int i = 0; i < num; i++)
                    {
                        int index = rand.Next(buttons.Count);
                        buttons[index].Image = bmpB;
                        buttons[index].Tag = "B";
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var btn in buttons)
            {
                this.Controls.Remove(btn);
            }
            button1_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
