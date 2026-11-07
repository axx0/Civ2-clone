using System.Numerics;
using Moq;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using Xunit;

namespace RaylibUI.Tests;

public class BaseControlMouseTests
{
    private sealed class Control(IControlLayout layout) : BaseControl(layout)
    {
        public bool LeftDown { get; set; }
        public bool RightDown { get; set; }
        protected override bool IsMouseButtonDown(MouseButton button) =>
            button == MouseButton.Left ? LeftDown : RightDown;
    }

    private static Control CreateControl()
    {
        var layout = new Mock<IControlLayout>();
        layout.As<IComponent>().SetupGet(l => l.Visible).Returns(true);
        layout.As<IComponent>().SetupGet(l => l.Bounds).Returns(new Rectangle(0, 0, 800, 600));
        var control = new Control(layout.Object) { Width = 100, Height = 100 };
        layout.SetupGet(l => l.Controls).Returns([control]);
        layout.As<IComponent>().SetupGet(l => l.Controls).Returns([control]);
        return control;
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(false, 120)]
    [InlineData(true, 1)]
    [InlineData(true, 120)]
    public void HoldingButtonRaisesOnePressAndOneRelease(bool right, int frames)
    {
        var control = CreateControl();
        var presses = new List<MouseButton>();
        var clicks = new List<MouseButton>();
        control.MouseDown += (_, e) => presses.Add(e.Button);
        control.Click += (_, e) => clicks.Add(e.Button);
        control.OnMouseEnter();
        control.LeftDown = !right;
        control.RightDown = right;
        for (var frame = 0; frame < frames; frame++) control.OnMouseMove(Vector2.Zero);
        Assert.Single(presses);
        Assert.Empty(clicks);
        control.LeftDown = control.RightDown = false;
        for (var frame = 0; frame < 3; frame++) control.OnMouseMove(Vector2.Zero);
        Assert.Single(clicks);
        Assert.Equal(right ? MouseButton.Right : MouseButton.Left, clicks[0]);
        Assert.Equal(presses, clicks);
    }

    [Fact]
    public void LeavingControlCancelsPendingClick()
    {
        var control = CreateControl();
        var clicks = 0;
        control.Click += (_, _) => clicks++;
        control.OnMouseEnter();
        control.LeftDown = true;
        control.OnMouseMove(Vector2.Zero);
        control.OnMouseLeave();
        control.LeftDown = false;
        control.OnMouseEnter();
        control.OnMouseMove(Vector2.Zero);
        Assert.Equal(0, clicks);
    }

    [Fact]
    public void ReleasingThenPressingAgainRaisesASecondGesture()
    {
        var control = CreateControl();
        var presses = 0;
        var clicks = 0;
        control.MouseDown += (_, _) => presses++;
        control.Click += (_, _) => clicks++;
        control.OnMouseEnter();
        for (var gesture = 0; gesture < 2; gesture++)
        {
            control.LeftDown = true;
            control.OnMouseMove(Vector2.Zero);
            control.LeftDown = false;
            control.OnMouseMove(Vector2.Zero);
        }
        Assert.Equal(2, presses);
        Assert.Equal(2, clicks);
    }
}
