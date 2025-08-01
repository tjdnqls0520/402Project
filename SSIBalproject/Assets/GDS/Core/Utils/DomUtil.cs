using System;

using GDS.Core.Events;
using UnityEngine;
using UnityEngine.UIElements;
namespace GDS.Core {
    /// <summary>
    /// Contains factory methods for creating various visual elements
    /// The terms Dom and Div come from the web
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/API/Document_Object_Model/Introduction"/>
    /// </summary>
    /// 

    public static class Dom {
        public static VisualElement Div() => new VisualElement().PickIgnore();
        public static VisualElement Div(params VisualElement[] children) => Div().Add(children);
        public static VisualElement Div(string className, params VisualElement[] children) => Div().Add(className, children);
        public static VisualElement Div(string className, string name, params VisualElement[] children) => Div().WithName(name).Add(className, children);

        public static Button Button(string text, Action click) => new(click) { text = text };
        public static Button Button(string className, string text, Action click) => Button(text, click).WithClass(className);
        public static Button Button(string className, string text, Action click, params VisualElement[] children) => Button(className, text, click).Add(children);

        public static Label Label(string text) => new Label(text).PickIgnore();
        public static Label Label(string className, string text) => Label(text).WithClass(className);
        public static Label Title(string text) => Label("title", text);
    }


    /// <summary>
    /// Contains extension methods for working with visual elements
    /// </summary>
    public static class DomExt {

        public static T Add<T>(this T element, params VisualElement[] collection) where T : VisualElement {
            foreach (var item in collection) element.Add(item);
            return element;
        }

        public static T Add<T>(this T element, string className, params VisualElement[] collection) where T : VisualElement {
            foreach (var item in collection) element.Add(item);
            return element.WithClass(className);
        }

        public static T WithName<T>(this T element, string name) where T : VisualElement {
            element.name = name;
            return element;
        }

        public static T ClearClass<T>(this T element) where T : VisualElement {
            element.ClearClassList();
            return element;
        }

        /// <summary>
        /// Adds a class or a list of classes (if separated by ' ') to the element
        /// </summary>
        /// <returns></returns>
        public static T WithClass<T>(this T element, string className) where T : VisualElement {
            return element.WithClass(className.Split(' '));
        }

        public static T WithClass<T>(this T element, params string[] classNames) where T : VisualElement {
            for (var i = 0; i < classNames.Length; i++) element.AddToClassList(classNames[i]);
            return element;
        }
        /// <summary>
        /// Removes a class or a list of classes (if separated by ' ') from the element
        /// </summary>
        public static T WithoutClass<T>(this T element, string className) where T : VisualElement {
            if (className.Contains(" ")) {
                var classNames = className.Split(' ');
                for (var i = 0; i < classNames.Length; i++) element.RemoveFromClassList(classNames[i]);
                return element;
            }

            element.RemoveFromClassList(className);
            return element;
        }

        /// <summary>
        /// Hides an element by appending a 'display-none' class. Requires said class be present in the stylesheet
        /// </summary>
        public static T Hide<T>(this T element) where T : VisualElement => element.WithClass("display-none");

        /// <summary>
        /// Shows an element by removing the 'display-none' class.
        /// </summary>
        public static T Show<T>(this T element) where T : VisualElement => element.WithoutClass("display-none");

        public static T SetVisible<T>(this T element, bool visible) where T : VisualElement => visible ? element.Show() : element.Hide();

        public static T Toggle<T>(this T element) where T : VisualElement => element.ClassListContains("display-none") ? element.Show() : element.Hide();


        public static T PickIgnore<T>(this T element) where T : VisualElement {
            element.pickingMode = PickingMode.Ignore;
            return element;
        }

        public static T PickIgnoreChildren<T>(this T element) where T : VisualElement {
            foreach (var child in element.Children()) child.PickIgnore();
            return element;
        }

        public static T PickIgnoreAll<T>(this T element) where T : VisualElement {
            return element.PickIgnore().PickIgnoreChildren();
        }

        public static T PickDefault<T>(this T element) where T : VisualElement {
            element.pickingMode = PickingMode.Position;
            return element;
        }

        public static VisualElement SetSize(this VisualElement element, Size size, int scale = 1) {
            element.style.width = size.W * scale;
            element.style.height = size.H * scale;
            return element;
        }

        public static T SetSize<T>(this T element, int sizePx) where T : VisualElement {
            element.style.width = sizePx;
            element.style.height = sizePx;
            return element;
        }

