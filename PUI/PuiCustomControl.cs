using System;
using Polaris.Drawing;
using UnityEngine;
using XX;

namespace Polaris.PUI
{
    /// <summary>
    /// 把一个 <see cref="IPuiCustomControl"/> 挂到占位控件上：占位控件是一个不赋图的 <see cref="DsnDataImg"/>
    /// （只用来在 .pui 的自动排布里占一块 Width×Height 的地方），真正的内容由一个屏幕空间
    /// <see cref="DrawingSurface"/> 绘制。首次构建、以及每次 <see cref="DrawNode.Invalidate"/> 时都会回调
    /// <paramref name="control"/> 的 Draw；每帧把 Surface 位置同步到占位控件的实际屏幕坐标，让内容跟着所在
    /// 窗口移动。编译期 codegen 与热重载两条路径共用这一个方法。
    /// </summary>
    public static class PuiCustomControl
    {
        /// <param name="anchor">占位用的 <see cref="DsnDataImg"/>；不要再手动给它 Assign 图片。</param>
        /// <param name="anchorBlock"><c>designer.addImg(anchor)</c> 的返回值，用来取占位控件的运行期 Transform。</param>
        /// <param name="control">实际绘制内容的后端实现。</param>
        /// <param name="width">.pui 里为这个 Custom 元素声明的宽（像素）。</param>
        /// <param name="height">.pui 里为这个 Custom 元素声明的高（像素）。</param>
        public static DrawNode Attach(DsnDataImg anchor, FillImageBlock anchorBlock, IPuiCustomControl control, float width, float height)
        {
            if (anchor == null)
            {
                throw new ArgumentNullException(nameof(anchor));
            }

            if (anchorBlock == null)
            {
                throw new ArgumentNullException(nameof(anchorBlock));
            }

            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            var surface = DrawingAPI.CreateSurface(new DrawingSurfaceOptions
            {
                Space = DrawSpace.Screen,
                Plane = DrawPlane.Hud,
                Lifetime = DrawLifetime.Manual,
                DebugName = anchor.name,
            });

            var bounds = new PuiCustomControlBounds(width, height);
            DrawNode node = surface.Add(ctx => control.Draw(ctx, bounds));

            var follower = anchorBlock.getGob().AddComponent<PuiCustomControlFollower>();
            follower.Init(surface, anchorBlock.getTransform());

            return node;
        }

        /// <summary>
        /// 挂在占位控件 GameObject 上的纯同步组件：每帧把 Surface 的锚点对齐到占位控件的世界坐标
        /// （这套 GUI 系统的 Screen 图层没有相机变换，世界坐标分量本身就是 GUI 像素），占位控件所在的
        /// GameObject 销毁时（关窗/Teardown）随之 OnDestroy，顺带释放 Surface——Lifetime.Manual 不会自动收。
        /// </summary>
        sealed class PuiCustomControlFollower : MonoBehaviour
        {
            DrawingSurface surface;
            Transform anchor;

            internal void Init(DrawingSurface surface, Transform anchor)
            {
                this.surface = surface;
                this.anchor = anchor;
            }

            void LateUpdate()
            {
                if (anchor != null)
                {
                    Vector3 pos = anchor.position;
                    surface.Position = new DrawPoint(pos.x, pos.y);
                }
            }

            void OnDestroy() => surface?.Dispose();
        }
    }
}
