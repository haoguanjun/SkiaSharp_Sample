using SkiaSharp;

public class SkiaSample
{
    public void Sample12()
    {
        float[] intervals = new [] { 10f, 20f };

        // 定义画刷
        var paint = new SKPaint()
        {
            Color = new SKColor(180, 180, 180, 128), //颜色
            StrokeWidth = 2, //画笔宽度
            Typeface = SKTypeface.FromFamilyName(null, SKFontStyle.Normal), //字体
            TextSize = 32,  //字体大小
            Style = SKPaintStyle.Stroke, //类型：填充 或 画边界 或全部
            PathEffect = SKPathEffect.CreateDash(intervals, 0),   //绘制虚线
        };

        // 定义图片规格
        SKImageInfo imageInfo = new SKImageInfo(400, 400);
        
        // 创建绘制平面
        using (SKSurface surface = SKSurface.Create(imageInfo))
        {
            // 获得画布
            SKCanvas canvas = surface.Canvas;

            // 输出文本
            canvas.DrawText("你好，Skia", 200, 60, paint);

            // 定义绘制路径
            var path = new SKPath();
            path.MoveTo(50, 50);    //起点
            path.LineTo(50, 150);
            path.LineTo(150, 150);
            path.LineTo(150, 50);
            path.LineTo(50, 50);

            //原图像 默认黑色
            canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke });

            canvas.Save();
            //绕点(100,100)旋转45度，绘制成红色
            canvas.RotateDegrees(45, 100, 100);
            canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Red });
            canvas.Restore();

            canvas.Save();
            //缩放 横轴与纵轴方向缩小一倍，缩放中心为(100, 100), 绘制成绿色
            canvas.Scale(0.5f, 0.5f, 100, 100);
            canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Green });
            canvas.Restore();

            canvas.Save();
            //平移 向右平移150，向下平移150，绘制成蓝色
            canvas.Translate(150, 150);
            canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Blue });
            canvas.Restore();

            

            // 导出图片文件
            using (SKImage image = surface.Snapshot())
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))  //指定图片格式及质量
            
            using (var fsStream = new FileStream("Sample12.png", FileMode.CreateNew))
            {
                byte[] bytes = data.ToArray();
                fsStream.Write(bytes);
            }
        }
    }
}