        public static VisualElement Translate(this VisualElement element, Pos pos, int scale = 1) {
            element.style.translate = new Translate(pos.X * scale, pos.Y * scale);
            return element;
        }

        public static VisualElement Translate(this VisualElement element, Size pos, int scale = 1) {
            element.style.translate = new Translate(pos.W * scale, pos.H * scale);
            return element;
        }

        public static VisualElement Translate(this VisualElement element, int x, int y) {
            element.style.translate = new Translate(x, y);
            return element;
        }

        public static T Rotate<T>(this T element, float deg, bool additive = false) where T : VisualElement {
            var d = additive ? deg + element.style.rotate.value.angle.value : deg;
            element.style.rotate = new StyleRotate(new Rotate(d));
            return element;
        }

        public static T BackgroundImage<T>(this T element, Sprite image) where T : VisualElement {
            element.style.backgroundImage = new StyleBackground(image);
            return element;
        }

        // TODO: Find a way to include the context (TElement) in the callback (perhaps create a wrapper?). This will simplify usage by not needing to close over TElement instance
        /// <summary>
        /// Adds a callback to Observable OnChange event and calls it once (like an Init).
        /// Subs/unsubs automatically on attach/detach.
        /// </summary>
        public static TElement Observe<TElement, T>(this TElement element, Observable<T> observable, Action<T> callback) where TElement : VisualElement {
            callback.Invoke(observable.Value);
            element.RegisterCallback<AttachToPanelEvent>(e => observable.OnChange += callback);
            element.RegisterCallback<DetachFromPanelEvent>(e => observable.OnChange -= callback);
            return element;
        }

        public static VisualElement SubscribeTo<T>(this VisualElement element, EventBus bus, Action<CustomEvent> callback) where T : CustomEvent => SubscribeTo<VisualElement, T>(element, bus, callback);
        public static TElement SubscribeTo<TElement, T>(this TElement element, EventBus bus, Action<CustomEvent> callback) where TElement : VisualElement where T : CustomEvent {
            element.RegisterCallback<AttachToPanelEvent>(e => bus.On<T>(callback));
            element.RegisterCallback<DetachFromPanelEvent>(e => bus.Off<T>(callback));
            return element;
        }

        public static T TriggerClassAnimation<T>(this T element, string className, int delay = 100) where T : VisualElement {
            element
                .WithClass(className)
                .schedule
                    .Execute(() => element.WithoutClass(className))
                    .ExecuteLater(delay);
            return element;
        }

        public static T TriggerClassAnimation<T>(this T element, string className, int delay, Action callback) where T : VisualElement {
            element
                .WithClass(className)
                .schedule
                    .Execute(() => {
                        element.WithoutClass(className);
                        callback();
                    })
                    .ExecuteLater(delay);
            return element;
        }

    }

    public static class DomExtensions {

        public static VisualElement PseudoFirstChild(this VisualElement element, string className = "first-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                foreach (var child in element.Children()) child.RemoveFromClassList(className);
                element[0].AddToClassList(className);
            });
            return element;
        }

        public static VisualElement PseudoLastChild(this VisualElement element, string className = "last-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                foreach (var child in element.Children()) child.RemoveFromClassList(className);
                element[element.childCount - 1].AddToClassList(className);
            });
            return element;
        }

        public static VisualElement PseudoEvenChild(this VisualElement element, string className = "even-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                for (int i = 0; i < element.childCount; i++)
                    if (i % 2 == 0) element[i].AddToClassList(className);
                    else element[i].RemoveFromClassList(className);
            });
            return element;
        }

        public static VisualElement PseudoOddChild(this VisualElement element, string className = "odd-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                for (int i = 0; i < element.childCount; i++)
                    if (i % 2 != 0) element[i].AddToClassList(className);
                    else element[i].RemoveFromClassList(className);
            });
            return element;
        }

        public static VisualElement Gap(this VisualElement element, int gap) {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                var direction = element.resolvedStyle.flexDirection;
                for (int i = 0; i < element.childCount - 1; i++)
                    switch (direction) {
                        case FlexDirection.Column: element[i].style.marginBottom = gap; break;
                        case FlexDirection.ColumnReverse: element[i].style.marginTop = gap; break;
                        case FlexDirection.RowReverse: element[i].style.marginLeft = gap; break;
                        default: element[i].style.marginRight = gap; break;
                    }
            });
            return element;
        }
    }
}
