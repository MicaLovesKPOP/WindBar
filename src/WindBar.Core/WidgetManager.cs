using System;
using System.Collections.Generic;

namespace WindBar.Core
{
    /// <summary>
    /// Manages the registration and retrieval of widgets.
    /// </summary>
    public class WidgetManager
    {
        private readonly List<IWidget> _widgets = new List<IWidget>();

        public IEnumerable<IWidget> Widgets => _widgets.AsReadOnly();

        public void RegisterWidget(IWidget widget)
        {
            if (widget == null) throw new ArgumentNullException(nameof(widget));
            _widgets.Add(widget);
        }

        public IEnumerable<IWidget> GetWidgetsForOrientation(string orientation)
        {
            foreach (var widget in _widgets)
            {
                if (orientation.Equals("Horizontal", StringComparison.OrdinalIgnoreCase) && widget.Horizontal)
                    yield return widget;
                if (orientation.Equals("Vertical", StringComparison.OrdinalIgnoreCase) && widget.Vertical)
                    yield return widget;
            }
        }
    }
}
