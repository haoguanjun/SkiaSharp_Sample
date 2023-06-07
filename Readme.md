
## SkiaSharp 

### 屏幕坐标系

屏幕的坐标系原点在屏幕的左上角，水平往右、竖直往下为正。屏幕的坐标横坐标用“x”表示，纵坐标用“y”表示，坐标的单位为像素。坐标（4， 2）用表示当前点在原点右方4个像素处，在原点下方2个像素处，

### 颜色 SKColor

颜色的构造方法有很多种，但最常见的是利用RGB三原色来构造，此外还可以加入透明度：
```csharp
var color = new SKColor(180, 180, 180, 128);    //四个参数表示red, green, blue, alpha
```

### 画刷 SKPaint

```csharp
var paint = new SKPaint()
{
    Color = new SKColor(180, 180, 180, 128), //颜色
    StrokeWidth = 2, //画笔宽度
    Typeface = SKTypeface.FromFamilyName("宋体", SKFontStyle.Normal), //字体
    TextSize = 32,  //字体大小
    Style = SKPaintStyle.Stroke, //类型：填充 或 画边界 或全部
    PathEffect = SKPathEffect.CreateDash(LongDash, 0),   //绘制虚线
};
```

### 画布 SKCanvas

Skia所有的绘制是基于画布的。画布来自于 SKSurface，SKSurface 一般从图像从获取。画布绘制通常直接调用其 DrawXXX方法，其函数意义及所需参数大都可通过其名称轻易判断。而本项目中海图直接显示窗体中的SKControl 控件上，该控件的的 PaintSurface 事件中存在画布。

```csharp
SKImageInfo imageInfo = new SKImageInfo(300, 250);

using (SKSurface surface = SKSurface.Create(imageInfo))
{
    SKCanvas canvas = surface.Canvas;
    canvas.DrawColor(SKColors.Red);  //填充颜色
}
```

### 绘制直线 DrawLine

最简单的绘制函数，输入参数为起点、终点的坐标和画刷。

```csharp
canvas.DrawLine(3, 5, 500, 100, paint);   //用paint画直线，起点(3, 5)，终点(500, 100)
```    

### 绘制文本

在指定的坐标处，用画笔来绘制指定的文本。指定的坐标可被近似的认为位于文本的左下角。
```csharp
canvas.DrawText("文本", 50, 50, paint);
```

### 绘制矩形

矩形由四个参数来表示：左上角横坐标。左上角纵坐标，矩形宽度，矩形高度。

```csharp
canvas.DrawRect(10, 10, 100, 100, paint);
```

### 绘制多点 DrawPoints

多个点可以代表孤立的点，可代表线段，也可代表多边形区域。因此，绘制多点时，最重要的是传入多点的绘制模式[SKPointMode]，SKPointMode是一个枚举，其中0=点，1=线段，2=多边形。

```csharp
public void DrawPoints(SKPointMode mode, SKPoint[] points, SKPaint paint);
```

### 绘制路径 SKPath / DrawPath

绘制多点的方式可以绘制多边形区域的，但如果多边形内部存在空洞，绘制多点则无能为力了。而路径功能则强大得多，路径有两个最常用的方法：MoveTo 添加起点和LineTo 添加拐点。路径默认的填充方式为Winding，此外还有EvenOdd、InverseWinding、InverseEvenOdd。通过填充方式来判断某一封闭区域是属于整个区域内部还是外部。缠绕算法和奇偶算法都基于从该区域绘制到无限远的假设线来确定是否填充了任何封闭区域。 该线与构成路径的一条或多条边界线交叉。 在缠绕模式下，如果在一个方向上绘制的边界线数量与在另一方向上绘制的边界线数量平衡，则不会填充该区域（外部）；否则，该区域将被填充（内部）。 如果边界线的数量为奇数，则奇偶算法将填充一个区域。直观感受为，外圈顺时针将点添加进路径，内圈逆时针将点添加进路径，就可在内部形成一个空洞，这与海图空间记录编码标准一致。

```csharp
var path = new SKPath();
//外圈 顺时针
path.MoveTo(50, 50);    //起点
path.LineTo(50, 350);
path.LineTo(350, 350);
path.LineTo(350, 50);
//内圈 逆时针
path.MoveTo(100, 100);  //起点
path.LineTo(200, 100);
path.LineTo(200, 200);
path.LineTo(100, 200);

//绘制路径
canvas.DrawPath(path, new SKPaint());
```

