using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RPR.Model
{
    public abstract class BaseView
    {
        static public Canvas View { get; protected set; }

        protected bool IsInitialized { get; set; }

        abstract public void Init();

        abstract public void DeInit();

        abstract public void Update(FrameworkElement element);

        abstract public void Delete(FrameworkElement element);

        abstract public void Delete(int elemID);

        abstract public void DeleteAll();

        abstract public void UpdateAll(List<FrameworkElement> element);
    }
}
