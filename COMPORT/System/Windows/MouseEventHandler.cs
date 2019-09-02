namespace System.Windows
{
    internal class MouseEventHandler
    {
        private Action<object, EventArgs> opcje_enter;

        public MouseEventHandler(Action<object, EventArgs> opcje_enter)
        {
            this.opcje_enter = opcje_enter;
        }
    }
}