using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RPR.Model
{
    public abstract class BaseView
    {
        protected Canvas View { get; set; }

        protected bool IsInitialized { get; set; }

        abstract public void Init();

        abstract public void DeInit();

        abstract public void Update(UIElement element);

        abstract public void Delete(UIElement element);

        abstract public void Delete(int elemID);

        abstract public void DeleteAll();
    }
}