### 截图

```csharp
using (SKImage image = e.Surface.Snapshot())
using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))  //指定图片格式及质量
using (var mStream = new MemoryStream(data.ToArray()))
{
    Bitmap bm = new Bitmap(mStream, false);
    pictureBox1.Image = bm;
}
```

### 坐标转换

有时绘制某一物标时，需要缩放一定比例、旋转一定角度或偏移一定的位置，这都涉及到坐标变换。任何平面坐标之间的转换关系可以直接用三维矩阵表示，也可以分步进行。分步变换时，每后一步的变换均在前一步变换基础之上的。

* 旋转（绕指定中心点旋转） public void RotateDegrees(float degrees, float px, float py);
* 缩放（绕指定中心点，分横轴与纵轴方向缩放）public void Scale(float sx, float sy, float px, float py);
* 平移 public void Translate(float dx, float dy);

如对一个路径，分别进行三次变换：

```csharp
var path = new SKPath();
path.MoveTo(50, 50);    //起点
path.LineTo(50, 150);
path.LineTo(150, 150);
path.LineTo(150, 50);
path.LineTo(50, 50);

//原图像 默认黑色
canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke });

//绕点(100,100)旋转45度，绘制成红色
canvas.RotateDegrees(45, 100, 100);
canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Red });

//缩放 横轴与纵轴方向缩小一倍，缩放中心为(100, 100), 绘制成绿色
canvas.Scale(0.5f, 0.5f, 100, 100);
canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Green });

//平移 向右平移150，向下平移150，绘制成蓝色
canvas.Translate(150, 150);
canvas.DrawPath(path, new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Blue });
```
如图所示，最后一步平移的实际结果，与初步设想（向右下方向平移150像素）不一样，是因为最后的平移需考虑前两步的旋转与缩放变换。

### 坐标系的保存与还原

由坐标变换可知，每一步变换都是全局的，都对之后的绘制的坐标系产生影响。当绘制电子海图物标需要执行不同变换时，为避免不同坐标系之间相互干扰，绘制流程一般如下：1. 记住标准坐标系；2. 根据物标需要变换坐标；3. 绘制物标；4. 还原坐标系（执行坐标变换的逆运算）。
而Skia中就提供了当前坐标保存Save()与还原Restore()的方法。

```csharp
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
```

### 中文字体

```csharp
result = SKTypeface.FromStream(stream);
```

或者

使用 SKFontManager 类来获取中文字体，代码如下：

```csharp
// 获取宋体在字体集合中的下标
var index = SKFontManager.Default.FontFamilies.ToList().IndexOf("宋体");
// 创建宋体字形
var songtiTypeface = SKFontManager.Default.GetFontStyles(index).CreateTypeface(0);
```


## 常见问题

1. dotnetcore项目使用 System.Drawing.Common 画图时会提示仅在 Windows 上支持，部署到 docker上 直接提示不支持该类库

这个问题微软官方文档有说明，在各个平台的画图方案，其中在linux平台推荐使用的是SkiaSharp库

2. dotnet 项目引用nuget包 SkiaShap 画图，在 Windows 运行没问题，跑到 Docker 里直接又报错‘System.DllNotFoundException: Unable to load shared library 'libSkiaSharp' or one of its dependencies’

这个问题也在网上搜了很多文章，最后自己试出来了，SkiaSharp 组件在windows上和linux上的包是不一样的，要想在linux上正常运行，需要在项目中把SkiaSharp包删掉，引用另一个包 [SkiaSharp.NativeAssets.Linux.NoDependencies](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux.NoDependencies)

重新编译，windows上运行成功，docker上也能运行成功

3. 使用SkiaSharp画文字的时候，在windows运行正常，到docker里文字不显示

这是因为docker环境里缺少字体，可以在基础镜像里添加字体或者通过其他方式将字体安装到docker中，可以去网上搜linux或者docker安装字体的教程

## 参考文档

* [SkiaSharp 文档](https://max.book118.com/html/2017/0327/97227850.shtm)
* [SkiaSharp 基本操作](https://www.jianshu.com/p/828d1b0f90ac)
* https://www.cnblogs.com/along007/p/15903991.html
* https://learn.microsoft.com/zh-cn/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/
* https://learn.microsoft.com/zh-cn/dotnet/api/skiasharp?view=skiasharp-2.88

