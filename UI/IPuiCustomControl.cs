using Polaris.Drawing;

namespace Polaris.UI
{
    /// <summary>
    /// "自定义控件"（.pui 里的 Custom 元素）的绘制契约：由模组自己的后端 C# 类实现，直接用
    /// <see cref="Polaris.Drawing"/> 的 DrawContext 画出控件内容，不走 DsnData* 声明式数据。
    /// 类型必须有公共无参构造函数——生成/热重载两条路径都靠反射按 .pui 里填的类型名 <c>new()</c> 出实例。
    /// </summary>
    public interface IPuiCustomControl
    {
        /// <summary>
        /// 首次构建、以及每次 <see cref="DrawNode.Invalidate"/> 时调用一次。<paramref name="ctx"/>
        /// 只在本次调用期间有效，不要跨帧持有；需要动态刷新时保存 <see cref="PuiCustomControl.Attach"/>
        /// 返回的 <see cref="DrawNode"/>，改变状态后调用它的 Invalidate()。
        /// </summary>
        void Draw(DrawContext ctx, PuiCustomControlBounds bounds);
    }

    /// <summary>传给 <see cref="IPuiCustomControl.Draw"/> 的占位尺寸：.pui 里为这个 Custom 元素声明的像素宽高。</summary>
    public readonly struct PuiCustomControlBounds
    {
        public PuiCustomControlBounds(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width { get; }

        public float Height { get; }
    }
}
