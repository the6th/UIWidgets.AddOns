using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace UIWidgets.AddOns
{
    public class SplitView : StatefulWidget
    {
        public Widget view1;
        public Widget view2;
        public SplitViewMode viewMode;
        public float gripSize;
        public float initialWeight;
        public Color gripColor;
        public float positionLimit;
        public ValueChanged<float> onWeightChanged;

        public SplitView(
                Widget view1,
                Widget view2,
                SplitViewMode viewMode,
                float gripSize = 8f,
                float initialWeight = 0.5f,
                float positionLimit = 20f,
                Color gripColor = default(Color),
                ValueChanged<float> onWeightChanged = null
             )
        {
            D.assert(view1 != null);
            D.assert(view2 != null);
            this.view1 = view1;
            this.view2 = view2;
            this.viewMode = viewMode;
            this.gripSize = gripSize;
            this.gripColor = gripColor ?? Colors.grey;
            this.initialWeight = initialWeight;
            this.positionLimit = positionLimit;
            this.onWeightChanged = onWeightChanged;
        }
        public override State createState()
        {
            return new _SplitViewState();
        }
    }

    public class _SplitViewState : State<SplitView>
    {
        float defaultWeight;
        float _prevWeight;
        ValueNotifier<float> weight;

        public override void initState()
        {
            base.initState();
            this.defaultWeight = widget.initialWeight;
        }

        public override Widget build(BuildContext context)
        {
            weight = new ValueNotifier<float>(defaultWeight);
            _prevWeight = defaultWeight;

            return new LayoutBuilder(
                builder: (___, constraints) =>
                {
                    return new ValueListenableBuilder<float>(
                        valueListenable: weight,
                        builder: (_, w, __) =>
                        {
                            if (widget.onWeightChanged != null && _prevWeight != w)
                            {
                                _prevWeight = w;
                                widget.onWeightChanged(w);
                            }
                            if (widget.viewMode == SplitViewMode.Vertical)
                            {
                                return BuildVerticalView(context, constraints, w);
                            }
                            else
                            {
                                return BuildHorizontalView(context, constraints, w);
                            }
                        }//builder
                    );//ValueListenableBuilder
                }
            );//LayoutBuilder
        }

        Stack BuildVerticalView(BuildContext context, BoxConstraints constraints, float w)
        {
            var top = constraints.maxHeight * w;
            var bottom = constraints.maxHeight * (1f - w);

            return new Stack(
                children: new List<Widget>
                {
                    new Positioned(
                        top:0,
                        left:0,
                        right:0,
                        bottom: bottom,
                        child: widget.view1
                    ),
                    new Positioned(
                        top:top,
                        left:0,
                        right:0,
                        bottom:0,
                        child: widget.view2
                    ),
                    new Positioned(
                        top: top - widget.gripSize / 2f,
                        left: 0,
                        right: 0,
                        bottom: bottom - widget.gripSize / 2f,
                        child: new GestureDetector(
                            behavior: HitTestBehavior.translucent,
                            onVerticalDragUpdate: (detail) =>
                            {
                                RenderBox container = context.findRenderObject() as RenderBox;
                                var pos = container.globalToLocal(detail.globalPosition);

                                if (pos.dy > widget.positionLimit && pos.dy < (container.size.height - widget.positionLimit))
                                {
                                    weight.value = pos.dy / container.size.height;
                                }
                            },
                            child: new Container(color: widget.gripColor)
                        )//GestureDetector
                    ),
                }//list
            );// return Stack
        }

        Widget BuildHorizontalView(BuildContext context, BoxConstraints constraints, float w)
        {
            float left = constraints.maxWidth * w;
            float right = constraints.maxWidth * (1f - w);

            return new Stack(
              children: new List<Widget> {
                new Positioned(
                  top: 0,
                  left: 0,
                  right: right,
                  bottom: 0,
                  child: widget.view1
                ),
                new Positioned(
                  top: 0,
                  left: left,
                  right: 0,
                  bottom: 0,
                  child: widget.view2
                ),
                new Positioned(
                    top: 0,
                    left: left - widget.gripSize / 2f,
                    right: right - widget.gripSize / 2f,
                    bottom: 0,
                    child: new GestureDetector(
                        behavior: HitTestBehavior.translucent,
                        onVerticalDragUpdate: (detail)=> {
                            RenderBox container = context.findRenderObject() as RenderBox;
                            var pos = container.globalToLocal(detail.globalPosition);
                            if (pos.dx > widget.positionLimit && pos.dx < (container.size.width - widget.positionLimit))
                            {
                                weight.value = pos.dx / container.size.width;
                            }
                        },
                        child:new  Container(color: widget.gripColor)
                    )//Gesture
                )//position
              }//List
            );//stack
        }
    }
    public enum SplitViewMode
    {
        Vertical,
        Horizontal,
    }
}